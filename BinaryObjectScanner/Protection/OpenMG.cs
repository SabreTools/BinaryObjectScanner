using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// OpenMG is a form of DRM created by Sony to control how music is copied and listened to on PC. 
    /// It is known to be used with multiple CD audio protections, such as XCP, LabelGate, and quite possibly Key2AudioXS.
    /// References:
    /// https://en.wikipedia.org/wiki/OpenMG
    /// </summary>
    public class OpenMG : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Found in many different OpenMG related files ("Touch" by Amerie).
            var name = pex.LegalTrademarks;
            if (name?.StartsWith("OpenMG", StringComparison.OrdinalIgnoreCase) == true)
                return $"OpenMG";

            // Found in "OMGDBP.OCX" ("Touch" by Amerie).
            name = pex.FileDescription;
            if (name?.StartsWith("LGDiscComp Module", StringComparison.OrdinalIgnoreCase) == true)
                return $"OpenMG";

            // Found in "OMGDWRAP.DLL" ("Touch" by Amerie).
            if (name?.StartsWith("LGDSimplePlayer Module", StringComparison.OrdinalIgnoreCase) == true)
                return $"OpenMG";

            // Found in "OMGLGD.DLL" ("Touch" by Amerie).
            if (name?.StartsWith("omglgd Module", StringComparison.OrdinalIgnoreCase) == true)
                return $"OpenMG";

            // Found in "OMGUTILS.DLL" ("Touch" by Amerie).
            if (name?.StartsWith("OpenMG Utility", StringComparison.OrdinalIgnoreCase) == true)
                return $"OpenMG";

            // Found in "SALWRAP.DLL" ("Touch" by Amerie).
            if (name?.StartsWith("Secure Application Loader Wrapper", StringComparison.OrdinalIgnoreCase) == true)
                return $"OpenMG";

            // Found in "SDKHM.DLL" ("Touch" by Amerie).
            // Not every copy of this file has this File Description (Redump entry 95010).
            if (name?.StartsWith("SDKHM (KEEP)", StringComparison.OrdinalIgnoreCase) == true)
                return $"OpenMG";

            // Found in "SDKHM.EXE" ("Touch" by Amerie).
            // Not every copy of this file has this File Description (Redump entry 95010).
            if (name?.StartsWith("SDKHM (KEPT)", StringComparison.OrdinalIgnoreCase) == true)
                return $"OpenMG";

            return null;
        }

        /// <inheritdoc/>
#if NET48
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
#else
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
#endif
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

            return MatchUtil.GetAllMatches(files ?? System.Array.Empty<string>(), matchers, any: false);
        }

        /// <inheritdoc/>
#if NET48
        public string CheckFilePath(string path)
#else
        public string? CheckFilePath(string path)
#endif
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
