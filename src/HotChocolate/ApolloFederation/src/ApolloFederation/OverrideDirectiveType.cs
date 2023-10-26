using HotChocolate.ApolloFederation.Constants;
using HotChocolate.ApolloFederation.Properties;

namespace HotChocolate.ApolloFederation;

/// <summary>
/// The @override directive is used to indicate that an object field is now resolved by this subgraph
/// instead of another subgraph where it's also defined.
/// This enables you to migrate a field from one subgraph to another
/// <example>
/// type Product @key(fields: "upc") {
///   upc: UPC!
///   quantity: Int @override(from: "Products")
/// }
/// </example>
/// </summary>
public class OverrideDirectiveType: DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Override)
            .Description(FederationResources.OverrideDirective_Description)
            .Location(DirectiveLocation.Field)
            .Argument(WellKnownArgumentNames.From)
            .Type<NonNullType<StringType>>();
}
