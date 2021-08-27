using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.Matching;
using BurnOutSharp.Tools;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction and verify that all versions are detected
    public class AdvancedInstaller : IContentCheck
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

            // Get the .rdata section, if it exists
            var rdataSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".rdata"));
            if (rdataSection != null)
            {
                int sectionAddr = (int)rdataSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)rdataSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // Software\Caphyon\Advanced Installer
                    new ContentMatchSet(new ContentMatch(
                        new byte?[]
                        {
                            0x53, 0x6F, 0x66, 0x74, 0x77, 0x61, 0x72, 0x65,
                            0x5C, 0x43, 0x61, 0x70, 0x68, 0x79, 0x6F, 0x6E,
                            0x5C, 0x41, 0x64, 0x76, 0x61, 0x6E, 0x63, 0x65,
                            0x64, 0x20, 0x49, 0x6E, 0x73, 0x74, 0x61, 0x6C,
                            0x6C, 0x65, 0x72
                        }, start: sectionAddr, end: sectionEnd),
                        "Caphyon Advanced Installer"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
            }

            return null;
        }
    }
}
