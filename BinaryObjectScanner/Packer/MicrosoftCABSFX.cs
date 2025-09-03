using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class MicrosoftCABSFX : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            var name = pex.InternalName;
            if (name.OptionalEquals("Wextract", StringComparison.OrdinalIgnoreCase))
                return $"Microsoft CAB SFX {GetVersion(pex)}";

            name = pex.OriginalFilename;
            if (name.OptionalEquals("WEXTRACT.EXE", StringComparison.OrdinalIgnoreCase))
                return $"Microsoft CAB SFX {GetVersion(pex)}";

            // Get the .data/DATA section strings, if they exist
            var strs = pex.GetFirstSectionStrings(".data") ?? pex.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("wextract_cleanup")))
                    return $"Microsoft CAB SFX {GetVersion(pex)}";
            }

            // Get the .text section strings, if they exist
            strs = pex.GetFirstSectionStrings(".text");
            if (strs != null)
            {
                // This detects a different but similar type of SFX that uses Microsoft CAB files.
                // Further research is needed to see if it's just a different version or entirely separate.
                if (strs.Exists(s => s.Contains("MSCFu")))
                    return $"Microsoft CAB SFX {GetVersion(pex)}";
            }

            return null;
        }

        private static string GetVersion(PortableExecutable pex)
        {
            // Check the internal versions
            var version = pex.GetInternalVersion();
            if (!string.IsNullOrEmpty(version))
                return $"v{version}";

            return string.Empty;
        }
    }
}
