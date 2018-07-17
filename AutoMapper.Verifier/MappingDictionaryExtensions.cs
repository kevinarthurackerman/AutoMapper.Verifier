using System.Collections.Generic;
using System.Linq;

namespace AutoMapper.Verifier
{
    internal static class MappingDictionaryExtensions
    {
        internal static void AddOrUpdateMapping(this Dictionary<MappingKey, Mapping> mappings, Mapping mapping)
        {
            var key = new MappingKey(mapping.From, mapping.To);
            if (mappings.TryGetValue(key, out var existingMapping))
            {
                mapping = new Mapping(
                        existingMapping.From,
                        existingMapping.To,
                        existingMapping.MapCreationCallSites.Concat(mapping.MapCreationCallSites).Distinct(),
                        existingMapping.MapUsageCallSites.Concat(mapping.MapUsageCallSites).Distinct(),
                        existingMapping.Errors.Concat(mapping.Errors).Distinct());
            }

            mappings[key] = mapping;
        }
    }
}
