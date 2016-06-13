using System.Reflection;
using Mono.Cecil;
using ResiLab.AssemblyPatcher.CodeGenerator.Contracts;

namespace ResiLab.AssemblyPatcher.CodeGenerator.Extensions
{
    public static class MonoCecilTypeLoaderExtensions
    {
        /// <summary>
        /// Get the full Reflected Method for this MethodReference.
        /// </summary>
        /// <param name="typeLoader"></param>
        /// <param name="methodReference"></param>
        /// <returns></returns>
        public static MethodBase GetMethod(this ITypeLoader typeLoader, MethodReference methodReference)
        {
            return typeLoader.GetMethod(methodReference.DeclaringType.FullName, methodReference.Name, methodReference.GetParameterTypes(typeLoader));
        }
    }
}