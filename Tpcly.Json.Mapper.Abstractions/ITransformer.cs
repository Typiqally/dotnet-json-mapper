using System.Text.Json.Nodes;

namespace Tpcly.Json.Mapper.Abstractions;

public interface ITransformer
{
    public bool CanTransform(JsonNode node);

    public void Apply(JsonNode node);
}