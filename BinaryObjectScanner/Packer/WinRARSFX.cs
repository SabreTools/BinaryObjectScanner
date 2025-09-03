using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    public class WinRARSFX : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            var name = pex.AssemblyDescription;
            if (name.OptionalContains("WinRAR archiver"))
                return "WinRAR SFX";

            if (pex.FindDialogByTitle("WinRAR self-extracting archive").Count > 0)
                return "WinRAR SFX";

            return null;
        }
    }
}
