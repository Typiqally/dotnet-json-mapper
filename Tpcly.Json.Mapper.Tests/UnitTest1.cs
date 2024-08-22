using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.Schema;
using Tpcly.Json.Mapper.Abstractions;
using Tpcly.Json.Mapper.Transformers;

namespace Tpcly.Json.Mapper.Tests;

public class Tests
{
    class CustomValueTransformer : ITransformer
    {
        public bool CanTransform(JsonNode node) => node is JsonValue;

        public void Apply(JsonNode node)
        {
            var updatedValue = node.GetValue<int>() * 1000;
            node.ReplaceWith(updatedValue);
        }
    }

    public record TestSource(
        IEnumerable<TestSourceChild> TestStrings,
        int TestNumber
    );

    public record TestSourceChild(
        string Aaa
    );


    public record TestDestination(
        string TestStringOne,
        string TestStringTwo,
        TestDestinationChildTwo ChildTwo
    );

    public record TestDestinationChild(
        string TestNumberString
    );

    public record TestDestinationChildTwo(
        IEnumerable<TestDestinationChild> TestDestinationChildren
    );

    [Test]
    public void Test1()
    {
        var source = new TestSource(new[] { new TestSourceChild("test_1"), new TestSourceChild("test_2") }, 5);

        var mapping = new ObjectTransformer(new List<KeyValuePair<string, IList<ITransformer>>>
        {
            new("TestNumber",
                new List<ITransformer>
                {
                    new CustomValueTransformer(),
                    new ValueKindTransformer(JsonValueKind.String),
                    new StringValueTransformer(StringValueTransformer.Action.Concat, "_concat"),
                    new NodePathTransformer("TestStringTwo")
                }),
            new("TestStrings", new List<ITransformer>
            {
                new ArrayTransformer(
                    new ObjectTransformer(new List<KeyValuePair<string, IList<ITransformer>>>
                    {
                        new("Aaa", new List<ITransformer>
                        {
                            new StringValueTransformer(StringValueTransformer.Action.Concat, "_concat"),
                            new NodePathTransformer("TestNumberString")
                        })
                    })
                ),
                new NodePathTransformer("ChildTwo.TestDestinationChildren")
            }),
        });

        var serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            TypeInfoResolver = new JsonMappingTransformerTypeResolver(new List<Type> { typeof(CustomValueTransformer) })
        };

        Console.WriteLine($"Using mapping: {JsonSerializer.Serialize(mapping, serializerOptions)}");

        var sourceNode = JsonSerializer.SerializeToNode(source, serializerOptions);
        Console.WriteLine($"Source element: {sourceNode.ToJsonString()}");

        var destinationElement = JsonMapper.Map(sourceNode, mapping);
        Console.WriteLine($"Destination element: {destinationElement.ToJsonString(serializerOptions)}");
        Console.WriteLine($"Destination: {destinationElement.Deserialize<TestDestination>()}");
    }
}