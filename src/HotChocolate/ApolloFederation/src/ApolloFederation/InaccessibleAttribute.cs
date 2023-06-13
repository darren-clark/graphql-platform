namespace HotChocolate.ApolloFederation;

using System.Reflection;
using HotChocolate.Types.Descriptors;

[AttributeUsage(
    AttributeTargets.Property
    | AttributeTargets.Class
    | AttributeTargets.Interface
    | AttributeTargets.Parameter
    | AttributeTargets.Enum
    | AttributeTargets.Field)]
public class InaccessibleAttribute : DescriptorAttribute
{
    protected internal override void TryConfigure(IDescriptorContext context, IDescriptor descriptor, ICustomAttributeProvider element)
    {
        switch (descriptor)
        {
            case IObjectFieldDescriptor objectFieldDescriptor:
                objectFieldDescriptor.Inaccessible();
                break;
            case IObjectTypeDescriptor objectTypeDescriptor:
                objectTypeDescriptor.Inaccessible();
                break;
            case IInterfaceTypeDescriptor interfaceTypeDescriptor:
                interfaceTypeDescriptor.Inaccessible();
                break;
            case IUnionTypeDescriptor unionTypeDescriptor:
                unionTypeDescriptor.Inaccessible();
                break;
            case IArgumentDescriptor argumentDescriptor:
                argumentDescriptor.Inaccessible();
                break;
            case IEnumTypeDescriptor enumTypeDescriptor:
                enumTypeDescriptor.Inaccessible();
                break;
            case IEnumValueDescriptor enumValueDescriptor:
                enumValueDescriptor.Inaccessible();
                break;
            case IInputObjectTypeDescriptor inputObjectTypeDescriptor:
                inputObjectTypeDescriptor.Inaccessible();
                break;
            case IInputFieldDescriptor inputFieldDescriptor:
                inputFieldDescriptor.Inaccessible();
                break;
        }
    }
}
