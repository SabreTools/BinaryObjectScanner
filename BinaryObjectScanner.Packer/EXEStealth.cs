using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Figure out how to more granularly determine versions like PiD
    // TODO: Detect 3.15 and up (maybe looking for `Metamorphism`)
    // TODO: Add extraction
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class EXEStealth : IContentCheck, IExtractable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug)
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            if (includeDebug)
            {
                var contentMatchSets = new List<ContentMatchSet>
                 {
                     // ??[[__[[_ + (char)0x00 + {{ + (char)0x0 + (char)0x00 + {{ + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x0 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + ?;??;??
                     new ContentMatchSet(new byte?[]
                     {
                         0x3F, 0x3F, 0x5B, 0x5B, 0x5F, 0x5F, 0x5B, 0x5B,
                         0x5F, 0x00, 0x7B, 0x7B, 0x00, 0x00, 0x7B, 0x7B,
                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                         0x00, 0x20, 0x3F, 0x3B, 0x3F, 0x3F, 0x3B, 0x3F,
                         0x3F
                     }, "EXE Stealth"),
                 };

                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.Model.SectionTable;
            if (sections == null)
                return null;

            // The ExeS/EXES/*mtw sections seem to map to the Import Table
            // 2.6/2.51
            //  `ExeStealth - www.webtoolmaster.com`
            // 2.72/2.73
            //  `Shareware - ExeStealth`
            //  `www.webtoolmaster.com`
            // 2.74
            //  `Shareware - ExeStealth`
            // 2.76
            //  `ExeStealth V2 Shareware not for public - This text not in registered version - www.webtoolmaster.com`

            // Get the ExeS/EXES section, if it exists
            bool exesSection = pex.ContainsSection("ExeS", exact: true) || pex.ContainsSection("EXES", exact: true);
            if (exesSection)
                return "EXE Stealth 2.41-2.75";

            // Get the mtw section, if it exists
            bool mtwSection = pex.ContainsSection("mtw", exact: true);
            if (mtwSection)
                return "EXE Stealth 1.1";

            // Get the rsrr section, if it exists
            bool rsrrSection = pex.ContainsSection("rsrr", exact: true);
            if (rsrrSection)
                return "EXE Stealth 2.76";

            return null;
        }

        /// <inheritdoc/>
        public string Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        public string Extract(Stream stream, string file, bool includeDebug)
        {
            return null;
        }
    }
}
