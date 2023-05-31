namespace HotChocolate.ApolloFederation;

using Types.Descriptors;

public class InterfaceObjectAttribute: ObjectTypeDescriptorAttribute
{
    protected override void OnConfigure(IDescriptorContext context, IObjectTypeDescriptor descriptor, Type type)
    {
        descriptor.Directive<InterfaceObjectDirectiveType>();
    }
}
