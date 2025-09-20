using System.IO;
using BinaryObjectScanner.Protection;
using Xunit;

namespace BinaryObjectScanner.Test.Protection
{
    public class ActiveMARKTests
    {
        [Fact]
        public void CheckContentsTest()
        {
            string file = "filename";
            byte[] fileContent = [0x01, 0x02, 0x03, 0x04];

            var checker = new ActiveMARK();
            string? actual = checker.CheckContents(file, fileContent, includeDebug: true);
            Assert.Null(actual);
        }

        [Fact]
        public void CheckPortableExecutableTest()
        {
            string file = "filename";
            SabreTools.Models.PortableExecutable.Executable model = new();
            Stream source = new MemoryStream(new byte[1024]);
            SabreTools.Serialization.Wrappers.PortableExecutable exe = new(model, source);

            var checker = new ActiveMARK();
            string? actual = checker.CheckExecutable(file, exe, includeDebug: false);
            Assert.Null(actual);
        }
    }
}