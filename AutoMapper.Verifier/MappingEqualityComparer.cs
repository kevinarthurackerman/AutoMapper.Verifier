using System.Collections.Generic;

namespace AutoMapper.Verifier
{
    class MappingEqualityComparer : IEqualityComparer<Mapping>
    {
        public bool Equals(Mapping x, Mapping y)
        {
            return x.From == y.From && x.To == y.To;
        }

        public int GetHashCode(Mapping obj)
        {
            if(obj.From == null)
            {
                return obj.To.GetHashCode();
            }

            if (obj.To == null)
            {
                return obj.From.GetHashCode();
            }

            return obj.From.GetHashCode() ^ obj.To.GetHashCode();
        }
    }
}
