using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    public class NSIS : IContentCheck
    {
        /// <inheritdoc/>
        public List<ContentMatchSet> GetContentMatchSets()
        {
            return new List<ContentMatchSet>
            {
                // Nullsoft Install System
                new ContentMatchSet(new byte?[]
                {
                    0x4e, 0x75, 0x6c, 0x6c, 0x73, 0x6f, 0x66, 0x74,
                    0x20, 0x49, 0x6e, 0x73, 0x74, 0x61, 0x6c, 0x6c,
                    0x20, 0x53, 0x79, 0x73, 0x74, 0x65, 0x6d
                }, GetVersion, "NSIS"),

                // NullsoftInst
                new ContentMatchSet(new byte?[]
                {
                    0x4e, 0x75, 0x6c, 0x6c, 0x73, 0x6f, 0x66, 0x74,
                    0x49, 0x6e, 0x73, 0x74
                }, "NSIS"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            // PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            // var sections = pex?.SectionTable;
            // if (sections == null)
            //     return null;
            
            // TODO: Implement resource finding instead of using the built in methods
            // Assembly information lives in the .rsrc section
            // I need to find out how to navigate the resources in general
            // as well as figure out the specific resources for both
            // file info and MUI (XML) info. Once I figure this out,
            // that also opens the doors to easier assembly XML checks.

            return null;
        }

        public static string GetVersion(string file, byte[] fileContent, List<int> positions)
        {
            try
            {
                int index = positions[0];
                index += 24;
                if (fileContent[index] != 'v')
                    return "(Unknown Version)";

                var versionBytes = new ReadOnlySpan<byte>(fileContent, index, 16).ToArray();
                var onlyVersion = versionBytes.TakeWhile(b => b != '<').ToArray();
                return Encoding.ASCII.GetString(onlyVersion);
            }
            catch
            {
                return "(Unknown Version)";
            }
        }
    }
}