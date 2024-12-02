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
            Assert.NotEmpty(actual);
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
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void PathCheckClasses_Populated()
        {
            var actual = StaticChecks.PathCheckClasses;
            Assert.NotEmpty(actual);
        }

        [Fact]
        public void PortableExecutableCheckClasses_Populated()
        {
            var actual = StaticChecks.PortableExecutableCheckClasses;
            Assert.NotEmpty(actual);
        }
    }
}