using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction - https://github.com/Bioruebe/UniExtract2
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class InstallerVISE : IExtractableExecutable<PortableExecutable>
    {
        //TODO: Add exact version detection for Windows builds, make sure versions before 3.X are detected as well, and detect the Mac builds.
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("ViseMain")))
                    return "Installer VISE";
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
