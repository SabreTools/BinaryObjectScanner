using System.Collections.Generic;
using BinaryObjectScanner.Data;
using Xunit;

namespace BinaryObjectScanner.Test.Data
{
    public class ProtectionDictionaryTests
    {
        #region ProcessProtectionString

        [Fact]
        public void ProcessProtectionString_Null_Empty()
        {
            string? protection = null;
            List<string> actual = ProtectionDictionary.ProcessProtectionString(protection);
            Assert.Empty(actual);
        }

        [Fact]
        public void ProcessProtectionString_Empty_Empty()
        {
            string? protection = string.Empty;
            List<string> actual = ProtectionDictionary.ProcessProtectionString(protection);
            Assert.Empty(actual);
        }

        [Fact]
        public void ProcessProtectionString_NoIndicator_Single()
        {
            string? protection = "item1";
            List<string> actual = ProtectionDictionary.ProcessProtectionString(protection);
            Assert.Single(actual);
        }

        [Fact]
        public void ProcessProtectionString_Indicator_Multiple()
        {
            string? protection = "item1;item2";
            List<string> actual = ProtectionDictionary.ProcessProtectionString(protection);
            Assert.Equal(2, actual.Count);
        }

        #endregion
    }
}