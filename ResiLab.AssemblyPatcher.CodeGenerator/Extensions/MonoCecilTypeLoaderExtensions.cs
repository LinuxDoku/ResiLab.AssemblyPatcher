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

        /// <summary>
        /// Get FieldInfo for the passed FieldReference.
        /// </summary>
        /// <param name="typeLoader"></param>
        /// <param name="fieldReference"></param>
        /// <returns></returns>
        public static FieldInfo GetField(this ITypeLoader typeLoader, FieldReference fieldReference) {
            return typeLoader.GetField(fieldReference.DeclaringType.FullName, fieldReference.Name);
        }
    }
}