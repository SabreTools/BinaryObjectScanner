using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // Packer used by all known SmartE games, but also used by some other non-SmartE protected software as well.
    // https://web.archive.org/web/20020806102129/http://www.bit-arts.com/windows_solutions.html
    // TODO: Other BitArts products may also use this same string. No samples have yet been found.
    public class Crunch : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // TODO: Investigate if this can be found by aligning to section containing entry point

            // Get the last section strings, if they exist
            var sections = exe.SectionTable ?? [];
            var strs = exe.GetSectionStrings(sections.Length - 1);
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("BITARTS")))
                    return "Crunch";
            }

            return null;
        }
    }
}
