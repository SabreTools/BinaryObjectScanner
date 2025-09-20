using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class MicrosoftCABSFX : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.InternalName;

            if (name.OptionalEquals("Wextract", StringComparison.OrdinalIgnoreCase))
                return $"Microsoft CAB SFX {GetVersion(exe)}";

            name = exe.OriginalFilename;
            
            if (name.OptionalEquals("WEXTRACT.EXE", StringComparison.OrdinalIgnoreCase))
                return $"Microsoft CAB SFX {GetVersion(exe)}";

            // Get the .data/DATA section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".data") ?? exe.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("wextract_cleanup")))
                    return $"Microsoft CAB SFX {GetVersion(exe)}";
            }

            // Get the .text section strings, if they exist
            strs = exe.GetFirstSectionStrings(".text");
            if (strs != null)
            {
                // This detects a different but similar type of SFX that uses Microsoft CAB files.
                // Further research is needed to see if it's just a different version or entirely separate.
                if (strs.Exists(s => s.Contains("MSCFu")))
                    return $"Microsoft CAB SFX {GetVersion(exe)}";
            }

            return null;
        }

        private static string GetVersion(PortableExecutable exe)
        {
            // Check the internal versions
            var version = exe.GetInternalVersion();
            if (!string.IsNullOrEmpty(version))
                return $"v{version}";

            return string.Empty;
        }
    }
}
