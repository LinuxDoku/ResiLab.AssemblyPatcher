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
        public void Replace(string cSharpCode)
        {
            ReplaceMethodBody(Value, cSharpCode);
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