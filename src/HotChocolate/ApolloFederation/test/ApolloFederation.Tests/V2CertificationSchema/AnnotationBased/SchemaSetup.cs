using System.Threading.Tasks;
using HotChocolate.ApolloFederation.V2CertificationSchema.AnnotationBased.Types;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.ApolloFederation.V2CertificationSchema.AnnotationBased;

public static class SchemaSetup
{
    public static async Task<IRequestExecutor> CreateAsync(FederationVersion version)
        => await new ServiceCollection()
            .AddSingleton<Data>()
            .AddGraphQL()
            .AddApolloFederation(version)
            .AddQueryType<Query>()
//            .AddType<ProductSku>()
            .RegisterService<Data>()
            .BuildRequestExecutorAsync();
}
