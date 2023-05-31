namespace HotChocolate.ApolloFederation;

public class InterfaceObjectDirectiveType: DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor.Name("interfaceObject")
            .Location(DirectiveLocation.Object);
}
