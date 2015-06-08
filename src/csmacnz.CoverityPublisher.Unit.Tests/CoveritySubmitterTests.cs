using System.IO;
using Xunit;

namespace csmacnz.CoverityPublisher.Unit.Tests
{
    public class CoveritySubmitterTests
    {
        [Fact]
        public void TestRunWithValidInputCompletesSuccessfully()
        {
            var fileName = Path.GetTempFileName();
            PublishPayload payload = new PublishPayload
            {
                Description = "test submission",
                Email = "test@example.com",
                Token = "ABCDEFGHIJK",
                FileName = fileName,
                Version = "1.0",
                SubmitToCoverity = false,
                RepositoryName = "csMACnz/publishcoverity"
            };

            var result = CoveritySubmitter.Submit(payload);

            Assert.True(result.Successful);
        }
    }
}
