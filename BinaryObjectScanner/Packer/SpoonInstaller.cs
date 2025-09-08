using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// Spoon Installer
    /// </summary>
    public class SpoonInstaller : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // <see href="https://www.virustotal.com/gui/file/ad876d9aa59a2c51af776ce7c095af69f41f2947c6a46cfe87a724ecf8745084/details"/>
            var name = exe.AssemblyDescription;
            if (name.OptionalEquals("Spoon Installer"))
                return "Spoon Installer";

            // TODO: Add assembly identity name check as well: "Illustrate.Spoon.Installer"
            // Requires adding a helper to get the first Assembly Identity whose name is not null

            return null;
        }
    }
}
