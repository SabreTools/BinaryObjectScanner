using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class TapeArchiveTests
    {
        private static readonly SabreTools.Serialization.Wrappers.TapeArchive wrapper
            = new(new SabreTools.Models.TAR.Archive(), new MemoryStream());

        [Fact]
        public void ExtractFile_EmptyString_False()
        {
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new TapeArchive(wrapper);

            bool actual = extractable.Extract(file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractStream_Null_False()
        {
            Stream? stream = null;
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new TapeArchive(wrapper);

            bool actual = extractable.Extract(stream, file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractStream_Empty_False()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new TapeArchive(wrapper);

            bool actual = extractable.Extract(stream, file, outDir, includeDebug: false);
            Assert.False(actual);
        }
    }
}
