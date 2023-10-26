using System.Reflection;
using HotChocolate.Types.Descriptors;

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
[AttributeUsage(
    AttributeTargets.Property
    | AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Interface
    | AttributeTargets.Parameter
    | AttributeTargets.Method
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
