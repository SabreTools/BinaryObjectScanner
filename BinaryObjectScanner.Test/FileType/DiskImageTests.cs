using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class DiskImageTests
    {
        private static readonly SabreTools.Serialization.Wrappers.ISO9660 wrapper
            = new(new SabreTools.Data.Models.ISO9660.Volume(), new MemoryStream(new byte[1024]));

        [Fact]
        public void DetectFile_EmptyString_Null()
        {
            string file = string.Empty;
            var detectable = new ISO9660(wrapper);

            string? actual = detectable.Detect(file, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void DetectStream_EmptyStream_Empty()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            var detectable = new ISO9660(wrapper);

            string? actual = detectable.Detect(stream, file, includeDebug: false);
            Assert.NotNull(actual);
            Assert.Empty(actual);
        }
    }
}