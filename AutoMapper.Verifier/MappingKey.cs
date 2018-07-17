using System;

namespace AutoMapper.Verifier
{
    internal struct MappingKey
    {
        internal MappingKey(Type from, Type to)
        {
            From = from;
            To = to;
        }

        internal Type From { get; }
        internal Type To { get; }
    }
}
