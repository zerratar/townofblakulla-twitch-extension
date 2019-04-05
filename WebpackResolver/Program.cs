using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WebpackResolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Webpack.Resolve("G:\\git\\townofblakulla\\TownOfBlakulla");
        }
    }

    public class CommandManager
    {
        public static bool Run(string cmd, string args, string workingDirectory, out string output)
        {
            var process = new Process();
            process.StartInfo.FileName = "cmd";
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.Arguments = $"/c {cmd} {args}"; // Note the /c command (*)
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            //* Read the output (or the error)
            var outputError = "";
            var outputValue = "";
            process.OutputDataReceived += (s, e) =>
            {
                Console.WriteLine(e.Data);
                outputValue += e.Data;
            };

            process.ErrorDataReceived += (s, e) =>
            {
                Console.WriteLine(e.Data);
                outputError += e.Data;
            };

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            //var output = process.StandardOutput.ReadToEnd();
            //var err = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(outputError))
            {
                output = outputError;
                return false;
            }

            output = outputValue;
            return true;
        }
    }

    public class Webpack
    {
        public static void Resolve(string projectFolder)
        {
            while (!CommandManager.Run("webpack", "", projectFolder, out var error))
            {
                var moduleName = error.Split("find module '")[1].Split("'")[0];
                CommandManager.Run($"npm install {moduleName} -g", "", projectFolder, out _);
            }
        }
    }
}
