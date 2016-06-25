using Microsoft.CodeAnalysis;
using Mono.Cecil;

namespace ResiLab.AssemblyPatcher.CodeGenerator.Extensions
{
    public static class MethodDefinitionExtensions
    {
        public static Accessibility ToAccessibility(this MethodDefinition method)
        {
            if (method.IsPrivate) {
                return Accessibility.Private;
            }

            if (method.IsPublic) {
                return Accessibility.Public;
            }

            if (method.IsAssembly) {
                return Accessibility.Internal;
            }

            if (method.IsFamily) {
                return Accessibility.Protected;
            }
            
            return Accessibility.Public;
        }
    }
}