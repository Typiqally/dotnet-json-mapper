using System.Text.Json.Nodes;
using Tpcly.Json.Mapper.Abstractions;

namespace Tpcly.Json.Mapper.Transformers;

public class NodePathTransformer(string destinationPath) : ITransformer
{
    public string DestinationPath { get; set; } = destinationPath;

    public string PathSeparator { get; set; } = ".";

    public bool CanTransform(JsonNode node) => node.Parent is JsonObject;

    public void Apply(JsonNode node)
    {
        var parent = node.Parent!.AsObject();
        var nodeClone = JsonNode.Parse(node.ToJsonString());
        var (destinationNode, propertyName) = parent.AsObject().TraverseOrCreate(DestinationPath, PathSeparator);

        parent.Remove(node.GetPropertyName());
        destinationNode!.Add(propertyName, nodeClone);
    }
}