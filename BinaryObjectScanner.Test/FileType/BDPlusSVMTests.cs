using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class BDPlusSVMTests
    {
        private static readonly SabreTools.Serialization.Wrappers.BDPlusSVM wrapper
            = new(new SabreTools.Models.BDPlus.SVM(), new MemoryStream());

        [Fact]
        public void DetectFile_EmptyString_Null()
        {
            string file = string.Empty;
            var detectable = new BDPlusSVM(wrapper);

            string? actual = detectable.Detect(file, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void DetectStream_EmptyStream_DefaultValue()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            var detectable = new BDPlusSVM(wrapper);

            string? actual = detectable.Detect(stream, file, includeDebug: false);
            Assert.Equal("BD+ 0000-00-00", actual);
        }
    }
}

