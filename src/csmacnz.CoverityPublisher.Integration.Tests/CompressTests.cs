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
            var compressionFolder = CreateValidCompressionFolder();
            var fileNameToCreate = TestFolders.DefineTempFile("FileNameToCreate.zip");
            var results = RunExe(fileNameToCreate, compressionFolder);

            Assert.Equal(0, results.ExitCode);
            Assert.True(File.Exists(fileNameToCreate));
        }

        [Fact]
        public void ValidInputDryRunSuccessfulWithoutFile()
        {
            var compressionFolder = CreateValidCompressionFolder();
            var fileNameToCreate = TestFolders.DefineTempFile("FileNameToCreate.zip");
            var results = DryRunExe(fileNameToCreate, compressionFolder);

            Assert.Equal(0, results.ExitCode);
            Assert.False(File.Exists(fileNameToCreate));
        }
        
        [Fact]
        public void FolderDoesntExistErrors()
        {
            var fileNameToCreate = TestFolders.DefineTempFile("FileNameToCreate.zip");
            var results = RunExe(fileNameToCreate, "doesntExistFolder");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input folder 'doesntExistFolder' cannot be found.", results.StandardError);
        }

        [Fact]
        public void InputFolderExistsWithoutBuildMetricsErrors()
        {
            var compressionFolder = TestFolders.CreateTempFolder();
            var fileNameToCreate = TestFolders.DefineTempFile("FileNameToCreate.zip");
            var results = RunExe(fileNameToCreate, compressionFolder);

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input folder '" + compressionFolder + "' is not recognised as Coverity Scan results.", results.StandardError);
        }

        [Fact]
        public void InputFolderExistsWithFailuresWithoutAbortSuccessful()
        {
            var fileNameToCreate = TestFolders.DefineTempFile("FileNameToCreate.zip");
            var compressionFolder = CreateValidCompressionFolderWithFailures();
            var results = RunExe(fileNameToCreate, compressionFolder);

            Assert.Equal(0, results.ExitCode);
        }

        [Fact]
        public void InputFolderExistsWithFailuresWithAbortOnFailuresErrors()
        {
            var fileNameToCreate = TestFolders.DefineTempFile("FileNameToCreate.zip");
            var compressionFolder = CreateValidCompressionFolderWithFailures();
            var results = RunExe(fileNameToCreate, compressionFolder, "--abortOnFailures");

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Input folder '" + compressionFolder + "' has recorded failures.", results.StandardError);
        }

        [Fact]
        public void InputFolderExistsWithoutFailuresWithAbortOnFailuresSuccessful()
        {
            var fileNameToCreate = TestFolders.DefineTempFile("FileNameToCreate.zip");
            var compressionFolder = CreateValidCompressionFolder();
            var results = RunExe(fileNameToCreate, compressionFolder, "--abortOnFailures");

            Assert.Equal(0, results.ExitCode);
        }

        [Fact]
        public void FileExistsWithoutOverwriteErrors()
        {
            var existingZip = TestFolders.CreateTempFile("zip");
            var compressionFolder = CreateValidCompressionFolder();
            var results = RunExe(existingZip, compressionFolder);

            Assert.NotEqual(0, results.ExitCode);
            Assert.Contains("Output file '" + existingZip + "' already exists.", results.StandardError);
        }

        [Fact]
        public void FileExistsWithOverwriteErrors()
        {
            var existingZip = TestFolders.CreateTempFile("zip");
            var compressionFolder = CreateValidCompressionFolder();
            var results = RunExe(existingZip, compressionFolder, "--overwrite");

            Assert.Equal(0, results.ExitCode);
            Assert.Contains("Overwritting file '" + existingZip + "' with new compression data.", results.StandardOutput);
            Assert.True(File.Exists(existingZip));
        }

        [Fact]
        public void DefaultShowsLogo()
        {
            var results = ExecuteCompress();

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
            var results = ExecuteCompress("--nologo");


            Assert.Equal(0, results.ExitCode);
            Assert.DoesNotContain(@"|  __ \     | |   | (_)   | |    / ____|                  (_) |", results.StandardOutput);
            Assert.DoesNotContain(@"| |__) |   _| |__ | |_ ___| |__ | |     _____   _____ _ __ _| |_ _   _", results.StandardOutput);
            Assert.DoesNotContain(@"|  ___/ | | | '_ \| | / __| '_ \| |    / _ \ \ / / _ \ '__| | __| | | |", results.StandardOutput);
            Assert.DoesNotContain(@"| |   | |_| | |_) | | \__ \ | | | |___| (_) \ V /  __/ |  | | |_| |_| |", results.StandardOutput);
            Assert.DoesNotContain(@"|_|    \__,_|_.__/|_|_|___/_| |_|\_____\___/ \_/ \___|_|  |_|\__|\__, |", results.StandardOutput);
        }

        private static string CreateValidCompressionFolder()
        {
            var compressionFolder = TestFolders.CreateTempFolder();
            var metricsFile = Path.Combine(compressionFolder, BuildMetrics.FileName);
            File.WriteAllText(metricsFile, BuildMetrics.GetValidContents());
            return compressionFolder;
        }

        private static string CreateValidCompressionFolderWithFailures()
        {
            var compressionFolder = CreateValidCompressionFolder();
            File.WriteAllText(Path.Combine(compressionFolder, BuildMetrics.FileName), BuildMetrics.GetContentsWithFailures());
            return compressionFolder;
        }

        private RunResults ExecuteCompress(string options = "")
        {
            var compressionFolder = CreateValidCompressionFolder();
            var fileNameToCreate = TestFolders.DefineTempFile("FileNameToCreate.zip");
            var results = DryRunExe(fileNameToCreate, compressionFolder, options);
            return results;
        }

        private static RunResults RunExe(string output, string input, string options = "")
        {
            return ExeTestRunner.RunExe(string.Format("compress -o {0} -i {1} {2}", output, input, options));
        }

        private static RunResults DryRunExe(string output, string input, string options = "")
        {
            return ExeTestRunner.RunExe(string.Format("compress -o {0} -i {1} --dryrun {2}", output, input, options));
        }
    }
}