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
            WrapperType.BFPK,
            WrapperType.BSP,
            WrapperType.BZip2,
            WrapperType.CFB,
            //WrapperType.CIA,
            //WrapperType.Executable, // TODO: This needs to be split internally
            WrapperType.GCF,
            WrapperType.GZip,
            WrapperType.InstallShieldArchiveV3,
            WrapperType.InstallShieldCAB,
            WrapperType.LZKWAJ,
            WrapperType.LZQBasic,
            WrapperType.LZSZDD,
            WrapperType.MicrosoftCAB,
            WrapperType.MoPaQ,
            //WrapperType.N3DS,
            //WrapperType.NCF,
            //WrapperType.Nitro,
            WrapperType.PAK,
            WrapperType.PFF,
            WrapperType.PKZIP,
            //WrapperType.PlayJAudioFile, // TODO: Create wrapper to reenable test
            //WrapperType.Quantum,
            WrapperType.RAR,
            WrapperType.SevenZip,
            WrapperType.SFFS,
            WrapperType.SGA,
            WrapperType.TapeArchive,
            WrapperType.VBSP,
            WrapperType.VPK,
            WrapperType.WAD,
            WrapperType.XZ,
            WrapperType.XZP,
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