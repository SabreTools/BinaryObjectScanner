using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// GkWare Self-Extracting installer
    /// </summary>
    public class GkWareSFX : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.FileDescription;

            // <see href="https://www.virustotal.com/gui/file/867726b7afc1b2343651497bbbe35618f781bb82491a2a768922117c44a897d3/details"/>
            if (name.OptionalContains("GkWare Self extractor"))
                return "GkWare SFX";

            name = exe.ProductName;

            // <see href="https://www.virustotal.com/gui/file/867726b7afc1b2343651497bbbe35618f781bb82491a2a768922117c44a897d3/details"/>
            if (name.OptionalContains("GkWare Self extractor"))
                return "GkWare SFX";

            return null;
        }
    }
}
