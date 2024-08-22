using System.Text.Json;
using System.Text.Json.Nodes;
using Tpcly.Json.Mapper.Abstractions;

namespace Tpcly.Json.Mapper;

public static class JsonMapper
{
    public static JsonNode Map(JsonNode sourceNode, ITransformer transformer)
    {
        transformer.Apply(sourceNode);

        return sourceNode;
    }

    public static T? Map<T>(JsonNode sourceNode, ITransformer transformer)
    {
        return Map(sourceNode, transformer).Deserialize<T>();
    }
}