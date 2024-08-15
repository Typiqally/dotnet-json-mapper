using System.Text.Json.Nodes;

namespace Tpcly.Json.Mapper;

public class JsonMapper
{
    private readonly JsonMapperOptions _defaultOptions = new();

    public JsonNode Map(JsonNode sourceNode, JsonMapping mapping, JsonMapperOptions? options = null)
    {
        // If no options are provided, use the default instance
        options ??= _defaultOptions;

        var sourceEvalResults = mapping.SourceSchema.Evaluate(sourceNode);
        if (!sourceEvalResults.IsValid)
        {
            throw new ArgumentException("Source element does not match source schema");
        }

        var activeNode = sourceNode;

        foreach (var rule in mapping.Rules)
        {
            var transformer = options.GetTransformer(activeNode, rule);
            activeNode = transformer.Apply(activeNode, rule);
        }

        var destEvalResults = mapping.DestinationSchema.Evaluate(activeNode);
        if (!destEvalResults.IsValid)
        {
            throw new ArgumentException("Destination element does not match destination schema, something may have gone wrong during mapping");
        }

        return activeNode;
    }

    private void SetToken(JsonObject jsonObject, string path, JsonNode value)
    {
        var parts = path.Split('.');
        var current = jsonObject;

        for (var i = 0; i < parts.Length - 1; i++)
        {
            if (!current.ContainsKey(parts[i]))
            {
                current[parts[i]] = new JsonObject();
            }

            current = current[parts[i]] as JsonObject;
        }

        current[parts[^1]] = value;
    }
}