using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ResiLab.AssemblyPatcher.CodeGenerator.Extensions;

namespace ResiLab.AssemblyPatcher.CodeGenerator
{
    /// <summary>
    /// Query result of the inspector, which is able to modify the code.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AssemblyInspectorResult<T> where T : MemberReference
    {
        protected AssemblyInspectorResult(AssemblyInspector inspector, T value)
        {
            Inspector = inspector;
            Value = value;
        }

        /// <summary>
        /// Assembly Inspector.
        /// </summary>
        public AssemblyInspector Inspector { get; }

        /// <summary>
        /// Member Defition
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Replace the Member code with the given C# code.
        /// </summary>
        /// <param name="cSharpCode"></param>
        public abstract void Replace(string cSharpCode);

        /// <summary>
        /// Remove the Member from the assembly.
        /// </summary>
        public abstract void Remove();

        /// <summary>
        /// Replace the IL Instructions of the ILProcessor body with the passed instructions.
        /// 
        /// The references are processed by the reference processor, to ensure integrity.
        /// </summary>
        /// <param name="ilProcessor"></param>
        /// <param name="instructions"></param>
        protected void ReplaceInstructions(ILProcessor ilProcessor, IEnumerable<Instruction> instructions) {
            ilProcessor.Body.Instructions.Clear();

            foreach (var instruction in instructions) {
                ilProcessor.Append(ProcessInstruction(instruction));
            }
        }

        /// <summary>
        /// Process the instruction and update references, otherwise return this instruction.
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns></returns>
        protected Instruction ProcessInstruction(Instruction instruction) {
            // Methods
            var methodReference = instruction.Operand as MethodReference;
            if (methodReference != null) {
                MethodReference newMethodReference = GetOriginalMethodReference(methodReference);

                // external reference
                if (newMethodReference == null) {
                    var methodBase = Inspector.TypeLoader.GetMethod(methodReference);
                    newMethodReference = Value.Module.Import(methodBase);
                }

                return Instruction.Create(instruction.OpCode, newMethodReference);
            }

            // Fields
            var fieldReference = instruction.Operand as FieldReference;
            if (fieldReference != null) {
                FieldReference newFieldReference = GetOriginalFieldReference(fieldReference);

                // external reference
                if (newFieldReference == null) {
                    var fieldInfo = Inspector.TypeLoader.GetField(fieldReference);
                    newFieldReference = Value.Module.Import(fieldInfo);
                }

                return Instruction.Create(instruction.OpCode, newFieldReference);
            }

            // normal Instruction
            return instruction;
        }

        /// <summary>
        /// Get the MethodReference in the inspected assembly by MethodReference from another assembly (the fresh compiled one).
        /// </summary>
        /// <param name="methodReference"></param>
        /// <returns></returns>
        protected MethodReference GetOriginalMethodReference(MethodReference methodReference)
        {
            return FindType(methodReference)?.Methods.FirstOrDefault(x => x.Name == methodReference.Name);
        }

        /// <summary>
        /// Get the FieldReference in the inspected assembly by FieldReference from another assembly (the fresh compiled one).
        /// </summary>
        /// <param name="fieldReference"></param>
        /// <returns></returns>
        protected FieldReference GetOriginalFieldReference(FieldReference fieldReference)
        {
            return FindType(fieldReference)?.Fields.FirstOrDefault(x => x.Name == fieldReference.Name);
        }

        /// <summary>
        /// Find the DeclaringType of a MemberReference in the inspected assembly.
        /// </summary>
        /// <param name="memberReference"></param>
        /// <returns></returns>
        protected TypeDefinition FindType(MemberReference memberReference)
        {
            return Inspector.AssemblyQuery.FindType(memberReference.DeclaringType.FullName);
        }
    }
}