using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    // TODO: Add version checking, if possible
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class Armadillo : IExtractable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .nicode section, if it exists
            bool nicodeSection = pex.ContainsSection(".nicode", exact: true);
            if (nicodeSection)
                return "Armadillo";

            // Loop through all "extension" sections -- usually .data1 or .text1
            foreach (var sectionName in pex.SectionNames.Where(s => s != null && s.EndsWith("1")))
            {
                // Get the section strings, if they exist
                List<string> strs = pex.GetFirstSectionStrings(sectionName);
                if (strs != null)
                {
                    if (strs.Any(s => s.Contains("ARMDEBUG")))
                        return "Armadillo";
                }
            }

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
