using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class RealArcadeMezzanineTests
    {
        [Fact]
        public void DetectFileTest()
        {
            string file = string.Empty;
            var detectable = new RealArcadeMezzanine();

            string? actual = detectable.Detect(file, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void DetectStream()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            var detectable = new RealArcadeMezzanine();

            string? actual = detectable.Detect(stream, file, includeDebug: false);
            Assert.Null(actual);
        }
    }
}

