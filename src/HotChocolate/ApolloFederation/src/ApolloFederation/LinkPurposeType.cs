using static HotChocolate.ApolloFederation.Constants.WellKnownTypeNames;

namespace HotChocolate.ApolloFederation;

public class LinkPurposeType: EnumType<LinkPurpose>
{
    protected override void Configure(IEnumTypeDescriptor<LinkPurpose> descriptor)
    {
        descriptor.Name(Purpose);
    }
}
