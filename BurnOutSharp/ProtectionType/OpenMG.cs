using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// OpenMG is a form of DRM created by Sony to control how music is copied and listened to on PC. 
    /// It is known to be used with multiple CD audio protections, such as XCP, LabelGate, and quite possibly Key2AudioXS.
    /// References:
    /// https://en.wikipedia.org/wiki/OpenMG
    /// </summary>
    public class OpenMG : IPathCheck, IPortableExecutableCheck
    {
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Found in many different OpenMG related files ("Touch" by Amerie).
            string name = pex.LegalTrademarks;
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("OpenMG", StringComparison.OrdinalIgnoreCase))
                return $"OpenMG";

            // Found in "OMGDBP.OCX" ("Touch" by Amerie).
            name = pex.FileDescription;
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("LGDiscComp Module", StringComparison.OrdinalIgnoreCase))
                return $"OpenMG";

            // Found in "OMGDWRAP.DLL" ("Touch" by Amerie).
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("LGDSimplePlayer Module", StringComparison.OrdinalIgnoreCase))
                return $"OpenMG";

            // Found in "OMGLGD.DLL" ("Touch" by Amerie).
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("omglgd Module", StringComparison.OrdinalIgnoreCase))
                return $"OpenMG";

            // Found in "OMGUTILS.DLL" ("Touch" by Amerie).
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("OpenMG Utility", StringComparison.OrdinalIgnoreCase))
                return $"OpenMG";

            // Found in "SALWRAP.DLL" ("Touch" by Amerie).
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("Secure Application Loader Wrapper", StringComparison.OrdinalIgnoreCase))
                return $"OpenMG";

            // Found in "SDKHM.DLL" ("Touch" by Amerie).
            // Not every copy of this file has this File Description (Redump entry 95010).
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("SDKHM (KEEP)", StringComparison.OrdinalIgnoreCase))
                return $"OpenMG";

            // Found in "SDKHM.EXE" ("Touch" by Amerie).
            // Not every copy of this file has this File Description (Redump entry 95010).
            if (!string.IsNullOrWhiteSpace(name) && name.StartsWith("SDKHM (KEPT)", StringComparison.OrdinalIgnoreCase))
                return $"OpenMG";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                // So far found in every known release that uses OpenMG ("Touch" by Amerie, Redump entry 95010, and product ID SVWC-7185).
                // Files with the extension ".OMA" in the directory "OMGAUDIO" are the encrypted audio files, and files with in the directory "OMGEXTRA" the extension ".000" are bonus content.
                // TODO: Investigate the consistency of "\OMGEXTRA\INDX0000.XML" and "\OMGEXTRA\INDX0001.XML", they seem to only appear when bonus content is present ("Touch" by Amerie).
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch(Path.Combine("OMGAUDIO", "00AUDTOC.DAT").Replace("\\", "/"), useEndsWith: true),
                    new PathMatch(Path.Combine("OMGAUDIO", "01AUDSTR.DAT").Replace("\\", "/"), useEndsWith: true),
                    new PathMatch(Path.Combine("OMGAUDIO", "05SRPCDS.DAT").Replace("\\", "/"), useEndsWith: true),
                    new PathMatch(Path.Combine("OMGEXTRA", "OMGSVC.DAT").Replace("\\", "/"), useEndsWith: true),
                }, "OpenMG"),

                // Always found together on OpenMG releases ("Touch" by Amerie, Redump entry 95010, and product ID SVWC-7185).
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch(Path.Combine("SDKHM.DLL").Replace("\\", "/"), useEndsWith: true),
                    new PathMatch(Path.Combine("SDKHM.EXE").Replace("\\", "/"), useEndsWith: true),
                }, "OpenMG"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // So far found in every known release that uses OpenMG ("Touch" by Amerie, Redump entry 95010, and product ID SVWC-7185).
                new PathMatchSet(new PathMatch("00AUDTOC.DAT", useEndsWith: true), "OpenMG"),
                new PathMatchSet(new PathMatch("01AUDSTR.DAT", useEndsWith: true), "OpenMG"),
                new PathMatchSet(new PathMatch("05SRPCDS.DAT", useEndsWith: true), "OpenMG"),
                new PathMatchSet(new PathMatch("OMGSVC.DAT", useEndsWith: true), "OpenMG"),

                // Always found together on OpenMG releases ("Touch" by Amerie, Redump entry 95010, and product ID SVWC-7185).
                new PathMatchSet(new PathMatch("SDKHM.DLL", useEndsWith: true), "OpenMG"),
                new PathMatchSet(new PathMatch("SDKHM.EXE", useEndsWith: true), "OpenMG"),

                // Found together on one specific release ("Touch" by Amerie).
                // TODO: Verify if these are OR or AND
                new PathMatchSet(new PathMatch("OMGDBP.OCX", useEndsWith: true), "OpenMG"),
                new PathMatchSet(new PathMatch("OMGDWRAP.DLL", useEndsWith: true), "OpenMG"),
                new PathMatchSet(new PathMatch("OMGLGD.DLL", useEndsWith: true), "OpenMG"),
                new PathMatchSet(new PathMatch("OMGUTILS.DLL", useEndsWith: true), "OpenMG"),
                new PathMatchSet(new PathMatch("SALWRAP.DLL", useEndsWith: true), "OpenMG"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
