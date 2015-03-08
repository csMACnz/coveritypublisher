using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace csmacnz.CoverityPublisher.Integration.Tests.TestFramework
{
    internal static class TestFolders
    {
        public static readonly string UniqueId = DateTime.Now.ToString("s", CultureInfo.InvariantCulture).Replace(":", "");

        public static string InputFolder
        {
            get { return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath); }
        }

        public static string OutputFolder
        {
            //a simple solution that puts everything to the output folder directly would look like this: get { return Directory.GetCurrentDirectory(); }
            get
            {
                var outputFolder = Path.Combine(Directory.GetCurrentDirectory(), UniqueId);
                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);
                return outputFolder;
            }
        }

        public static string TempFolder
        {
            get { return Path.GetTempPath(); }
        }

        // very simple helper methods that can improve the test code readability

        public static string GetInputFilePath(string fileName)
        {
            return Path.GetFullPath(Path.Combine(InputFolder, fileName));
        }

        public static string GetOutputFilePath(string fileName)
        {
            return Path.GetFullPath(Path.Combine(OutputFolder, fileName));
        }

        public static string DefineTempFile(string fileName)
        {
            var randomFolderName = Path.GetRandomFileName();
            var dirInfo = Directory.CreateDirectory(Path.Combine(TempFolder, randomFolderName));
            return Path.GetFullPath(Path.Combine(dirInfo.FullName, fileName));
        }

        public static string CreateTempFile(string extension)
        {
            var path = Path.GetTempFileName();
            var newPath = path.Replace(".tmp", "." + extension);
            File.Move(path, newPath);
            return newPath;
        }
    }
}
