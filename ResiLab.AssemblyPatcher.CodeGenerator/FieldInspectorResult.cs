using System;
using Mono.Cecil;

namespace ResiLab.AssemblyPatcher.CodeGenerator
{
    public class FieldInspectorResult : AssemblyInspectorResult<FieldDefinition>
    {
        public FieldInspectorResult(AssemblyInspector inspector, FieldDefinition value) : base(inspector, value) {}

        /// <summary>
        /// Remove the Member from the assembly.
        /// </summary>
        public override void Remove()
        {
            Value.DeclaringType.Fields.Remove(Value);
        }
    }
}