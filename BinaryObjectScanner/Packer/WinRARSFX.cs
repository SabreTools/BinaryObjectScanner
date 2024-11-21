using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    public class WinRARSFX : IExtractableExecutable<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.AssemblyDescription;
            if (name?.Contains("WinRAR archiver") == true)
                return "WinRAR SFX";

            if (pex.FindDialogByTitle("WinRAR self-extracting archive").Count > 0)
                return "WinRAR SFX";

            return null;
        }

        /// <inheritdoc/>
        public bool Extract(string file, PortableExecutable pex, string outDir, bool includeDebug)
        {
            var rar = new FileType.RAR();
            return rar.Extract(file, outDir, lookForHeader: true, includeDebug);
        }
    }
}
