using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class RealArcadeMezzanineTests
    {
        private static readonly SabreTools.Serialization.Wrappers.RealArcadeMezzanine wrapper
            = new(new SabreTools.Models.RealArcade.Mezzanine(), new MemoryStream(new byte[1024]));

        [Fact]
        public void DetectFile_EmptyString_Null()
        {
            string file = string.Empty;
            var detectable = new RealArcadeMezzanine(wrapper);

            string? actual = detectable.Detect(file, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void DetectStream_EmptyStream_Null()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            var detectable = new RealArcadeMezzanine(wrapper);

            string? actual = detectable.Detect(stream, file, includeDebug: false);
            Assert.Equal("RealArcade Mezzanine", actual);
        }
    }
}

