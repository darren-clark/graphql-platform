using System.Collections.Generic;
using System.Linq;
using HotChocolate.ApolloFederation;
using HotChocolate.ApolloFederation.Constants;
using HotChocolate.ApolloFederation.Descriptors;
using HotChocolate.Language;
using static HotChocolate.ApolloFederation.Properties.FederationResources;
using static HotChocolate.ApolloFederation.Constants.WellKnownContextData;

namespace HotChocolate.Types;

using HotChocolate.Types.Descriptors.Definitions;
using HotChocolate.Types.Helpers;

/// <summary>
/// Provides extensions for type system descriptors.
/// </summary>
public static partial class ApolloFederationDescriptorExtensions
{
    /// <summary>
    /// Adds the @external directive which is used to mark a field as owned by another service.
    /// This allows service A to use fields from service B while also knowing at runtime
    /// the types of that field.
    ///
    /// <example>
    /// # extended from the Users service
    /// extend type User @key(fields: "email") {
    ///   email: String @external
    ///   reviews: [Review]
    /// }
    /// </example>
    /// </summary>
    /// <param name="descriptor">
    /// The object field descriptor on which this directive shall be annotated.
    /// </param>
    /// <returns>
    /// Returns the object field descriptor.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="descriptor"/> is <c>null</c>.
    /// </exception>
    public static IObjectFieldDescriptor External(
        this IObjectFieldDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        return descriptor.Directive(WellKnownTypeNames.External);
    }

    /// <summary>
    /// Adds the @key directive which is used to indicate a combination of fields that
    /// can be used to uniquely identify and fetch an object or interface.
    /// <example>
    /// type Product @key(fields: "upc") {
    ///   upc: UPC!
    ///   name: String
    /// }
    /// </example>
    /// </summary>
    /// <param name="descriptor">
    /// The object type descriptor on which this directive shall be annotated.
    /// </param>
    /// <param name="fieldSet">
    /// The field set that describes the key.
    /// Grammatically, a field set is a selection set minus the braces.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="descriptor"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="fieldSet"/> is <c>null</c> or <see cref="string.Empty"/>.
    /// </exception>
    public static IEntityResolverDescriptor Key(
        this IObjectTypeDescriptor descriptor,
        string fieldSet)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        if (string.IsNullOrEmpty(fieldSet))
        {
            throw new ArgumentException(
                FieldDescriptorExtensions_Key_FieldSet_CannotBeNullOrEmpty,
                nameof(fieldSet));
        }

        descriptor.Directive(
            WellKnownTypeNames.Key,
            new ArgumentNode(
                WellKnownArgumentNames.Fields,
                new StringValueNode(fieldSet)));

        return new EntityResolverDescriptor<object>(descriptor);
    }

    /// <summary>
    /// Adds the @requires directive which is used to annotate the required
    /// input fieldset from a base type for a resolver. It is used to develop
    /// a query plan where the required fields may not be needed by the client, but the
    /// service may need additional information from other services.
    ///
    /// <example>
    /// # extended from the Users service
    /// extend type User @key(fields: "id") {
    ///   id: ID! @external
    ///   email: String @external
    ///   reviews: [Review] @requires(fields: "email")
    /// }
    /// </example>
    /// </summary>
    /// <param name="descriptor">
    /// The object field descriptor on which this directive shall be annotated.
    /// </param>
    /// <param name="fieldSet">
    /// The <paramref name="fieldSet"/> describes which fields may
    /// not be needed by the client, but are required by
    /// this service as additional information from other services.
    /// Grammatically, a field set is a selection set minus the braces.
    /// </param>
    /// <returns>
    /// Returns the object field descriptor.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="descriptor"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="fieldSet"/> is <c>null</c> or <see cref="string.Empty"/>.
    /// </exception>
    public static IObjectFieldDescriptor Requires(
        this IObjectFieldDescriptor descriptor,
        string fieldSet)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        if (string.IsNullOrEmpty(fieldSet))
        {
            throw new ArgumentException(
                FieldDescriptorExtensions_Requires_FieldSet_CannotBeNullOrEmpty,
                nameof(fieldSet));
        }

        return descriptor.Directive(
            WellKnownTypeNames.Requires,
            new ArgumentNode(
                WellKnownArgumentNames.Fields,
                new StringValueNode(fieldSet)));
    }

    /// <summary>
    /// Adds the @provides directive which is used to annotate the expected returned
    /// fieldset from a field on a base type that is guaranteed to be selectable by
    /// the gateway.
    ///
    /// <example>
    /// # extended from the Users service
    /// type Review @key(fields: "id") {
    ///     product: Product @provides(fields: "name")
    /// }
    ///
    /// extend type Product @key(fields: "upc") {
    ///     upc: String @external
    ///     name: String @external
    /// }
    /// </example>
    /// </summary>
    /// <param name="descriptor">
    /// The object field descriptor on which this directive shall be annotated.
    /// </param>
    /// <param name="fieldSet">
    /// The fields that are guaranteed to be selectable by the gateway.
    /// Grammatically, a field set is a selection set minus the braces.
    /// </param>
    /// <returns>
    /// Returns the object field descriptor.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="descriptor"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="fieldSet"/> is <c>null</c> or <see cref="string.Empty"/>.
    /// </exception>
    public static IObjectFieldDescriptor Provides(
        this IObjectFieldDescriptor descriptor,
        string fieldSet)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        if (string.IsNullOrEmpty(fieldSet))
        {
            throw new ArgumentException(
                FieldDescriptorExtensions_Provides_FieldSet_CannotBeNullOrEmpty,
                nameof(fieldSet));
        }

        return descriptor.Directive(
            WellKnownTypeNames.Provides,
            new ArgumentNode(
                WellKnownArgumentNames.Fields,
                new StringValueNode(fieldSet)));
    }

    /// <summary>
    /// Mark the type as an extension
    /// of a type that is defined by another service when
    /// using apollo federation.
    /// </summary>
    public static IObjectTypeDescriptor ExtendServiceType(
        this IObjectTypeDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        descriptor
            .Extend()
            .OnBeforeCreate(d => d.ContextData[ExtendMarker] = true);

        return descriptor;
    }

    public static IObjectTypeDescriptor Shareable(this IObjectTypeDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        descriptor.Directive(WellKnownTypeNames.Shareable);

        return descriptor;
    }

    public static IObjectFieldDescriptor Shareable(this IObjectFieldDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        descriptor.Directive(WellKnownTypeNames.Shareable);

        return descriptor;
    }

    public static IObjectFieldDescriptor Override(this IObjectFieldDescriptor descriptor, string from)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        if (string.IsNullOrEmpty(from))
        {
            throw new ArgumentException(
                FieldDescriptorExtensions_Override_From_CannotBeNullOrEmpty,
                nameof(from));
        }

        descriptor.Directive(WellKnownTypeNames.Override,
            new ArgumentNode(
                WellKnownArgumentNames.From,
                new StringValueNode(from)));


        return descriptor;
    }

    public static IObjectFieldDescriptor Inaccessible(this IObjectFieldDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        descriptor.Directive(WellKnownTypeNames.Inaccessible);

        return descriptor;
    }


    public static IObjectTypeDescriptor Inaccessible(this IObjectTypeDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        descriptor.Directive(WellKnownTypeNames.Inaccessible);

        return descriptor;
    }

    public static IInterfaceTypeDescriptor Inaccessible(this IInterfaceTypeDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        descriptor.Directive(WellKnownTypeNames.Inaccessible);

        return descriptor;
    }

    public static IUnionTypeDescriptor Inaccessible(this IUnionTypeDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        descriptor.Directive(WellKnownTypeNames.Inaccessible);

        return descriptor;
    }

    public static IArgumentDescriptor Inaccessible(this IArgumentDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        descriptor.Directive(WellKnownTypeNames.Inaccessible);

        return descriptor;
    }

    public static IEnumTypeDescriptor Inaccessible(this IEnumTypeDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        descriptor.Directive(WellKnownTypeNames.Inaccessible);

        return descriptor;
    }

    public static IEnumValueDescriptor Inaccessible(this IEnumValueDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        descriptor.Directive(WellKnownTypeNames.Inaccessible);

        return descriptor;
    }

    public static IInputObjectTypeDescriptor Inaccessible(this IInputObjectTypeDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        descriptor.Directive(WellKnownTypeNames.Inaccessible);

        return descriptor;
    }

    public static IInputFieldDescriptor Inaccessible(this IInputFieldDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        descriptor.Directive(WellKnownTypeNames.Inaccessible);

        return descriptor;
    }

    public static ISchemaTypeDescriptor Link(this ISchemaTypeDescriptor descriptor, string url,
        IReadOnlyList<LinkImport>? import = null, string? @as = null, string? @purpose = null)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        var urlNode = new StringValueNode(url);
        var wut = urlNode.ToString();
        descriptor.Directive(WellKnownTypeNames.Link,
            new[]
            {
                new ArgumentNode(WellKnownArgumentNames.Url, new StringValueNode(url)),
                new ArgumentNode(WellKnownArgumentNames.Import,
                    import == null
                        ? NullValueNode.Default
                        : new ListValueNode(import.Select(i => i.ToValueNode()).ToArray())),
                new ArgumentNode(WellKnownArgumentNames.As,
                    @as == null ? NullValueNode.Default : new StringValueNode(@as)),
                new ArgumentNode(WellKnownArgumentNames.For,
                    @purpose == null ? NullValueNode.Default : new StringValueNode(@purpose))
            });

        return descriptor;
    }
}
