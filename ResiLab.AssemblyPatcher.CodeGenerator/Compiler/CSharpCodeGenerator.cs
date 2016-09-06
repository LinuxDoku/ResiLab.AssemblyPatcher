using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Mono.Cecil;
using ResiLab.AssemblyPatcher.CodeGenerator.Extensions;

namespace ResiLab.AssemblyPatcher.CodeGenerator.Compiler {
    /// <summary>
    /// C# Code generator, which converts Mono Cecil References and Definitions into C# Code and allows to inject custom code.
    /// 
    /// Maybe add here an interface to support other .NET languages like F# or VB.NET later.
    /// </summary>
    public class CSharpCodeGenerator 
    {

        /// <summary>
        /// Format a list of members.
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        public string Members(params string[] members)
        {
            return Format(members);
        }

        /// <summary>
        /// Generate the required program code for an assembly - Namespace, Class and the "class body".
        /// </summary>
        /// <param name="typeDefinition"></param>
        /// <param name="classBody"></param>
        /// <returns></returns>
        public string GenerateProgram(TypeDefinition typeDefinition, string classBody)
        {
            return GenerateNamespace(typeDefinition.Namespace, GenerateClass(typeDefinition, classBody));
        }

        /// <summary>
        /// Generate the namespace with body.
        /// </summary>
        /// <param name="namespaceName"></param>
        /// <param name="namespaceBody"></param>
        /// <returns></returns>
        public string GenerateNamespace(string namespaceName, string namespaceBody)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"namespace {namespaceName} {{");
            builder.AppendLine(namespaceBody);
            builder.AppendLine("}");

            return builder.ToString();
        }

        /// <summary>
        /// Generate the class with body.
        /// </summary>
        /// <param name="typeDefinition"></param>
        /// <param name="classBody"></param>
        /// <returns></returns>
        public string GenerateClass(TypeDefinition typeDefinition, string classBody)
        {
            var builder = new StringBuilder();

            string inheritance = "";

            // base type
            if (typeDefinition.BaseType != null && typeDefinition.BaseType.FullName != typeof(object).FullName)
            {
                inheritance = " : " + typeDefinition.BaseType.FullName;
            }

            // interfaces
            if (typeDefinition.HasInterfaces)
            {
                if (inheritance.Length == 0)
                {
                    inheritance = " : ";
                }

                inheritance = inheritance + string.Join(", ", typeDefinition.Interfaces.Select(x => x.FullName));
            }
            
            // generate class
            builder.AppendLine(1, $"{(typeDefinition.IsPublic ? "public" : "private")} class {typeDefinition.Name}{inheritance} {{");
            builder.AppendLine(classBody);
            builder.AppendLine(1, "}");

            return builder.ToString();
        }

        /// <summary>
        /// Generate the fields of the type with default value assignment as source code.
        /// </summary>
        /// <param name="typeDefinition"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public string GenerateFieldStubs(TypeDefinition typeDefinition, Func<FieldDefinition, bool> skip = null) {
            var builder = new StringBuilder();

            IEnumerable<FieldDefinition> fields = WithoutCompilerGenerated(typeDefinition.Fields);
            if (skip != null) {
                fields = fields.Where(x => skip(x) == false);
            }

            // generate all fields of this type
            foreach (var field in fields) {
                builder.AppendLine(2, $"{(field.IsPublic ? "public" : "private")} {(field.IsStatic ? "static" : "")} {field.FieldType.FullName} {field.Name} = default({field.FieldType.FullName});");
            }

            return builder.ToString();
        }

        public string GeneratePropertyStubs(TypeDefinition typeDefinition, Func<PropertyDefinition, bool> skip = null)
        {
            var builder = new StringBuilder();

            var properties = WithoutCompilerGenerated(typeDefinition.Properties);
            if (skip != null)
            {
                properties = properties.Where(x => skip(x) == false);
            }

            foreach (var property in properties)
            {
                var isPublic = property.GetMethod != null && property.GetMethod.IsPublic || property.SetMethod != null && property.SetMethod.IsPublic;
                var isStatic = property.GetMethod != null && property.GetMethod.IsStatic || property.SetMethod != null && property.SetMethod.IsStatic;

                builder.AppendLine(2, $"{(isPublic ? "public" : "private")} {(isStatic ? "static" : "")} {property.PropertyType.FullName} {property.Name}");
                builder.AppendLine(2, "{");

                if (property.GetMethod != null)
                {
                    builder.AppendLine(3, $"{(property.GetMethod.IsPrivate ?  "private" : "")} get {{ return default({property.PropertyType.FullName}); }} ");
                }

                if (property.SetMethod != null)
                {
                    builder.AppendLine(3, $"{(property.SetMethod.IsPrivate ? "private" : "")} set {{ }}");
                }

                builder.AppendLine(2, "}");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generate methods of the type with default return as source code, except the skipped.
        /// </summary>
        /// <param name="typeDefinition"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public string GenerateMethodStubs(TypeDefinition typeDefinition, Func<MethodDefinition, bool> skip = null) {
            IEnumerable<MethodDefinition> methods = WithoutCompilerGenerated(typeDefinition.Methods);
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
        public string GenerateMethodStub(MethodDefinition method) {
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
            //builder.AppendLine(2, GenerateAttributes(method));
            builder.AppendLine(2, $"{ToAccessor(method.ToAccessibility())} {(method.IsVirtual ? "virtual" : "")} {(method.IsStatic ? "static" : "")} {ConvertType(method.ReturnType)} {method.Name}({GenerateMethodParameters(method.Parameters)})");

            // brace and method body
            builder.AppendLine("       {");
            if (methodBodyCode != null) {
                builder.AppendLine(3, methodBodyCode);
            }
            builder.AppendLine(2, "}");

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

        protected IEnumerable<T> WithoutCompilerGenerated<T>(IEnumerable<T> list) where T : ICustomAttributeProvider
        {
            return list.Where(x => x.HasCustomAttributes == false || x.CustomAttributes.Any(y => y.AttributeType.FullName != typeof(CompilerGeneratedAttribute).FullName)); 
        }

        protected string ToAccessor(Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    return "private";
                
                case Accessibility.Internal:
                    return "internal";
                
                case Accessibility.Protected:
                    return "protected";

                case Accessibility.Public:
                default:
                    return "public";
            }
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

        protected string GenerateAttributes(ICustomAttributeProvider customAttributeProvider)
        {
            var builder = new StringBuilder();

            foreach (var attribute in customAttributeProvider.CustomAttributes)
            {
                builder.AppendLine($"[{attribute.AttributeType.FullName}]"); // TODO add arguments
            }

            return builder.ToString();
        }

        /// <summary>
        /// Format the lines of a source code.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private static string Format(params string[] lines) {
            return string.Join(Environment.NewLine, lines) + Environment.NewLine;
        }
    }
}