#nullable enable
using HotChocolate.Language;

namespace HotChocolate.Types.Descriptors.Definitions;

/// <summary>
/// Defines the properties of a GraphQL scalar type.
/// </summary>
public sealed class ScalarTypeDefinition : TypeDefinitionBase<ScalarTypeDefinitionNode>
{
    /// <summary>
    /// Defines if this scalar is visible when scalar introspection is enabled.
    /// </summary>
    public bool IsPublic { get; set; } = true;
}
