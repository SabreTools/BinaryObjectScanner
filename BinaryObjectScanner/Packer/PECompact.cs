using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Better version detection - https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    // TODO: Add extraction
    public class PECompact : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // 0x4F434550 is "PECO"
            if (exe.COFFFileHeader?.PointerToSymbolTable == 0x4F434550)
                return "PE Compact v1.x";

            // TODO: Get more granular version detection. PiD is somehow able to detect version ranges based
            // on the data in the file. This may be related to information in other fields

            // Investigate the ".pec" section, seemingly related to "pec1"

            // Get the pec1 section, if it exists
            if (exe.ContainsSection("pec1", exact: true))
                return "PE Compact v1.x";

            // TODO: The last 4 bytes of the PEC2 section name appear to be the version
            // For v2.20 - v3.02 this value is 0
            // For internal version v20240 this is [10 4F 00 00] (20240)

            // Get the PEC2 section, if it exists
            var pec2Section = exe.GetFirstSection("PEC2", exact: false);
            if (pec2Section?.PointerToRelocations == 0x32434550)
            {
                if (pec2Section.PointerToLinenumbers != 0)
                    return $"PE Compact v{pec2Section.PointerToLinenumbers} (internal version)";

                return "PE Compact v2.x (or newer)";
            }

            return null;
        }
    }
}
