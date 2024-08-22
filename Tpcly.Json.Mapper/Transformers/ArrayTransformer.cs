using System.Text.Json.Nodes;
using Tpcly.Json.Mapper.Abstractions;

namespace Tpcly.Json.Mapper.Transformers;

public class ArrayTransformer(ITransformer transformer) : ITransformer
{
    public ITransformer Transformer { get; set; } = transformer;

    public bool CanTransform(JsonNode node) => node is JsonArray;

    public void Apply(JsonNode node)
    {
        var array = node.AsArray();

        foreach (var arrayNode in array)
        {
            if (arrayNode != null)
            {
                Transformer.Apply(arrayNode);
            }
        }
    }
}