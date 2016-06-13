using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using ResiLab.AssemblyPatcher.CodeGenerator;

namespace ResiLab.AssemblyPatcher.Tests
{
    public class AssemblyPatcherIntegrationTests
    {
        [Test]
        public void Should_Replace_Method_Body()
        {
            var dir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            var sourceFile = Path.Combine(dir, "Static/HelloWorld.exe");
            var patchedFile = Path.Combine(dir, "HelloWorld.Patched.exe");

            // patch the assembly
            var typeLoader = new TypeLoader();
            var inspector = new AssemblyInspector(sourceFile, typeLoader);

            inspector.Method(x => x.FindMethod("HelloWorld.Program", "Main")).Replace("System.Console.WriteLine(\"This is a patched Hello World!\");");

            inspector.SaveAs(patchedFile);

            // execute the patched assembly
            var process = Process.Start(new ProcessStartInfo {
                FileName = patchedFile,
                RedirectStandardOutput = true,
                UseShellExecute = false
            });
            process.WaitForExit(1000);
            var output = process.StandardOutput.ReadToEnd();

            // validate std out
            Assert.AreEqual("This is a patched Hello World!\r\n", output);
        }
    }
}