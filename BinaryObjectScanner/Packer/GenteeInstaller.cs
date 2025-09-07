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
