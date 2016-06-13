using ResiLab.AssemblyPatcher.CodeGenerator;

namespace ResiLab.AssemblyPatcher {
    public class Program {
        public static void Main(string[] args)
        {
            var inspector = new AssemblyInspector("../../../HelloWorld/bin/Debug/HelloWorld.exe");

            inspector.Method(x => x.FindMethod("HelloWorld.Program", "Main")).Replace("System.Console.WriteLine(\"This is a patched Hello World!\");");

            inspector.SaveAs("HelloWorld.Patched.exe");
        }
    }
}
