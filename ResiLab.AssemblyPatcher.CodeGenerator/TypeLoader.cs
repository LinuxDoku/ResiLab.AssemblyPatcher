using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ResiLab.AssemblyPatcher.CodeGenerator.Contracts;

namespace ResiLab.AssemblyPatcher.CodeGenerator
{
    /// <summary>
    /// Used to load types while compiling or generating references to existing source code.
    /// 
    /// This loader should contain all relevant references. Otherwise the code generation will not work.
    /// </summary>
    public class TypeLoader : ITypeLoader
    {
        private readonly List<Assembly> _loadedAssemblies;
        private readonly List<Type> _typeCache;
        private bool _typeCacheCommit = true;
        private readonly IEnumerable<string> _assemblySearchPaths;

        public TypeLoader(IEnumerable<string> assemblySearchPaths=null)
        {
            _loadedAssemblies = new List<Assembly>();
            _typeCache = new List<Type>();
            _assemblySearchPaths = assemblySearchPaths;

            AddLoadedAssemblies();
            AddAssembliesFromSearchPaths();
        }
        
        /// <summary>
        /// Get a Type by it's full type name.
        /// 
        /// The type has to exist in a loaded assembly or an assembly located in the assembly search paths.
        /// </summary>
        /// <param name="fullTypeName"></param>
        /// <returns></returns>
        public Type GetType(string fullTypeName)
        {
            return _typeCache.FirstOrDefault(x => x.FullName == fullTypeName);
        }

        /// <summary>
        /// Get MethodBase by the delcaring type full name, the method name and it's parameter types.
        /// </summary>
        /// <param name="declaringTypeFullName"></param>
        /// <param name="methodName"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public MethodBase GetMethod(string declaringTypeFullName, string methodName, Type[] parameterTypes)
        {
            var declaringType = GetType(declaringTypeFullName);
            return declaringType?.GetMethod(methodName, parameterTypes);
        }

        /// <summary>
        /// Get FieldInfo by the declaring type full naem and the field name.
        /// </summary>
        /// <param name="declaringTypeFullName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public FieldInfo GetField(string declaringTypeFullName, string fieldName)
        {
            var declaringType = GetType(declaringTypeFullName);
            return declaringType?.GetField(fieldName);
        }

        /// <summary>
        /// Add all already loaded assemblies to the loaded assemblies cache.
        /// </summary>
        private void AddLoadedAssemblies()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                LoadAssembly(assembly);
            }
        }

        /// <summary>
        /// Add all assemblies found at the search paths to the assembly cache.
        /// </summary>
        private void AddAssembliesFromSearchPaths()
        {
            if (_assemblySearchPaths == null)
            {
                return;
            }

            _typeCacheCommit = false;

            foreach (var searchPath in _assemblySearchPaths)
            {
                if (File.Exists(searchPath) == false)
                {
                    continue;
                }

                // get assemblies of this directory
                var assemblies = Directory.EnumerateFiles(searchPath, "*.*", SearchOption.TopDirectoryOnly)
                                          .Where(x => x.EndsWith(".dll") || x.EndsWith(".exe"));

                foreach (var assembly in assemblies)
                {
                    try
                    {
                        LoadAssembly(Assembly.Load(assembly));
                    }
                    catch
                    {
                    }
                }
            }

            _typeCacheCommit = true;
            RefreshTypeCache();
        }

        /// <summary>
        /// Load assembly from the assembly file path to the cache.
        /// </summary>
        /// <param name="assemblyFilePath"></param>
        protected void LoadAssembly(string assemblyFilePath)
        {
            LoadAssembly(Assembly.LoadFrom(assemblyFilePath));
        }

        /// <summary>
        /// Load assembly to assembly cache.
        /// </summary>
        /// <param name="assembly"></param>
        protected void LoadAssembly(Assembly assembly)
        {
            _loadedAssemblies.Add(assembly);
            RefreshTypeCache();
        }

        /// <summary>
        /// Refresh the type cache after loading an assembly to the assembly cache.
        /// </summary>
        private void RefreshTypeCache()
        {
            if (_typeCacheCommit == false)
            {
                return;
            }

            _typeCache.Clear();
            _typeCache.AddRange(_loadedAssemblies.SelectMany(x => x.GetTypes()));
        }
    }
}