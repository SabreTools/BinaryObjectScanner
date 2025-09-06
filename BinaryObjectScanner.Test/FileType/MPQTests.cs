using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class MPQTests
    {
        private static readonly SabreTools.Serialization.Wrappers.MoPaQ wrapper
            = new(new SabreTools.Models.MoPaQ.Archive(), new MemoryStream());

        [Fact]
        public void ExtractFile_EmptyString_False()
        {
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new MPQ(wrapper);

            bool actual = extractable.Extract(file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractStream_Null_False()
        {
            Stream? stream = null;
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new MPQ(wrapper);

            bool actual = extractable.Extract(stream, file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractStream_Empty_False()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new MPQ(wrapper);

            bool actual = extractable.Extract(stream, file, outDir, includeDebug: false);
            Assert.False(actual);
        }
    }
}
