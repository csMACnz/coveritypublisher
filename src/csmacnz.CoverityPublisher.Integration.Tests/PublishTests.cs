using csmacnz.CoverityPublisher.Integration.Tests.TestFramework;
using Xunit;

namespace csmacnz.CoverityPublisher.Integration.Tests
{
    public class PublishTests
    {
        [Fact]
        public void FileDoesntExist()
        {
            var results = ExeTestRunner.RunExe("publish -z doesntExist.zip -r USER/REPO -t FAKE_TOKEN -e a@b.com");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input file 'doesntExist.zip' cannot be found", results.StandardError);
        }

        [Fact]
        public void FileInQuotesDoesntExist()
        {
            var results = ExeTestRunner.RunExe("publish -z \"doesntExist.zip\" -r USER/REPO -t FAKE_TOKEN -e a@b.com");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input file 'doesntExist.zip' cannot be found", results.StandardError);
        }
    }
}