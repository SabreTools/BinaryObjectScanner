using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    // TODO: Verify that all versions are detected
    // Using the file "ds-1.0.6-patch.exe", it seems like the way that AdvancedInstaller
    // works is by packing all of the files sequentially in the overlay and referencing
    // them somehow. This has the unfortunate effect that, if any of these files are
    // executables, then every single file appears embedded in the first's overlay.
    // While this technically extracts the data, it does so improperly. It may require
    // using the size of image and headers for overlay-extracted data.
    public class AdvancedInstaller : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Get the .rdata section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("Software\\Caphyon\\Advanced Installer")))
                    return "Caphyon Advanced Installer";
            }

            return null;
        }
    }
}
