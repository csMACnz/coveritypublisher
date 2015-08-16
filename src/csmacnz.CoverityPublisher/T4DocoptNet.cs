
using System.Collections.Generic;
using DocoptNet;

namespace csmacnz.CoverityPublisher
{
    // Generated class for Main.usage.txt
    public class MainArgs
    {
        public const string Usage = @"PublishCoverity - a simple command-line publishing tool for Coverity Scan - Static Analysis results.

Usage:
  PublishCoverity compress [-o <file>] [-i <folder>] [--overwrite] [--abortOnFailures] [--dryrun] [--nologo]
  PublishCoverity publish -t <token> -e <email> [-r <name>] [-z <file>] [-d <desc>] [--codeVersion <version>] [--dryrun] [--nologo]
  PublishCoverity --version
  PublishCoverity --help


Options:
 -i <folder>, --directory <folder>        The folder to zip up for coverity. [default: cov-int]
 -o <file>, --output <file>               The location to save the zip file to. [default: coverity.zip]
 --overwrite                              If provided, will automatically overwrite the output file location.
 --abortOnFailures                        If Coverity Scan has reported any errors, abort the compression.
 -z <file>, --zip <file>                  The zip file to upload. [default: coverity.zip]
 -r <name>, --repoName <name>             Your repository name in the form of USER/REPO. If missing, will attempt to resolve using Environment variables. [default: ]
 -e <email>, --email <name>               The email address to notify of the scan.
 -t <token>, --token <token>              Your Coverity token.
 -d <desc>, --description <desc>          The optional description you want to pass to coverity.  [default: Published by PublishCoverity.exe]
 --codeVersion <version>                  The version of the application to report the coverage for. [default: 0.1.0]
 --dryrun                                 Does everything except the post to Coverity. Useful for testing.
 --nologo                                 If this is set, will not show the logo when run.
 -h, --help                               Show this screen.
 --version                                Show version.

What its for:
 With compress, you can produce the zip file from your cov-int results folder.
 With publish, it reads your results zip, and submits it to
 Coverity Scan Static Analysis service.
 These can be used by your build scripts or with a CI builder server.
";
        private readonly IDictionary<string, ValueObject> _args;
        public MainArgs(ICollection<string> argv, bool help = true,
                                                      object version = null, bool optionsFirst = false, bool exit = false)
        {
            _args = new Docopt().Apply(Usage, argv, help, version, optionsFirst, exit);
        }

        public IDictionary<string, ValueObject> Args
        {
            get { return _args; }
        }

        public bool CmdCompress { get { return _args["compress"].IsTrue; } }
        public string OptOutput { get { return _args["--output"].ToString(); } }
        public string OptDirectory { get { return _args["--directory"].ToString(); } }
        public bool OptOverwrite { get { return _args["--overwrite"].IsTrue; } }
        public bool OptAbortonfailures { get { return _args["--abortOnFailures"].IsTrue; } }
        public bool OptDryrun { get { return _args["--dryrun"].IsTrue; } }
        public bool OptNologo { get { return _args["--nologo"].IsTrue; } }
        public bool CmdPublish { get { return _args["publish"].IsTrue; } }
        public string OptToken { get { return _args["--token"].ToString(); } }
        public string OptEmail { get { return _args["--email"].ToString(); } }
        public string OptReponame { get { return _args["--repoName"].ToString(); } }
        public string OptZip { get { return _args["--zip"].ToString(); } }
        public string OptDescription { get { return _args["--description"].ToString(); } }
        public string OptCodeversion { get { return _args["--codeVersion"].ToString(); } }
        public bool OptVersion { get { return _args["--version"].IsTrue; } }
        public bool OptHelp { get { return _args["--help"].IsTrue; } }
    }

}

