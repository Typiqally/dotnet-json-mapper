using System.ComponentModel;
using System.Text.Json.Nodes;
using Json.Schema;
using Tpcly.Json.Mapper.Abstractions;

namespace Tpcly.Json.Mapper.Transformers;

public class TypeTransformer : Transformer<TypeTransformer.Rule>
{
    public override bool CanTransform(JsonNode node, ITransformRule rule)
    {
        return rule is Rule && node is JsonObject;
    }

    public override JsonNode Apply(JsonNode node, Rule rule)
    {
        var sourceObj = node.AsObject();

        foreach (var (path, targetType) in rule.TypeMappings)
        {
            var (propertyNode, propertyName) = sourceObj.Traverse(path);
            var propertyValue = propertyNode?.AsObject()[propertyName];

            if (propertyValue is not JsonValue) continue;

            var value = propertyValue.AsValue().GetValue<object>();
            var test = GetValueType(targetType);
            
            var converter = TypeDescriptor.GetConverter(test);

            propertyNode[propertyName] = JsonValue.Create(converter.ConvertTo(value, test));
        }

        return sourceObj;
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
    
    public record Rule : ITransformRule
    {
        public Dictionary<string, SchemaValueType> TypeMappings { get; init; } = new();
    }
}