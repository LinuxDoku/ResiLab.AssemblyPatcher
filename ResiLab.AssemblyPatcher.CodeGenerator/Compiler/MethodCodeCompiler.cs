using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ResiLab.AssemblyPatcher.CodeGenerator.Compiler
{
    /// <summary>
    /// Compile method code to .NET binary and return it as a mono cecil method body, which could be merged into existing assemblies.
    /// </summary>
    public class MethodCodeCompiler
    {
        /// <summary>
        /// Compile the C# source code to a new IL MethodBody, which should replace the Body of the passed MethodDefinition.
        /// </summary>
        /// <param name="cSharpMethodBodyCode"></param>
        /// <param name="methodDefinition"></param>
        /// <returns></returns>
        public static MethodBody Compile(string cSharpMethodBodyCode, MethodDefinition methodDefinition)
        {
            var assemblyStream = new MemoryStream();

            // generte the program code and syntax tree
            var programCode = GenerateProgramCode(cSharpMethodBodyCode, methodDefinition);
            var syntaxTree = CSharpSyntaxTree.ParseText(programCode);

            // compile the syntax tree to binary
            var compilation = CSharpCompilation.Create(
                Guid.NewGuid().ToString(),
                syntaxTrees: new [] { syntaxTree },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                // TODO: add more metadata references, based on syntax tree information
                references: new [] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) }
            );

            // write the IL code to the stream
            var result = compilation.Emit(assemblyStream);
            if (result.Success)
            {
                // seek to first bit
                assemblyStream.Seek(0, SeekOrigin.Begin);

                // load the assembly defintion
                var assemblyDefintion = AssemblyDefinition.ReadAssembly(assemblyStream);
                var generatedType = assemblyDefintion.MainModule.Types.First(x => x.FullName == methodDefinition.DeclaringType.FullName);
                var method = generatedType.Methods.First(x => x.Name == methodDefinition.Name);

                return method.Body;
            }
            
            throw new Exception($"Could not compile C# method body {Environment.NewLine}{programCode}{Environment.NewLine}. Errors: {string.Join(Environment.NewLine, result.Diagnostics)}");
        }

        /// <summary>
        /// Generate the complete program code, required for an assembly.
        /// 
        /// The MethodDefinition is used to generate namespace, class and members exactly to the original assembly.
        /// Then the generated source code can have references to all class members.
        /// </summary>
        /// <param name="cSharpMethodCode"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static string GenerateProgramCode(string cSharpMethodCode, MethodDefinition method)
        {
            var returnType = method.ReturnType.Name;
            if (returnType == "Void")
            {
                returnType = returnType.ToLowerInvariant();
            }

            // generate the namespace, class and method declartion
            return $"namespace {method.DeclaringType.Namespace} {{ " + Environment.NewLine +
                   $"   public class {method.DeclaringType.Name} {{ " + Environment.NewLine +
                   GenerateFields(method) + Environment.NewLine +
                   $"       public {(method.IsStatic ? "static" : "")} {returnType} {method.Name}() {{  " + Environment.NewLine +
                   $"           {cSharpMethodCode} " + Environment.NewLine +
                   $"       }} " + Environment.NewLine +
                   $"   }} " + Environment.NewLine +
                   $"}}";
        }

        /// <summary>
        /// Generate the fields of the DeclaringType from the MethodDefintion as C# Source code.
        /// 
        /// This allows to reference class member fields from the method body.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static string GenerateFields(MethodDefinition method)
        {
            var builder = new StringBuilder();

            // get all fields from declaring type and generate them
            foreach (var field in method.DeclaringType.Fields)
            {
                builder.AppendLine($"       public {(field.IsStatic ? "static" : "")} {field.FieldType.FullName} {field.Name} = default({field.FieldType.FullName});");
            }

            return builder.ToString();
        }
    }
}