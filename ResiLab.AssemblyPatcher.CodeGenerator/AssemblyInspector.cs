using Mono.Cecil;
using System;
using ResiLab.AssemblyPatcher.CodeGenerator.Contracts;

namespace ResiLab.AssemblyPatcher.CodeGenerator
{
    /// <summary>
    /// Inspect a modify assemblies.
    /// </summary>
    public class AssemblyInspector
    {
        public AssemblyDefinition AssemblyDefinition;
        public AssemblyQuery AssemblyQuery;
        public ITypeLoader TypeLoader;

        public AssemblyInspector(string assemblyPath, ITypeLoader typeLoader)
        {
            AssemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath);
            TypeLoader = typeLoader;
            AssemblyQuery = new AssemblyQuery(AssemblyDefinition);
        }

        /// <summary>
        /// Query a method and provide code modification tools.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public MethodInspectorResult Method(Func<AssemblyQuery, MethodDefinition> query)
        {
            return new MethodInspectorResult(this, query(AssemblyQuery));
        }

        /// <summary>
        /// Query a property and provide code modification tools.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public PropertyInspectorResult Property(Func<AssemblyQuery, PropertyDefinition> query)
        {
            return new PropertyInspectorResult(this, query(AssemblyQuery));
        }

        /// <summary>
        /// Save the modified assembly with a new name or at a new path.
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveAs(string fileName)
        {
            AssemblyDefinition.Write(fileName);
        }
    }
}