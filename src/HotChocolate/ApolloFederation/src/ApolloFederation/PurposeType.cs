using HotChocolate.ApolloFederation.Constants;
using HotChocolate.Language;
using static HotChocolate.ApolloFederation.ThrowHelper;

namespace HotChocolate.ApolloFederation;

public class PurposeType: ScalarType<string, EnumValueNode>
{
    public PurposeType() : base(WellKnownTypeNames.Purpose)
    {
    }

    public override IValueNode ParseResult(object? resultValue) => resultValue switch
    {
        null => NullValueNode.Default,
        string s => new EnumValueNode(s),
        _ => throw Scalar_CannotParseValue(this, resultValue.GetType())
    };

    protected override string ParseLiteral(EnumValueNode valueSyntax) => valueSyntax.Value;

    protected override EnumValueNode ParseValue(string runtimeValue) => new(runtimeValue);
}
