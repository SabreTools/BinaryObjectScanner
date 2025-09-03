using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    public class DotFuscator : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the .text section strings, if they exist
            var strs = FileType.Executable.GetFirstSectionStrings(pex, ".text");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("DotfuscatorAttribute")))
                    return "dotFuscator";
            }

            return null;
        }
    }
}
