using System.Linq;
using HotChocolate.ApolloFederation.Constants;
using HotChocolate.Types;
using HotChocolate.Utilities;
using Snapshooter.Xunit;

namespace HotChocolate.ApolloFederation.Directives;

public class SharableDirectiveTests : FederationTypesTestBase
{
    [Fact]
    public void AddSharableDirective_EnsureAvailableInSchema()
    {
        // arrange
        var schema = CreateSchema(b => b.AddDirectiveType<ShareableDirectiveType>());

        // act
        var directive =
            schema.DirectiveTypes.FirstOrDefault(
                t => t.Name.EqualsOrdinal(WellKnownTypeNames.Shareable));

        // assert
        Assert.NotNull(directive);
        Assert.IsType<ShareableDirectiveType>(directive);
        Assert.Equal(WellKnownTypeNames.Shareable, directive!.Name);
        Assert.Empty(directive.Arguments);
        Assert.Equal(DirectiveLocation.Object | DirectiveLocation.FieldDefinition, directive.Locations);
    }

    [Fact]
    public void AnnotateShareableToTypeSchemaFirst()
    {
        // arrange
        Snapshot.FullName();

        var schema = SchemaBuilder.New()
            .AddDocumentFromString(
                @"
                    type Review @key(fields: ""id"") {
                        id: Int!
                        product: Product!
                    }

                    type Product @sharable {
                        name: String!
                    }

                    type Query {
                        someField(a: Int): Product
                    }
                ")
            .AddDirectiveType<ShareableDirectiveType>()
            .Use(_ => _ => default)
            .Create();

        // act
        var testType = schema.GetType<ObjectType>("Product");

        // assert
        Assert.Collection(testType.Directives,
            sharableDirective =>
            {
                Assert.Equal(WellKnownTypeNames.Shareable, sharableDirective.Type.Name);
            });

        schema.ToString().MatchSnapshot();
    }

    [Fact]
    public void AnnotateShareableToFieldSchemaFirst()
    {
        // arrange
        Snapshot.FullName();

        var schema = SchemaBuilder.New()
            .AddDocumentFromString(
                @"
                    type Product {
                        name: String! @sharable
                    }

                    type Query {
                        someField(a: Int): Product
                    }
                ")
            .AddDirectiveType<ShareableDirectiveType>()
            .Use(_ => _ => default)
            .Create();

        // act
        var testType = schema.GetType<ObjectType>("Product");

        // assert
        Assert.Collection(testType.Fields[0].Directives,
            sharableDirective =>
            {
                Assert.Equal(WellKnownTypeNames.Shareable, sharableDirective.Type.Name);
            });

        schema.ToString().MatchSnapshot();
    }

    [Fact]
    public void AnnotateShareableToTypeCodeFirst()
    {
        // arrange
        Snapshot.FullName();

        var schema = SchemaBuilder.New()
            .AddType(
                new ObjectType(o =>
                {
                    o.Name("Product");
                    o.Field("name").Type<StringType>();
                    o.Shareable();
                }))
            .AddQueryType(
                new ObjectType(o =>
                {
                    o.Name("Query");
                    o.Field("someField")
                        .Argument("a", a => a.Type<IntType>())
                        .Type("Product");
                }))
            .AddType<ShareableDirectiveType>()
            .Use(_ => _ => default)
            .Create();

        // act
        var testType = schema.GetType<ObjectType>("Product");

        // assert
        Assert.Collection(testType.Directives,
            sharableDirective =>
            {
                Assert.Equal(WellKnownTypeNames.Shareable, sharableDirective.Type.Name);
            });

        schema.ToString().MatchSnapshot();
    }

    [Fact]
    public void AnnotateShareableToFieldCodeFirst()
    {
        // arrange
        Snapshot.FullName();

        var schema = SchemaBuilder.New()
            .AddType(
                new ObjectType(o =>
                {
                    o.Name("Product");
                    o.Field("name").Type<StringType>().Shareable();
                }))
            .AddQueryType(
                new ObjectType(o =>
                {
                    o.Name("Query");
                    o.Field("someField")
                        .Argument("a", a => a.Type<IntType>())
                        .Type("Product");
                }))
            .AddType<ShareableDirectiveType>()
            .Use(_ => _ => default)
            .Create();

        // act
        var testType = schema.GetType<ObjectType>("Product");

        // assert
        Assert.Collection(testType.Fields[0].Directives,
            sharableDirective =>
            {
                Assert.Equal(WellKnownTypeNames.Shareable, sharableDirective.Type.Name);
            });

        schema.ToString().MatchSnapshot();
    }

    [Fact]
    public void AnnotateShareableToTypeAttributePureCodeFirst()
    {
        // arrange
        Snapshot.FullName();

        var schema = SchemaBuilder.New()
            .AddApolloFederation()
            .AddType<ShareableDirectiveType>()
            .AddQueryType<ShareableTypeQuery>()
            .Create();

        // act
        var testType = schema.GetType<ObjectType>("Product");

        // assert
        Assert.Collection(testType.Directives,
            sharableDirective =>
            {
                Assert.Equal(WellKnownTypeNames.Shareable, sharableDirective.Type.Name);
            });

        schema.ToString().MatchSnapshot();
    }

    [Fact]
    public void AnnotateShareableToFieldAttributePureCodeFirst()
    {
        // arrange
        Snapshot.FullName();

        var schema = SchemaBuilder.New()
            .AddApolloFederation()
            .AddType<ShareableDirectiveType>()
            .AddQueryType<ShareableFieldQuery>()
            .Create();

        // act
        var testType = schema.GetType<ObjectType>("Product");

        // assert
        Assert.Collection(testType.Fields[0].Directives,
            sharableDirective =>
            {
                Assert.Equal(WellKnownTypeNames.Shareable, sharableDirective.Type.Name);
            });

        schema.ToString().MatchSnapshot();
    }

    public class ShareableTypeQuery
    {
        public ShareableTypeReview SomeField(int id) => default!;
    }

    public class ShareableFieldQuery
    {
        public ShareableFieldReview SomeField(int id) => default!;
    }

    public class ShareableTypeReview
    {
        [Key]
        public int Id { get; set; }

        [Provides("name")]
        public ShareableTypeProduct  Product { get; set; } = default!;
    }

    public class ShareableFieldReview
    {
        [Key]
        public int Id { get; set; }

        [Provides("name")]
        public ShareableFieldProduct Product { get; set; } = default!;
    }

    [Shareable]
    public class ShareableTypeProduct
    {
        public string Name { get; set; } = default!;
    }

    public class ShareableFieldProduct
    {
        [Shareable]
        public string Name { get; set; } = default!;
    }
}
