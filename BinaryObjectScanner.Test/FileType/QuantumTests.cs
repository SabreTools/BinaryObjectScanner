using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class QuantumTests
    {
        [Fact]
        public void ExtractFile_EmptyString_False()
        {
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new Quantum();

            bool actual = extractable.Extract(file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractStream_Null_False()
        {
            Stream? stream = null;
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new Quantum();

            bool actual = extractable.Extract(stream, file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractStream_Empty_False()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            string outDir = string.Empty;
            var extractable = new Quantum();

            bool actual = extractable.Extract(stream, file, outDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractAll_EmptyModel_False()
        {
            var model = new SabreTools.Models.Quantum.Archive();
            var data = new MemoryStream();
            var item = new SabreTools.Serialization.Wrappers.Quantum(model, data);
            string outputDirectory = string.Empty;

            bool actual = Quantum.ExtractAll(item, outputDirectory);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractFile_EmptyModel_False()
        {
            var model = new SabreTools.Models.Quantum.Archive();
            var data = new MemoryStream();
            var item = new SabreTools.Serialization.Wrappers.Quantum(model, data);
            string outputDirectory = string.Empty;

            bool actual = Quantum.ExtractFile(item, 0, outputDirectory);
            Assert.False(actual);
        }
    }
}
