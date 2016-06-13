using Mono.Cecil;

namespace ResiLab.AssemblyPatcher.CodeGenerator
{
    public class PropertyInspectorResult : AssemblyInspectorResult<PropertyDefinition>
    {
        public PropertyInspectorResult(AssemblyInspector inspector, PropertyDefinition value) : base(inspector, value) {}

        public override void Replace(string cSharpCode)
        {
            // TODO: implement code generation for properties
            throw new System.NotImplementedException();
        }

        public override void Remove()
        {
            Value.DeclaringType.Properties.Remove(Value);
        }
    }
}