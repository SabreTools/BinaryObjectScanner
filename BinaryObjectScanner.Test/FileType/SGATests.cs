using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class SGATests
    {
        [Fact]
        public void ExtractFile_EmptyString_False()
        {
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new SGA();

            bool actual = extractable.Extract(file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractStream_Null_False()
        {
            Stream? stream = null;
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new SGA();

            bool actual = extractable.Extract(stream, file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractStream_Empty_False()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new SGA();

            bool actual = extractable.Extract(stream, file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractAll_EmptyModel_False()
        {
            var model = new SabreTools.Models.SGA.Archive();
            var data = new MemoryStream();
            var item = new SabreTools.Serialization.Wrappers.SGA(model, data);
            string outputDirectory = string.Empty;

            bool actual = SGA.ExtractAll(item, outputDirectory);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractFile_EmptyModel_False()
        {
            var model = new SabreTools.Models.SGA.Archive();
            var data = new MemoryStream();
            var item = new SabreTools.Serialization.Wrappers.SGA(model, data);
            string outputDirectory = string.Empty;

            bool actual = SGA.ExtractFile(item, 0, outputDirectory);
            Assert.False(actual);
        }
    }
}
