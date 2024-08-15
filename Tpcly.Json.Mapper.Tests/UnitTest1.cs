using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using Json.Schema.Generation;
using Tpcly.Json.Mapper.Transformers;

namespace Tpcly.Json.Mapper.Tests;

public class Tests
{
    private static readonly JsonMapperOptions DefaultMapperOptions = new();

    private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        TypeInfoResolver = new JsonMappingTransformRuleTypeResolver(DefaultMapperOptions)
    };

    public record TestSource(
        string TestString,
        int TestNumber
    );

    public record TestDestination(
        string TestStringOne,
        string TestStringTwo,
        TestDestinationChild TestDestinationChild
    );

    public record TestDestinationChild(
        string TestNumberString
    );

    [Test]
    public void Test1()
    {
        var sourceSchema = new JsonSchemaBuilder().FromType<TestSource>().Build();
        var destSchema = new JsonSchemaBuilder().FromType<TestDestination>().Build();
        var mapping = new JsonMapping(sourceSchema, destSchema)
        {
            Rules =
            {
                new TransposeTransformer.Rule
                {
                    Properties = new Dictionary<string, string>
                    {
                        { "TestString", "TestStringOne" },
                        { "TestNumber", "TestDestinationChild.TestNumberString" }
                    }
                },
                new TypeTransformer.Rule
                {
                    TypeMappings = new Dictionary<string, SchemaValueType>
                    {
                        { "TestDestinationChild.TestNumberString", SchemaValueType.String }
                    }
                },
            }
        };
        
        var json = JsonSerializer.Serialize(mapping, DefaultSerializerOptions);
        Console.WriteLine(json);

        var deserializedMapping = JsonSerializer.Deserialize<JsonMapping>(json, DefaultSerializerOptions);
    }

    [Test]
    public void Test2()
    {
        var sourceNode = JsonNode.Parse("""{"TestString":"test_str","TestNumber":123}""")!;
        Console.WriteLine($"Source element: {sourceNode.ToJsonString()}");
        
        var mapping = JsonSerializer.Deserialize<JsonMapping>(
            """
            {
              "sourceSchema": {
                "type": "object",
                "properties": {
                  "TestString": {
                    "type": "string"
                  },
                  "TestNumber": {
                    "type": "integer"
                  }
                }
              },
              "destinationSchema": {
                "type": "object",
                "properties": {
                  "TestStringOne": {
                    "type": "string"
                  },
                  "TestStringTwo": {
                    "type": "string"
                  },
                  "TestDestinationChild": {
                    "type": "object",
                    "properties": {
                      "TestNumberString": {
                        "type": "string"
                      }
                    }
                  }
                }
              },
              "rules": [
                {
                  "$type": "Tpcly.Json.Mapper.Transformers.TransposeTransformer+Rule",
                  "properties": {
                    "TestString": "TestStringOne",
                    "TestNumber": "TestDestinationChild.TestNumberString"
                  },
                  "pathSeparator": "."
                },
                {
                  "$type": "Tpcly.Json.Mapper.Transformers.TypeTransformer+Rule",
                  "typeMappings": {
                    "TestDestinationChild.TestNumberString": "string"
                  }
                }
              ]
            }
            """,
            DefaultSerializerOptions
        );
        
        var map = new JsonMapper();
        var destinationElement = map.Map(sourceNode, mapping, DefaultMapperOptions);

        Console.WriteLine($"Destination element: {destinationElement.ToJsonString()}");
    }
}