using System.ComponentModel;
using System.Text.Json.Nodes;
using Json.Schema;
using Tpcly.Json.Mapper.Abstractions;

namespace Tpcly.Json.Mapper.Transformers;

public class TypeTransformer(SchemaValueType type) : ITransformer
{
    public SchemaValueType Type { get; set; } = type;

    public bool CanTransform(JsonNode node) => node is JsonValue;

    public void Apply(JsonNode node)
    {
        var value = node.AsValue().GetValue<object>();
        var valueType = GetValueType(Type);
        var converter = TypeDescriptor.GetConverter(valueType);
        node.ReplaceWith(JsonValue.Create(converter.ConvertTo(value, valueType)));
    }

    private Type GetValueType(SchemaValueType valueType)
    {
        switch (valueType)
        {
            case SchemaValueType.Null:
            case SchemaValueType.Object:
                return typeof(object);
            case SchemaValueType.Boolean:
                return typeof(bool);
            case SchemaValueType.String:
                return typeof(string);
            case SchemaValueType.Number:
                return typeof(long);
            case SchemaValueType.Integer:
                return typeof(int);
            case SchemaValueType.Array:
            default:
                throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null);
        }
    }
}