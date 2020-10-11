using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Execution;
using HotChocolate.Language;
using HotChocolate.Utilities;

#nullable enable

namespace HotChocolate.Stitching.Pipeline
{
    internal class HttpRequestClient
    {
        private static readonly (string Key, string Value) _contentType =
            ("Content-Type", "application/json; charset=utf-8");

        private static readonly JsonWriterOptions _jsonWriterOptions =
            new JsonWriterOptions
            {
                SkipValidation = true,
                Indented = false
            };

        private readonly IHttpClientFactory _clientFactory;
        private readonly IErrorHandler _errorHandler;
        private readonly IHttpStitchingRequestInterceptor _requestInterceptor;

        public HttpRequestClient(
            IHttpClientFactory clientFactory,
            IErrorHandler errorHandler,
            IHttpStitchingRequestInterceptor requestInterceptor)
        {
            _clientFactory = clientFactory;
            _errorHandler = errorHandler;
            _requestInterceptor = requestInterceptor;
        }

        public async Task<IQueryResult> FetchAsync(
            IQueryRequest request,
            NameString targetSchema,
            CancellationToken cancellationToken = default)
        {
            HttpRequestMessage requestMessage =
                await CreateRequestAsync(request, targetSchema, cancellationToken)
                    .ConfigureAwait(false);

            return await FetchAsync(
                request,
                requestMessage,
                targetSchema,
                cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<IQueryResult> FetchAsync(
            IQueryRequest request,
            HttpRequestMessage requestMessage,
            NameString targetSchema,
            CancellationToken cancellationToken)
        {
            try
            {
                using HttpClient httpClient = _clientFactory.CreateClient(targetSchema);

                using HttpResponseMessage responseMessage = await httpClient
                    .SendAsync(requestMessage, cancellationToken)
                    .ConfigureAwait(false);

                responseMessage.EnsureSuccessStatusCode();

                using Stream stream = await responseMessage.Content
                    .ReadAsStreamAsync()
                    .ConfigureAwait(false);

                IReadOnlyDictionary<string, object?> response =
                    await BufferHelper.ReadAsync(
                            stream,
                            ParseResponse,
                            cancellationToken)
                        .ConfigureAwait(false);

                IQueryResult result = HttpResponseDeserializer.Deserialize(response);

                return await _requestInterceptor.OnReceivedResultAsync(
                        targetSchema,
                        request,
                        result,
                        responseMessage,
                        cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                IError error = _errorHandler.CreateUnexpectedError(ex)
                    .SetCode(ErrorCodes.Stitching.HttpRequestException)
                    .Build();

                return QueryResultBuilder.CreateError(error);
            }
            catch(Exception ex)
            {
                IError error = _errorHandler.CreateUnexpectedError(ex)
                    .SetCode(ErrorCodes.Stitching.UnknownRequestException)
                    .Build();

                return QueryResultBuilder.CreateError(error);
            }
            finally
            {
                requestMessage.Dispose();
            }
        }

        private async ValueTask<HttpRequestMessage> CreateRequestAsync(
            IQueryRequest request,
            NameString targetSchema,
            CancellationToken cancellationToken = default)
        {
            using var writer = new ArrayWriter();
            await using var jsonWriter = new Utf8JsonWriter(writer, _jsonWriterOptions);

            WriteJsonRequest(writer, jsonWriter, request);
            await jsonWriter.FlushAsync(cancellationToken).ConfigureAwait(false);

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new ByteArrayContent(writer.GetInternalBuffer(), 0, writer.Length)
                {
                    Headers = { { _contentType.Key, _contentType.Value } }
                }
            };

            await _requestInterceptor
                .OnCreateRequestAsync(targetSchema, request, requestMessage, cancellationToken)
                .ConfigureAwait(false);

            return requestMessage;
        }

        private static IReadOnlyDictionary<string, object?> ParseResponse(
            byte[] buffer, int bytesBuffered) =>
            Utf8GraphQLRequestParser.ParseResponse(buffer.AsSpan(0, bytesBuffered))!;

        private static void WriteJsonRequest(
            IBufferWriter<byte> writer,
            Utf8JsonWriter jsonWriter,
            IQueryRequest request)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("query", request.Query!.AsSpan());

            if (request.OperationName is { })
            {
                jsonWriter.WriteString("operationName", request.OperationName);
            }

            WriteJsonRequestVariables(writer, jsonWriter, request.VariableValues);
            jsonWriter.WriteEndObject();
        }

        private static void WriteJsonRequestVariables(
            IBufferWriter<byte> writer,
            Utf8JsonWriter jsonWriter,
            IReadOnlyDictionary<string, object?>? variables)
        {
            if (variables is not null  && variables.Count > 0)
            {
                jsonWriter.WritePropertyName("variables");

                jsonWriter.WriteStartObject();

                foreach (KeyValuePair<string, object?> variable in variables)
                {
                    jsonWriter.WritePropertyName(variable.Key);
                    WriteValue(writer, jsonWriter, variable.Value);
                }

                jsonWriter.WriteEndObject();
            }
        }

        private static void WriteValue(
            IBufferWriter<byte> writer,
            Utf8JsonWriter jsonWriter,
            object? value)
        {
            if (value is null || value is NullValueNode)
            {
                jsonWriter.WriteNullValue();
            }
            else
            {
                switch (value)
                {
                    case ObjectValueNode obj:
                        jsonWriter.WriteStartObject();

                        foreach (ObjectFieldNode field in obj.Fields)
                        {
                            jsonWriter.WritePropertyName(field.Name.Value);
                            WriteValue(writer, jsonWriter, field.Value);
                        }

                        jsonWriter.WriteEndObject();
                        break;

                    case ListValueNode list:
                        jsonWriter.WriteStartArray();

                        foreach (IValueNode item in list.Items)
                        {
                            WriteValue(writer, jsonWriter, item);
                        }

                        jsonWriter.WriteEndArray();
                        break;

                    case StringValueNode s:
                        jsonWriter.WriteStringValue(s.Value);
                        break;

                    case EnumValueNode e:
                        jsonWriter.WriteStringValue(e.Value);
                        break;

                    case IntValueNode i:
                        jsonWriter.WriteStringValue(i.Value);
                        RemoveQuotes(writer, jsonWriter, i.Value.Length);
                        break;

                    case FloatValueNode f:
                        jsonWriter.WriteStringValue(f.Value);
                        RemoveQuotes(writer, jsonWriter, f.Value.Length);
                        break;

                    case BooleanValueNode b:
                        jsonWriter.WriteBooleanValue(b.Value);
                        break;

                    default:
                        throw new NotSupportedException(
                            "Unknown variable value kind.");
                }
            }
        }

        private static void RemoveQuotes(
            IBufferWriter<byte> writer,
            Utf8JsonWriter jsonWriter,
            int length)
        {
            jsonWriter.Flush();
            writer.Advance(-(length + 2));
            Span<byte> span = writer.GetSpan(length + 2);
            span.Slice(1, length + 1).CopyTo(span);
            writer.Advance(length);
        }
    }
}
