using System.Text.Json.Nodes;

namespace Tpcly.Json.Mapper.Abstractions;

public interface ITransformer
{
    public string Name { get; }
    public bool CanTransform(JsonNode node, ITransformRule rule);

    public JsonNode Apply(JsonNode node, ITransformRule rule);
}

public interface ITransformer<in T> : ITransformer
{
    public JsonNode Apply(JsonNode node, T rule);
}