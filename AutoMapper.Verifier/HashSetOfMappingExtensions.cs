using System.Collections.Generic;
using System.Linq;

namespace AutoMapper.Verifier
{
    internal static class HashSetOfMappingExtensions
    {
        internal static void AddOrUpdateMapping(this HashSet<Mapping> mappings, Mapping mapping)
        {
            if (mappings.TryGetValue(mapping, out var existingMapping))
            {
                mapping = new Mapping(
                        existingMapping.From,
                        existingMapping.To,
                        existingMapping.CreateCallSites.Concat(mapping.CreateCallSites).Distinct(),
                        existingMapping.MapCallSites.Concat(mapping.MapCallSites).Distinct(),
                        existingMapping.Errors.Concat(mapping.Errors).Distinct());

                mappings.Remove(existingMapping);
            }

            mappings.Add(mapping);
        }
    }
}
