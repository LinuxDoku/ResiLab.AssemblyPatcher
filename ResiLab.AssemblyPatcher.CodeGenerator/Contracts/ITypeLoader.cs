using System;
using System.Reflection;

namespace ResiLab.AssemblyPatcher.CodeGenerator.Contracts
{
    public interface ITypeLoader
    {
        /// <summary>
        /// Get a Type by it's full type name.
        /// 
        /// The type has to exist in a loaded assembly or an assembly located in the assembly search paths.
        /// </summary>
        /// <param name="fullTypeName"></param>
        /// <returns></returns>
        Type GetType(string fullTypeName);

        /// <summary>
        /// Get MethodBase by the delcaring type information, the method name and it's parameter types.
        /// </summary>
        /// <param name="declaringTypeFullName"></param>
        /// <param name="methodName"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        MethodBase GetMethod(string declaringTypeFullName, string methodName, Type[] parameterTypes);
    }
}