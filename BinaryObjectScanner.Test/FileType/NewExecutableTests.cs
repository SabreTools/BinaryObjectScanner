using System.IO;
using BinaryObjectScanner.FileType;
using Xunit;

namespace BinaryObjectScanner.Test.FileType
{
    public class NewExecutableTests
    {
        private static readonly SabreTools.Serialization.Wrappers.NewExecutable wrapper
            = new(new SabreTools.Models.NewExecutable.Executable(), new MemoryStream());

        [Fact]
        public void DetectFile_EmptyString_Null()
        {
            string file = string.Empty;
            var detectable = new NewExecutable(wrapper);

            string? actual = detectable.Detect(file, includeDebug: false);
            Assert.Null(actual);
        }

        [Fact]
        public void DetectStream_EmptyStream_Empty()
        {
            Stream? stream = new MemoryStream();
            string file = string.Empty;
            var detectable = new NewExecutable(wrapper);

            string? actual = detectable.Detect(stream, file, includeDebug: false);
            Assert.NotNull(actual);
            Assert.Empty(actual);
        }
    }
}
