﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using BCLExtensions;

namespace csmacnz.CoverityPublisher
{
    public class Program
    {
        public static void Main(string[] argv)
        {
            var args = new MainArgs(argv, exit: true, version: GetDisplayVersion());
            if (!args.OptNologo)
            {
                PrintLogo();
            }
            if (args.CmdPublish)
            {
                var payload = ParsePublishInput(args);

                var results  =CoveritySubmitter.Submit(payload);
                ProcessResult(results);
            }
            else if (args.CmdCompress)
            {
                var payload = ParseCompressPayload(args);
                var results = ZipCompressor.Compress(payload);
                ProcessResult(results);
            }
        }

        private static string GetDisplayVersion()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion;
        }

        private static CompressPayload ParseCompressPayload(MainArgs args)
        {
            var payload = new CompressPayload
            {
                Output = args.OptOutput,
                Directory = args.OptDirectory,
                OverwriteExistingFile = args.OptOverwrite,
                ProduceZipFile = !args.OptDryrun,
                AbortOnFailures = args.OptAbortonfailures
            };
            return payload;
        }

        private static void ProcessResult(ActionResult results)
        {
            if (results.Successful)
            {
                Console.WriteLine(results.Message);
            }
            else
            {
                Console.Error.WriteLine(results.Message);
                Environment.Exit(1);
            }
        }

        private static void PrintLogo()
        {
            Console.WriteLine(@"
  _____       _     _ _     _      _____                    _ _
 |  __ \     | |   | (_)   | |    / ____|                  (_) |
 | |__) |   _| |__ | |_ ___| |__ | |     _____   _____ _ __ _| |_ _   _
 |  ___/ | | | '_ \| | / __| '_ \| |    / _ \ \ / / _ \ '__| | __| | | |
 | |   | |_| | |_) | | \__ \ | | | |___| (_) \ V /  __/ |  | | |_| |_| |
 |_|    \__,_|_.__/|_|_|___/_| |_|\_____\___/ \_/ \___|_|  |_|\__|\__, |
                                                                   __/ |
                                                                  |___/
");
        }

        private static PublishPayload ParsePublishInput(MainArgs args)
        {
            string coverityFileName = args.OptZip;
            if (!File.Exists(coverityFileName))
            {
                Console.Error.WriteLine("Input file '{0}' cannot be found.", coverityFileName);
                Environment.Exit(1);
            }

            string repoName = args.OptReponame.Unquoted();
            if (repoName.IsNullOrWhitespace())
            {
                repoName = Environment.GetEnvironmentVariable("APPVEYOR_REPO_NAME").Unquoted();
                if (repoName.IsNullOrWhitespace())
                {
                    Console.Error.WriteLine("No repository name provided, and could not be resolved.");
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("Resolved repository name '{0}' from $env:APPVEYOR_REPO_NAME", repoName);
                }
            }
            
            string coverityToken = args.OptToken;
            string description = args.OptDescription;
            string email = args.OptEmail;
            if (!Regex.IsMatch(email, "^[^@]+@[^@]+$"))
            {
                Console.Error.WriteLine("Invalid email '{0}' provided.", email);
                Environment.Exit(1);
            }
            string version = args.OptCodeversion;
            bool dryrun = args.OptDryrun;
            var payload = new PublishPayload
            {
                FileName = coverityFileName,
                RepositoryName = repoName,
                Token = coverityToken,
                Email = email,
                Version = version,
                Description = description,
                SubmitToCoverity = !dryrun
            };
            return payload;
        }
    }
}
