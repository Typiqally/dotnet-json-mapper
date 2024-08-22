using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using Tpcly.Json.Mapper.Abstractions;

namespace Tpcly.Json.Mapper.Transformers;

public class ValueKindTransformer(JsonValueKind kind) : ITransformer
{
    public JsonValueKind Kind { get; set; } = kind;

    public bool CanTransform(JsonNode node) => node is JsonValue;

    public void Apply(JsonNode node)
    {
        var value = node.AsValue().GetValue<object>();
        var valueType = GetValueType(Kind);
        var converter = TypeDescriptor.GetConverter(valueType);
        node.ReplaceWith(JsonValue.Create(converter.ConvertTo(value, valueType)));
    }

    private Type GetValueType(JsonValueKind valueType)
    {
        switch (valueType)
        {
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
            case JsonValueKind.Object:
                return typeof(object);
            case JsonValueKind.True:
            case JsonValueKind.False:
                return typeof(bool);
            case JsonValueKind.String:
                return typeof(string);
            case JsonValueKind.Number:
                return typeof(long);
            case JsonValueKind.Array:
            default:
                throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null);
        }
    }
}