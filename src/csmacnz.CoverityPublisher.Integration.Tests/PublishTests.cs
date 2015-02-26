using System.IO;
using csmacnz.CoverityPublisher.Integration.Tests.TestFramework;
using Xunit;

namespace csmacnz.CoverityPublisher.Integration.Tests
{
    public class PublishTests
    {
        [Fact]
        public void FileDoesntExist()
        {
            var results = RunExe("doesntExist.zip");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input file 'doesntExist.zip' cannot be found", results.StandardError);
        }

        [Fact]
        public void FileInQuotesDoesntExist()
        {
            var results = RunExe("\"doesntExist.zip\"");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input file 'doesntExist.zip' cannot be found.", results.StandardError);
        }


        [Fact]
        public void NameWithoutSlashFails()
        {
            var testfilePath = CreateTempFile("test.zip");

            var results = RunExe(testfilePath, "USERREPO");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Invalid repository name 'USERREPO' provided.", results.StandardError);
        }

        [Fact]
        public void EmptyStringNameFails()
        {
            var testfilePath = CreateTempFile("test.zip");

            var results = RunExe(testfilePath, "\"\"");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Invalid repository name '' provided.", results.StandardError);
        }

        [Fact]
        public void WhitespaceStringNameFails()
        {
            var testfilePath = CreateTempFile("test.zip");

            var results = RunExe(testfilePath, "\"   \"");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Invalid repository name '   ' provided.", results.StandardError);
        }

        [Fact]
        public void QuotedRepoNameSuccess()
        {
            var testfilePath = CreateTempFile("test.zip");

            var results = RunExe(testfilePath, "\"USER/REPO\"");

            Assert.Equal(0, results.ExitCode);
        }

        [Fact]
        public void RepoNameSuccess()
        {
            var testfilePath = CreateTempFile("test.zip");

            var results = RunExe(testfilePath, "USER/REPO");

            Assert.Equal(0, results.ExitCode);
        }

        private static string CreateTempFile(string fileName)
        {
            var testfilePath = TestFolders.GetTempFilePath(fileName);
            var file = File.Create(testfilePath);
            file.Close();
            return "\"" + testfilePath + "\"";
        }

        private static RunResults RunExe(string filePath, string repository = "USER/REPO")
        {
            return ExeTestRunner.RunExe(string.Format("publish -z {0} -r {1} -t FAKE_TOKEN -e a@b.com --dryrun", filePath, repository));
        }

    }
}