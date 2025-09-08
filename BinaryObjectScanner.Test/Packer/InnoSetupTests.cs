using System.IO;
using BinaryObjectScanner.Packer;
using Xunit;

namespace BinaryObjectScanner.Test.Packer
{
    public class InnoSetupTests
    {
        [Fact]
        public void CheckNewExecutableTest()
        {
            string file = "filename";
            SabreTools.Models.NewExecutable.Executable model = new();
            Stream source = new MemoryStream();
            SabreTools.Serialization.Wrappers.NewExecutable exe = new(model, source);

            var checker = new InnoSetup();
            string? actual = checker.CheckExecutable(file, exe, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void CheckPortableExecutableTest()
        {
            string file = "filename";
            SabreTools.Models.PortableExecutable.Executable model = new();
            Stream source = new MemoryStream();
            SabreTools.Serialization.Wrappers.PortableExecutable exe = new(model, source);

            var checker = new InnoSetup();
            string? actual = checker.CheckExecutable(file, exe, includeDebug: false);
            Assert.Null(actual);
        }
    }
}