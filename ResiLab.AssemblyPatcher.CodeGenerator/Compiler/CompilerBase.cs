using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace ResiLab.AssemblyPatcher.CodeGenerator.Compiler
{
    public abstract class CompilerBase
    {
        /// <summary>
        /// Generate the required program code for an assembly - Namespace, Class and the "class body".
        /// </summary>
        /// <param name="typeReference"></param>
        /// <param name="cSharpCode"></param>
        /// <returns></returns>
        protected string GenerateProgramCode(TypeReference typeReference, string cSharpCode)
        {
            return $"namespace {typeReference.Namespace} {{ " + Environment.NewLine +
                   $"   public class {typeReference.Name} {{ " + Environment.NewLine +
                   cSharpCode +
                   $"   }} " + Environment.NewLine +
                   $"}}";
        }

        /// <summary>
        /// Generate the fields of the type with default value assignment as C# source code.
        /// </summary>
        /// <param name="typeDefinition"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        protected string GenerateFieldStubs(TypeDefinition typeDefinition, Func<FieldDefinition, bool> skip=null) 
        {
            var builder = new StringBuilder();

            IEnumerable<FieldDefinition> fields = typeDefinition.Fields;
            if (skip != null)
            {
                fields = fields.Where(x => skip(x) == false);
            }

            // generate all fields of this type
            foreach (var field in fields) {
                builder.AppendLine($"       public {(field.IsStatic ? "static" : "")} {field.FieldType.FullName} {field.Name} = default({field.FieldType.FullName});");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generate methods of the type with default return values as C# source code.
        /// </summary>
        /// <param name="typeDefinition"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        protected string GenerateMethodStubs(TypeDefinition typeDefinition, Func<MethodDefinition, bool> skip = null)
        {
            IEnumerable<MethodDefinition> methods = typeDefinition.Methods;
            if (skip != null) {
                methods = methods.Where(x => skip(x) == false);
            }

            return Format(methods.Select(GenerateMethodStub).ToArray());
        }

        /// <summary>
        /// Generate the method with default return value as C# source code.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        protected string GenerateMethodStub(MethodDefinition method)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"       public {(method.IsStatic ? "static" : "")} {ConvertType(method.ReturnType)} {method.Name}({GenerateMethodParameters(method.Parameters)})");

            if (method.ReturnType.FullName != typeof(void).FullName)
            {
                builder.AppendLine("       {");
                builder.AppendLine($"            return default({method.ReturnType.FullName});");
                builder.AppendLine("       }");
            }
            else
            {
                builder.AppendLine("       {}");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generate the parameters of a method.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected string GenerateMethodParameters(IEnumerable<ParameterDefinition> parameters)
        {
            return string.Join(", ", parameters.Select(x => $"{x.ParameterType.FullName} {x.Name}"));
        }

        /// <summary>
        /// Format the lines of a source code.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        protected string Format(params string[] lines)
        {
            return string.Join(Environment.NewLine, lines) + Environment.NewLine;
        }

        /// <summary>
        /// Convert the TypeReference to a valid C# 
        /// </summary>
        /// <param name="typeReference"></param>
        /// <returns></returns>
        protected string ConvertType(TypeReference typeReference)
        {
            var returnType = typeReference.FullName;

            switch (returnType)
            {
                case "System.Void":
                    returnType = "void";
                    break;
            }

            return returnType;
        }
    }
}