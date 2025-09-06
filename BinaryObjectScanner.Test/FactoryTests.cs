using System;
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;
using Xunit;

namespace BinaryObjectScanner.Test
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

        #region CreateExtractable

        private static readonly List<WrapperType> _extractableTypes =
        [
            // WrapperType.BFPK, // TODO: Create wrapper to reenable test
            // WrapperType.BSP, // TODO: Create wrapper to reenable test
            // WrapperType.BZip2, // TODO: Create wrapper to reenable test
            // WrapperType.CFB, // TODO: Create wrapper to reenable test
            // WrapperType.CIA,
            // WrapperType.Executable, // TODO: This needs to be split internally
            // WrapperType.GCF, // TODO: Create wrapper to reenable test
            // WrapperType.GZip, // TODO: Create wrapper to reenable test
            // WrapperType.InstallShieldArchiveV3, // TODO: Create wrapper to reenable test
            // WrapperType.InstallShieldCAB, // TODO: Create wrapper to reenable test
            // WrapperType.LZKWAJ, // TODO: Create wrapper to reenable test
            // WrapperType.LZQBasic, // TODO: Create wrapper to reenable test
            // WrapperType.LZSZDD, // TODO: Create wrapper to reenable test
            // WrapperType.MicrosoftCAB, // TODO: Create wrapper to reenable test
            // WrapperType.MoPaQ, // TODO: Create wrapper to reenable test
            // WrapperType.N3DS,
            // WrapperType.NCF,
            // WrapperType.Nitro,
            // WrapperType.PAK, // TODO: Create wrapper to reenable test
            // WrapperType.PFF, // TODO: Create wrapper to reenable test
            // WrapperType.PKZIP, // TODO: Create wrapper to reenable test
            // WrapperType.PlayJAudioFile, // TODO: Create wrapper to reenable test
            // WrapperType.Quantum, // TODO: Create wrapper to reenable test
            // WrapperType.RAR, // TODO: Create wrapper to reenable test
            // WrapperType.SevenZip, // TODO: Create wrapper to reenable test
            WrapperType.SFFS,
            // WrapperType.SGA, // TODO: Create wrapper to reenable test
            // WrapperType.TapeArchive, // TODO: Create wrapper to reenable test
            // WrapperType.VBSP, // TODO: Create wrapper to reenable test
            // WrapperType.VPK, // TODO: Create wrapper to reenable test
            // WrapperType.WAD, // TODO: Create wrapper to reenable test
            // WrapperType.XZ, // TODO: Create wrapper to reenable test
            // WrapperType.XZP, // TODO: Create wrapper to reenable test
        ];

        [Theory]
        [MemberData(nameof(GenerateIExtractableTestData))]
        public void CreateExtractableTests(WrapperType type, bool expectNull)
        {
            IExtractable? actual = Factory.CreateExtractable(type);

            if (expectNull)
                Assert.Null(actual);
            else
                Assert.NotNull(actual);
        }

        public static List<object?[]> GenerateIExtractableTestData()
        {
            var testData = new List<object?[]>();
            foreach (WrapperType type in Enum.GetValues(typeof(WrapperType)))
            {
                if (_extractableTypes.Contains(type))
                    testData.Add([type, false]);
                else
                    testData.Add([type, true]);
            }

            return testData;
        }

        #endregion
    }
}