using System;
using System.Collections.Generic;

namespace AutoMapper.Verifier
{
    internal class AutoMapperVerificationException : Exception
    {
        internal AutoMapperVerificationException(
            Type from,
            Type to,
            IEnumerable<string> createCallSites,
            IEnumerable<string> mapCallSites,
            string message) : base(message)
        {
            From = from;
            To = to;
            CreateCallSites = createCallSites;
            MapCallSites = mapCallSites;
        }

        public Type From { get; }
        public Type To { get; }
        public IEnumerable<string> CreateCallSites { get; }
        public IEnumerable<string> MapCallSites { get; }
    }
}
