using System;
using System.IO;
using System.Linq;
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
        private const string GeneratorNamespace = "CodeGeneration";
        private const string GeneratorClassName = "CodeGenerationProgram";
        private const string GeneratorMethodName = "CodeGeneratorMethod";

        /// <summary>
        /// Compile the C# method body to a mono cecil method body, which contains IL statements.
        /// </summary>
        /// <param name="cSharpMethodBodyCode"></param>
        /// <returns></returns>
        public static MethodBody Compile(string cSharpMethodBodyCode)
        {
            var assemblyStream = new MemoryStream();

            // generte the program code and syntax tree
            var programCode = GenerateProgramCode(cSharpMethodBodyCode);
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
                var generatedType = assemblyDefintion.MainModule.Types.First(x => x.FullName == $"{GeneratorNamespace}.{GeneratorClassName}");
                var method = generatedType.Methods.First(x => x.Name == GeneratorMethodName);

                return method.Body;
            }
            
            throw new Exception($"Could not compile C# method body \"{cSharpMethodBodyCode}\". Errors: {result.Diagnostics}");
        }

        /// <summary>
        /// Generate the complete program code, required for an assembly.
        /// </summary>
        /// <param name="cSharpMethodCode"></param>
        /// <returns></returns>
        private static string GenerateProgramCode(string cSharpMethodCode)
        {
            return $"namespace {GeneratorNamespace} {{ " +
                   $"   public class {GeneratorClassName} {{ " +
                   $"       public static void {GeneratorMethodName}() {{ " +
                   $"           {cSharpMethodCode} " +
                   $"       }} " +
                   $"   }} " +
                   $"}}";
        }
    }
}