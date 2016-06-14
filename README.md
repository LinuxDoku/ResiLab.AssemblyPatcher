# ResiLab AssemblyPatcher

This is a first draft version, which was coded in 3 hours.
In future support for variable/property refereces,
imports and more data structures and their respective access modifiers is planned.

## Example
Here we replace the Main method of an assembly with some new code:

```csharp
var inspector = new AssemblyInspector("HelloWorld.exe");
  
inspector.Method(x => x.FindMethod("HelloWorld.Program", "Main"))
        .Replace("System.Console.WriteLine(\"This is a patched Hello World!\");");
  
inspector.SaveAs("HelloWorld.Patched.exe");
```