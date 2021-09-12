using System.Collections.Generic;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class UPX : IContentCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug, PortableExecutable pex, NewExecutable nex)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Standard UPX
            int foundPosition = FindData(pex, "UPX");
            if (foundPosition > -1)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // UPX!
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x55, 0x50, 0x58, 0x21 }, end: foundPosition),
                        GetVersion,
                        "UPX"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
            }

            // NOS Variant
            foundPosition = FindData(pex, "NOS");
            if (foundPosition > -1)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // NOS 
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x4E, 0x4F, 0x53, 0x20 }, end: foundPosition),
                        GetVersion,
                        "UPX (NOS Variant)"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
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
        /// <returns>Real address of the section data, -1 on error</returns>
        private int FindData(PortableExecutable pex, string sectionPrefix)
        {
            // Get the two matching sections, if possible
            var firstSection = pex.GetFirstSection($"{sectionPrefix}0", exact: true);
            var secondSection = pex.GetFirstSection($"{sectionPrefix}1", exact: true);

            // If either section is null, we can't do anything
            if (firstSection == null || secondSection == null)
                return -1;

            // Return the first section address
            return (int)firstSection.PointerToRawData;
        }
    }
}