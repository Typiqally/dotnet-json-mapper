using System.Text.Json.Nodes;

namespace Tpcly.Json.Mapper;

public static class JsonElementExtensions
{
    public static (JsonNode? node, string propertyName) Traverse(this JsonNode node, string path, string pathSeparator = ".")
    {
        return Traverse(node, path.Split(pathSeparator));
    }

    public static (JsonNode? node, string propertyName) Traverse(this JsonNode node, params string[] pathParts)
    {
        var current = node;

        for (var i = 0; i < pathParts.Length - 1; i++)
        {
            current = current?[pathParts[i]];
        }

        return (current, pathParts[^1]);
    }

    public static (JsonObject?, string) TraverseOrCreate(this JsonObject node, string path, string pathSeparator = ".")
    {
        return TraverseOrCreate(node, path.Split(pathSeparator));
    }

    public static (JsonObject?, string) TraverseOrCreate(this JsonObject obj, params string[] pathParts)
    {
        var current = obj;

        for (var i = 0; i < pathParts.Length - 1; i++)
        {
            var part = pathParts[i];

            if (current != null && !current.ContainsKey(part))
            {
                current[part] = new JsonObject();
            }

            current = current?[part]?.AsObject();
        }

        return (current, pathParts[^1]);
    }
}