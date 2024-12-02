using System;
using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class ExecutableTests
    {
        [Fact]
        public void DetectFile_EmptyString_Null()
        {
            string file = string.Empty;
            var detectable = new Executable();

            string? actual = detectable.Detect(file, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void DetectStream_EmptyStream_Null()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            var detectable = new Executable();

            string? actual = detectable.Detect(stream, file, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void DetectDict_EmptyStream_Empty()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            Func<string, ProtectionDictionary>? getProtections = null;
            var detectable = new Executable();

            ProtectionDictionary actual = detectable.DetectDict(stream, file, getProtections, includeDebug: false);
            Assert.Empty(actual);
        }
    }
}
