using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO;
using SabreTools.IO.Extensions;
using SabreTools.IO.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// OpenMG is a form of DRM created by Sony to control how music is copied and listened to on PC. 
    /// It is known to be used with multiple CD audio protections, such as XCP, LabelGate, and quite possibly Key2AudioXS.
    /// References:
    /// https://en.wikipedia.org/wiki/OpenMG
    /// </summary>
    public class OpenMG : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            string? name = exe.LegalTrademarks;

            // Found in many different OpenMG related files ("Touch" by Amerie).
            if (name.OptionalStartsWith("OpenMG", StringComparison.OrdinalIgnoreCase))
                return "OpenMG";

            name = exe.FileDescription;

            // Found in "OMGDBP.OCX" ("Touch" by Amerie).
            if (name.OptionalStartsWith("LGDiscComp Module", StringComparison.OrdinalIgnoreCase))
                return "OpenMG";

            // Found in "OMGDWRAP.DLL" ("Touch" by Amerie).
            if (name.OptionalStartsWith("LGDSimplePlayer Module", StringComparison.OrdinalIgnoreCase))
                return "OpenMG";

            // Found in "OMGLGD.DLL" ("Touch" by Amerie).
            if (name.OptionalStartsWith("omglgd Module", StringComparison.OrdinalIgnoreCase))
                return "OpenMG";

            // Found in "OMGUTILS.DLL" ("Touch" by Amerie).
            if (name.OptionalStartsWith("OpenMG Utility", StringComparison.OrdinalIgnoreCase))
                return "OpenMG";

            // Found in "SALWRAP.DLL" ("Touch" by Amerie).
            if (name.OptionalStartsWith("Secure Application Loader Wrapper", StringComparison.OrdinalIgnoreCase))
                return "OpenMG";

            // Found in "SDKHM.DLL" ("Touch" by Amerie).
            // Not every copy of this file has this File Description (Redump entry 95010).
            if (name.OptionalStartsWith("SDKHM (KEEP)", StringComparison.OrdinalIgnoreCase))
                return "OpenMG";

            // Found in "SDKHM.EXE" ("Touch" by Amerie).
            // Not every copy of this file has this File Description (Redump entry 95010).
            if (name.OptionalStartsWith("SDKHM (KEPT)", StringComparison.OrdinalIgnoreCase))
                return "OpenMG";

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // So far found in every known release that uses OpenMG ("Touch" by Amerie, Redump entry 95010, and product ID SVWC-7185).
                // Files with the extension ".OMA" in the directory "OMGAUDIO" are the encrypted audio files, and files with in the directory "OMGEXTRA" the extension ".000" are bonus content.
                // TODO: Investigate the consistency of "\OMGEXTRA\INDX0000.XML" and "\OMGEXTRA\INDX0001.XML", they seem to only appear when bonus content is present ("Touch" by Amerie).
                new(
                [
                    new FilePathMatch(Path.Combine("OMGAUDIO", "00AUDTOC.DAT")),
                    new FilePathMatch(Path.Combine("OMGAUDIO", "01AUDSTR.DAT")),
                    new FilePathMatch(Path.Combine("OMGAUDIO", "05SRPCDS.DAT")),
                    new FilePathMatch(Path.Combine("OMGEXTRA", "OMGSVC.DAT")),
                ], "OpenMG"),

                // Always found together on OpenMG releases ("Touch" by Amerie, Redump entry 95010, and product ID SVWC-7185).
                new(
                [
                    new FilePathMatch("SDKHM.DLL"),
                    new FilePathMatch("SDKHM.EXE"),
                ], "OpenMG"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // So far found in every known release that uses OpenMG ("Touch" by Amerie, Redump entry 95010, and product ID SVWC-7185).
                new(new FilePathMatch("00AUDTOC.DAT"), "OpenMG"),
                new(new FilePathMatch("01AUDSTR.DAT"), "OpenMG"),
                new(new FilePathMatch("05SRPCDS.DAT"), "OpenMG"),
                new(new FilePathMatch("OMGSVC.DAT"), "OpenMG"),

                // Always found together on OpenMG releases ("Touch" by Amerie, Redump entry 95010, and product ID SVWC-7185).
                new(new FilePathMatch("SDKHM.DLL"), "OpenMG"),
                new(new FilePathMatch("SDKHM.EXE"), "OpenMG"),

                // Found together on one specific release ("Touch" by Amerie).
                // TODO: Verify if these are OR or AND
                new(new FilePathMatch("OMGDBP.OCX"), "OpenMG"),
                new(new FilePathMatch("OMGDWRAP.DLL"), "OpenMG"),
                new(new FilePathMatch("OMGLGD.DLL"), "OpenMG"),
                new(new FilePathMatch("OMGUTILS.DLL"), "OpenMG"),
                new(new FilePathMatch("SALWRAP.DLL"), "OpenMG"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
