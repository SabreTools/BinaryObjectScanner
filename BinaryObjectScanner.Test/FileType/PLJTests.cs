using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class PLJTests
    {
        private static readonly SabreTools.Serialization.Wrappers.PlayJAudioFile wrapper
            = new(new SabreTools.Data.Models.PlayJ.AudioFile(), new MemoryStream(new byte[1024]));

        [Fact]
        public void DetectFile_EmptyString_Null()
        {
            string file = string.Empty;
            var detectable = new PLJ(wrapper);

            string? actual = detectable.Detect(file, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void DetectStream_EmptyStream_DefaultValue()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            var detectable = new PLJ(wrapper);

            string? actual = detectable.Detect(stream, file, includeDebug: false);
            Assert.Equal("PlayJ Audio File", actual);
        }
    }
}
