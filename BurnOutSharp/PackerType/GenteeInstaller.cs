using System.Collections.Generic;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Matching;

namespace BurnOutSharp.PackerType
{
    // TODO: Add extraction
    public class GenteeInstaller : IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .data section, if it exists
            if (pex.DataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // Gentee installer
                    new ContentMatchSet(new byte?[]
                    {
                        0x47, 0x65, 0x6E, 0x74, 0x65, 0x65, 0x20, 0x69,
                        0x6E, 0x73, 0x74, 0x61, 0x6C, 0x6C, 0x65, 0x72,
                    }, "Gentee Installer"),

                    // ginstall.dll
                    new ContentMatchSet(new byte?[]
                    {
                        0x67, 0x69, 0x6E, 0x73, 0x74, 0x61, 0x6C, 0x6C,
                        0x2E, 0x64, 0x6C, 0x6C,
                    }, "Gentee Installer"),
                };

                return MatchUtil.GetFirstMatch(file, pex.DataSectionRaw, matchers, includeDebug);
            }

            return null;
        }
    }
}
