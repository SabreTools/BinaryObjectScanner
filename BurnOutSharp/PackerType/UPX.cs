using System.Collections.Generic;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class UPX : IPEContentCheck
    {
        /// <inheritdoc/>
        public string CheckPEContents(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Standard UPX
            var sectionData = FindData(pex, "UPX");
            if (sectionData != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // UPX!
                    new ContentMatchSet(new byte?[] { 0x55, 0x50, 0x58, 0x21 }, GetVersion, "UPX"),
                };

                return MatchUtil.GetFirstMatch(file, sectionData, matchers, includeDebug);
            }

            // NOS Variant
            sectionData = FindData(pex, "NOS");
            if (sectionData != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // NOS 
                    new ContentMatchSet(new byte?[] { 0x4E, 0x4F, 0x53, 0x20 }, GetVersion, "UPX (NOS Variant)"),
                };

                return MatchUtil.GetFirstMatch(file, sectionData, matchers, includeDebug);
            }

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
    
        /// <summary>
        /// Find the location of the first matched section, if possible
        /// </summary>
        /// <param name="pex">PortableExecutable representing the read-in file</param>
        /// <param name="sectionPrefix">Prefix of the sections to check for</param>
        /// <returns>Section data, null on error</returns>
        private byte[] FindData(PortableExecutable pex, string sectionPrefix)
        {
            // Get the two matching sections, if possible
            var firstSection = pex.GetFirstSection($"{sectionPrefix}0", exact: true);
            var secondSection = pex.GetFirstSection($"{sectionPrefix}1", exact: true);

            // If either section is null, we can't do anything
            if (firstSection == null || secondSection == null)
                return null;

            // This subtract is needed because the version is before the section
            return pex.ReadRawSection($"{sectionPrefix}0", first: true, offset: -128);
        }
    }
}