using System;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Investigate the connection between this and VOB Protect CD/DVD
    public class ProtectDISC : IContentCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets() => null;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the 4th section, if it exists (example names: ACE4)
            var fourthSection = sections.Length < 4 ? null : sections[3];
            if (fourthSection != null)
            {
                int sectionAddr = (int)fourthSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)fourthSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // ACE-PCD
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x41, 0x43, 0x45, 0x2D, 0x50, 0x43, 0x44 }, start: sectionAddr, end: sectionEnd),
                    GetVersion6till8, "ProtectDISC"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            // Get the last section (example names: ACE5, akxpxgcv, and piofinqb)
            var lastSection = sections.LastOrDefault();
            if (lastSection != null)
            {
                int sectionAddr = (int)lastSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)lastSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // HúMETINF
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x48, 0xFA, 0x4D, 0x45, 0x54, 0x49, 0x4E, 0x46 }, start: sectionAddr, end: sectionEnd),
                    GetVersion76till10, "ProtectDISC"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
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
    }
}
