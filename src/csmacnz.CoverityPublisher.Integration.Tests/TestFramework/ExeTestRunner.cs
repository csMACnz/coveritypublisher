using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit.Sdk;

namespace csmacnz.CoverityPublisher.Integration.Tests.TestFramework
{
    public static class ExeTestRunner
    {
        private const string Exe = "PublishCoverity.exe";

        public static RunResults RunExe(string arguments)
        {
            var exePath = Path.Combine(GetExePath(), Exe);
            var argumentsToUse = arguments;
            var fileNameToUse = exePath;
            if (Environment.GetEnvironmentVariable("MONO_INTEGRATION_MODE") == "True")
            {
                fileNameToUse = GetMonoPath();

                argumentsToUse = exePath + " " + arguments;
            }

            var process = new Process();
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = fileNameToUse,
                Arguments = argumentsToUse,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };
            process.StartInfo = startInfo;

            string results;
            string errorsResults;
            int exitCode;
            using (process)
            {
                process.Start();

                results = process.StandardOutput.ReadToEnd();
                errorsResults = process.StandardError.ReadToEnd();
                Console.WriteLine(results);

                const int timeoutInMilliseconds = 10000;
                if (!process.WaitForExit(timeoutInMilliseconds))
                {
                    throw new XunitException(string.Format("Test execution time exceeded: {0}ms", timeoutInMilliseconds));
                }
                exitCode = process.ExitCode;
            }

            return new RunResults
            {
                StandardOutput = results,
                StandardError = errorsResults,
                ExitCode = exitCode
            };
        }

        private static string GetMonoPath()
        {
            var monoApp = "mono";
            var monoPath = Environment.GetEnvironmentVariable("MONO_INTEGRATION_MONOPATH");
            if (!string.IsNullOrWhiteSpace(monoPath))
            {
                monoApp = monoPath + Path.DirectorySeparatorChar + "mono";
            }
            return monoApp;
        }

        private static string GetExePath()
        {
#if DEBUG
            const string configuration = "Debug";
#else
            const string configuration = "Release";
#endif
            return Path.Combine("..", "..", "..", "csmacnz.CoverityPublisher", "bin", configuration);
        }
    }
}