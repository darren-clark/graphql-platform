using System.Reflection;
using HotChocolate.Types.Descriptors;


namespace HotChocolate.ApolloFederation;

/// <summary>
/// The @sharable directive is used to indicate that an object type's field is allowed
/// to be resolved by multiple subgraphs
/// (by default in Federation 2, object fields can be resolved by only one subgraph)
/// <example>
/// @sharable
/// type PreductDimension  {
///   size: String!
///   weight: Number!
/// }
/// </example>
/// </summary>
[AttributeUsage(
    AttributeTargets.Class |
    AttributeTargets.Struct |
    AttributeTargets.Interface |
    AttributeTargets.Property)]
public sealed class ShareableAttribute : DescriptorAttribute
{
    protected internal override void TryConfigure(
        IDescriptorContext context,
        IDescriptor descriptor,
        ICustomAttributeProvider element)
    {
        switch (descriptor)
        {
            case IObjectTypeDescriptor objectTypeDescriptor when element is Type runtimeType:
            {
                objectTypeDescriptor.Shareable();
                break;
            }

            case IObjectFieldDescriptor objectFieldDescriptor when element is MemberInfo:
                objectFieldDescriptor.Shareable();
                break;
        }
    }
}
