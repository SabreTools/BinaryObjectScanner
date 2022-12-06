using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class UPX : IPortableExecutableCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Check header padding data
            var headerPaddingData = pex.HeaderPaddingData;
            if (headerPaddingData != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // UPX!
                    new ContentMatchSet(new byte?[] { 0x55, 0x50, 0x58, 0x21 }, GetVersion, "UPX"),

                    // NOS 
                    new ContentMatchSet(new byte?[] { 0x4E, 0x4F, 0x53, 0x20 }, GetVersion, "UPX (NOS Variant)"),
                };

                string match = MatchUtil.GetFirstMatch(file, headerPaddingData, matchers, includeDebug);
                if (!string.IsNullOrEmpty(match))
                    return match;
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
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