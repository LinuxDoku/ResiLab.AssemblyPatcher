using System;

namespace ResiLab.AssemblyPatcher.CodeGenerator.Compiler
{
    public class GeneratorUtil
    {
        /// <summary>
        /// Format the lines of a source code.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static string Format(params string[] lines) {
            return string.Join(Environment.NewLine, lines) + Environment.NewLine;
        }
    }
}