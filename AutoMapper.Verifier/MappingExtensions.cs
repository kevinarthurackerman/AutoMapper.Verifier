using System.Linq;

namespace AutoMapper.Verifier
{
    internal static class MappingExtensions
    {
        internal static Mapping AddError(this Mapping mapping, string error)
        {
            return new Mapping(mapping.From, mapping.To, mapping.CreateCallSites, mapping.MapCallSites, mapping.Errors.Concat(new[] { error }));
        }
    }
}
