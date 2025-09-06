using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class SFFSTests
    {
        [Fact]
        public void DetectFile_EmptyString_Null()
        {
            string file = string.Empty;
            var detectable = new SFFS();

            string? actual = detectable.Detect(file, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void DetectStream_EmptyStream_Null()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            var detectable = new SFFS();

            string? actual = detectable.Detect(stream, file, includeDebug: false);
            Assert.Null(actual);
        }
    }
}
