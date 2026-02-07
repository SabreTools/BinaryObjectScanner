using BinaryObjectScanner.Data;
using Xunit;

namespace BinaryObjectScanner.Test.Data
{
    public class StaticChecksTests
    {
        [Fact]
        public void ContentCheckClasses_Populated()
        {
            var actual = StaticChecks.ContentCheckClasses;
            Assert.Equal(6, actual.Length);
        }

        [Fact]
        public void LinearExecutableCheckClasses_Empty()
        {
            var actual = StaticChecks.LinearExecutableCheckClasses;
            Assert.Empty(actual); // No implementations exist yet
        }

        [Fact]
        public void MSDOSExecutableCheckClasses_Empty()
        {
            var actual = StaticChecks.MSDOSExecutableCheckClasses;
            Assert.Empty(actual); // No implementations exist yet
        }

        [Fact]
        public void NewExecutableCheckClasses_Populated()
        {
            var actual = StaticChecks.NewExecutableCheckClasses;
            Assert.Equal(7, actual.Length);
        }

        [Fact]
        public void PathCheckClasses_Populated()
        {
            var actual = StaticChecks.PathCheckClasses;
            Assert.Equal(68, actual.Length);
        }

        [Fact]
        public void PortableExecutableCheckClasses_Populated()
        {
            var actual = StaticChecks.PortableExecutableCheckClasses;
            Assert.Equal(106, actual.Length);
        }
    }
}
