using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    public class WinRARSFX : IExtractablePortableExecutable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.AssemblyDescription;
            if (name?.Contains("WinRAR archiver") == true)
                return "WinRAR SFX";

            var resources = pex.FindDialogByTitle("WinRAR self-extracting archive");
            if (resources.Any())
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
