namespace HotChocolate.ApolloFederation;

using HotChocolate.ApolloFederation.Constants;
using HotChocolate.ApolloFederation.Properties;

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
