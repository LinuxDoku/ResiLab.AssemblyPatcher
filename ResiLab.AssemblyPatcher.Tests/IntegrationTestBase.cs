using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ResiLab.AssemblyPatcher.Tests
{
    public abstract class IntegrationTestBase
    {
        protected static string TestDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        protected string RunExecutable(string executablePath)
        {
            var process = Process.Start(new ProcessStartInfo {
                FileName = executablePath,
                RedirectStandardOutput = true,
                UseShellExecute = false
            });
            process.WaitForExit();
            
            return process.StandardOutput.ReadToEnd();
        }
    }
}