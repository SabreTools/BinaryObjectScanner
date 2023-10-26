using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    public class dotFuscator : IExtractable, IPortableExecutableCheck
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

            // Get the .text section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".text");
            if (strs != null)
            {
                if (strs.Any(s => s.Contains("DotfuscatorAttribute")))
                    return "dotFuscator";
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
