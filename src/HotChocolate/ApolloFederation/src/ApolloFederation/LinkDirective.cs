using System.Collections.Generic;
using HotChocolate.Language;
using HotChocolate.Utilities;

namespace HotChocolate.ApolloFederation;

public sealed class LinkDirective
{
    public LinkDirective(string url, string? @as = null, IReadOnlyList<LinkImport>? import = null, LinkPurpose? purpose = null)
    {
        Url = url;
        As = @as;
        Import = import;
        Purpose = purpose;
    }

    public string Url { get; }
    public string? As { get; }
    public IReadOnlyList<LinkImport>? Import { get; }
    public LinkPurpose? Purpose { get;}
}