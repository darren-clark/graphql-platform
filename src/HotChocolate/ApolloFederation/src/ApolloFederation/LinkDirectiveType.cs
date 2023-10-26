using HotChocolate.ApolloFederation.Constants;
using HotChocolate.ApolloFederation.Properties;

namespace HotChocolate.ApolloFederation;

public class LinkDirectiveType: DirectiveType<LinkDirective>
{

    protected override void Configure(IDirectiveTypeDescriptor<LinkDirective> descriptor)
    {
        descriptor
            .Name(WellKnownTypeNames.Link)
            .Description(FederationResources.LinkDirective_Description)
            .Location(DirectiveLocation.Schema)
            .Repeatable();


        descriptor
            .Argument(WellKnownArgumentNames.Import)
            .Type<ListType<LinkImportType>>();

        descriptor
            .Argument(WellKnownArgumentNames.For)
            .Type<LinkPurposeType>();
    }
}
