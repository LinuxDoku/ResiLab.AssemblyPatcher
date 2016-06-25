using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Mono.Cecil;
using NUnit.Framework;
using ResiLab.AssemblyPatcher.CodeGenerator.Extensions;

namespace ResiLab.AssemblyPatcher.Tests.Extensions
{
    internal class AccessibilityTestClass
    {
        public static string PrivateMethodName = nameof(PrivateMethod);
        public static string ProtectedMethodName = nameof(ProtectedMethod);

        public void PublicMethod() { }
        private void PrivateMethod() { }
        protected void ProtectedMethod() { }
        internal void InternalMethod() { }
    }

    public class MethodDefinitionExtensionsTests
    {
        private TypeDefinition GetTypeDefinition()
        {
            var assembly = AssemblyDefinition.ReadAssembly(Assembly.GetExecutingAssembly().Location);
            return assembly.MainModule.GetType(typeof(AccessibilityTestClass).FullName);
        }

        private MethodDefinition GetMethod(string name)
        {
            return GetTypeDefinition().Methods.First(x => x.Name == name);
        }

        [Test]
        public void Should_Detect_Public_Accessibility()
        {
            Assert.AreEqual(Accessibility.Public, GetMethod(nameof(AccessibilityTestClass.PublicMethod)).ToAccessibility());
        }

        [Test]
        public void Should_Detect_Private_Accessibility()
        {
            Assert.AreEqual(Accessibility.Private, GetMethod(AccessibilityTestClass.PrivateMethodName).ToAccessibility());
        }

        [Test]
        public void Should_Detect_Protected_Accessibility()
        {
            Assert.AreEqual(Accessibility.Protected, GetMethod(AccessibilityTestClass.ProtectedMethodName).ToAccessibility());
        }

        [Test]
        public void Should_Detect_Internal_Accessibility() {
            Assert.AreEqual(Accessibility.Internal, GetMethod(nameof(AccessibilityTestClass.InternalMethod)).ToAccessibility());
        }
    }
}