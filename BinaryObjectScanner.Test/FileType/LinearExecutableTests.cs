using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class LinearExecutableTests
    {
        private static readonly SabreTools.Serialization.Wrappers.LinearExecutable wrapper
            = new(new SabreTools.Models.LinearExecutable.Executable(), new MemoryStream(new byte[1024]));

        [Fact]
        public void DetectFile_EmptyString_Null()
        {
            string file = string.Empty;
            var detectable = new LinearExecutable(wrapper);

            string? actual = detectable.Detect(file, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void DetectStream_EmptyStream_Empty()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            var detectable = new LinearExecutable(wrapper);

            string? actual = detectable.Detect(stream, file, includeDebug: false);
            Assert.NotNull(actual);
            Assert.Empty(actual);
        }
    }
}
