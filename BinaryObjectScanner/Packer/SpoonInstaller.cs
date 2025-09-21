using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
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
            string? name = exe.AssemblyDescription;

            // <see href="https://www.virustotal.com/gui/file/ad876d9aa59a2c51af776ce7c095af69f41f2947c6a46cfe87a724ecf8745084/details"/>
            if (name.OptionalEquals("Spoon Installer"))
                return "Spoon Installer";

            name = exe.AssemblyName;

            // <see href="https://www.virustotal.com/gui/file/40e222d35fe8bdd94360462e2f2b870ec7e2c184873e2a481109408db790bfe8/details"/>
            // This was found in a "Create Install 2003"-made installer
            if (name.OptionalEquals("Illustrate.Spoon.Installer"))
                return "Spoon Installer";

            return null;
        }
    }
}
