using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    // TODO: Add version checking, if possible
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class Armadillo : IExtractable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .nicode section, if it exists
            bool nicodeSection = pex.ContainsSection(".nicode", exact: true);
            if (nicodeSection)
                return "Armadillo";

            // Loop through all "extension" sections -- usually .data1 or .text1
            if (pex.SectionNames != null)
            {
                foreach (var sectionName in pex.SectionNames.Where(s => s != null && s.EndsWith("1")))
                {
                    // Get the section strings, if they exist
                    var strs = pex.GetFirstSectionStrings(sectionName);
                    if (strs != null)
                    {
                        if (strs.Any(s => s.Contains("ARMDEBUG")))
                            return "Armadillo";
                    }
                }
            }

            return null;
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(string file, bool includeDebug)
#else
        public string? Extract(string file, bool includeDebug)
#endif
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(Stream stream, string file, bool includeDebug)
#else
        public string? Extract(Stream? stream, string file, bool includeDebug)
#endif
        {
            return null;
        }
    }
}
