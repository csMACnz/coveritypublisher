
using System.Collections;
using System.Collections.Generic;
using DocoptNet;

namespace csmacnz.CoverityPublisher
{
    // Generated class for Main.usage.txt
    public class MainArgs
    {
        public const string USAGE = @"Example usage for T4 Docopt.NET

Usage:
  prog command ARG <myarg> [OPTIONALARG] [-o -s=<arg> --long=ARG --switch]
  prog files FILE...

Options:
 -o           Short switch.
 -s=<arg>     Short option with arg.
 --long=ARG   Long option with arg.
 --switch     Long switch.

Explanation:
 This is an example usage file that needs to be customized.
 Every time you change this file, run the Custom Tool command
 on T4DocoptNet.tt to re-generate the MainArgs class
 (defined in T4DocoptNet.cs).
 You can then use the MainArgs classed as follows:

    class Program
    {

       static void DoStuff(string arg, bool flagO, string longValue)
       {
         // ...
       }

        static void Main(string[] argv)
        {
            // Automatically exit(1) if invalid arguments
            var args = new MainArgs(argv, exit: true);
            if (args.CmdCommand)
            {
                Console.WriteLine(""First command"");
                DoStuff(args.ArgArg, args.OptO, args.OptLong);
            }
        }
    }

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

		public bool CmdCommand { get { return _args["command"].IsTrue; } }
		public string ArgArg { get { return _args["ARG"].ToString(); } }
		public string ArgMyarg  { get { return _args["<myarg>"].ToString(); } }
		public string ArgOptionalarg { get { return _args["OPTIONALARG"].ToString(); } }
		public bool OptO { get { return _args["-o"].IsTrue; } }
		public string OptS { get { return _args["-s"].ToString(); } }
		public string OptLong { get { return _args["--long"].ToString(); } }
		public bool OptSwitch { get { return _args["--switch"].IsTrue; } }
		public bool CmdFiles { get { return _args["files"].IsTrue; } }
		public ArrayList ArgFile { get { return _args["FILE"].AsList; } }
	
    }

	
}

