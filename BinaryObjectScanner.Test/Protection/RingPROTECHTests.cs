using System.Collections.Generic;
using BinaryObjectScanner.Protection;
using Xunit;

namespace BinaryObjectScanner.Test.Protection
{
    public class RingPROTECHTests
    {
        [Fact]
        public void CheckContentsTest()
        {
            string file = "filename";
            byte[] fileContent = [0x01, 0x02, 0x03, 0x04];

            var checker = new RingPROTECH();
            string? actual = checker.CheckContents(file, fileContent, includeDebug: true);
            Assert.Null(actual);
        }

        [Fact]
        public void CheckDirectoryPathTest()
        {
            string path = "path";
            List<string> files = [];

            var checker = new RingPROTECH();
            List<string> actual = checker.CheckDirectoryPath(path, files);
            Assert.Empty(actual);
        }

        [Fact]
        public void CheckFilePathTest()
        {
            string path = "path";

            var checker = new RingPROTECH();
            string? actual = checker.CheckFilePath(path);
            Assert.Null(actual);
        }
    }
}