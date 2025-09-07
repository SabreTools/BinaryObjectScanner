using System.IO;
using BinaryObjectScanner.Packer;
using Xunit;

namespace BinaryObjectScanner.Test.Packer
{
    public class HyperTechCrackProofTests
    {
        [Fact]
        public void CheckPortableExecutableTest()
        {
            string file = "filename";
            SabreTools.Models.PortableExecutable.Executable model = new();
            Stream source = new MemoryStream();
            SabreTools.Serialization.Wrappers.PortableExecutable exe = new(model, source);

            var checker = new HyperTechCrackProof();
            string? actual = checker.CheckExecutable(file, exe, includeDebug: false);
            Assert.Null(actual);
        }
    }
}