using System.Text.Json;
using System.Text.Json.Nodes;
using Tpcly.Json.Mapper.Abstractions;

namespace Tpcly.Json.Mapper.Transformers;

public class StringValueTransformer(StringValueTransformer.Action action, string? input = null) : ITransformer
{
    public enum Action
    {
        Concat,
        Replace,
        ToUpper,
        ToLower
    }

    public bool CanTransform(JsonNode node) => node is JsonValue && node.GetValueKind() == JsonValueKind.String;

    public void Apply(JsonNode node)
    {
        var value = node.AsValue().GetValue<string>();

        switch (action)
        {
            case Action.Concat:
                value += input;
                break;
            case Action.Replace:
                value = input;
                break;
            case Action.ToUpper:
                value = value.ToUpper();
                break;
            case Action.ToLower:
                value = value.ToLower();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }

        node.ReplaceWith(value);
    }
}