using System;
using System.Collections.Generic;
using System.Text;
using BinaryObjectScanner.Interfaces;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO;
using SabreTools.IO.Extensions;
using SabreTools.IO.Matching;
using SabreTools.Serialization.Wrappers;
using static SabreTools.Data.Models.SecuROM.Constants;

#pragma warning disable IDE0059 // Unnecessary assignment of value
namespace BinaryObjectScanner.Protection
{
    // TODO: Investigate SecuROM for Macintosh
    // TODO: Think of a way to detect dfe
    public partial class SecuROM : IDiskImageCheck<ISO9660>, IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckDiskImage(string file, ISO9660 diskImage, bool includeDebug)
        {

            if (diskImage.VolumeDescriptorSet.Length == 0)
                return null;
            if (diskImage.VolumeDescriptorSet[0] is not PrimaryVolumeDescriptor pvd)
                return null;

            // Application Use is too inconsistent to include or exclude

            // There needs to be noteworthy data in the reserved 653 bytes
            if (!FileType.ISO9660.NoteworthyReserved653Bytes(pvd))
                return null;

            var applicationUse = pvd.ApplicationUse;
            var reserved653Bytes = pvd.Reserved653Bytes;

            #region Read Application Use

            var offset = 0;

            // Either there's nothing of note, or it's empty other than a 4-byte value at the start.
            if (FileType.ISO9660.NoteworthyApplicationUse(pvd))
            {
                uint appUseUint = applicationUse.ReadUInt32LittleEndian(ref offset);
                var appUseZeroBytes = applicationUse.ReadBytes(ref offset, 508);

                if (appUseUint == 0 || !Array.TrueForAll(appUseZeroBytes, b => b == 0x00))
                    return null;
            }

            #endregion

            #region Read Reserved 653 Bytes

            offset = 0;

            var reservedZeroBytesOne = reserved653Bytes.ReadBytes(ref offset, 489);
            uint reservedHundredValue = reserved653Bytes.ReadUInt32LittleEndian(ref offset);
            var reserveDataBytesOne = reserved653Bytes.ReadBytes(ref offset, 80);
            var reservedZeroBytesTwo = reserved653Bytes.ReadBytes(ref offset, 12);
            uint reservedUintOne = reserved653Bytes.ReadUInt32LittleEndian(ref offset);
            uint reservedUintTwoLow = reserved653Bytes.ReadUInt32LittleEndian(ref offset); // Low value
            var reservedZeroBytesThree = reserved653Bytes.ReadBytes(ref offset, 4);
            uint reservedUintThree = reserved653Bytes.ReadUInt32LittleEndian(ref offset);
            var reservedZeroBytesFour = reserved653Bytes.ReadBytes(ref offset, 12);
            uint reservedUintFour = reserved653Bytes.ReadUInt32LittleEndian(ref offset);
            uint reservedOneValue = reserved653Bytes.ReadUInt32LittleEndian(ref offset);
            var reservedZeroBytesFive = reserved653Bytes.ReadBytes(ref offset, 4);
            var reservedDataBytesTwo = reserved653Bytes.ReadBytes(ref offset, 12);
            byte reservedLowByteValueOne = reserved653Bytes.ReadByteValue(ref offset);
            byte reservedLowByteValueTwo = reserved653Bytes.ReadByteValue(ref offset);
            byte reservedLowByteValueThree = reserved653Bytes.ReadByteValue(ref offset);
            byte reservedLowByteValueFour = reserved653Bytes.ReadByteValue(ref offset);
            var reservedDataBytesThree = reserved653Bytes.ReadBytes(ref offset, 12);

            #endregion

            // True for all discs
            if (!Array.TrueForAll(reservedZeroBytesOne, b => b == 0x00)
                || !Array.TrueForAll(reservedZeroBytesTwo, b => b == 0x00)
                || !Array.TrueForAll(reservedZeroBytesThree, b => b == 0x00)
                || !Array.TrueForAll(reservedZeroBytesFour, b => b == 0x00)
                || !Array.TrueForAll(reservedZeroBytesFive, b => b == 0x00))
            {
                return null;
            }

            #region Early SecuROM Checks

            // This duplicates a lot of code. This region is like this because it's still possible to detect early vers,
            // but it should be easy to remove this section if it turns out this leads to conflicts or false positives
            if (Array.TrueForAll(reserveDataBytesOne, b => b == 0x00)
                && Array.TrueForAll(reservedDataBytesTwo, b => b == 0x00)
                && reservedHundredValue == 0 && reservedOneValue == 0
                && reservedUintOne == 0 && reservedUintTwoLow == 0 && reservedUintThree == 0 && reservedUintFour == 0
                && reservedLowByteValueOne == 0 && reservedLowByteValueTwo == 0 && reservedLowByteValueThree == 0)
            {
                if (FileType.ISO9660.IsPureData(reservedDataBytesThree))
                {
                    if (reservedLowByteValueFour == 0)
                        return "SecuROM 3.x-4.6x";
                    else if (reservedLowByteValueFour < 0x20)
                        return "SecuROM 4.7x-4.8x";
                    else
                        return null;
                }

                offset = 0;
                var earlyFirstFourBytes = reservedDataBytesThree.ReadBytes(ref offset, 4);
                var earlyLastEightBytes = reservedDataBytesThree.ReadBytes(ref offset, 8);

                if (Array.TrueForAll(earlyFirstFourBytes, b => b == 0x00) && FileType.ISO9660.IsPureData(earlyLastEightBytes))
                    return "SecuROM 2.x-3.x";
            }

            #endregion

            // If this uint32 is 100, the next 80 bytes should be data. Otherwise, both should only ever be zero.

            switch (reservedHundredValue)
            {
                case 0:
                    if (!Array.TrueForAll(reserveDataBytesOne, b => b == 0x00))
                        return null;
                    break;
                case 100:
                    if (!FileType.ISO9660.IsPureData(reserveDataBytesOne))
                        return null;
                    break;
                default:
                    return null;
            }

            //If you go back to early 4.0 CDs, only the above can be guaranteed to pass. CDs can already be identified via normal
            //dumping, though, and  (as well as most later CDs) should always pass these remaining checks.
            if (reservedUintOne < 0xFFFF || reservedUintTwoLow > 0xFFFF || reservedUintThree < 0xFFFF || reservedUintFour < 0xFFFF)
                return null;

            if (reservedOneValue != 1)
                return null;

            if (reservedLowByteValueOne > 0x20 || reservedLowByteValueTwo > 0x20 || reservedLowByteValueThree > 0x20 ||
                reservedLowByteValueFour > 0x20)
                return null;

            // TODO: RID 127715 fails this because the first 8 bytes of reservedDataBytesTwo happen to be "afsCafsC"
            if (!FileType.ISO9660.IsPureData(reservedDataBytesTwo)
                || !FileType.ISO9660.IsPureData(reservedDataBytesThree))
                return null;

            return "SecuROM 4.8x+";
        }

        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Check if executable is a SecuROM PA module
            var paModule = CheckProductActivation(exe);
            if (paModule is not null)
                return paModule;

            // Check if executable contains a SecuROM Matroschka Package
            var package = exe.MatroschkaPackage;
            if (package is not null)
            {
                var packageType = CheckMatroschkaPackage(package, includeDebug);
                if (packageType is not null)
                    return packageType;
            }

            // Alf.dll
            string? name = exe.ProductName;
            if (name.OptionalEquals("DFA Unlock Dll"))
                return $"SecuROM DFA Unlock v{exe.GetInternalVersion()}";

            if (name.OptionalEquals("Release Control Unlock Dll"))
                return $"SecuROM Release Control Unlock v{exe.GetInternalVersion()}";

            // Dfa.dll and ca.dll. The former seems to become the latter later on.
            name = exe.FileDescription;
            if (name.OptionalEquals("SecuROM Data File Activation Library"))
                return $"SecuROM Data File Activation v{exe.GetInternalVersion()}";

            // Copyright is only checked because "Content Activation Library" seems broad on its own.
            if (name.OptionalEquals("Content Activation Library") && exe.LegalCopyright.OptionalContains("Sony DADC Austria AG"))
                return $"SecuROM Content Activation v{exe.GetInternalVersion()}";

            if (exe.ContainsSection(".dsstext", exact: true))
                return $"SecuROM 8.03.03+";

            // Get the .securom section, if it exists
            if (exe.ContainsSection(".securom", exact: true))
                return $"SecuROM {GetV7Version(exe)}";

            // Get the .sll section, if it exists
            if (exe.ContainsSection(".sll", exact: true))
                return $"SecuROM SLL Protected (for SecuROM v8.x)";

            // Search after the last section
            string? v4Version = GetV4Version(exe);
            if (v4Version is not null)
                return $"SecuROM {v4Version}";

            // TODO: Investigate if this can be found by aligning to section containing entry point

            // Get the sections 5+, if they exist (example names: .fmqyrx, .vcltz, .iywiak)
            var sections = exe.SectionTable;
            for (int i = 4; i < sections.Length; i++)
            {
                var nthSection = sections[i];
                if (nthSection is null)
                    continue;

                string nthSectionName = Encoding.ASCII.GetString(nthSection.Name ?? []).TrimEnd('\0');
                if (nthSectionName != ".idata" && nthSectionName != ".rsrc")
                {
                    var nthSectionData = exe.GetFirstSectionData(nthSectionName);
                    if (nthSectionData is null)
                        continue;

                    var matchers = new List<ContentMatchSet>
                    {
                        // (char)0xCA + (char)0xDD + (char)0xDD + (char)0xAC + (char)0x03
                        new(new byte?[] { 0xCA, 0xDD, 0xDD, 0xAC, 0x03 }, GetV5Version, "SecuROM"),
                    };

                    var match = MatchUtil.GetFirstMatch(file, nthSectionData, matchers, includeDebug);
                    if (!string.IsNullOrEmpty(match))
                        return match;
                }
            }

            // Get the .rdata section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".rdata");
            if (strs is not null)
            {
                // Both have the identifier found within `.rdata` but the version is within `.data`
                if (strs.Exists(s => s.Contains("/secuexp")))
                    return $"SecuROM {GetV8WhiteLabelVersion(exe)} (White Label)";
                else if (strs.Exists(s => s.Contains("SecuExp.exe")))
                    return $"SecuROM {GetV8WhiteLabelVersion(exe)} (White Label)";
            }

            // Get the .cms_d and .cms_t sections, if they exist -- TODO: Confirm if both are needed or either/or is fine
            if (exe.ContainsSection(".cmd_d", true))
                return $"SecuROM 1-3";
            if (exe.ContainsSection(".cms_t", true))
                return $"SecuROM 1-3";

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // TODO: Verify if these are OR or AND
                new(new FilePathMatch("CMS16.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS_95.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS_NT.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS32_95.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS32_NT.DLL"), "SecuROM"),

                // TODO: Verify if these are OR or AND
                new(new FilePathMatch("SINTF32.DLL"), "SecuROM New"),
                new(new FilePathMatch("SINTF16.DLL"), "SecuROM New"),
                new(new FilePathMatch("SINTFNT.DLL"), "SecuROM New"),

                // TODO: Find more samples of this for different versions
                new(
                [
                    new FilePathMatch("securom_v7_01.bak"),
                    new FilePathMatch("securom_v7_01.dat"),
                    new FilePathMatch("securom_v7_01.tmp"),
                ], "SecuROM 7.01"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("CMS16.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS_95.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS_NT.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS32_95.DLL"), "SecuROM"),
                new(new FilePathMatch("CMS32_NT.DLL"), "SecuROM"),

                new(new FilePathMatch("SINTF32.DLL"), "SecuROM New"),
                new(new FilePathMatch("SINTF16.DLL"), "SecuROM New"),
                new(new FilePathMatch("SINTFNT.DLL"), "SecuROM New"),

                new(new FilePathMatch("securom_v7_01.bak"), "SecuROM 7.01"),
                new(new FilePathMatch("securom_v7_01.dat"), "SecuROM 7.01"),
                new(new FilePathMatch("securom_v7_01.tmp"), "SecuROM 7.01"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        /// <summary>
        /// Try to get the SecuROM v4 version from the overlay, if possible
        /// </summary>
        /// <returns>The version on success, null otherwise</returns>
        private static string? GetV4Version(PortableExecutable exe)
        {
            // Cache the overlay data for easier access
            var overlayData = exe.OverlayData;
            if (overlayData is null || overlayData.Length < 20)
                return null;

            // Search for the "AddD" string in the overlay
            bool found = false;
            int index = 0;
            for (; index < 0x20 && index + 4 < overlayData.Length; index++)
            {
                int temp = index;
                byte[] overlaySample = overlayData.ReadBytes(ref temp, 0x04);
                if (overlaySample.EqualsExactly(AddDMagicBytes))
                {
                    found = true;
                    break;
                }
            }

            // If the string wasn't found in the first 0x20 bytes
            if (!found)
                return null;

            // Deserialize the AddD header
            var reader = new SabreTools.Serialization.Readers.SecuROMAddD();
            var addD = reader.Deserialize(overlayData, index);
            if (addD is null)
                return null;

            // All samples have had 3 entries -- Revisit if needed
            if (addD.EntryCount != 3)
                return null;

            // Format the version
            string version = $"{addD.Version}.{addD.Build}";
            if (!char.IsNumber(version[0]))
                return "(very old, v3 or less)";

            return version;
        }

        /// <summary>
        /// Try to get the SecuROM v5 version from section data, if possible
        /// </summary>
        /// <returns>The version on success, null otherwise</returns>
        private static string? GetV5Version(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent is null)
                return null;

            int index = positions[0] + 8; // Begin reading after "ÊÝÝ¬"
            byte major = (byte)(fileContent[index] & 0x0F);
            index += 2;

            byte[] minor = new byte[2];
            minor[0] = (byte)(fileContent[index] ^ 36);
            index++;
            minor[1] = (byte)(fileContent[index] ^ 28);
            index += 2;

            byte[] patch = new byte[2];
            patch[0] = (byte)(fileContent[index] ^ 42);
            index++;
            patch[1] = (byte)(fileContent[index] ^ 8);
            index += 2;

            byte[] revision = new byte[4];
            revision[0] = (byte)(fileContent[index] ^ 16);
            index++;
            revision[1] = (byte)(fileContent[index] ^ 116);
            index++;
            revision[2] = (byte)(fileContent[index] ^ 34);
            index++;
            revision[3] = (byte)(fileContent[index] ^ 22);

            if (major == 0 || major > 9)
                return string.Empty;

            return $"{major}.{minor[0]}{minor[1]}.{patch[0]}{patch[1]}.{revision[0]}{revision[1]}{revision[2]}{revision[3]}";
        }

        /// <summary>
        /// Try to get the SecuROM v7 version from MS-DOS stub data, if possible
        /// </summary>
        /// <returns>The version on success, null otherwise</returns>
        private static string GetV7Version(PortableExecutable exe)
        {
            // If SecuROM is stripped, the MS-DOS stub might be shorter.
            // We then know that SecuROM -was- there, but we don't know what exact version.
            if (exe.StubExecutableData is null)
                return "7 remnants";

            //SecuROM 7 new and 8 -- 64 bytes for DOS stub, 236 bytes in total
            int index = 172;
            if (exe.StubExecutableData.Length >= 176 && exe.StubExecutableData[index + 3] == 0x5C)
            {
                int major = exe.StubExecutableData[index + 0] ^ 0xEA;
                int minor = exe.StubExecutableData[index + 1] ^ 0x2C;
                int patch = exe.StubExecutableData[index + 2] ^ 0x08;

                return $"{major}.{minor:00}.{patch:0000}";
            }

            // SecuROM 7 old -- 64 bytes for DOS stub, 122 bytes in total
            index = 58;
            if (exe.StubExecutableData.Length >= 62)
            {
                int minor = exe.StubExecutableData[index + 0] ^ 0x10;
                int patch = exe.StubExecutableData[index + 1] ^ 0x10;

                //return "7.01-7.10"
                return $"7.{minor:00}.{patch:0000}";
            }

            // If SecuROM is stripped, the MS-DOS stub might be shorter.
            // We then know that SecuROM -was- there, but we don't know what exact version.
            return "7 remnants";
        }

        /// <summary>
        /// Try to get the SecuROM v8 (White Label) version from the .data section, if possible
        /// </summary>
        /// <returns>The version on success, null otherwise</returns>
        private static string GetV8WhiteLabelVersion(PortableExecutable exe)
        {
            // Get the .data/DATA section, if it exists
            var dataSectionRaw = exe.GetFirstSectionData(".data") ?? exe.GetFirstSectionData("DATA");
            if (dataSectionRaw is null)
                return "8";

            // Search .data for the version indicator
            var matcher = new ContentMatch(
            [
                0x29, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null,
                0x82, 0xD8, 0x0C, 0xAC
            ]);

            int position = matcher.Match(dataSectionRaw);

            // If we can't find the string, we default to generic
            if (position < 0)
                return "8";

            int major = dataSectionRaw[position + 0xAC + 0] ^ 0xCA;
            int minor = dataSectionRaw[position + 0xAC + 1] ^ 0x39;
            int patch = dataSectionRaw[position + 0xAC + 2] ^ 0x51;

            return $"{major}.{minor:00}.{patch:0000}";
        }

        /// <summary>
        /// Helper method to run checks on a SecuROM Matroschka Package
        /// </summary>
        private static string? CheckMatroschkaPackage(SecuROMMatroschkaPackage package, bool includeDebug)
        {
            // Check for all 0x00 required, as at least one known non-RC matroschka has the field, just empty.
            if (package.KeyHexString is null || package.KeyHexString.Trim('\0').Length == 0)
                return "SecuROM Matroschka Package";

            if (package.Entries is null || package.Entries.Length == 0)
                return "SecuROM Matroschka Package - No Entries? - Please report to us on GitHub";

            // The second entry in a Release Control matroschka package is always the encrypted executable
            var entry = package.Entries[1];

            if (entry.MD5 is null || entry.MD5.Length == 0)
                return "SecuROM Matroschka Package - No MD5? - Please report to us on GitHub";

            string md5String = BitConverter.ToString(entry.MD5!);
            md5String = md5String.ToUpperInvariant().Replace("-", string.Empty);

            // TODO: Not used yet, but will be in the future
            var fileData = package.ReadFileData(entry, includeDebug);

            // Check if encrypted executable is known via hash
            if (MatroschkaHashDictionary.TryGetValue(md5String, out var gameName))
                return $"SecuROM Release Control -  {gameName}";

            // If not known, check if encrypted executable is likely an alt signing of a known executable
            // Filetime could be checked here, but if it was signed at a different time, the time will vary anyways
            var readPath = entry.Path;
            if (readPath is null || readPath.Length == 0)
                return $"SecuROM Release Control - Unknown executable {md5String},{entry.Size} - Please report to us on GitHub!";

            var readPathName = readPath.TrimEnd('\0');
            if (MatroschkaSizeFilenameDictionary.TryGetValue(entry.Size, out var pathName) && pathName == readPathName)
                return $"SecuROM Release Control - Unknown possible alt executable of size {entry.Size} - Please report to us on GitHub";

            return $"SecuROM Release Control - Unknown executable {readPathName},{md5String},{entry.Size} - Please report to us on GitHub";
        }

        /// <summary>
        /// Helper method to check if a given PortableExecutable is a SecuROM PA module.
        /// </summary>
        private static string? CheckProductActivation(PortableExecutable exe)
        {
            string? name = exe.FileDescription;
            if (name.OptionalContains("SecuROM PA"))
                return $"SecuROM Product Activation v{exe.GetInternalVersion()}";

            name = exe.InternalName;

            // Checks if ProductName isn't drEAm to organize custom module checks at the end.
            if (name.OptionalEquals("paul.dll", StringComparison.OrdinalIgnoreCase) ^ exe.ProductName.OptionalEquals("drEAm"))
                return $"SecuROM Product Activation v{exe.GetInternalVersion()}";
            else if (name.OptionalEquals("paul_dll_activate_and_play.dll"))
                return $"SecuROM Product Activation v{exe.GetInternalVersion()}";
            else if (name.OptionalEquals("paul_dll_preview_and_review.dll"))
                return $"SecuROM Product Activation v{exe.GetInternalVersion()}";

            name = exe.OriginalFilename;
            if (name.OptionalEquals("paul_dll_activate_and_play.dll"))
                return $"SecuROM Product Activation v{exe.GetInternalVersion()}";

            name = exe.ProductName;
            if (name.OptionalContains("SecuROM Activate & Play"))
                return $"SecuROM Product Activation v{exe.GetInternalVersion()}";

            // Custom Module Checks

            if (exe.ProductName.OptionalEquals("drEAm"))
                return $"SecuROM Product Activation v{exe.GetInternalVersion()} - EA Game Authorization Management";

            // Fallback for PA if none of the above occur, in the case of companies that used their own modified PA
            // variants. PiD refers to this as "SecuROM Modified PA Module".
            // Found in Redump entries 111997 (paul.dll) and 56373+56374 (AurParticleSystem.dll). The developers of
            // both, Softstar and Aurogon respectively(?), seem to have some connection, and use similar-looking
            // modified PA. It probably has its own name like EA's GAM, but I don't currently know what that would be.
            // Regardless, even if these are given their own named variant later, this check should remain in order to
            // catch other modified PA variants (this would have also caught EA GAM, for example) and to match PiD's
            // detection abilities.

            name = exe.ExportNameTable?.Strings?[0];
            if (name.OptionalEquals("drm_pagui_doit"))
            {
                // Not all of them are guaranteed to have an internal version
                var version = exe.GetInternalVersion();
                if (string.IsNullOrEmpty(version))
                    return $"SecuROM Product Activation - Modified";

                return $"SecuROM Product Activation v{exe.GetInternalVersion()} - Modified";
            }

            return null;
        }
    }
}
