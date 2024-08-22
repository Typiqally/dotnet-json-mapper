using System.Text.Json;
using System.Text.Json.Nodes;
using Tpcly.Json.Mapper.Abstractions;

namespace Tpcly.Json.Mapper.Transformers;

public class NumberValueTransformer(NumberValueTransformer.Action action, long? input = null) : ITransformer
{
    public enum Action
    {
        Replace,
        Add,
        Subtract,
        Multiply,
        Divide,
        Remainder,
        Exponentiate,
    }

    public bool CanTransform(JsonNode node) => node is JsonValue && node.GetValueKind() == JsonValueKind.Number;

    public void Apply(JsonNode node)
    {
        var value = node.AsValue().GetValue<long?>();

        switch (action)
        {
            case Action.Replace:
                value = input;
                break;
            case Action.Add:
                value += input;
                break;
            case Action.Subtract:
                value -= input;
                break;
            case Action.Multiply:
                value *= input;
                break;
            case Action.Divide:
                value /= input;
                break;
            case Action.Remainder:
                value %= input;
                break;
            case Action.Exponentiate:
                value ^= input;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }

        node.ReplaceWith(value);
    }
}