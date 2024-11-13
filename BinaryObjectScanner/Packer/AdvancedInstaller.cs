using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    // TODO: Verify that all versions are detected
    public class AdvancedInstaller : IExtractableExecutable<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the .rdata section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("Software\\Caphyon\\Advanced Installer")))
                    return "Caphyon Advanced Installer";
            }

            return null;
        }

        /// <inheritdoc/>
        public bool Extract(string file, PortableExecutable pex, string outDir, bool includeDebug)
        {
            return false;
        }
    }
}
