using System.Reflection;
using HotChocolate.ApolloFederation.Descriptors;
using HotChocolate.Types.Descriptors;
using static HotChocolate.ApolloFederation.ThrowHelper;

namespace HotChocolate.ApolloFederation;

/// <summary>
/// The reference resolver enables your gateway's query planner to resolve a particular
/// entity by whatever unique identifier your other subgraphs use to reference it.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method,
    AllowMultiple = true)]
public class ReferenceResolverAttribute : DescriptorAttribute
{
    public string? EntityResolver { get; set; }

    public Type? EntityResolverType { get; set; }

    protected internal override void TryConfigure(
        IDescriptorContext context,
        IDescriptor descriptor,
        ICustomAttributeProvider element)
    {

        if (descriptor is IObjectTypeDescriptor objectTypeDescriptor)
        {
            switch (element)
            {
                case Type type:
                    OnConfigure(new EntityResolverDescriptor<object>(objectTypeDescriptor), type);
                    break;

                case MethodInfo method:
                    OnConfigure(new EntityResolverDescriptor<object>(objectTypeDescriptor), method);
                    break;
            }
        }

        if (descriptor is IInterfaceTypeDescriptor interfaceTypeDescriptor)
        {
            switch (element)
            {
                case Type type:
                    OnConfigure(new InterfaceEntityResolverDescriptor<object>(interfaceTypeDescriptor), type);
                    break;

                case MethodInfo method:
                    OnConfigure(new InterfaceEntityResolverDescriptor<object>(interfaceTypeDescriptor), method);
                    break;
            }
        }
    }

    private void OnConfigure<TDescriptor>(IEntityResolverDescriptor<TDescriptor,object> entityResolverDescriptor, Type type)
    {
        if (EntityResolverType is not null)
        {
            if (EntityResolver is not null)
            {
                var method = EntityResolverType.GetMethod(EntityResolver);

                if (method is null)
                {
                    throw ReferenceResolverAttribute_EntityResolverNotFound(
                        EntityResolverType,
                        EntityResolver);
                }

                entityResolverDescriptor.ResolveReferenceWith(method);
            }
            else
            {
                entityResolverDescriptor.ResolveReferenceWith(EntityResolverType);
            }
        }
        else if (EntityResolver is not null)
        {
            var method = type.GetMethod(EntityResolver);

            if (method is null)
            {
                throw ReferenceResolverAttribute_EntityResolverNotFound(
                    type,
                    EntityResolver);
            }

            entityResolverDescriptor.ResolveReferenceWith(method);
        }
        else
        {
            entityResolverDescriptor.ResolveReferenceWith(type);
        }
    }

    private void OnConfigure<TDescriptor>(IEntityResolverDescriptor<TDescriptor,object> entityResolverDescriptor, MethodInfo method)
    {
        entityResolverDescriptor.ResolveReferenceWith(method);
    }
}
