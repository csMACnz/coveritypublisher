using System.Text.RegularExpressions;
using csmacnz.CoverityPublisher.Integration.Tests.TestFramework;
using Xunit;

namespace csmacnz.CoverityPublisher.Integration.Tests
{
    public class UsageTests
    {
        [Fact]
        public void NoArguments_ExitCodeNotSuccess()
        {
            var results = ExeTestRunner.RunExe(string.Empty);

            Assert.NotEqual(0, results.ExitCode);
        }

        [Fact]
        public void InvalidArgument_ExitCodeNotSuccess()
        {
            var results = ExeTestRunner.RunExe("--notanoption");

            Assert.NotEqual(0, results.ExitCode);
        }

        [Fact]
        public void FileDoesntExist()
        {
            var results = ExeTestRunner.RunExe("-z doesntExist.zip -r USER/REPO -t FAKE_TOKEN -e a@b.com");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input file 'doesntExist.zip' cannot be found", results.StandardError);
        }

        [Fact]
        public void FileInQuotesDoesntExist()
        {
            var results = ExeTestRunner.RunExe("-z \"doesntExist.zip\" -r USER/REPO -t FAKE_TOKEN -e a@b.com");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input file 'doesntExist.zip' cannot be found", results.StandardError);
        }

        [Fact]
        public void Version()
        {
            var results = ExeTestRunner.RunExe("--version");

            Assert.True(Regex.IsMatch(results.StandardOutput, @"\d+.\d+.\d+.\d+"));
        }

        [Fact]
        public void HelpLongHand()
        {
            var results = ExeTestRunner.RunExe("--help");

            Assert.Equal(0, results.ExitCode);
            ContainsStandardUsageText(results);
        }

        [Fact]
        public void HelpShortHand()
        {
            var results = ExeTestRunner.RunExe("-h");

            Assert.Equal(0, results.ExitCode);
            ContainsStandardUsageText(results);
        }

        private static void ContainsStandardUsageText(RunResults results)
        {
            Assert.Contains("Usage:", results.StandardOutput);
            Assert.Contains("PublishCoverity --help", results.StandardOutput);
            Assert.Contains("Options:", results.StandardOutput);
            Assert.Contains("What its for:", results.StandardOutput);
        }
    }
}
