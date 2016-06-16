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
    public class MethodCodeCompiler : CompilerBase
    {
        /// <summary>
        /// Create a new instance of the MethodCodeCompiler.
        /// </summary>
        /// <returns></returns>
        public static MethodCodeCompiler Create()
        {
            return new MethodCodeCompiler();
        }
        
        /// <summary>
        /// Compile the C# source code to a new IL MethodBody, which should replace the Body of the passed MethodDefinition.
        /// </summary>
        /// <param name="cSharpMethodBodyCode"></param>
        /// <param name="methodDefinition"></param>
        /// <returns></returns>
        public MethodBody Compile(string cSharpMethodBodyCode, MethodDefinition methodDefinition)
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
        /// <param name="methodBodyCode"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private string GenerateProgramCode(string methodBodyCode, MethodDefinition method)
        {
            return Generator.GenerateProgram(method.DeclaringType, Generator.Members(
                // generate required stuff
                Generator.GenerateFieldStubs(method.DeclaringType),
                Generator.GenerateMethodStubs(method.DeclaringType, x => x.Name == method.Name || x.IsConstructor),

                // generate method
                Generator.GenerateMethod(method, methodBodyCode)
            ));
        }
    }
}