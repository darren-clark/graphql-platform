using HotChocolate.ApolloFederation.Constants;
using HotChocolate.ApolloFederation.Properties;

namespace HotChocolate.ApolloFederation;

/// <summary>
/// The @inaccessible directive is used to indicate that a definition in the subgraph schema should be omitted from the router's API schema, even if that definition is also present in other subgraphs. This means that the field is not exposed to clients at all.
/// Common use cases for @inaccessible include:
///
/// Avoiding composition errors while making staggered updates to a definition that's shared across multiple subgraphs (such as a value type)
///
/// Using a private field as part of an entity's @key without exposing that field to clients
/// </summary>
/// <example>
/// SubgraphA:
/// type Position @shareable {
///     x: Int!
///     y: Int!
///     z: Int! @inaccessible
/// }
///
/// SubgraphB:
/// type Position @shareable {
///     x: Int!
///     y: Int!
///     #subrgraph is not yet updated
/// }
/// </example>
public class InaccessibleDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Inaccessible)
            .Description(FederationResources.InaccessibleDirective_Description)
            .Location(DirectiveLocation.FieldDefinition |
                      DirectiveLocation.Object |
                      DirectiveLocation.Interface |
                      DirectiveLocation.Union |
                      DirectiveLocation.ArgumentDefinition |
                      DirectiveLocation.Scalar |
                      DirectiveLocation.Enum |
                      DirectiveLocation.EnumValue |
                      DirectiveLocation.InputObject |
                      DirectiveLocation.InputFieldDefinition
            );
}
