using HotChocolate.ApolloFederation.Constants;
using HotChocolate.ApolloFederation.Properties;

namespace HotChocolate.ApolloFederation;

public class LinkDirectiveType: DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
    {
        base.Configure(descriptor);
        descriptor
            .Name(WellKnownTypeNames.Link)
            .Description(FederationResources.LinkDirective_Description)
            .Location(DirectiveLocation.Schema)
            .Repeatable()
            .Internal();

        descriptor
            .Argument(WellKnownArgumentNames.Url)
            .Type<NonNullType<StringType>>();

        descriptor
            .Argument(WellKnownArgumentNames.As)
            .Type<StringType>();

        descriptor
            .Argument(WellKnownArgumentNames.Import)
            .Type<ListType<ImportType>>();

        descriptor
            .Argument(WellKnownArgumentNames.For)
            .Type<PurposeType>();

    }
}
