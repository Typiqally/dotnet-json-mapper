using System.Text.Json.Nodes;
using Tpcly.Json.Mapper.Abstractions;

namespace Tpcly.Json.Mapper.Transformers;

public class FilterTransformer : Transformer<FilterTransformer.Rule>
{
    public override bool CanTransform(JsonNode node, ITransformRule rule)
    {
        return rule is Rule && node is JsonArray;
    }

    public override JsonNode Apply(JsonNode node, Rule rule)
    {
        throw new NotImplementedException();
    }

    public record Rule : ITransformRule
    {
    }
}