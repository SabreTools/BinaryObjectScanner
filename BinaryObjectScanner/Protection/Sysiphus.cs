using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    public class Sysiphus : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                var str = strs.Find(s => s.Contains("V SUHPISYS"));
                if (str != null)
                    return $"Sysiphus {GetVersion(str)}";
            }

            return null;
        }

        private static string GetVersion(string matchedString)
        {
            // The string is reversed
            var matchedChars = matchedString.ToCharArray();
            Array.Reverse(matchedChars);
            matchedString = new string(matchedChars).Trim();

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
