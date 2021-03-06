﻿using System.IO;
using NUnit.Framework;
using ResiLab.AssemblyPatcher.CodeGenerator;

namespace ResiLab.AssemblyPatcher.Tests
{
    [TestFixture(Category = "AssemblyPatcherIntegration")]
    public class AssemblyPatcherIntegrationTests : IntegrationTestBase
    {
        private static readonly string SourceFile = Path.Combine(TestDirectory, "Static/HelloWorld.exe");
        private static readonly string PatchedFile = Path.Combine(TestDirectory, "HelloWorld.Patched.exe");

        [SetUp]
        public void Setup()
        {
            if (File.Exists(PatchedFile))
            {
                File.Delete(PatchedFile);
            }
        }

        [Test]
        public void Should_Replace_Method_Body_With_Method_Call()
        {
            // patch the assembly
            var typeLoader = new TypeLoader();
            var inspector = new AssemblyInspector(SourceFile, typeLoader);

            inspector.Method(x => x.FindMethod("HelloWorld.Program", "Main")).Replace("System.Console.WriteLine(\"This is a patched Hello World!\");");

            inspector.SaveAs(PatchedFile);

            // execute the patched assembly
            var output = RunExecutable(PatchedFile);

            // validate std out
            Assert.AreEqual("This is a patched Hello World!\r\n", output);
        }

        [Test]
        public void Should_Replace_Method_Body_With_Property_Access()
        {
            // patch the assembly
            var typeLoader = new TypeLoader();
            var inspector = new AssemblyInspector(SourceFile, typeLoader);

            inspector.Method(x => x.FindMethod("HelloWorld.Program", "Main")).Replace("System.Console.WriteLine(_helloWorld + _helloWorld);");

            inspector.SaveAs(PatchedFile);

            // execute the patched assembly
            var output = RunExecutable(PatchedFile);

            // validate std out
            Assert.AreEqual("Hello WorldHello World\r\n", output);
        }

        [Test]
        public void Should_Replace_Method_Body_With_Method_Call_And_Method_Call_To_Class_Member() {
            // patch the assembly
            var typeLoader = new TypeLoader();
            var inspector = new AssemblyInspector(SourceFile, typeLoader);

            inspector.Method(x => x.FindMethod("HelloWorld.Program", "Main")).Replace("System.Console.WriteLine(GenerateHelloWorld());");

            inspector.SaveAs(PatchedFile);

            // execute the patched assembly
            var output = RunExecutable(PatchedFile);

            // validate std out
            Assert.AreEqual("Hello World\r\n", output);
        }

        [Test]
        public void Should_Replace_Property_Getter_Body()
        {
            // patch the assembly
            var typeLoader = new TypeLoader();
            var inspector = new AssemblyInspector(SourceFile, typeLoader);

            inspector.Property(x => x.FindProperty("HelloWorld.Program", "HelloWorldProperty")).ReplaceGet("return \"This is a Test\";");
            inspector.Method(x => x.FindMethod("HelloWorld.Program", "Main")).Replace("System.Console.WriteLine(HelloWorldProperty);");

            inspector.SaveAs(PatchedFile);

            // execute the patched assembly
            var output = RunExecutable(PatchedFile);

            // validate std out
            Assert.AreEqual("This is a Test\r\n", output);
        }
    }
}