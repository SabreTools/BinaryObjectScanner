using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // Packer used by all known SmartE games, but also used by some other non-SmartE protected software as well.
    // https://web.archive.org/web/20020806102129/http://www.bit-arts.com/windows_solutions.html
    // TODO: Other BitArts products may also use this same string. No samples have yet been found.
    public class Crunch : IExtractableExecutable<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the last section strings, if they exist
            var sections = pex.Model.SectionTable ?? [];
            var strs = pex.GetSectionStrings(sections.Length - 1);
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("BITARTS")))
                    return "Crunch";
            }

            return null;
        }

        /// <inheritdoc/>
        public bool Extract(string file, PortableExecutable pex, string outDir, bool includeDebug)
        {
            return false;
        }
    }
}
