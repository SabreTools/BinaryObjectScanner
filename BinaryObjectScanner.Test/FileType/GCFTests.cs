using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class GCFTests
    {
        private static readonly SabreTools.Serialization.Wrappers.GCF wrapper
            = new(new SabreTools.Data.Models.GCF.File(), new MemoryStream(new byte[1024]));

        [Fact]
        public void DetectFile_EmptyString_Null()
        {
            string file = string.Empty;
            var detectable = new GCF(wrapper);

            string? actual = detectable.Detect(file, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void DetectStream_EmptyStream_DefaultValue()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            var detectable = new GCF(wrapper);

            string? actual = detectable.Detect(stream, file, includeDebug: false);
            Assert.Equal("AACS (Unknown Version)", actual);
        }
    }
}
