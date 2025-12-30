using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BinaryObjectScanner.Interfaces;
using SabreTools.Data.Models.ISO9660;
using SabreTools.IO;
using SabreTools.IO.Extensions;
using SabreTools.IO.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class StarForce : IDiskImageCheck<ISO9660>, IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <summary>
        /// If a StarForce Keyless hash is known to not fit the format, but is in some way a one-off.
        /// Key is the SF Keyless Key, value is redump ID
        /// </summary>
        private static readonly Dictionary<string, uint> UnusualStarforceKeylessKeys = new()
        {
            {"FYFYILOVEYOU", 60270},
        };

        /// <inheritdoc/>
        public string? CheckDiskImage(string file, ISO9660 diskImage, bool includeDebug)
        {
            if (diskImage.VolumeDescriptorSet.Length == 0)
                return null;
            if (diskImage.VolumeDescriptorSet[0] is not PrimaryVolumeDescriptor pvd)
                return null;

            int offset = 0;

            // StarForce Keyless check: the key is stored in the Data Preparer identifier.
            string? dataPreparerIdentiferString = pvd.DataPreparerIdentifier.ReadNullTerminatedAnsiString(ref offset)?.Trim();

            if (dataPreparerIdentiferString != null
                && dataPreparerIdentiferString.Length != 0
                && Regex.IsMatch(dataPreparerIdentiferString, "^[A-Z0-9-]*$", RegexOptions.Compiled))
            {
                // It is returning the key, as it tells you what set of DPM your disc corresponds to, and it would also
                // help show why a disc might be an alt of another disc (there are at least a decent amount of StarForce
                // Keyless alts that would amtch otherwise). Unclear if this is desired by the users of BOS or those
                // affected by it.

                // Thus far, the StarForce Keyless key is always made up of a number of characters, all either capital letters or
                // numbers, sometimes with dashes in between. Thus far, 4 formats have been observed:
                // 24 characters has been observed on one single disc (RID 68149). Maybe one is a space?
                // XXXXXXXXXXXXXXXXXXXXXXXXX (25 characters)
                // XXXXX-XXXXX-XXXXX-XXXXX-XXXXX (25 characters, plus 4 dashes seperating 5 groups of 5)
                // XXXXXXXXXXXXXXXXXXXXXXXXXXXX (28 characters)
                // XXXX-XXXXXX-XXXXXX-XXXXXX-XXXXXX (28 characters, with 4 dashes)
                if (Regex.IsMatch(dataPreparerIdentiferString, "^[A-Z0-9]{24,25}$", RegexOptions.Compiled)
                    || Regex.IsMatch(dataPreparerIdentiferString, "^[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}$", RegexOptions.Compiled)
                    || Regex.IsMatch(dataPreparerIdentiferString, "^[A-Z0-9]{28}$", RegexOptions.Compiled)
                    || Regex.IsMatch(dataPreparerIdentiferString, "^[A-Z0-9]{4}-[A-Z0-9]{6}-[A-Z0-9]{6}-[A-Z0-9]{6}-[A-Z0-9]{6}$", RegexOptions.Compiled))
                {
                    return $"StarForce Keyless - {dataPreparerIdentiferString}";
                }

                // Redump ID 60270 is a unique case, there could possibly be more.
                if (UnusualStarforceKeylessKeys.ContainsKey(dataPreparerIdentiferString))
                    return $"StarForce Keyless - {dataPreparerIdentiferString}";
            }

            // Starforce general check: the reserved 653 bytes start with a 32-bit LE number that's slightly less
            // than the length of the volume size space. The difference varies, it's usually around 10. Check 500 to be
            // safe. The rest of the data is all 0x00. Not many starforce discs have this, but some do, and it's
            // currently the only known non-keyless check. Redump ID 60266, 72531, 87181, 91734, 106732, 105356, 74578,
            // 78200 are some examples.
            if (FileType.ISO9660.NoteworthyApplicationUse(pvd))
                return null;

            if (!FileType.ISO9660.NoteworthyReserved653Bytes(pvd))
                return null;

            offset = 0;

            var reserved653Bytes = pvd.Reserved653Bytes;
            uint initialValue = reserved653Bytes.ReadUInt32LittleEndian(ref offset);
            var zeroBytes = reserved653Bytes.ReadBytes(ref offset, 649);

            // It's unfortunately not known to be possible to detect many non-keyless StarForce discs, so some will slip
            // through here.
            if (initialValue > pvd.VolumeSpaceSize || initialValue + 500 < pvd.VolumeSpaceSize || !Array.TrueForAll(zeroBytes, b => b == 0x00))
                return null;

            return "StarForce";
        }

        // TODO: Bring up to par with PiD.
        // Known issues:
        // "Game.exe" not detected (Redump entry 96137).
        // "HR.exe" Themida not detected, doesn't detect "[Builder]" (Is that the default StarForce?) (Redump entry 94805).
        // "ChromeEngine3.dll" and "SGP4.dll" not detected, doesn't detect "[FL Disc]" (Redump entry 93098).
        // "Replay.exe" not detected, doesn't detect "[FL Disc]" (Redump entry 81756).
        // Doesn't detect "[Pro]" (Redump entry 91336).
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.FileDescription;

            // There are some File Description checks that are currently too generic to use.
            // "Host Library" - Found in "protect.dll" in Redump entry 81756.
            // "User Interface Application" - Found in "protect.exe" in Redump entry 81756.
            // "Helper Application" - Found in "protect.x64" and "protect.x86" in Redump entry 81756.

            // Found in "sfdrvrem.exe" in Redump entry 102677.
            if (name.OptionalContains("FrontLine Drivers Removal Tool"))
                return "StarForce FrontLine Driver Removal Tool";

            // Found in "protect.exe" in Redump entry 94805.
            if (name.OptionalContains("FrontLine Protection GUI Application"))
                return $"StarForce {exe.GetInternalVersion()}";

            // Found in "protect.dll" in Redump entry 94805.
            if (name.OptionalContains("FrontLine Protection Library"))
                return $"StarForce {exe.GetInternalVersion()}";

            // Found in "protect.x64" and "protect.x86" in Redump entry 94805.
            if (name.OptionalContains("FrontLine Helper"))
                return $"StarForce {exe.GetInternalVersion()}";

            // TODO: Find a sample of this check.
            if (name.OptionalContains("Protected Module"))
                return "StarForce 5";

            name = exe.LegalCopyright;

            if (name.OptionalStartsWith("(c) Protection Technology")) // (c) Protection Technology (StarForce)?
                return $"StarForce {exe.GetInternalVersion()}";
            else if (name.OptionalContains("Protection Technology")) // Protection Technology (StarForce)?
                return $"StarForce {exe.GetInternalVersion()}";

            name = exe.TradeName;

            // FrontLine ProActive (digital activation), samples:
            // https://dbox.tools/titles/pc/46450FA4/
            // https://dbox.tools/titles/pc/4F430FA0/
            // https://dbox.tools/titles/pc/53450FA1/
            name = exe.TradeName;
            if (name.OptionalContains("FL ProActive"))
                return $"StarForce FrontLine ProActive {exe.GetInternalVersion()}";

            // StarForce Crypto (SF Crypto)
            // Found in "pcnsl.exe" in Redump entry 119679
            // TODO: return version?
            if (name.OptionalContains("SF Crypto"))
                return "StarForce Crypto";

            // TODO: Decide if internal name checks are safe to use.
            name = exe.InternalName;

            // Found in "protect.x64" and "protect.x86" in Redump entry 94805.
            if (name.OptionalEquals("CORE.ADMIN", StringComparison.Ordinal))
                return $"StarForce {exe.GetInternalVersion()}";

            // These checks currently disabled due being possibly too generic:
            // Found in "protect.dll" in Redump entry 94805.
            // if (name.OptionalEquals("CORE.DLL", StringComparison.Ordinal))
            //     return $"StarForce {Tools.Utilities.GetInternalVersion(exe)}";
            //
            // Found in "protect.exe" in Redump entry 94805.
            // if (name.OptionalEquals("CORE.EXE", StringComparison.Ordinal))
            //     return $"StarForce {Tools.Utilities.GetInternalVersion(exe)}";
            //
            // else if (name.OptionalEquals("protect.exe", StringComparison.Ordinal))
            //     return $"StarForce {Tools.Utilities.GetInternalVersion(exe)}";

            // Check the export name table
            if (exe.ExportNameTable?.Strings != null)
            {
                // TODO: Should we just check for "PSA_*" instead of a single entry?
                if (Array.Exists(exe.ExportNameTable.Strings, s => s == "PSA_GetDiscLabel"))
                    return $"StarForce {exe.GetInternalVersion()}";
            }

            // TODO: Check to see if there are any missing checks
            // https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/StarForce.2.sg

            // Get the .brick section, if it exists
            if (exe.ContainsSection(".brick", exact: true))
                return "StarForce 3-5";

            // Get the .sforce* section, if it exists
            if (exe.ContainsSection(".sforce", exact: false))
                return "StarForce 3-5";

            // TODO: Investigate the .common and .ps4 sections found in apache.exe
            // .common doesn't map to any table
            //      The section is largely empty with a 3 or 4 byte value at
            //      offset 0x40. Sample has "D4 DB DD 00"
            // .ps4 has a virtual size of 4096 and a physical size of 0
            //      The physical offset is the end of the file

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // This file combination is found in Redump entry 21136.
                new(
                [
                    new FilePathMatch("protect.x86"),
                    new FilePathMatch("protect.x64"),
                    new FilePathMatch("protect.dll"),
                    new FilePathMatch("protect.exe"),
                    new FilePathMatch("protect.msg"),
                ], "StarForce"),

                // This file combination is found in multiple games, such as Redump entries 81756, 91336, and 93657.
                new(
                [
                    new FilePathMatch("protect.x86"),
                    new FilePathMatch("protect.x64"),
                    new FilePathMatch("protect.dll"),
                    new FilePathMatch("protect.exe"),
                ], "StarForce"),

                // This file combination is found in Redump entry 96137.
                new(
                [
                    new FilePathMatch("protect.x86"),
                    new FilePathMatch("protect.dll"),
                    new FilePathMatch("protect.exe"),
                ], "StarForce"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            // TODO: Determine if there are any file name checks that aren't too generic to use on their own.
            return null;
        }
    }
}
