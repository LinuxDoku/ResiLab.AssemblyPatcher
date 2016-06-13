using Mono.Cecil;
using ResiLab.AssemblyPatcher.CodeGenerator.Compiler;
using ResiLab.AssemblyPatcher.CodeGenerator.Extensions;

namespace ResiLab.AssemblyPatcher.CodeGenerator
{
    public class MethodInspectorResult : AssemblyInspectorResult<MethodDefinition>
    {
        public MethodInspectorResult(AssemblyInspector inspector, MethodDefinition value) : base(inspector, value) {}

        public override void Replace(string cSharpCode)
        {
            var ilProcessor = Value.Body.GetILProcessor();
            ilProcessor.Body.Instructions.Clear();

            var il = MethodCodeCompiler.Compile(cSharpCode);
            foreach (var instruction in il.Instructions) {
                if (instruction.Operand is MethodReference) {
                    var methodReference = instruction.Operand as MethodReference;
                    var methodType = Inspector.TypeLoader.GetMethod(methodReference);
                    var newMethodReference = Value.Module.Import(methodType);

                    ilProcessor.Emit(instruction.OpCode, newMethodReference);
                } else {
                    ilProcessor.Append(instruction);
                }
            }
        }

        public override void Remove()
        {
            Value.DeclaringType.Methods.Remove(Value);
        }
    }
}