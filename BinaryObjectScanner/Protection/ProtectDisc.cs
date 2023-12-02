﻿using System;
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // This protection was called VOB ProtectCD / ProtectDVD in versions prior to 6
    // ProtectDISC 9/10 checks for the presence of CSS on the disc to run, but don't encrypt any sectors or check for keys. Confirmed in Redump entries 78367 and 110095.
    public class ProtectDISC : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the 4th and 5th sections, if they exist (example names: ACE4/ACE5) (Found in Redump entries 94792, 94793)
            for (int i = 3; i < sections.Length; i++)
            {
                var nthSectionData = pex.GetSectionData(i);
                if (nthSectionData == null)
                    continue;

                var matchers = new List<ContentMatchSet>
                {
                    // ACE-PCD
                    new(new byte?[] { 0x41, 0x43, 0x45, 0x2D, 0x50, 0x43, 0x44 }, GetVersion6till8, "ProtectDISC"),
                };

                var match = MatchUtil.GetFirstMatch(file, nthSectionData, matchers, includeDebug);
                if (!string.IsNullOrEmpty(match))
                    return match;
            }

            // Get the .data/DATA section, if it exists
            var dataSectionRaw = pex.GetFirstSectionData(".data") ?? pex.GetFirstSectionData("DATA");
            if (dataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // DCP-BOV + (char)0x00 + (char)0x00
                    new(new byte?[] { 0x44, 0x43, 0x50, 0x2D, 0x42, 0x4F, 0x56, 0x00, 0x00 }, GetVersion3till6, "VOB ProtectCD/DVD"),
                };

                var match = MatchUtil.GetFirstMatch(file, dataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrEmpty(match))
                    return match;
            }

            // Get the second to last section
            if (sections.Length > 1)
            {
                // Get the n - 1 section strings, if they exist
                var strs = pex.GetSectionStrings(sections.Length - 2);
                if (strs != null)
                {
                    var str = strs.FirstOrDefault(s => s.Contains("VOB ProtectCD"));
                    if (str != null)
                        return $"VOB ProtectCD {GetOldVersion(str)}";
                }
            }

            // Get the last section (example names: ACE5, akxpxgcv, and piofinqb)
            if (sections.Length > 0)
            {
                var lastSectionData = pex.GetSectionData(sections.Length - 1);
                if (lastSectionData != null)
                {
                    var matchers = new List<ContentMatchSet>
                {
                    // HúMETINF
                    new(new byte?[] { 0x48, 0xFA, 0x4D, 0x45, 0x54, 0x49, 0x4E, 0x46 }, GetVersion76till10, "ProtectDISC"),

                    // DCP-BOV + (char)0x00 + (char)0x00
                    new(new byte?[] { 0x44, 0x43, 0x50, 0x2D, 0x42, 0x4F, 0x56, 0x00, 0x00 }, GetVersion3till6, "VOB ProtectCD/DVD"),
                };

                    var match = MatchUtil.GetFirstMatch(file, lastSectionData, matchers, includeDebug);
                    if (!string.IsNullOrEmpty(match))
                        return match;
                }
            }

            // Get the .vob.pcd section, if it exists
            bool vobpcdSection = pex.ContainsSection(".vob.pcd", exact: true);
            if (vobpcdSection)
                return "VOB ProtectCD";

            return null;
        }

        public static string GetOldVersion(string matchedString)
        {
            // Remove unnecessary parts
            matchedString = matchedString.Trim()
                .Replace("----====#### ", string.Empty)
                .Replace(" ####====----", string.Empty);

            // Split the string
            string[] matchedStringArray = matchedString.Split(' ');

            // If we have no version
            if (matchedStringArray.Length <= 2)
                return "old";
            else if (matchedString.StartsWith("VOB ProtectCD with LEGACY SYSIPHOS Support V"))
                return $"with LEGACY SYSIPHOS Support {matchedStringArray[6].TrimStart('V')}";

            // Return the version string with leading `V` trimmed
            return matchedStringArray[2].TrimStart('V');
        }

        public static string? GetVersion3till6(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent == null)
                return null;

            string version = GetVOBVersion(fileContent, positions[0]);
            if (version.Length > 0)
                return version;

            return $"5.9-6.0 {GetVOBBuild(fileContent, positions[0])}";
        }

        public static string? GetVersion6till8(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent == null)
                return null;

            string version, strBuild = string.Empty;
            int index = positions[0] - 12;

            // Version 6-7 with Build Number in plain text
#if NET20 || NET35 || NET40
            byte[] temp = new byte[4];
            Array.Copy(fileContent, index, temp, 0, 4);
            if (temp.SequenceEqual(new byte[] { 0x0A, 0x0D, 0x0A, 0x0D }))
#else
            if (new ArraySegment<byte>(fileContent, index, 4).SequenceEqual(new byte[] { 0x0A, 0x0D, 0x0A, 0x0D }))
#endif
            {
                index = positions[0] - 12 - 6;

                // Version 7
#if NET20 || NET35 || NET40
                temp = new byte[6];
                Array.Copy(fileContent, index, temp, 0, 6);
                if (new string(temp.Select(b => (char)b).ToArray()) == "Henrik")
#else
                if (new string(new ArraySegment<byte>(fileContent, index, 6).Select(b => (char)b).ToArray()) == "Henrik")
#endif
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

#if NET20 || NET35 || NET40
                temp = new byte[6];
                Array.Copy(fileContent, index, temp, 0, 6);
                char[] arrBuild = temp.Select(b => (char)b).ToArray();
#else
                char[] arrBuild = new ArraySegment<byte>(fileContent, index, 6).Select(b => (char)b).ToArray();
#endif
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

        public static string? GetVersion76till10(string file, byte[]? fileContent, List<int> positions)
        {
            // If we have no content
            if (fileContent == null)
                return null;

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
