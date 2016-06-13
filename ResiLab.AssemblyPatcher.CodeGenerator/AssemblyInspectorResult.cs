using Mono.Cecil;

namespace ResiLab.AssemblyPatcher.CodeGenerator
{
    /// <summary>
    /// Query result of the inspector, which is able to modify the code.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AssemblyInspectorResult<T> where T : IMemberDefinition
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
    }
}