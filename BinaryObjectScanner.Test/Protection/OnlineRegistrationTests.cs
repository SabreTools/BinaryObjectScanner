using System.IO;
using BinaryObjectScanner.Protection;
using Xunit;

namespace BinaryObjectScanner.Test.Protection
{
    public class OnlineRegistrationTests
    {
        [Fact]
        public void CheckPortableExecutableTest()
        {
            string file = "filename";
            SabreTools.Models.PortableExecutable.Executable model = new();
            Stream source = new MemoryStream();
            SabreTools.Serialization.Wrappers.PortableExecutable exe = new(model, source);

            var checker = new OnlineRegistration();
            string? actual = checker.CheckExecutable(file, exe, includeDebug: false);
            Assert.Null(actual);
        }
    }
}