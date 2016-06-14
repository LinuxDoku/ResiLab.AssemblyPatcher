# ResiLab AssemblyPatcher
The AssemblyPatcher is under active development, and the aim is to create a tool, which allows
easy .NET assembly modification without struggling with IL code.

Write a query with some easy macro instructions to target the method or field, which should be replaced,
deleted or add a whole new method to an existing type with plain C# which is compiled by Roslyn.

At the moment only an early version of the .NET library exists, which is the core for the later 
macro tool. At the moment it could help your to weaving .NET assemblys without purchasing 
expensive tools for this job ;-)

## Example
Here we replace the Main method of an assembly with some new code:

```csharp
var inspector = new AssemblyInspector("HelloWorld.exe");
  
inspector.Method(x => x.FindMethod("HelloWorld.Program", "Main"))
        .Replace("System.Console.WriteLine(\"This is a patched Hello World!\");");
  
inspector.SaveAs("HelloWorld.Patched.exe");
```

Here we delete a method from an assembly:

```csharp
var inspector = new AssemblyInspector("HelloWorld.exe");

inspector.Method(x => x.FindMethod("HelloWorld.Program", "GenerateHelloWorld")).Delete();

inspector.SaveAs("HelloWorld.Patched.exe");
```

## Features

- [x] Query Assemblies
- [x] Save modified assemblies
- [ ] Methods
	- [ ] Add Methods
	- [x] Delete Methods
	- [x] Replace Method Body with C# Code
	  - [x] Call Methods of an external assembly
	  - [x] Call Methods of same assembly
	  - [x] Call Methods of same class
	  - [x] Reference a field
	  - [x] Static and instance support
	  - [ ] Support for Generics
- [ ] Properties
	- [ ] Add Properties
	- [x] Delete Properties
	- [ ] Replace Property Body
		- [ ] Replace Get Method
		- [ ] Replace Set Method
- [ ] Fields
	- [ ] Add Fields
	- [x] Delete Fields
	- [ ] Replace Field Value