using static HotChocolate.ApolloFederation.FederationVersion;
using static HotChocolate.ApolloFederation.Constants.WellKnownLinkUrls;

namespace HotChocolate.ApolloFederation;

public enum FederationVersion
{
    Minimum,
    v1,
    v2_0
}

internal static class FederationVersionExtensions
{
    public static string ToLinkUrl(this FederationVersion version) => version switch {
        v2_0 => string.Format(Federation, "v2.0"),
        Minimum => throw new ArgumentException("Expected specific Apollo federation version", nameof(version)),
        v1 => throw new ArgumentException("Apollo federation v1 does not use @link directive", nameof(version)),
        _ => throw new ArgumentException($"Unexpected Apollo federation version {version}", nameof(version))
    };
}
