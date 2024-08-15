using System.Text.Json.Nodes;
using Tpcly.Json.Mapper.Abstractions;
using Tpcly.Json.Mapper.Transformers;

namespace Tpcly.Json.Mapper;

public class JsonMapperOptions
{
    private static readonly ITransformer[] DefaultConverters =
    {
        new FilterTransformer(),
        new TransposeTransformer(),
        new TypeTransformer()
    };

    public IEnumerable<ITransformer> CustomTransformers { get; } = new List<ITransformer>();

    public IEnumerable<ITransformer> Transformers => DefaultConverters.Concat(CustomTransformers);

    public ITransformer GetTransformer(JsonNode node, ITransformRule rule)
    {
        var transformers = Transformers.FirstOrDefault(t => t.CanTransform(node, rule));

        return transformers ?? throw new InvalidOperationException($"Could not find transformer for rule {rule}");
    }
}