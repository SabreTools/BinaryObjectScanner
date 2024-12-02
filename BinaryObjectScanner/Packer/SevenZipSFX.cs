using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    public class SevenZipSFX : IExtractableExecutable<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the assembly description, if possible
            if (pex.AssemblyDescription.OptionalStartsWith("7-Zip Self-extracting Archive"))
                return $"7-Zip SFX {pex.AssemblyDescription!.Substring("7-Zip Self-extracting Archive ".Length)}";

            // Get the file description, if it exists
            if (pex.FileDescription.OptionalEquals("7z SFX"))
                return "7-Zip SFX";
            if (pex.FileDescription.OptionalEquals("7z Self-Extract Setup"))
                return "7-Zip SFX";

            // Get the original filename, if it exists
            if (pex.OriginalFilename.OptionalEquals("7z.sfx.exe"))
                return "7-Zip SFX";
            else if (pex.OriginalFilename.OptionalEquals("7zS.sfx"))
                return "7-Zip SFX";

            // Get the internal name, if it exists
            if (pex.InternalName.OptionalEquals("7z.sfx"))
                return "7-Zip SFX";
            else if (pex.InternalName.OptionalEquals("7zS.sfx"))
                return "7-Zip SFX";

            // If any dialog boxes match
            if (pex.FindDialogByTitle("7-Zip self-extracting archive").Count > 0)
                return "7-Zip SFX";

            return null;
        }

        /// <inheritdoc/>
        public bool Extract(string file, PortableExecutable pex, string outDir, bool includeDebug)
        {
            var sevenZip = new FileType.SevenZip();
            return sevenZip.Extract(file, outDir, lookForHeader: true, includeDebug);
        }
    }
}
