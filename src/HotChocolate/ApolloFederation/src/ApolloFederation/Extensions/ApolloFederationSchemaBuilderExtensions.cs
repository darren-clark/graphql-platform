using HotChocolate.ApolloFederation;
using AnyType = HotChocolate.ApolloFederation.AnyType;
using static HotChocolate.ApolloFederation.Constants.WellKnownContextData;

namespace HotChocolate;

/// <summary>
/// Provides extensions to <see cref="ISchemaBuilder"/>.
/// </summary>
public static class ApolloFederationSchemaBuilderExtensions
{
    /// <summary>
    /// Adds support for Apollo Federation to the schema.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="ISchemaBuilder"/>.
    /// </param>
    /// <returns>
    /// Returns the <see cref="ISchemaBuilder"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="builder"/> is <c>null</c>.
    /// </exception>
    public static ISchemaBuilder AddApolloFederation(
        this ISchemaBuilder builder, FederationVersion version = FederationVersion.v1)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AddType<AnyType>();
        builder.AddType<EntityType>();
        builder.AddType<ServiceType>();
        builder.AddType<FieldSetType>();
        builder.AddType<ExternalDirectiveType>();
        builder.AddType<ProvidesDirectiveType>();
        builder.AddType<RequiresDirectiveType>();
        builder.AddDirectiveType(new KeyDirectiveType(version));
        // V2.0 added directives.
        if (version != FederationVersion.v1)
        {
            builder.AddType<RequiresDirectiveType>();
            builder.AddType<LinkDirectiveType>();
            builder.AddType<OverrideDirectiveType>();
            builder.AddType<InaccessibleDirectiveType>();
            builder.AddType<ShareableDirectiveType>();
        }

        builder.ContextData[FederationVersionKey] = version;
        builder.TryAddTypeInterceptor<FederationTypeInterceptor>();
        return builder;
    }
}
