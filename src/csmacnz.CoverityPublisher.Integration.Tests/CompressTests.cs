using System.IO;
using csmacnz.CoverityPublisher.Integration.Tests.TestFramework;
using Xunit;

namespace csmacnz.CoverityPublisher.Integration.Tests
{
    public class CompressTests
    {
        [Fact]
        public void ValidInputSuccessful()
        {
            var compressionFolder = TestFolders.OutputFolder;
            var fileNameToCreate = TestFolders.DefineTempFile("FileNameToCreate.zip");
            var results = RunExe(fileNameToCreate, compressionFolder);

            Assert.Equal(0, results.ExitCode);
            Assert.True(File.Exists(fileNameToCreate));
        }

        [Fact]
        public void ValidInputDryRunSuccessfulWithoutFile()
        {
            var compressionFolder = TestFolders.OutputFolder;
            var fileNameToCreate = TestFolders.DefineTempFile("FileNameToCreate.zip");
            var results = DryRunExe(fileNameToCreate, compressionFolder);

            Assert.Equal(0, results.ExitCode);
            Assert.False(File.Exists(fileNameToCreate));
        }
        
        [Fact]
        public void FolderDoesntExistErrors()
        {
            var results = RunExe("FileNameToCreate.zip", "doesntExistFolder");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input folder 'doesntExistFolder' cannot be found.", results.StandardError);
        }

        [Fact]
        public void FileExistsWithoutOverwriteErrors()
        {
            var existingZip = TestFolders.CreateTempFile("zip");
            var compressionFolder = TestFolders.OutputFolder;
            var results = RunExe(existingZip, compressionFolder);

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Output file '" + existingZip + "' already exists.", results.StandardError);
        }


        [Fact]
        public void FileExistsWithOverwriteErrors()
        {
            var existingZip = TestFolders.CreateTempFile("zip");
            var compressionFolder = TestFolders.OutputFolder;
            var results = RunExe(existingZip, compressionFolder);

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Output file '" + existingZip + "' already exists.", results.StandardError);
        }

        private static RunResults RunExe(string output, string input)
        {
            return ExeTestRunner.RunExe(string.Format("compress -o {0} -i {1}", output, input));
        }

        private static RunResults DryRunExe(string output, string input)
        {
            return ExeTestRunner.RunExe(string.Format("compress -o {0} -i {1} --dryrun", output, input));
        }
    }
}