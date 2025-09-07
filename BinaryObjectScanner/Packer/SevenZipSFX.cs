using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    public class SevenZipSFX : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Get the assembly description, if possible
            if (exe.AssemblyDescription.OptionalStartsWith("7-Zip Self-extracting Archive"))
                return $"7-Zip SFX {exe.AssemblyDescription!.Substring("7-Zip Self-extracting Archive ".Length)}";

            // Get the file description, if it exists
            if (exe.FileDescription.OptionalEquals("7z SFX"))
                return "7-Zip SFX";
            if (exe.FileDescription.OptionalEquals("7z Self-Extract Setup"))
                return "7-Zip SFX";

            // Get the original filename, if it exists
            if (exe.OriginalFilename.OptionalEquals("7z.sfx.exe"))
                return "7-Zip SFX";
            else if (exe.OriginalFilename.OptionalEquals("7zS.sfx"))
                return "7-Zip SFX";

            // Get the internal name, if it exists
            if (exe.InternalName.OptionalEquals("7z.sfx"))
                return "7-Zip SFX";
            else if (exe.InternalName.OptionalEquals("7zS.sfx"))
                return "7-Zip SFX";

            // If any dialog boxes match
            if (exe.FindDialogByTitle("7-Zip self-extracting archive").Count > 0)
                return "7-Zip SFX";

            return null;
        }
    }
}
