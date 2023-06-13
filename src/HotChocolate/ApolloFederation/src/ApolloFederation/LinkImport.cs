using HotChocolate.ApolloFederation.Constants;
using HotChocolate.Language;

namespace HotChocolate.ApolloFederation;

public class LinkImport
{
    public LinkImport(string name, string? @as = null)
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

    public static implicit operator LinkImport(string name) => new(name);
}
