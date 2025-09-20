using System.IO;
using BinaryObjectScanner.GameEngine;
using Xunit;

namespace BinaryObjectScanner.Test.GameEngine
{
    public class RenderWareTests
    {
        [Fact]
        public void CheckPortableExecutableTest()
        {
            string file = "filename";
            SabreTools.Models.PortableExecutable.Executable model = new();
            Stream source = new MemoryStream(new byte[1024]);
            SabreTools.Serialization.Wrappers.PortableExecutable exe = new(model, source);

            var checker = new RenderWare();
            string? actual = checker.CheckExecutable(file, exe, includeDebug: false);
            Assert.Null(actual);
        }
    }
}