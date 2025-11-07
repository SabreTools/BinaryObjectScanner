using System.IO;
using BinaryObjectScanner.Protection;
using Xunit;

namespace BinaryObjectScanner.Test.Protection
{
    public class CopyLokTests
    {
        [Fact]
        public void CheckPortableExecutableTest()
        {
            string file = "filename";
            SabreTools.Data.Models.PortableExecutable.Executable model = new();
            Stream source = new MemoryStream(new byte[1024]);
            SabreTools.Serialization.Wrappers.PortableExecutable exe = new(model, source);

            var checker = new CopyLok();
            string? actual = checker.CheckExecutable(file, exe, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void CheckDiskImageTest()
        {
            string file = "filename";
            SabreTools.Data.Models.ISO9660.Volume model = new();
            Stream source = new MemoryStream(new byte[1024]);
            SabreTools.Serialization.Wrappers.ISO9660 iso = new(model, source);

            var checker = new CopyLok();
            string? actual = checker.CheckDiskImage(file, iso, includeDebug: false);
            Assert.Null(actual);
        }
    }
}
