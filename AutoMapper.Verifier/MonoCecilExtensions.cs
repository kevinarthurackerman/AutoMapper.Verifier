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

            var assembly = typeReference.Scope.MetadataScopeType == MetadataScopeType.AssemblyNameReference
                 ? typeReference.Scope.ToString()
                 : typeReference.Module.Assembly.FullName;
            var fullyQualifiedType = typeReference.FullName.Replace('/', '+') + ", " + assembly;
            var type = Type.GetType(fullyQualifiedType);
            return type;
        }
    }
}
