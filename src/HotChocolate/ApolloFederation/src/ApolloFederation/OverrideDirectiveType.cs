namespace HotChocolate.ApolloFederation;

using HotChocolate.ApolloFederation.Constants;
using HotChocolate.ApolloFederation.Properties;

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
