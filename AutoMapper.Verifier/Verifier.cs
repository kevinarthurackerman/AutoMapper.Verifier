using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoMapper.Verifier
{
    public static class Verifier
    {
        public static IEnumerable<Mapping> VerifyMappings() => VerifyMappings(x => { });

        public static IEnumerable<Mapping> VerifyMappings(ErrorActions onError) => VerifyMappings(x => x.SetAllErrorActions(onError));

        public static IEnumerable<Mapping> VerifyMappings(Action<VerifierConfiguration> configAction)
        {
            var config = new VerifierConfiguration();
            configAction(config);
            
            var mappings = new Dictionary<MappingKey, Mapping>();

            // find all mappings
            var assemblyDefinitions = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic)
                .Select(a => AssemblyDefinition.ReadAssembly(a.Location))
                .ToArray();

            var types = assemblyDefinitions
                .SelectMany(x => x.Modules.SelectMany(y => y.GetTypes()))
                .Where(x => x.IsClass && !x.IsAbstract)
                .ToArray();

            foreach (var type in types)
            {
                var methods = type.Methods
                    .Where(x => x.HasBody)
                    .ToArray();

                foreach(var method in methods)
                {
                    var methodCalls = method.Body.Instructions
                        .Where(i => i.OpCode == OpCodes.Call || i.OpCode == OpCodes.Callvirt)
                        .Select(i => i.Operand as MethodReference)
                        .Where(m => m?.DeclaringType.Namespace == "AutoMapper")
                        .Where(m => m.Name == "CreateMap" || m.Name == "ReverseMap" || m.Name == "Map")
                        .ToArray();
                    
                    foreach(var methodCall in methodCalls)
                    {
                        var callSite = $"{method.FullName} => {methodCall.FullName}";
                        mappings.AddOrUpdateMapping(CreateMapping(methodCall, callSite));
                    }
                }
            }
            
            // verify that we have all the mappings we need and that we don't have any that we don't need
            foreach(var mapping in mappings.Values.ToArray())
            {
                if (mapping.MapCreationCallSites.Count() > 1)
                {
                    AddError(mapping, "Mappings cannot be declared at more than one call site.", config.OnMultiplyDeclaredMapping);
                }

                if(!mapping.MapCreationCallSites.Any())
                {
                    AddError(mapping, "Mapping is not declared or could not be found because the source or destination type could not be determined.", config.OnUndeclaredMapping);
                }

                if(mapping.From == null || mapping.To == null)
                {
                    AddError(mapping, "Could not determine if mapping is used because the source or destination type could not be determined.", config.OnIndeterminantMapping);

                    if (mapping.From == null)
                    {
                        AddError(mapping, "Could not determine source type.", config.OnIndeterminantMapping);
                    }

                    if (mapping.To == null)
                    {
                        AddError(mapping, "Could not determine destination type.", config.OnIndeterminantMapping);
                    }
                }
                else
                {
                    if (mapping.MapUsageCallSites.Count() == 0)
                    {
                        AddError(mapping, "Mapping is not used.", config.OnUnusedMapping);
                    }
                }
            }
            
            return mappings.Values;

            Mapping CreateMapping(MethodReference methodReference, string callSite)
            {
                var genericInstance = methodReference as IGenericInstance ?? methodReference.DeclaringType as IGenericInstance;

                TypeReference srcType = null;
                TypeReference destType = null;
                string createCallSite = null;
                string usageCallSite = null;
                switch (methodReference.Name)
                {
                    case "CreateMap":
                        genericInstance?.GenericArguments.TryGetIndex(0, out srcType);
                        genericInstance?.GenericArguments.TryGetIndex(1, out destType);
                        createCallSite = callSite;
                        break;
                    case "ReverseMap":
                        genericInstance?.GenericArguments.TryGetIndex(1, out srcType);
                        genericInstance?.GenericArguments.TryGetIndex(0, out destType);
                        createCallSite = callSite;
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
                        usageCallSite = callSite;
                        break;
                }

                return new Mapping(
                    srcType.GetCSharpType(), 
                    destType.GetCSharpType(),
                    createCallSite != null ? new[] { createCallSite } : null,
                    usageCallSite != null ? new[] { usageCallSite } : null);
            }

            void AddError(Mapping mapping, string error, ErrorActions action)
            {
                switch(action)
                {
                    case ErrorActions.Ignore:
                        break;
                    case ErrorActions.LogError:
                        mappings.AddOrUpdateMapping(new Mapping(mapping.From, mapping.To, null, null, new[] { error }));
                        break;
                    case ErrorActions.ThrowException:
                        throw new AutoMapperVerificationException(mapping.From, mapping.To, mapping.MapCreationCallSites, mapping.MapUsageCallSites, error);
                    default:
                        throw new NotSupportedException($"ErrorAction value of '{Enum.GetName(typeof(ErrorActions),action)}' is not supported.");
                }
            }
        }
    }
}
