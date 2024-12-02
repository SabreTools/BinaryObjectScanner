using BinaryObjectScanner.Protection;
using Xunit;

namespace BinaryObjectScanner.Test.Protection
{
    public class CactusDataShieldTests
    {
        [Fact]
        public void CheckContentsTest()
        {
            string file = "filename";
            byte[] fileContent = [0x01, 0x02, 0x03, 0x04];

            var checker = new CactusDataShield();
            string? actual = checker.CheckContents(file, fileContent, includeDebug: true);
            Assert.Null(actual);
        }
    }
}