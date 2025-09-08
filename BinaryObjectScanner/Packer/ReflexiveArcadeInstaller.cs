using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// Reflexive Arcade Installer
    /// </summary>
    public class ReflexiveArcadeInstaller : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Get the .data/DATA section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".data") ?? exe.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                // <see href="https://www.virustotal.com/gui/file/33b98b675d78b88ed317e7e52dca21ca07bd84e79211294fcec72cab48d11184"/>
                if (strs.Exists(s => s.Contains("ReflexiveArcade")))
                    return "Reflexive Arcade Installer";

                // <see href="https://www.virustotal.com/gui/file/33b98b675d78b88ed317e7e52dca21ca07bd84e79211294fcec72cab48d11184"/>
                if (strs.Exists(s => s.Contains("Reflexive Arcade")))
                    return "Reflexive Arcade Installer";
            }

            return null;
        }
    }
}
