using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.ExecutableType.Microsoft.Headers;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class UPX : IContentCheck
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

            // Standard UPX
            int foundPosition = FindData(fileContent, sections, "UPX");
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
            foundPosition = FindData(fileContent, sections, "NOS");
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
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <param name="sections">Array of sections to check against</param>
        /// <param name="sectionPrefix">Prefix of the sections to check for</param>
        /// <returns>Real address of the section data, -1 on error</returns>
        private int FindData(byte[] fileContent, SectionHeader[] sections, string sectionPrefix)
        {
            // Get the two matching sections, if possible
            var firstSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith($"{sectionPrefix}0"));
            var secondSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith($"{sectionPrefix}1"));

            // If either section is null, we can't do anything
            if (firstSection == null || secondSection == null)
                return -1;

            // Return the first section address
            return (int)firstSection.PointerToRawData;
        }
    }
}