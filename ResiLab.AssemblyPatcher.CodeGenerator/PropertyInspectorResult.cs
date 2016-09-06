using Mono.Cecil;

namespace ResiLab.AssemblyPatcher.CodeGenerator
{
    public class PropertyInspectorResult : AssemblyInspectorResult<PropertyDefinition>
    {
        public PropertyInspectorResult(AssemblyInspector inspector, PropertyDefinition value) : base(inspector, value) {}

        /// <summary>
        /// Replace the body of the getter.
        /// </summary>
        /// <param name="cSharpCode"></param>
        public void ReplaceGet(string cSharpCode)
        {
            ReplaceMethodBody(Value.GetMethod, cSharpCode);
        }

        /// <summary>
        /// Replace the body of the setter.
        /// </summary>
        /// <param name="cSharpCode"></param>
        public void ReplaceSet(string cSharpCode)
        {
            ReplaceMethodBody(Value.SetMethod, cSharpCode);
        }
        
        public override void Remove()
        {
            Value.DeclaringType.Properties.Remove(Value);
        }
    }
}