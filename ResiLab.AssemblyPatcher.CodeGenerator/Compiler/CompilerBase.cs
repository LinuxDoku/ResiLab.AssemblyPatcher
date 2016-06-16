using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace ResiLab.AssemblyPatcher.CodeGenerator.Compiler
{
    public abstract class CompilerBase
    {
        protected readonly CSharpCodeGenerator Generator;

        protected CompilerBase()
        {
            Generator = new CSharpCodeGenerator();
        }
    }
}