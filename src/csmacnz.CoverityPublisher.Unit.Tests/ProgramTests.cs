using Xunit;

namespace csmacnz.CoverityPublisher.Unit.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void DoubleQuotedString_ReturnsWithoutQuotes()
        {
            var theString = "\"Test\"";

            var result = Program.Unquoted(theString);

            Assert.Equal("Test", result);
        }

        [Fact]
        public void SingleQuotedString_ReturnsWithoutQuotes()
        {
            var theString = "\'Test\'";

            var result = Program.Unquoted(theString);

            Assert.Equal("Test", result);
        }


        [Fact]
        public void NoQuotedString_ReturnsWithoutQuotes()
        {
            var theString = "Test";

            var result = Program.Unquoted(theString);

            Assert.Equal("Test", result);
        }
    }
}
