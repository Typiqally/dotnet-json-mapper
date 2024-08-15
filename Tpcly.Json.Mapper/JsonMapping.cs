using System.Collections.ObjectModel;
using Json.Schema;
using Tpcly.Json.Mapper.Abstractions;

namespace Tpcly.Json.Mapper;

public record JsonMapping(JsonSchema SourceSchema, JsonSchema DestinationSchema)
{
    public IList<ITransformRule> Rules { get; set; } = new Collection<ITransformRule>();
}