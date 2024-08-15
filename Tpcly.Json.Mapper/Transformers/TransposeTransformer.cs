using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Tpcly.Json.Mapper.Abstractions;

namespace Tpcly.Json.Mapper.Transformers;

public class TransposeTransformer : Transformer<TransposeTransformer.Rule>
{
    public override bool CanTransform(JsonNode node, ITransformRule rule)
    {
        return rule is Rule && node is JsonObject;
    }

    public override JsonNode Apply(JsonNode node, Rule rule)
    {
        var destObj = new JsonObject();

        // TODO: Could probably use some performance optimizations
        foreach (var (sourcePath, destPath) in rule.Properties)
        {
            var (currentSourceNode, propertyName) = node.Traverse(sourcePath, rule.PathSeparator);
            var currentSourceObj = currentSourceNode.AsObject();

            // Move the property
            if (!currentSourceObj.ContainsKey(propertyName)) continue;

            var propertyNode = currentSourceObj[propertyName];

            // Clone the node to remove it from the current parent
            var newPropertyNode = JsonNode.Parse(propertyNode.ToJsonString());

            // Traverse or create the path in the destination JSON
            var (currentDestNode, newPropertyName) = destObj.TraverseOrCreate(destPath, rule.PathSeparator);
            var currentDestObj = currentDestNode.AsObject();

            // Get the final property name in the destination path
            currentDestObj[newPropertyName] = newPropertyNode;
            currentSourceObj.Remove(propertyName);
        }

        return destObj;
    }

    public record Rule : ITransformRule
    {
        public IDictionary<string, string> Properties { get; init; } = new Dictionary<string, string>();

        public string PathSeparator { get; set; } = ".";
    }
}