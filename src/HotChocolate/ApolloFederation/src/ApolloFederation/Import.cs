using HotChocolate.ApolloFederation.Constants;
using HotChocolate.Language;

namespace HotChocolate.ApolloFederation;

public class Import
{
    public Import(string name) : this(name, null)
    {
    }

    public Import(string name, string? @as)
    {
        Name = name;
        As = @as;
    }

    public string Name { get; }
    public string? As { get; }

    public IValueNode ToValueNode() => this switch
    {
        { As: null } => new StringValueNode(Name),
        _ => new ObjectValueNode(new[]
        {
            new ObjectFieldNode(WellKnownFieldNames.Name, Name),
            new ObjectFieldNode(WellKnownFieldNames.As, As)
        })
    };

    public static implicit operator Import(string name) => new(name);
}
