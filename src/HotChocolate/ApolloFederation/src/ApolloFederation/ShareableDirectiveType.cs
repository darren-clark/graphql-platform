using HotChocolate.ApolloFederation.Constants;
using HotChocolate.ApolloFederation.Extensions;
using HotChocolate.ApolloFederation.Properties;
using static HotChocolate.ApolloFederation.Properties.FederationResources;

namespace HotChocolate.ApolloFederation;

/// <summary>
/// The @sharable directive is used to indicate that an object type's field is allowed
/// to be resolved by multiple subgraphs
/// (by default in Federation 2, object fields can be resolved by only one subgraph)
///
/// If applied to an object type, all fields of that type will be sharable.
/// <example>
/// @sharable
/// type ProductDimension  {
///   size: String!
///   weight: Number!
/// }
/// </example>
/// </summary>
public class ShareableDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Shareable)
            .Description(FederationResources.ShareableDirective_Description)
            .Location(DirectiveLocation.Object | DirectiveLocation.FieldDefinition)
            .RequiresApolloVersion(FederationVersion.V2_0, Feature_Shareable);
}
