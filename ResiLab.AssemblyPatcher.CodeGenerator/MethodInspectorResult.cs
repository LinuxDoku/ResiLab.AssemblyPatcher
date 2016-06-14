using Mono.Cecil;
using ResiLab.AssemblyPatcher.CodeGenerator.Compiler;

namespace ResiLab.AssemblyPatcher.CodeGenerator
{
    public class MethodInspectorResult : AssemblyInspectorResult<MethodDefinition>
    {
        public MethodInspectorResult(AssemblyInspector inspector, MethodDefinition value) : base(inspector, value) {}

        /// <summary>
        /// Replace the Member code with the given C# code.
        /// </summary>
        /// <param name="cSharpCode"></param>
        public override void Replace(string cSharpCode)
        {
            var il = MethodCodeCompiler.Compile(cSharpCode, Value);
            var ilProcessor = Value.Body.GetILProcessor();

            ReplaceInstructions(ilProcessor, il.Instructions);
        }

        /// <summary>
        /// Remove the Member from the assembly.
        /// </summary>
        public override void Remove()
        {
            Value.DeclaringType.Methods.Remove(Value);
        }
    }
}