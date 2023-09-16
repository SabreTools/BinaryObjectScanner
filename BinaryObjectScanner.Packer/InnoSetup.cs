using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction - https://github.com/dscharrer/InnoExtract
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class InnoSetup : IExtractable, INewExecutableCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckNewExecutable(string file, NewExecutable nex, bool includeDebug)
        {
            // Check we have a valid executable
            if (nex == null)
                return null;
            
            // Check for "Inno" in the reserved words
            if (nex.Model.Stub?.Header?.Reserved2[4] == 0x6E49 && nex.Model.Stub?.Header?.Reserved2[5] == 0x6F6E)
            {
                string version = GetOldVersion(file, nex);
                if (!string.IsNullOrWhiteSpace(version))
                    return $"Inno Setup {version}";
                
                return "Inno Setup (Unknown Version)";
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .data/DATA section strings, if they exist
            List<string> strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                string str = strs.FirstOrDefault(s => s.StartsWith("Inno Setup Setup Data"));
                if (str != null)
                {
                    return str.Replace("Inno Setup Setup Data", "Inno Setup")
                        .Replace("(u)", "[Unicode]")
                        .Replace("(", string.Empty)
                        .Replace(")", string.Empty)
                        .Replace("[Unicode]", "(Unicode)");
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public string Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        public string Extract(Stream stream, string file, bool includeDebug)
        {
            return null;
        }

        private static string GetOldVersion(string file, NewExecutable nex)
        {
            // Notes:
            // Look into `SETUPLDR` in the resident-name table
            // Look into `SETUPLDR.EXE` in the nonresident-name table

            // TODO: Don't read entire file
            // TODO: Only 64 bytes at the end of the file is needed
            var data = nex.ReadArbitraryRange();
            if (data != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // "rDlPtS02" + (char)0x87 + "eVx"
                    new ContentMatchSet(new byte?[] { 0x72, 0x44, 0x6C, 0x50, 0x74, 0x53, 0x30, 0x32, 0x87, 0x65, 0x56, 0x78 }, "1.2.16 or earlier"),
                };

                return MatchUtil.GetFirstMatch(file, data, matchers, false) ?? "Unknown 1.X";
            }
            
            return "Unknown 1.X"; 
        }
    }
}
