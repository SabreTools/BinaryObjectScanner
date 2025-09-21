using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    public class WinRARSFX : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.AssemblyDescription;

            if (name.OptionalContains("WinRAR archiver"))
                return "WinRAR SFX";

            if (exe.FindDialogByTitle("WinRAR self-extracting archive").Count > 0)
                return "WinRAR SFX";

            return null;
        }
    }
}
