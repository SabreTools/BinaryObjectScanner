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
            string? name = exe.AssemblyDescription;
            if (name.OptionalStartsWith("7-Zip Self-extracting Archive"))
                return $"7-Zip SFX {exe.AssemblyDescription!.Substring("7-Zip Self-extracting Archive ".Length)}";

            // Get the file description, if it exists
            name = exe.FileDescription;
            if (name.OptionalEquals("7z SFX"))
                return "7-Zip SFX";
            if (name.OptionalEquals("7z Self-Extract Setup"))
                return "7-Zip SFX";

            // Get the original filename, if it exists
            name = exe.OriginalFilename;
            if (name.OptionalEquals("7z.sfx.exe"))
                return "7-Zip SFX";
            else if (name.OptionalEquals("7zS.sfx"))
                return "7-Zip SFX";

            // Get the internal name, if it exists
            name = exe.InternalName;
            if (name.OptionalEquals("7z.sfx"))
                return "7-Zip SFX";
            else if (name.OptionalEquals("7zS.sfx"))
                return "7-Zip SFX";

            // If any dialog boxes match
            if (exe.FindDialogByTitle("7-Zip self-extracting archive").Count > 0)
                return "7-Zip SFX";

            return null;
        }
    }
}
