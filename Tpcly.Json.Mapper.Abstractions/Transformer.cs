using System.Text.Json.Nodes;

namespace Tpcly.Json.Mapper.Abstractions;

public abstract class Transformer : ITransformer
{
    public string Name => GetType().Name;

    public abstract bool CanTransform(JsonNode element, ITransformRule rule);

    public abstract JsonNode Apply(JsonNode node, ITransformRule rule);
}

public abstract class Transformer<T> : Transformer where T : class
{
    public override JsonNode Apply(JsonNode node, ITransformRule rule)
    {
        return Apply(node, rule as T ?? throw new InvalidOperationException("Invalid rule provided to transformer"));
    }

    public abstract JsonNode Apply(JsonNode node, T rule);
}