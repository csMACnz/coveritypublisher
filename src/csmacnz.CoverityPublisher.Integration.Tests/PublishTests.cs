using System;
using csmacnz.CoverityPublisher.Integration.Tests.Helper;
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
            var results = RunMinimumValidExeWithRepository("doesntExist.zip");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input file 'doesntExist.zip' cannot be found", results.StandardError);
        }

        [Fact]
        public void FileInQuotesDoesntExist()
        {
            var results = RunMinimumValidExeWithRepository("\"doesntExist.zip\"");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input file 'doesntExist.zip' cannot be found.", results.StandardError);
        }

        public class RepoNameTests
        {
            [Fact]
            public void RepoNameArgNotProvidedAppveyorRepoNameSetSucceeds()
            {
                var testfilePath = CreateTempZipFile();
                SetAppveyorRepoName("Test/Repo");

                var results = RunMinimumValidExe(testfilePath);

                Assert.Equal(0, results.ExitCode);
            }

            [Fact]
            public void EmptyStringNameFails()
            {
                var testfilePath = CreateTempZipFile();
                ClearAppveyorRepoName();

                var results = RunMinimumValidExeWithRepository(testfilePath, repository: "\"\"");

                Assert.NotEqual(0, results.ExitCode);
                Assert.Contains("No repository name provided, and could not be resolved.", results.StandardError);
            }

            [Fact]
            public void EmptyStringNameAppveyorRepoNameSetSucceeds()
            {
                var testfilePath = CreateTempZipFile();
                SetAppveyorRepoName("Test/Repo");

                var results = RunMinimumValidExeWithRepository(testfilePath, repository: "\"\"");

                Assert.Equal(0, results.ExitCode);
            }

            [Fact]
            public void WhitespaceStringNameFails()
            {
                var testfilePath = CreateTempZipFile();
                ClearAppveyorRepoName();

                var results = RunMinimumValidExeWithRepository(testfilePath, repository: "\"   \"");

                Assert.NotEqual(0, results.ExitCode);
                Assert.Contains("No repository name provided, and could not be resolved.", results.StandardError);
            }

            [Fact]
            public void WhitespaceStringNameAppveyorRepoNameSetSucceeds()
            {
                var testfilePath = CreateTempZipFile();
                SetAppveyorRepoName("Test/Repo");

                var results = RunMinimumValidExeWithRepository(testfilePath, repository: "\"   \"");

                Assert.Equal(0, results.ExitCode);
            }

            [Fact]
            public void QuotedRepoNameSuccess()
            {
                var testfilePath = CreateTempZipFile();

                var results = RunMinimumValidExeWithRepository(testfilePath, repository: "\"USER/REPO\"");

                Assert.Equal(0, results.ExitCode);
            }

            [Fact]
            public void RepoNameSuccess()
            {
                var testfilePath = CreateTempZipFile();

                var results = RunMinimumValidExeWithRepository(testfilePath, repository: ValidRepositoryName);

                Assert.Equal(0, results.ExitCode);
            }
        }

        public class EmailTests
        {
            [Fact]
            public void EmailSuccess()
            {
                var testfilePath = CreateTempZipFile();

                var results = RunMinimumValidExeWithRepository(testfilePath, email:"csmacnz@csmac.co.nz");

                Assert.Equal(0, results.ExitCode);
            }

            [Fact]
            public void NonEmailFails()
            {
                var testfilePath = CreateTempZipFile();

                var results = RunMinimumValidExeWithRepository(testfilePath, email: "notanemail");

                Assert.NotEqual(0, results.ExitCode);
                Assert.Contains("Invalid email 'notanemail' provided.", results.StandardError);
            }


            [Fact]
            public void EmailWithTwoAtSymbolsFails()
            {
                var testfilePath = CreateTempZipFile();

                var results = RunMinimumValidExeWithRepository(testfilePath, email: "not@an@email");

                Assert.NotEqual(0, results.ExitCode);
                Assert.Contains("Invalid email 'not@an@email' provided.", results.StandardError);
            }
        }

        [Fact]
        public void DefaultShowsLogo()
        {
            var results = ExecutePublish();

            Assert.Equal(0, results.ExitCode);
            LogoAssert.ContainsLogo(results.StandardOutput);
        }

        [Fact]
        public void NoLogoSuccess()
        {
            var results = ExecutePublish("--nologo");


            Assert.Equal(0, results.ExitCode);
            LogoAssert.DoesNotContainLogo(results.StandardOutput);
        }

        private static string CreateTempZipFile()
        {
            var testfilePath = TestFolders.CreateTempFile("zip");
            return "\"" + testfilePath + "\"";
        }

        private static RunResults ExecutePublish(string optionalParameters = null)
        {
            var testfilePath = CreateTempZipFile();

            var results = RunMinimumValidExeWithRepository(testfilePath, repository: ValidRepositoryName, optionalParameters: optionalParameters);
            return results;
        }

        private static RunResults RunMinimumValidExeWithRepository(string filePath, string token = "FAKE_TOKEN", string email = "a@b.com", string repository = ValidRepositoryName, string optionalParameters = null)
        {
            return RunMinimumValidExe(filePath, token, email, string.Format("-r {0} {1}", repository, optionalParameters));
        }

        private static RunResults RunMinimumValidExe(string filePath, string token = "FAKE_TOKEN", string email = "a@b.com", string optionalParameters = null)
        {
            return ExeTestRunner.RunExe(string.Format("publish -z {0} -t {1} -e {2} --dryrun {3}", filePath, token, email, optionalParameters));
        }

        private static void ClearAppveyorRepoName()
        {
            SetAppveyorRepoName(null);
        }

        private static void SetAppveyorRepoName(string name)
        {
            Environment.SetEnvironmentVariable("APPVEYOR_REPO_NAME", name, EnvironmentVariableTarget.Process);
        }
    }
}