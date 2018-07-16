using System;
using System.Collections.Generic;

namespace AutoMapper.Verifier
{
    public class Mapping
    {
        internal Mapping(Type from, Type to, IEnumerable<string> createCallSites, IEnumerable<string> mapCallSites, IEnumerable<string> errors = null)
        {
            From = from;
            To = to;
            CreateCallSites = createCallSites ?? new string[0];
            MapCallSites = mapCallSites ?? new string[0];
            Errors = errors ?? new string[0];
        }

        public Type From { get; }
        public Type To { get; }
        public IEnumerable<string> CreateCallSites { get; }
        public IEnumerable<string> MapCallSites { get; }
        public IEnumerable<string> Errors { get; }

        public override string ToString()
        {
            return $"Mapping {From?.FullName ?? "???"} => {To?.FullName ?? "???"}";
        }
    }
}
