using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class UPX : IPortableExecutableCheck, IScannable
    {
        private static readonly Regex _oldUpxVersionMatch = new Regex(@"\$Id: UPX (.*?) Copyright \(C\)", RegexOptions.Compiled);

        private static readonly Regex _upxVersionMatch = new Regex(@"^([0-9]\.[0-9]{2})$", RegexOptions.Compiled);

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Check header padding strings
            if (pex.HeaderPaddingStrings.Any())
            {
                string match = pex.HeaderPaddingStrings.FirstOrDefault(s => s.Contains("UPX!"));
                //if (match != null)
                //    return "UPX";

                match = pex.HeaderPaddingStrings.FirstOrDefault(s => s.StartsWith("$Id: UPX"));
                if (match != null)
                {
                    var regexMatch = _oldUpxVersionMatch.Match(match);
                    if (regexMatch.Success)
                        return $"UPX {regexMatch.Groups[1].Value}";
                    else
                        return "UPX (Unknown Version)";
                }

                match = pex.HeaderPaddingStrings.FirstOrDefault(s => _upxVersionMatch.IsMatch(s));
                if (pex.HeaderPaddingStrings.Any(s => s == "UPX!" && match != null))
                {
                    var regexMatch = _upxVersionMatch.Match(match);
                    if (regexMatch.Success)
                        return $"UPX {regexMatch.Groups[1].Value}";
                    else
                        return "UPX (Unknown Version)";
                }
                else if (pex.HeaderPaddingStrings.Any(s => s == "NOS " && match != null))
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

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            return null;
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            try
            {
                // Check the normal version location first
                int index = positions[0] - 5;
                string versionString = Encoding.ASCII.GetString(fileContent, index, 4);
                if (char.IsNumber(versionString[0]))
                    return versionString;
                
                // Check for the old-style string
                //
                // Example:
                // $Info: This file is packed with the UPX executable packer http://upx.tsx.org $
                // $Id: UPX 1.02 Copyright (C) 1996-2000 the UPX Team. All Rights Reserved. $
                // UPX!
                index = positions[0] - 67;
                versionString = Encoding.ASCII.GetString(fileContent, index, 4);
                if (char.IsNumber(versionString[0]))
                    return versionString;

                return "(Unknown Version)";
            }
            catch
            {
                return "(Unknown Version)";
            }
        }
    }
}