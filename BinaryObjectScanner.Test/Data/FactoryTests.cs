using System;
using System.Collections.Generic;
using BinaryObjectScanner.Data;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;
using Xunit;

namespace BinaryObjectScanner.Test.Data
{
    public class FactoryTests
    {
        #region CreateDetectable

        private static readonly List<WrapperType> _detectableTypes =
        [
            // WrapperType.AACSMediaKeyBlock, // TODO: Create wrapper to reenable test
            // WrapperType.BDPlusSVM, // TODO: Create wrapper to reenable test
            // WrapperType.CIA,
            // WrapperType.Executable, // TODO: This needs to be split internally
            WrapperType.LDSCRYPT,
            // WrapperType.N3DS,
            // WrapperType.Nitro,
            // WrapperType.PlayJAudioFile, // TODO: Create wrapper to reenable test
            WrapperType.RealArcadeInstaller,
            WrapperType.RealArcadeMezzanine,
            WrapperType.SFFS,
            WrapperType.Textfile,
        ];

        [Theory]
        [MemberData(nameof(GenerateIDetectableTestData))]
        public void CreateDetectableTests(WrapperType type, bool expectNull)
        {
            IDetectable? actual = Factory.CreateDetectable(type);

            if (expectNull)
                Assert.Null(actual);
            else
                Assert.NotNull(actual);
        }

        public static List<object?[]> GenerateIDetectableTestData()
        {
            var testData = new List<object?[]>() { new object?[] { null, true } };
            foreach (WrapperType type in Enum.GetValues(typeof(WrapperType)))
            {
                if (_detectableTypes.Contains(type))
                    testData.Add([type, false]);
                else
                    testData.Add([type, true]);
            }

            return testData;
        }

        #endregion
    }
}