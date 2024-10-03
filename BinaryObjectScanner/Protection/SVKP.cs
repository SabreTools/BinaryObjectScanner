#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// "SVK - Protector"/"Slovak Protector"/SVKP (https://web.archive.org/web/20020604155614/http://www.anticracking.sk/) was a packer created by Pavol Cerven that focused on protecting and obfuscating executables.
    /// It offered features such as encryption, debugger detection, and protection against memory dumps (https://web.archive.org/web/20020805143050fw_/http://www.anticracking.sk/svkp.html).
    /// 
    /// Additional resources and documentation:
    /// SVKP 1.05 demo: https://web.archive.org/web/20020614222838/http://www.anticracking.sk/svkp_setup.rar
    /// SVKP 1.051 demo: https://web.archive.org/web/20020805001642/http://www.anticracking.sk/svkp_setup.rar
    /// SVKP 1.11 demo: https://web.archive.org/web/20030424094050/http://www.anticracking.sk/svkp_setup.rar
    /// SVKP 1.32 demo: https://web.archive.org/web/20030818210217/http://www.anticracking.sk/svkp_setup.rar
    /// CD Media World article for SVKP: https://www.cdmediaworld.com/hardware/cdrom/cd_protections_svkp.shtml
    /// Unofficial PEiD detections for SVKP: https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    /// DiE detections for SVKP: https://github.com/horsicq/Detect-It-Easy/blob/master/db/PE/SVK%20Protector.2.sg
    /// </summary>
    public class SVKProtector : IPathCheck, IPortableExecutableCheck
    {
        // TODO: Find 1.4+ samples.

        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // TODO: Investigate the "Debugger or tool for monitoring detected!!!.Application cannot be run with debugger or monitoring tool(s) loaded!.            Please unload it and restart the application" strings present in seemingly every version.

            // Get the entry point data, if it exists.
            if (pex.EntryPointData != null)
            {
                // Found in the SVKP 1.05 demo.
                if (pex.EntryPointData.StartsWith(new byte?[]
                {
                    0xEB, 0x03, 0xC7, 0x84, 0xE8, 0x60, 0xEB, 0x03,
                    0xC7, 0x84, 0xE8, 0xEB, 0x03, 0xC7, 0x84, 0x9A,
                    0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81, 0xED,
                    0x15, 0x00, 0x00, 0x00, 0xEB, 0x03, 0xC7, 0x84,
                    0xE9, 0x64, 0xA0
                }))
                    return "SVKP v1.05";

                // Found in the SVKP 1.051 demo.
                if (pex.EntryPointData.StartsWith(new byte?[]
                {
                    0x60, 0xEB, 0x03, 0xC7, 0x84, 0xE8, 0xEB, 0x03,
                    0xC7, 0x84, 0x9A, 0xE8, 0x00, 0x00, 0x00, 0x00,
                    0x5D, 0x81, 0xED, 0x10, 0x00, 0x00, 0x00, 0xEB,
                    0x03, 0xC7, 0x84, 0xE9, 0x64, 0xA0, 0x23, 0x00,
                    0x00, 0x00, 0xEB
                }))
                    return "SVKP v1.051";

                // Found in the SVKP 1.11 demo.
                if (pex.EntryPointData.StartsWith(new byte?[]
                {
                    0x60, 0xE8, null, null, null, null, 0x5D, 0x81,
                    0xED, 0x06, null, null, null, 0x64, 0xA0, 0x23
                }))
                    return "SVKP v1.11";

                // Found in the SVKP 1.32 demo and Redump entry 84122.
                if (pex.EntryPointData.StartsWith(new byte?[]
                {
                    0x60, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x81,
                    0xED, 0x06, 0x00, 0x00, 0x00, 0xEB, 0x05, 0xB8,
                    null, null, null, null, 0x64, 0xA0, 0x23
                }))
                    return "SVKP v1.3+";
            }

            // 0x504B5653 is "SVKP"
            if (pex.Model.COFFFileHeader?.PointerToSymbolTable == 0x504B5653)
                return "SVKP";

            // Get the .svkp section, if it exists.
            // This section is present in at least versions 1.05-1.32, but isn't present in every known sample of these versions. Removing this section name may be a perk of the licensed version.
            bool neolitSection = pex.ContainsSection(".svkp", exact: true);
            if (neolitSection)
                return "SVKP";

            return null;
        }

        /// <inheritdoc/>
#if NET20 || NET35
        public Queue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in the SVKP 1.05-1.32 demos.
                new(new FilePathMatch("svkp.exe"), "SVKP"),
                new(new FilePathMatch("svkp.key"), "SVKP"),

                // Found in the SVKP 1.32 demo.
                new(new FilePathMatch("svkpnd.dll"), "SVKP"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in the SVKP 1.05-1.32 demos.
                new(new FilePathMatch("svkp.exe"), "SVKP"),
                new(new FilePathMatch("svkp.key"), "SVKP"),

                // Found in the SVKP 1.32 demo.
                new(new FilePathMatch("svkpnd.dll"), "SVKP"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
