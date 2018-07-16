using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoMapper.Verifier
{
    internal static class IListExtensions
    {
        internal static bool TryGetIndex<T>(this IList<T> list, int index, out T value)
        {
            if(list.Count() >= index + 1)
            {
                value = list[index];
                return true;
            }

            value = default(T);
            return false;
        }
    }
}
