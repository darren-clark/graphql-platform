namespace HotChocolate.ApolloFederation;

using System.ComponentModel;
using static ThrowHelper;
using System.Diagnostics.CodeAnalysis;
using Configuration;
using Constants;
using HotChocolate.Properties;
using Language;
using Types.Descriptors.Definitions;

public class ImportType: ScalarType<Import>
{
    public ImportType() : base(WellKnownTypeNames.Import)
    {

    }

    protected override ScalarTypeDefinition CreateDefinition(ITypeDiscoveryContext context)
    {
        return new() { Name = Name, Description = Description, IsPublic = false };
    }

    public override IValueNode ParseResult(object? resultValue) => resultValue switch
    {
        null => NullValueNode.Default,
        string s => new StringValueNode(s),
        Import i => ParseValue(i),
        _ => throw Scalar_CannotParseValue(this, resultValue.GetType())
    };

    public override bool IsInstanceOfType(IValueNode valueSyntax) => valueSyntax switch
    {
        StringValueNode => true,
        ObjectValueNode o => TryParseObject(o, out _),
        _ => false
    };


    public override object? ParseLiteral(IValueNode literal)
        => literal switch
        {
            StringValueNode s => new Import(s.Value),
            ObjectValueNode o => TryParseObject(o, out var i) ? i : throw new SerializationException(
                TypeResourceHelper.Scalar_Cannot_ParseLiteral(Name, literal.GetType()),
                this),
            _ => throw new SerializationException(
                TypeResourceHelper.Scalar_Cannot_ParseLiteral(Name, literal.GetType()),
                this),
        };

    public override IValueNode ParseValue(object? runtimeValue)
        => runtimeValue switch
        {
            null => NullValueNode.Default,
            Import i => i.ToValueNode(),
            _ => throw new SerializationException(TypeResourceHelper.Scalar_Cannot_ParseValue(Name, runtimeValue.GetType()), this)
        };

    private bool TryParseObject(ObjectValueNode node, [NotNullWhen(true)] out Import? import)
    {
        (string? Name, string? As)? args = null;
        import = null;

        static (string? Name, string? As)? ParseField(ObjectFieldNode field, (string? Name, string? As)? args) => field switch
        {
            { Name.Value: WellKnownFieldNames.Name, Value: StringValueNode s } when args?.Name is null  => (s.Value, args?.As),
            { Name.Value: WellKnownFieldNames.As, Value: StringValueNode s } when args?.As is null  => (args?.Name, s.Value),
            _ => null
        };

        foreach(var field in node.Fields)
        {
            args = ParseField(field, args);
            // Short circuit if we get bad arguments.
            if (args == null) return false;
        }

        // Args not null here because of short circuiting above.
        var (name, @as) = args!.Value;

        // Need at least a name.
        if (name == null) return false;

        import = new(name, @as);
        return true;
    }
}
