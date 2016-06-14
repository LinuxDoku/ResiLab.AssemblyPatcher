using System;
using Mono.Cecil;

namespace ResiLab.AssemblyPatcher.CodeGenerator
{
    public class FieldInspectorResult : AssemblyInspectorResult<FieldDefinition>
    {
        public FieldInspectorResult(AssemblyInspector inspector, FieldDefinition value) : base(inspector, value) {}

        /// <summary>
        /// Replace the Member code with the given C# code.
        /// </summary>
        /// <param name="cSharpCode"></param>
        public override void Replace(string cSharpCode)
        {
            // TOOD: implement code replacemenet
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove the Member from the assembly.
        /// </summary>
        public override void Remove()
        {
            Value.DeclaringType.Fields.Remove(Value);
        }
    }
}