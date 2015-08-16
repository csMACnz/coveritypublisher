using System.IO;

namespace csmacnz.CoverityPublisher.Integration.Tests.TestFramework
{
    internal static class TestFolders
    {
        private static string TempFolder
        {
            get { return Path.GetTempPath(); }
        }

        public static string DefineTempFile(string fileName)
        {
            var dirInfo = CreateTempFolder();
            return Path.GetFullPath(Path.Combine(dirInfo, fileName));
        }

        public static string CreateTempFolder()
        {
            var randomFolderName = Path.GetRandomFileName();
            var dirInfo = Directory.CreateDirectory(Path.Combine(TempFolder, randomFolderName));
            return dirInfo.FullName;
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
