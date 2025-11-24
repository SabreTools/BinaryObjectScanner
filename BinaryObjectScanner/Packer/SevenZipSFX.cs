using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    public class SevenZipSFX : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.AssemblyDescription;

            if (name.OptionalStartsWith("7-Zip Self-extracting Archive"))
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
                return $"7-Zip SFX {exe.AssemblyDescription!["7-Zip Self-extracting Archive ".Length..]}";
#else
                return $"7-Zip SFX {exe.AssemblyDescription!.Substring("7-Zip Self-extracting Archive ".Length)}";
#endif

            name = exe.FileDescription;

            if (name.OptionalEquals("7z SFX"))
                return "7-Zip SFX";
            if (name.OptionalEquals("7z Self-Extract Setup"))
                return "7-Zip SFX";

            name = exe.OriginalFilename;

            if (name.OptionalEquals("7z.sfx.exe"))
                return "7-Zip SFX";
            else if (name.OptionalEquals("7zS.sfx"))
                return "7-Zip SFX";

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
