using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Verify that all versions are detected
    public class AdvancedInstaller : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Get the .rdata section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".rdata");
            if (strs is not null)
            {
                if (strs.Exists(s => s.Contains("Software\\Caphyon\\Advanced Installer")))
                    return "Caphyon Advanced Installer SFX";
            }

            return null;
        }
    }
}
