using System.Reflection;
using HotChocolate.Types.Descriptors;

namespace HotChocolate.ApolloFederation;

/// <summary>
/// The @override directive is used to indicate that an object field is now resolved by this subgraph
/// instead of another subgraph where it's also defined.
/// This enables you to migrate a field from one subgraph to another
/// <example>
/// type Product @key(fields: "upc") {
///   upc: UPC!
///   quantity: Int @override(from: "Products")
/// }
/// </example>
/// </summary>
public class OverrideAttribute: ObjectFieldDescriptorAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="OverrideAttribute"/>.
    /// </summary>
    /// <param name="from">
    /// The subgraph to override with this subgraph resolver for this field.
    /// </param>
    public OverrideAttribute(string from)
    {
        From = from;
    }

    /// <summary>
    /// Gets the name of the subgraph to override with this subgraph resolver for this field.
    /// </summary>
    public string From { get; }
    
    protected override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
    {
        descriptor.Override(From);
    }
}
