using csmacnz.CoverityPublisher.Integration.Tests.TestFramework;
using Xunit;

namespace csmacnz.CoverityPublisher.Integration.Tests
{
    public class CompressTests
    {
        [Fact]
        public void FolderDoesntExist()
        {
            var results = RunExe("FileNameToCreate.zip", "doesntExistFolder");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input folder 'doesntExistFolder' cannot be found.", results.StandardError);
        }

        private static RunResults RunExe(string output, string input)
        {
            return ExeTestRunner.RunExe(string.Format("compress -o {0} -i {1}", output, input));
        }

    }
}