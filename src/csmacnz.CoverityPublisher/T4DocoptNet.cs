
using System.Collections;
using System.Collections.Generic;
using DocoptNet;

namespace csmacnz.CoverityPublisher
{
    // Generated class for Main.usage.txt
    public class MainArgs
    {
        public const string USAGE = @"PublishCoverity - a simple command-line publishing tool for coverity's static analysis results.

Usage:
  PublishCoverity 
  PublishCoverity --version
  PublishCoverity --help


Options:
 -h, --help   Show this screen.
 --version    Show version.

What its for:
 Reads your coverity static anaysis results as a zip, and submits it to
 coverity's service. This can be used by your build scripts or with a
 CI builder server.
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

		public bool OptVersion { get { return _args["--version"].IsTrue; } }
		public bool OptHelp { get { return _args["--help"].IsTrue; } }
	
    }

	
}

