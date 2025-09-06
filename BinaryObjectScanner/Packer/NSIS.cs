using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    public class NSIS : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            var name = pex.AssemblyDescription;
            if (name.OptionalStartsWith("Nullsoft Install System"))
                return $"NSIS {name!.Substring("Nullsoft Install System".Length).Trim()}";

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("NullsoftInst")))
                    return "NSIS";
            }

            return null;
        }
    }
}