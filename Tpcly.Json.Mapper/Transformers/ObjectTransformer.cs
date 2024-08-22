using System.Text.Json.Nodes;
using Json.Schema;
using Tpcly.Json.Mapper.Abstractions;

namespace Tpcly.Json.Mapper.Transformers;

public class ObjectTransformer(IList<KeyValuePair<string, IList<ITransformer>>> propertyTransformers) : ITransformer
{
    public IList<KeyValuePair<string, IList<ITransformer>>> Transformers { get; set; } = propertyTransformers;

    public JsonSchema? SourceSchema { get; set; }

    public JsonSchema? DestinationSchema { get; set; }

    public bool CanTransform(JsonNode node) => node is JsonObject;

    public void Apply(JsonNode node)
    {
        if (SourceSchema != null)
        {
            var sourceEvalResults = SourceSchema.Evaluate(node);
            if (!sourceEvalResults.IsValid)
            {
                throw new ArgumentException("Source element does not match source schema");
            }
        }

        foreach (var (propertyName, transformers) in Transformers)
        {
            foreach (var transformer in transformers)
            {
                var propertyNode = node[propertyName];
                if (propertyNode == null)
                {
                    throw new ArgumentException($"Property \"{propertyName}\" could not be found in source node");
                }
                
                if (!transformer.CanTransform(propertyNode))
                {
                    throw new ArgumentException($"Cannot transform \"{propertyName}\" using \"{transformer.GetType().Name}\", pre-condition failed");
                }
                
                transformer.Apply(propertyNode);
            }
        }

        if (DestinationSchema != null)
        {
            var destEvalResults = DestinationSchema.Evaluate(node);
            if (!destEvalResults.IsValid)
            {
                throw new ArgumentException("Destination element does not match destination schema, something may have gone wrong during mapping");
            }
        }
    }
}