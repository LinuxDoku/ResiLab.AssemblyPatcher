using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using ResiLab.AssemblyPatcher.CodeGenerator.Contracts;

namespace ResiLab.AssemblyPatcher.CodeGenerator.Extensions
{
    public static class MethodReferenceExtensions
    {
        /// <summary>
        /// Get the parameter types for a MethodReference.
        /// </summary>
        /// <param name="methodReference"></param>
        /// <param name="typeLoader"></param>
        /// <returns></returns>
        public static Type[] GetParameterTypes(this MethodReference methodReference, ITypeLoader typeLoader)
        {
            var typeReferences =  methodReference.Parameters.Select(x => x.ParameterType);
            if (typeReferences.Any() == false)
            {
                return new Type[0];
            }

            var result = new List<Type>();
            foreach (var typeReference in typeReferences)
            {
                var type = typeLoader.GetType(typeReference.FullName);
                if (type == null)
                {
                    throw new Exception($"The Type \"{typeReference.FullName}\" is missing! Please add an assembly with this type to the application folder, source folder of the assembly which should be patched, or add an assembly search path!");
                }

                result.Add(type);
            }

            return result.ToArray();
        }
    }
}