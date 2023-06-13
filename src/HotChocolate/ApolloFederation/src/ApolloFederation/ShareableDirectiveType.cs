using HotChocolate.ApolloFederation.Constants;
using HotChocolate.ApolloFederation.Properties;

namespace HotChocolate.ApolloFederation;

public class ShareableDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Shareable)
            .Description(FederationResources.ShareableDirective_Description)
            .Location(DirectiveLocation.Object | DirectiveLocation.FieldDefinition);

}
