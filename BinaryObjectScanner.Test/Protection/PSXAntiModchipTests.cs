using BinaryObjectScanner.Protection;
using Xunit;

namespace BinaryObjectScanner.Test.Protection
{
    public class PSXAntiModchipTests
    {
        [Fact]
        public void CheckContentsTest()
        {
            string file = "filename";
            byte[] fileContent = [0x01, 0x02, 0x03, 0x04];

            var checker = new PSXAntiModchip();
            string? actual = checker.CheckContents(file, fileContent, includeDebug: true);
            Assert.Null(actual);
        }
    }
}