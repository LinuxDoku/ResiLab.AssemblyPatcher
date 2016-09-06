using System;
using System.Linq;
using Mono.Cecil;

namespace ResiLab.AssemblyPatcher.CodeGenerator
{
    /// <summary>
    /// Query the AssemblyDefinition with some easy to use methods.
    /// </summary>
    public class AssemblyQuery
    {
        private readonly AssemblyDefinition _assemblyDefinition;

        public AssemblyQuery(AssemblyDefinition assemblyDefinition)
        {
            _assemblyDefinition = assemblyDefinition;
        }

        /// <summary>
        /// Find a Type by it's full name.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public TypeDefinition FindType(string typeName) {
            return _assemblyDefinition.MainModule.Types.FirstOrDefault(x => x.FullName == typeName);
        }

        /// <summary>
        /// Find a method by it's delcaring type's full name and method name.
        /// </summary>
        /// <param name="declaringTypeFullName"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public MethodDefinition FindMethod(string declaringTypeFullName, string methodName)
        {
            var type = FindType(declaringTypeFullName);
            return type?.Methods.SingleOrDefault(x => x.Name == methodName);
        }
        
        /// <summary>
        /// Find a method by it's declaring type's full name, method name and the parameter types.
        /// </summary>
        /// <param name="declaringTypeFullName"></param>
        /// <param name="methodName"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public MethodDefinition FindMethod(string declaringTypeFullName, string methodName, Type[] parameterTypes)
        {
            var type = FindType(declaringTypeFullName);
            return type?.Methods.FirstOrDefault(x => x.Name == methodName 
                                                  && x.Parameters.Select(y => y.ParameterType.FullName).SequenceEqual(parameterTypes.Select(y => y.FullName)));
        }

        /// <summary>
        /// Find a property by it's declaring type's full name and the property name.
        /// </summary>
        /// <param name="declaringTypeFullName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyDefinition FindProperty(string declaringTypeFullName, string propertyName)
        {
            var type = FindType(declaringTypeFullName);
            return type?.Properties.FirstOrDefault(x => x.Name == propertyName);
        }

        /// <summary>
        /// Find a field by it's declaring type's full name and the field name.
        /// </summary>
        /// <param name="declartingTypeFullName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public FieldDefinition FindField(string declartingTypeFullName, string fieldName)
        {
            var type = FindType(declartingTypeFullName);
            return type?.Fields.FirstOrDefault(x => x.Name == fieldName);
        }
    }
}