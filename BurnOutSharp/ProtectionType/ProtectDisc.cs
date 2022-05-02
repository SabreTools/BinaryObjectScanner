using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    // This protection was called VOB ProtectCD / ProtectDVD in versions prior to 6
    public class ProtectDISC : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the 4th section, if it exists (example names: ACE4)
            var fourthSection = sections.Length < 4 ? null : sections[3];
            if (fourthSection != null)
            {
                var fourthSectionData = pex.ReadRawSection(fourthSection.NameString, first: true);
                if (fourthSectionData != null)
                {
                    var matchers = new List<ContentMatchSet>
                    {
                        // ACE-PCD
                        new ContentMatchSet(new byte?[] { 0x41, 0x43, 0x45, 0x2D, 0x50, 0x43, 0x44 }, GetVersion6till8, "ProtectDISC"),
                    };

                    string match = MatchUtil.GetFirstMatch(file, fourthSectionData, matchers, includeDebug);
                    if (!string.IsNullOrWhiteSpace(match))
                        return match;
                }
            }

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // DCP-BOV + (char)0x00 + (char)0x00
                    new ContentMatchSet(new byte?[] { 0x44, 0x43, 0x50, 0x2D, 0x42, 0x4F, 0x56, 0x00, 0x00 }, GetVersion3till6, "VOB ProtectCD/DVD"),
                };

                string match = MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the second to last section
            var secondToLastSection = sections.Length > 1 ? sections[sections.Length - 2] : null;
            if (secondToLastSection != null)
            {
                var secondToLastSectionData = pex.ReadRawSection(secondToLastSection.NameString, first: true);
                if (secondToLastSectionData != null)
                {
                    var matchers = new List<ContentMatchSet>
                    {
                        // VOB ProtectCD
                        new ContentMatchSet(
                            new byte?[]
                            {
                                0x56, 0x4F, 0x42, 0x20, 0x50, 0x72, 0x6F, 0x74,
                                0x65, 0x63, 0x74, 0x43, 0x44
                            },
                            GetOldVersion,
                            "VOB ProtectCD/DVD"),
                    };

                    string match = MatchUtil.GetFirstMatch(file, secondToLastSectionData, matchers, includeDebug);
                    if (!string.IsNullOrWhiteSpace(match))
                        return match;
                }
            }

            // Get the last section (example names: ACE5, akxpxgcv, and piofinqb)
            var lastSection = sections.LastOrDefault();
            if (lastSection != null)
            {
                var lastSectionData = pex.ReadRawSection(lastSection.NameString, first: true);
                if (lastSectionData != null)
                {
                    var matchers = new List<ContentMatchSet>
                    {
                        // HúMETINF
                        new ContentMatchSet(new byte?[] { 0x48, 0xFA, 0x4D, 0x45, 0x54, 0x49, 0x4E, 0x46 }, GetVersion76till10, "ProtectDISC"),

                        // DCP-BOV + (char)0x00 + (char)0x00
                        new ContentMatchSet(new byte?[] { 0x44, 0x43, 0x50, 0x2D, 0x42, 0x4F, 0x56, 0x00, 0x00 }, GetVersion3till6, "VOB ProtectCD/DVD"),
                    };

                    string match = MatchUtil.GetFirstMatch(file, lastSectionData, matchers, includeDebug);
                    if (!string.IsNullOrWhiteSpace(match))
                        return match;
                }
            }

            // Get the .vob.pcd section, if it exists
            bool vobpcdSection = pex.ContainsSection(".vob.pcd", exact: true);
            if (vobpcdSection)
                return "VOB ProtectCD";

            return null;
        }

        public static string GetOldVersion(string file, byte[] fileContent, List<int> positions)
        {
            int position = positions[0] + 16; // Begin reading after "VOB ProtectCD"
            char[] version = new ArraySegment<byte>(fileContent, position, 4).Select(b => (char)b).ToArray();
            if (char.IsNumber(version[0]) && char.IsNumber(version[2]) && char.IsNumber(version[3]))
                return $"{version[0]}.{version[2]}{version[3]}";

            // Look for the legacy support version
            position = positions[0] + "VOB ProtectCD with LEGACY SYSIPHOS Support V".Length;
            version = new ArraySegment<byte>(fileContent, position, 7).Select(b => (char)b).ToArray();
            if (char.IsNumber(version[0]) && char.IsNumber(version[2]) && char.IsNumber(version[4]))
                return new string(version);

            return "old";
        }

        public static string GetVersion3till6(string file, byte[] fileContent, List<int> positions)
        {
            string version = GetVOBVersion(fileContent, positions[0]);
            if (version.Length > 0)
                return version;

            return $"5.9-6.0 {GetVOBBuild(fileContent, positions[0])}";
        }

        public static string GetVersion6till8(string file, byte[] fileContent, List<int> positions)
        {
            string version, strBuild = string.Empty;
            int index = positions[0] - 12;

            // Version 6-7 with Build Number in plain text
            if (new ArraySegment<byte>(fileContent, index, 4).SequenceEqual(new byte[] { 0x0A, 0x0D, 0x0A, 0x0D }))
            {
                index = positions[0] - 12 - 6;

                // Version 7
                if (new string(new ArraySegment<byte>(fileContent, index, 6).Select(b => (char)b).ToArray()) == "Henrik")
                {
                    version = "7.1-7.5";
                    index = positions[0] - 12 - 6 - 6;
                }

                // Version 6
                else
                {
                    // TODO: Figure out if the version can be more granular
                    version = "6";
                    index = positions[0] - 12 - 10;
                    while (true) //search for e.g. "Build 050913 -  September 2005"
                    {
                        if (Char.IsNumber((char)fileContent[index]))
                            break;

                        index--; // Search upwards
                    }

                    index -= 5;
                }

                char[] arrBuild = new ArraySegment<byte>(fileContent, index, 6).Select(b => (char)b).ToArray();
                if (!Int32.TryParse(new string(arrBuild), out int intBuild))
                    strBuild = $"[Build {new string(arrBuild).Trim()}]";
                else
                    strBuild = $"[Build 0x{intBuild:X} / {intBuild}]";

                return $"{version} {strBuild}".Trim();
            }
            else
            {
                index = positions[0] + 28;
                if (fileContent[index] == 0xFB)
                    return "7.6-7.x";
                else
                    return "8.0";
            }
        }

        public static string GetVersion76till10(string file, byte[] fileContent, List<int> positions)
        {
            int index = positions[0] + 37;
            byte subversion = fileContent[index];
            index += 2;
            byte version = fileContent[index];
            index = positions[0] + 49;
            int irefBuild = BitConverter.ToInt32(fileContent, index);
            index = positions[0] + 53;
            byte versionindicatorPD9 = fileContent[index];
            index = positions[0] + 0x40;
            byte subsubversionPD9x = fileContent[index];
            index++;
            byte subversionPD9x2 = fileContent[index];
            index++;
            byte subversionPD9x1 = fileContent[index];

            // Version 7
            if (version == 0xAC)
            {
                // TODO: Figure out if the version can be more granular
                return $"7.{subversion ^ 0x43} [Build 0x{irefBuild:X} / {irefBuild}]";
            }

            // Version 8
            else if (version == 0xA2)
            {
                if (subversion == 0x46)
                {
                    if ((irefBuild & 0x3A00) == 0x3A00)
                        return $"8.2 [Build 0x{irefBuild:X} / {irefBuild}]";
                    else
                        return $"8.1 [Build 0x{irefBuild:X} / {irefBuild}]";
                }

                // TODO: Figure out if the version can be more granular
                return $"8.{subversion ^ 0x47} [Build 0x{irefBuild:X} / {irefBuild}]";
            }

            // Version 9
            else if (version == 0xA3)
            {
                // Version removed or not given
                if ((subversionPD9x2 == 0x5F && subversionPD9x1 == 0x61) || (subversionPD9x1 == 0 && subversionPD9x2 == 0))
                {
                    if (versionindicatorPD9 == 0xB)
                    {
                        return $"9.0-9.4 [Build 0x{irefBuild:X} / {irefBuild}]";
                    }
                    else if (versionindicatorPD9 == 0xC)
                    {
                        // TODO: Figure out if the version can be more granular
                        if (subversionPD9x2 == 0x5F && subversionPD9x1 == 0x61)
                            return $"9.5-9.11 [Build 0x{irefBuild:X} / {irefBuild}]";
                        else if (subversionPD9x1 == 0 && subversionPD9x2 == 0)
                            return $"9.11-9.20 [Build 0x{irefBuild:X} / {irefBuild}]";
                    }
                }
                else
                {
                    return $"9.{subversionPD9x1}{subversionPD9x2}.{subsubversionPD9x} [Build 0x{irefBuild:X} / {irefBuild}]";
                }
            }

            // Version 10
            else if (version == 0xA0)
            {
                // Version removed -- TODO: Figure out if the version can be more granular
                if (subversionPD9x1 != 0 || subversionPD9x2 != 0)
                    return $"10.{subversionPD9x1}.{subsubversionPD9x} [Build 0x{irefBuild:X} / {irefBuild}]";
                else
                    return $"10.x [Build 0x{irefBuild:X} / {irefBuild}]";
            }

            // Unknown version
            else
            {
                return $"7.6-10.x [Build 0x{irefBuild:X} / {irefBuild}]";
            }

            return string.Empty;
        }
 
        private static string GetVOBBuild(byte[] fileContent, int position)
        {
            if (!char.IsNumber((char)fileContent[position - 13]))
                return string.Empty; //Build info removed

            int build = BitConverter.ToInt16(fileContent, position - 4); // Check if this is supposed to be a 4-byte read
            return $" (Build {build})";
        }

        // TODO: Ensure that this version finding works for all known versions
        private static string GetVOBVersion(byte[] fileContent, int position)
        {
            byte version = fileContent[position - 2];
            byte subVersion = (byte)((fileContent[position - 3] & 0xF0) >> 4);
            byte subSubVersion = (byte)((fileContent[position - 4] & 0xF0) >> 4);
            return $"{version}.{subVersion}.{subSubVersion}";
        }
    }
}
