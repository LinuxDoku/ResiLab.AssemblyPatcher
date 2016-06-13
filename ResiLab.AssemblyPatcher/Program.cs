using ResiLab.AssemblyPatcher.CodeGenerator;

namespace ResiLab.AssemblyPatcher {
    public class Program {
        public static void Main(string[] args)
        {
            var typeLoader = new TypeLoader();
            var inspector = new AssemblyInspector("../../../HelloWorld/bin/Debug/HelloWorld.exe", typeLoader);

            inspector.Method(x => x.FindMethod("HelloWorld.Program", "Main")).Replace("System.Console.WriteLine(\"This is a patched Hello World!\");");

            inspector.SaveAs("HelloWorld.Patched.exe");
        }
    }
}
