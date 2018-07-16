using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace AutoMapper.Verifier
{
    public static class Verifier
    {
        private static readonly IEqualityComparer<Mapping> equalityComparer = new MappingEqualityComparer();

        public static IImmutableSet<Mapping> Mappings { get; private set; }

        public static void VerifyMappings()
        {
            var mappings = new HashSet<Mapping>(equalityComparer);

            // find all mappings
            var assemblyFileName = Assembly.GetEntryAssembly().GetName().Name + ".dll";
            var currentAssembly = AssemblyDefinition.ReadAssembly(assemblyFileName);

            var types = currentAssembly.MainModule.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract);

            foreach (var type in types)
            {
                var methods = type.Methods
                    .Where(x => x.HasBody);

                foreach(var method in methods)
                {
                    var methodCalls = method.Body.Instructions
                        .Where(i => i.OpCode == OpCodes.Call || i.OpCode == OpCodes.Callvirt)
                        .Select(i => i.Operand as MethodReference)
                        .Where(m => m?.DeclaringType.Namespace == "AutoMapper")
                        .Where(m => m.Name == "CreateMap" || m.Name == "ReverseMap" || m.Name == "Map");
                    
                    foreach(var methodCall in methodCalls)
                    {
                        var callSite = $"{method.FullName} => {methodCall.FullName}";

                        switch (methodCall.Name)
                        {
                            case "CreateMap":
                                mappings.AddOrUpdateMapping(CreateMapping(methodCall, callSite, null));
                                break;
                            case "ReverseMap":
                                mappings.AddOrUpdateMapping(CreateMapping(methodCall, callSite, null));
                                break;
                            case "Map":
                                mappings.AddOrUpdateMapping(CreateMapping(methodCall, null, callSite));
                                break;
                        }
                    }
                }
            }
            
            // verify that we have all the mappings we need and that we don't have any that we don't need
            foreach(var mapping in mappings.ToArray())
            {
                if (mapping.CreateCallSites.Count() > 1)
                {
                    mappings.AddError(mapping.From, mapping.To, "Mappings cannot be declared at more than one call site.");
                }

                if(mapping.CreateCallSites.Count() == 0)
                {
                    mappings.AddError(mapping.From, mapping.To, "Mapping is not declared.");
                }

                if(mapping.MapCallSites.Count() == 0)
                {
                    mappings.AddError(mapping.From, mapping.To, "Mapping is not used.");
                }

                if(mapping.From == null)
                {
                    mappings.AddError(mapping.From, mapping.To, "Could not determine source type.");
                }

                if (mapping.To == null)
                {
                    mappings.AddError(mapping.From, mapping.To, "Could not determine destination type.");
                }
            }
            
            Mappings = mappings.ToImmutableHashSet();

            Mapping CreateMapping(MethodReference methodReference, string createCallSite, string mapCallSite)
            {
                var genericInstance = methodReference as IGenericInstance ?? methodReference.DeclaringType as IGenericInstance;

                TypeReference srcType = null;
                TypeReference destType = null;
                switch (methodReference.Name)
                {
                    case "CreateMap":
                        genericInstance?.GenericArguments.TryGetIndex(0, out srcType);
                        genericInstance?.GenericArguments.TryGetIndex(1, out destType);
                        break;
                    case "ReverseMap":
                        genericInstance?.GenericArguments.TryGetIndex(1, out srcType);
                        genericInstance?.GenericArguments.TryGetIndex(0, out destType);
                        break;
                    case "Map":
                        if (genericInstance?.GenericArguments.Count() == 1)
                        {
                            genericInstance?.GenericArguments.TryGetIndex(0, out destType);
                        }
                        else
                        {
                            genericInstance?.GenericArguments.TryGetIndex(0, out srcType);
                            genericInstance?.GenericArguments.TryGetIndex(1, out destType);
                        }
                        break;
                }

                srcType = srcType?.GetElementType();
                destType = destType?.GetElementType();

                return new Mapping(
                    srcType.GetCSharpType(), 
                    destType.GetCSharpType(),
                    createCallSite != null ? new[] { createCallSite } : null,
                    mapCallSite != null ? new[] { mapCallSite } : null);
            }
        }
    }
}
