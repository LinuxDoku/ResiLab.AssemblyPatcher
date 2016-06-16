using System.Text;

namespace ResiLab.AssemblyPatcher.CodeGenerator.Extensions
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Append a line with prefixed number of tabs.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="tabs"></param>
        /// <param name="line"></param>
        public static void AppendLine(this StringBuilder builder, int tabs, string line)
        {
            builder.AppendLine(Intent(tabs) + line);
        }

        private static string Intent(int tabs) 
        {
            var result = "";
            for (var i = 0; i < tabs; i++) 
            {
                result += "    ";
            }
            return result;
        }
    }
}