using System.IO;
using BinaryObjectScanner.Packer;
using Xunit;

namespace BinaryObjectScanner.Test.Packer
{
    public class WinZipSFXTests
    {
        [Fact]
        public void CheckNewExecutableTest()
        {
            string file = "filename";
            SabreTools.Models.NewExecutable.Executable model = new();
            Stream source = new MemoryStream();
            SabreTools.Serialization.Wrappers.NewExecutable nex = new(model, source);

            var checker = new WinZipSFX();
            string? actual = checker.CheckExecutable(file, nex, includeDebug: false);
            Assert.Null(actual);
        }
    
        [Fact]
        public void CheckPortableExecutableTest()
        {
            string file = "filename";
            SabreTools.Models.PortableExecutable.Executable model = new();
            Stream source = new MemoryStream();
            SabreTools.Serialization.Wrappers.PortableExecutable pex = new(model, source);

            var checker = new WinZipSFX();
            string? actual = checker.CheckExecutable(file, pex, includeDebug: false);
            Assert.Null(actual);
        }
    
        [Fact]
        public void ExtractNewExecutableTest()
        {
            string file = "filename";
            SabreTools.Models.NewExecutable.Executable model = new();
            Stream source = new MemoryStream();
            SabreTools.Serialization.Wrappers.NewExecutable nex = new(model, source);
            string outputDir = string.Empty;

            var checker = new WinZipSFX();
            bool actual = checker.Extract(file, nex, outputDir, includeDebug: false);
            Assert.False(actual);
        }

        [Fact]
        public void ExtractPortableExecutableTest()
        {
            string file = "filename";
            SabreTools.Models.PortableExecutable.Executable model = new();
            Stream source = new MemoryStream();
            SabreTools.Serialization.Wrappers.PortableExecutable pex = new(model, source);
            string outputDir = string.Empty;

            var checker = new WinZipSFX();
            bool actual = checker.Extract(file, pex, outputDir, includeDebug: false);
            Assert.False(actual);
        }
    }
}