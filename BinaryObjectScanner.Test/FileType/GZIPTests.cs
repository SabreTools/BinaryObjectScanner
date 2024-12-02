using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class GZIPTests
    {
        [Fact]
        public void ExtractFileTest()
        {
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new GZIP();

            bool actual = extractable.Extract(file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractStream_Null_False()
        {
            Stream? stream = null;
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new GZIP();

            bool actual = extractable.Extract(stream, file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractStream_Empty_False()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new GZIP();

            bool actual = extractable.Extract(stream, file, outDir, includeDebug: false);
            Assert.False(actual);
        }
    }
}
