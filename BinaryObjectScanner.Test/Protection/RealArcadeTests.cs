using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Protection;
using Xunit;

namespace BinaryObjectScanner.Test.Protection
{
    public class RealArcadeTests
    {
        [Fact]
        public void CheckPortableExecutableTest()
        {
            string file = "filename";
            SabreTools.Models.PortableExecutable.Executable model = new();
            Stream source = new MemoryStream();
            SabreTools.Serialization.Wrappers.PortableExecutable exe = new(model, source);

            var checker = new RealArcade();
            string? actual = checker.CheckExecutable(file, exe, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void CheckDirectoryPathTest()
        {
            string path = "path";
            List<string> files = [];

            var checker = new RealArcade();
            List<string> actual = checker.CheckDirectoryPath(path, files);
            Assert.Empty(actual);
        }

        [Fact]
        public void CheckFilePathTest()
        {
            string path = "path";

            var checker = new RealArcade();
            string? actual = checker.CheckFilePath(path);
            Assert.Null(actual);
        }
    }
}