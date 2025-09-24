using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class RealArcadeInstallerTests
    {
        private static readonly SabreTools.Serialization.Wrappers.RealArcadeInstaller wrapper
            = new(new SabreTools.Models.RealArcade.RgsFile(), new MemoryStream(new byte[1024]));

        [Fact]
        public void DetectFile_EmptyString_Null()
        {
            string file = string.Empty;
            var detectable = new RealArcadeInstaller(wrapper);

            string? actual = detectable.Detect(file, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void DetectStream_EmptyStream_Null()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            var detectable = new RealArcadeInstaller(wrapper);

            string? actual = detectable.Detect(stream, file, includeDebug: false);
            Assert.Equal("RealArcade Installer", actual);
        }
    }
}

