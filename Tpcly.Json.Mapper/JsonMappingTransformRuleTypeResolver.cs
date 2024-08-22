using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Tpcly.Json.Mapper.Abstractions;

namespace Tpcly.Json.Mapper;

public class JsonMappingTransformRuleTypeResolver(JsonMapperOptions options) : DefaultJsonTypeInfoResolver
{
    private readonly IList<JsonDerivedType> _derivedTypes = options.Transformers
        .Select(t => t.GetType().BaseType.GetGenericArguments()[0])
        .Distinct()
        .Select(t => new JsonDerivedType(t, t.FullName))
        .ToList();

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);
        var basePointType = typeof(ITransformRule);

        if (jsonTypeInfo.Type != basePointType)
        {
            return jsonTypeInfo;
        }

        jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
        {
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
        };

        foreach (var derivedType in _derivedTypes)
        {
            jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(derivedType);
        }

        return jsonTypeInfo;
    }
}