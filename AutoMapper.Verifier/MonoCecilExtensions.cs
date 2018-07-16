using Mono.Cecil;
using System;

namespace AutoMapper.Verifier
{
    internal static class MonoCecilExtensions
    {
        internal static Type GetCSharpType(this TypeReference typeReference)
        {
            if(typeReference == null)
            {
                return null;
            }

            return Type.GetType(typeReference.FullName + ", " + typeReference.Module.Assembly.FullName);
        }
    }
}
