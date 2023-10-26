using System.Collections.Generic;
using HotChocolate.Types.Descriptors.Definitions;
using static HotChocolate.ApolloFederation.Constants.WellKnownContextData;
using static HotChocolate.ApolloFederation.ThrowHelper;

namespace HotChocolate.ApolloFederation.Extensions;

public static partial class DescriptorExtensions
{

    public static IDescriptor<T> RequiresApolloVersion<T>(this IDescriptor<T> descriptor, FederationVersion version, string feature) where T : DefinitionBase
    {
        if (version == FederationVersion.Minimum)
        {
            throw Minimum_Version_Invalid();
        }

        descriptor.Extend().OnBeforeCreate((c, d) =>
        {
            if (!(c.ContextData.TryGetValue(VersionFeatures, out var versionFeaturesContext) &&
                  versionFeaturesContext is Dictionary<FederationVersion, HashSet<string>> versionFeatures))
            {
                c.ContextData[VersionFeatures] = versionFeatures = new Dictionary<FederationVersion, HashSet<string>>();
            }

            if (!versionFeatures.TryGetValue(version, out var features))
            {
                versionFeatures[version] = features = new HashSet<string>();
            }

            features.Add(feature);
        });

        return descriptor;
    }
}
