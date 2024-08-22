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
        var nearestRootNode = node.Parent!;
        while (nearestRootNode.Parent is JsonObject)
        {
            nearestRootNode = nearestRootNode.Parent;
        }

        var nearestRootObject = nearestRootNode.AsObject();
        
        var nodeClone = JsonNode.Parse(node.ToJsonString());
        var (destinationNode, propertyName) = nearestRootObject.TraverseOrCreate(DestinationPath, PathSeparator);

        nearestRootObject.Remove(node.GetPropertyName());
        destinationNode!.Add(propertyName, nodeClone);
    }
}