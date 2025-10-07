using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    // <see href="https://github.com/mcmilk/7-Zip/tree/master/CPP/7zip/Archive/Nsis"/>
    public class NSIS : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Investigate the ".ndata" section
            // NSIS is state-machine based, similar to Wise scripts

            string? name = exe.AssemblyDescription;

            if (name.OptionalStartsWith("Nullsoft Install System"))
                return $"NSIS {name!.Substring("Nullsoft Install System".Length).Trim()}";

            name = exe.AssemblyName;

            if (name.OptionalStartsWith("Nullsoft.NSIS"))
                return "NSIS";

            // Get the .data/DATA section strings, if they exist
                var strs = exe.GetFirstSectionStrings(".data") ?? exe.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("NullsoftInst")))
                    return "NSIS";
            }

            // Get the overlay strings, if they exist
            strs = exe.OverlayStrings;
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("NullsoftInst")))
                    return "NSIS";
            }

            return null;
        }
    }
}
