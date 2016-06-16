using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace ResiLab.AssemblyPatcher.CodeGenerator.Compiler
{
    /// <summary>
    /// C# Code generator, which converts Mono Cecil References and Definitions into C# Code and allows to inject custom code.
    /// 
    /// Maybe add here an interface to support other .NET languages like F# or VB.NET later.
    /// </summary>
    public class CSharpCodeGenerator
    {
        /// <summary>
        /// Generate the required program code for an assembly - Namespace, Class and the "class body".
        /// </summary>
        /// <param name="typeReference"></param>
        /// <param name="classBody"></param>
        /// <returns></returns>
        public string GenerateProgram(TypeReference typeReference, string classBody) {
            return $"namespace {typeReference.Namespace} {{ " + Environment.NewLine +
                   $"   public class {typeReference.Name} {{ " + Environment.NewLine +
                   classBody +
                   $"   }} " + Environment.NewLine +
                   $"}}";
        }

        /// <summary>
        /// Generate the fields of the type with default value assignment as source code.
        /// </summary>
        /// <param name="typeDefinition"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public string GenerateFieldStubs(TypeDefinition typeDefinition, Func<FieldDefinition, bool> skip = null) {
            var builder = new StringBuilder();

            IEnumerable<FieldDefinition> fields = typeDefinition.Fields;
            if (skip != null) {
                fields = fields.Where(x => skip(x) == false);
            }

            // generate all fields of this type
            foreach (var field in fields) {
                builder.AppendLine($"       public {(field.IsStatic ? "static" : "")} {field.FieldType.FullName} {field.Name} = default({field.FieldType.FullName});");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generate methods of the type with default return as source code, except the skipped.
        /// </summary>
        /// <param name="typeDefinition"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public string GenerateMethodStubs(TypeDefinition typeDefinition, Func<MethodDefinition, bool> skip = null) 
        {
            IEnumerable<MethodDefinition> methods = typeDefinition.Methods;
            if (skip != null) {
                methods = methods.Where(x => skip(x) == false);
            }

            return GeneratorUtil.Format(methods.Select(GenerateMethodStub).ToArray());
        }

        /// <summary>
        /// Generate the method with default return value as C# source code.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public string GenerateMethodStub(MethodDefinition method)
        {
            string methodBodyCode = null;

            // when the return type is not void, generate default return statement
            if (method.ReturnType.FullName != typeof(void).FullName) {
                methodBodyCode = $"return default({method.ReturnType.FullName});";
            }

            return GenerateMethod(method, methodBodyCode);
        }

        /// <summary>
        /// Generate the Method of the MethodDefintion with the passed methodBody code as Source Code.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="methodBodyCode"></param>
        /// <returns></returns>
        public string GenerateMethod(MethodDefinition method, string methodBodyCode) {
            var builder = new StringBuilder();

            // method declaration
            builder.AppendLine($"       public {(method.IsStatic ? "static" : "")} {ConvertType(method.ReturnType)} {method.Name}({GenerateMethodParameters(method.Parameters)})");
            
            // brace and method body
            builder.AppendLine("       {");
            if (methodBodyCode != null)
            {
                builder.AppendLine($"            {methodBodyCode}");
            }
            builder.AppendLine("       }");

            return builder.ToString();
        }



        /// <summary>
        /// Generate the parameters of a method.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected string GenerateMethodParameters(IEnumerable<ParameterDefinition> parameters) {
            return string.Join(", ", parameters.Select(x => $"{x.ParameterType.FullName} {x.Name}"));
        }

        /// <summary>
        /// Convert the TypeReference to a valid C# type.
        /// </summary>
        /// <param name="typeReference"></param>
        /// <returns></returns>
        protected string ConvertType(TypeReference typeReference) {
            var returnType = typeReference.FullName;

            switch (returnType) {
                case "System.Void":
                    returnType = "void";
                    break;
            }

            return returnType;
        }
    }
}