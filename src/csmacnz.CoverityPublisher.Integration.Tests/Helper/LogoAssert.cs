using Xunit;

namespace csmacnz.CoverityPublisher.Integration.Tests.Helper
{
    public static class LogoAssert
    {
        private const string LogoLine1 = @"|  __ \     | |   | (_)   | |    / ____|                  (_) |";
        private const string LogoLine2 = @"| |__) |   _| |__ | |_ ___| |__ | |     _____   _____ _ __ _| |_ _   _";
        private const string LogoLine3 = @"|  ___/ | | | '_ \| | / __| '_ \| |    / _ \ \ / / _ \ '__| | __| | | |";
        private const string LogoLine4 = @"| |   | |_| | |_) | | \__ \ | | | |___| (_) \ V /  __/ |  | | |_| |_| |";
        private const string LogoLine5 = @"|_|    \__,_|_.__/|_|_|___/_| |_|\_____\___/ \_/ \___|_|  |_|\__|\__, |";

        public static void ContainsLogo(string content)
        {
            Assert.Contains(LogoLine1, content);
            Assert.Contains(LogoLine2, content);
            Assert.Contains(LogoLine3, content);
            Assert.Contains(LogoLine4, content);
            Assert.Contains(LogoLine5, content);
        }

        public static void DoesNotContainLogo(string content)
        {
            Assert.DoesNotContain(LogoLine1, content);
            Assert.DoesNotContain(LogoLine2, content);
            Assert.DoesNotContain(LogoLine3, content);
            Assert.DoesNotContain(LogoLine4, content);
            Assert.DoesNotContain(LogoLine5, content);
        }
    }
}

