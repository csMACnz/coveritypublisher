using System.IO;
using csmacnz.CoverityPublisher.Integration.Tests.TestFramework;
using Xunit;

namespace csmacnz.CoverityPublisher.Integration.Tests
{
    public class PublishTests
    {
        private const string ValidRepositoryName = "USER/REPO";

        [Fact]
        public void FileDoesntExist()
        {
            var results = RunMinimumValidExeWithDefaultRepository("doesntExist.zip");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input file 'doesntExist.zip' cannot be found", results.StandardError);
        }

        [Fact]
        public void FileInQuotesDoesntExist()
        {
            var results = RunMinimumValidExeWithDefaultRepository("\"doesntExist.zip\"");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input file 'doesntExist.zip' cannot be found.", results.StandardError);
        }


        [Fact]
        public void NameWithoutSlashFails()
        {
            var testfilePath = CreateTempFile("test.zip");

            var results = RunMinimumValidExeWithRepository(testfilePath, "USERREPO");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Invalid repository name 'USERREPO' provided.", results.StandardError);
        }

        [Fact]
        public void EmptyStringNameFails()
        {
            var testfilePath = CreateTempFile("test.zip");

            var results = RunMinimumValidExeWithRepository(testfilePath, "\"\"");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("No repository name provided, and could not be resolved.", results.StandardError);
        }

        [Fact]
        public void WhitespaceStringNameFails()
        {
            var testfilePath = CreateTempFile("test.zip");

            var results = RunMinimumValidExeWithRepository(testfilePath, "\"   \"");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("No repository name provided, and could not be resolved.", results.StandardError);
        }

        [Fact]
        public void QuotedRepoNameSuccess()
        {
            var testfilePath = CreateTempFile("test.zip");

            var results = RunMinimumValidExeWithRepository(testfilePath, "\"USER/REPO\"");

            Assert.Equal(0, results.ExitCode);
        }

        [Fact]
        public void RepoNameSuccess()
        {
            var testfilePath = CreateTempFile("test.zip");

            var results = RunMinimumValidExeWithRepository(testfilePath, ValidRepositoryName);

            Assert.Equal(0, results.ExitCode);
        }

        [Fact]
        public void DefaultShowsLogo()
        {
            var results = ExecutePublish();

            Assert.Equal(0, results.ExitCode);
            Assert.Contains(@"|  __ \     | |   | (_)   | |    / ____|                  (_) |", results.StandardOutput);
            Assert.Contains(@"| |__) |   _| |__ | |_ ___| |__ | |     _____   _____ _ __ _| |_ _   _", results.StandardOutput);
            Assert.Contains(@"|  ___/ | | | '_ \| | / __| '_ \| |    / _ \ \ / / _ \ '__| | __| | | |", results.StandardOutput);
            Assert.Contains(@"| |   | |_| | |_) | | \__ \ | | | |___| (_) \ V /  __/ |  | | |_| |_| |", results.StandardOutput);
            Assert.Contains(@"|_|    \__,_|_.__/|_|_|___/_| |_|\_____\___/ \_/ \___|_|  |_|\__|\__, |", results.StandardOutput);
        }

        [Fact]
        public void NoLogoSuccess()
        {
            var results = ExecutePublish("--nologo");


            Assert.Equal(0, results.ExitCode);
            Assert.DoesNotContain(@"|  __ \     | |   | (_)   | |    / ____|                  (_) |", results.StandardOutput);
            Assert.DoesNotContain(@"| |__) |   _| |__ | |_ ___| |__ | |     _____   _____ _ __ _| |_ _   _", results.StandardOutput);
            Assert.DoesNotContain(@"|  ___/ | | | '_ \| | / __| '_ \| |    / _ \ \ / / _ \ '__| | __| | | |", results.StandardOutput);
            Assert.DoesNotContain(@"| |   | |_| | |_) | | \__ \ | | | |___| (_) \ V /  __/ |  | | |_| |_| |", results.StandardOutput);
            Assert.DoesNotContain(@"|_|    \__,_|_.__/|_|_|___/_| |_|\_____\___/ \_/ \___|_|  |_|\__|\__, |", results.StandardOutput);
        }

        private static string CreateTempFile(string fileName)
        {
            var testfilePath = TestFolders.GetTempFilePath(fileName);
            var file = File.Create(testfilePath);
            file.Close();
            return "\"" + testfilePath + "\"";
        }

        private static RunResults ExecutePublish(string optionalParameters = null)
        {
            var testfilePath = CreateTempFile("test.zip");

            var results = RunMinimumValidExeWithRepository(testfilePath, ValidRepositoryName, optionalParameters);
            return results;
        }

        private static RunResults RunMinimumValidExeWithDefaultRepository(string filePath, string optionalParameters = null)
        {
            return RunMinimumValidExeWithRepository(filePath, ValidRepositoryName, optionalParameters);
        }

        private static RunResults RunMinimumValidExeWithRepository(string filePath, string repository, string optionalParameters = null)
        {
            return RunMinimumValidExe(filePath, optionalParameters: string.Format("-r {0} {1}", repository, optionalParameters));
        }

        private static RunResults RunMinimumValidExe(string filePath, string token = "FAKE_TOKEN", string email = "a@b.com", string optionalParameters = null)
        {
            return ExeTestRunner.RunExe(string.Format("publish -z {0} -t {1} -e {2} --dryrun {3}", filePath, token, email, optionalParameters));
        }

    }
}