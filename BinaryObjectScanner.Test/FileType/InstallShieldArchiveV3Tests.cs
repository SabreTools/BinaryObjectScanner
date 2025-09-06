using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class InstallShieldArchiveV3Tests
    {
        private static readonly SabreTools.Serialization.Wrappers.InstallShieldArchiveV3 wrapper
            = new(new SabreTools.Models.InstallShieldArchiveV3.Archive(), new MemoryStream());

        [Fact]
        public void ExtractFile_EmptyString_False()
        {
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new InstallShieldArchiveV3(wrapper);

            bool actual = extractable.Extract(file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractStream_Null_False()
        {
            Stream? stream = null;
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new InstallShieldArchiveV3(wrapper);

            bool actual = extractable.Extract(stream, file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractStream_Empty_False()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new InstallShieldArchiveV3(wrapper);

            bool actual = extractable.Extract(stream, file, outDir, includeDebug: false);
            Assert.False(actual);
        }
    }
}
