using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class GenteeInstaller : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.AssemblyName;

            // <see href="https://www.virustotal.com/gui/file/40e222d35fe8bdd94360462e2f2b870ec7e2c184873e2a481109408db790bfe8/details"/>
            // This was found in a "Create Install 2003"-made installer
            if (name.OptionalEquals("Gentee.Installer.Install"))
                return "Gentee Installer";

            // Get the .data/DATA section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".data") ?? exe.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("Gentee installer")))
                    return "Gentee Installer";

                if (strs.Exists(s => s.Contains("ginstall.dll")))
                    return "Gentee Installer";
            }

            return null;
        }
    }
}
