using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Tpcly.Json.Mapper.Abstractions;
using Tpcly.Json.Mapper.Transformers;

namespace Tpcly.Json.Mapper;

public class JsonMappingTransformerTypeResolver(IList<Type>? additionalTransformerTypes = null) : DefaultJsonTypeInfoResolver
{
    private static readonly IList<Type> DefaultTransformerTypes = new List<Type>
    {
        typeof(ArrayTransformer),
        typeof(NodePathTransformer),
        typeof(ObjectTransformer),
        typeof(ValueTypeTransformer)
    };

    private readonly IEnumerable<JsonDerivedType> _derivedTypes = (additionalTransformerTypes ?? new List<Type>())
        .Concat(DefaultTransformerTypes)
        .Select(t => new JsonDerivedType(t, t.Name))
        .ToList();


    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);
        var basePointType = typeof(ITransformer);

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