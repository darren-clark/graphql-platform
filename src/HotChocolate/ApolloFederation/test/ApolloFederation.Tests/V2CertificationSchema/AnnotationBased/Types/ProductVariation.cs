using HotChocolate.Types.Relay;

namespace HotChocolate.ApolloFederation.V2CertificationSchema.AnnotationBased.Types;

//[Inaccessible]
public class ProductVariation
{
    public ProductVariation(string id)
    {
        Id = id;
    }

    [ID]
    public string Id { get; }
}
