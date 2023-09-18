using System;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class Sysiphus : IPortableExecutableCheck
    {
        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
#if NET48
                string str = strs.FirstOrDefault(s => s.Contains("V SUHPISYS"));
#else
                string? str = strs.FirstOrDefault(s => s.Contains("V SUHPISYS"));
#endif
                if (str != null)
                    return $"Sysiphus {GetVersion(str)}";
            }

            return null;
        }

        public static string GetVersion(string matchedString)
        {
            // The string is reversed
            matchedString = new string(matchedString.Reverse().ToArray()).Trim();

            // Check for the DVD extra string
            bool isDVD = matchedString.StartsWith("DVD");

            // Get the version string
            string version = matchedString.Substring(isDVD ? "V SUHPISYSDVD".Length : "V SUHPISYS".Length, 4).Trim();

            if (char.IsNumber(version[0]) && char.IsNumber(version[2]))
                return isDVD ? $"DVD {version}" : version;

            return isDVD ? "DVD" : string.Empty;
        }
    }
}
