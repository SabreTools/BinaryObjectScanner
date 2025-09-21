using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class UPX : IExecutableCheck<PortableExecutable>
    {
        private static readonly Regex _oldUpxVersionMatch = new Regex(@"\$Id: UPX (.*?) Copyright \(C\)", RegexOptions.Compiled);

        private static readonly Regex _upxVersionMatch = new Regex(@"^([0-9]\.[0-9]{2})$", RegexOptions.Compiled);

        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Check header padding strings
            if (exe.HeaderPaddingStrings != null && exe.HeaderPaddingStrings.Count > 0)
            {
                var match = exe.HeaderPaddingStrings.Find(s => s.Contains("UPX!"));
                //if (match != null)
                //    return "UPX";

                match = exe.HeaderPaddingStrings.Find(s => s.StartsWith("$Id: UPX"));
                if (match != null)
                {
                    var regexMatch = _oldUpxVersionMatch.Match(match);
                    if (regexMatch.Success)
                        return $"UPX {regexMatch.Groups[1].Value}";
                    else
                        return "UPX (Unknown Version)";
                }

                match = exe.HeaderPaddingStrings.Find(s => _upxVersionMatch.IsMatch(s));
                if (match != null && exe.HeaderPaddingStrings.Exists(s => s == "UPX!"))
                {
                    var regexMatch = _upxVersionMatch.Match(match);
                    if (regexMatch.Success)
                        return $"UPX {regexMatch.Groups[1].Value}";
                    else
                        return "UPX (Unknown Version)";
                }
                else if (match != null && exe.HeaderPaddingStrings.Exists(s => s == "NOS "))
                {
                    var regexMatch = _upxVersionMatch.Match(match);
                    if (regexMatch.Success)
                        return $"UPX (NOS Variant) {regexMatch.Groups[1].Value}";
                    else
                        return "UPX (NOS Variant) (Unknown Version)";
                }
            }

            return null;
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            try
            {
                // Check the normal version location first
                int index = positions[0] - 5;
                if (index >= 0 && index < fileContent.Length - 4)
                {
                    string versionString = Encoding.ASCII.GetString(fileContent, index, 4);
                    if (char.IsNumber(versionString[0]))
                        return versionString;
                }

                // Check for the old-style string
                //
                // Example:
                // $Info: This file is packed with the UPX executable packer http://upx.tsx.org $
                // $Id: UPX 1.02 Copyright (C) 1996-2000 the UPX Team. All Rights Reserved. $
                // UPX!
                index = positions[0] - 67;
                if (index >= 0 && index < fileContent.Length - 4)
                {
                    string versionString = Encoding.ASCII.GetString(fileContent, index, 4);
                    if (char.IsNumber(versionString[0]))
                        return versionString;
                }

                return "(Unknown Version)";
            }
            catch
            {
                return "(Unknown Version)";
            }
        }
    }
}
