
using System.Collections;
using System.Collections.Generic;
using DocoptNet;

namespace csmacnz.CoverityPublisher
{
    // Generated class for Main.usage.txt
    public class MainArgs
    {
        public const string USAGE = @"PublishCoverity - a simple command-line publishing tool for Coverity Scan - Static Analysis results.

Usage:
  PublishCoverity -z <file> -r <name> -t <token> -e <email> [-d <desc>] [--codeVersion <version>]
  PublishCoverity --version
  PublishCoverity --help


Options:
 -z <file>, --zip <file>         The zip file to upload.
 -r <name>, --repoName <name>    Your repository name in the form of USER/REPO.
 -e <email>, --email <name>      The email address to notify of the scan.
 -t <token>, --token <token>     Your Coverity token.
 -d <desc>, --description <desc> The optional description you want to pass to coverity.  [default: Published by PublishCoverity.exe]
 --codeVersion <version>         The version of the application to report the coverage for.
 -h, --help                      Show this screen.
 --version                       Show version.

What its for:
 Reads your cov-build results as a zip, and submits it to
 Coverity Scan Static Analysis service. This can be used by your build 
 scripts or with a CI builder server.
";
        private readonly IDictionary<string, ValueObject> _args;
        public MainArgs(ICollection<string> argv, bool help = true,
                                                      object version = null, bool optionsFirst = false, bool exit = false)
        {
            _args = new Docopt().Apply(USAGE, argv, help, version, optionsFirst, exit);
        }

        public IDictionary<string, ValueObject> Args
        {
            get { return _args; }
        }

		public string OptZip { get { return _args["--zip"].ToString(); } }
		public string OptReponame { get { return _args["--repoName"].ToString(); } }
		public string OptToken { get { return _args["--token"].ToString(); } }
		public string OptEmail { get { return _args["--email"].ToString(); } }
		public string OptDescription { get { return _args["--description"].ToString(); } }
		public string OptCodeversion { get { return _args["--codeVersion"].ToString(); } }
		public bool OptVersion { get { return _args["--version"].IsTrue; } }
		public bool OptHelp { get { return _args["--help"].IsTrue; } }
	
    }

	
}

