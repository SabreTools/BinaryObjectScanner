using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// CD-Guard is a DRM from Russia that's similar to CD-Cops and may be related to StarForce, meaning it likely needs DPM.
    /// It may have been developed by Russobit-M, though the same source also says that StarForce was created by the same company as well, which seems unlikely (https://m.linkdatasecurity.com/pnews6.htm).
    /// Others online have been confused by this as well (https://forum.ixbt.com/topic.cgi?id=31:009712).
    /// A game referred to as having CD-Guard by http://lastboss.ru/games/RUS/randevu-s-neznakomkoi-2 that was published by Russobit-M is known to have an early version of StarForce (Redump entry 97088).
    /// The FAQ on the game's official website indicates that StarForce specifically is present (https://web.archive.org/web/20011220224222/http://www.aha.ru/~exe_soft/russian/exesoft.htm). 
    /// It's unknown for sure if there were two separate versions of this game that contained separate protections, or if the game never actually contained CD-Guard, or if CD-Guard was an early name for the StarForce line of products.
    /// There is a re-release of an earlier game by the same developer that seems to include both CD-Guard and StarForce drivers, with the CD-Guard driver seemingly not used during installation, nor installed onto the system (IA item "pahgeby-he3hakomkou").
    /// 
    /// Additional resources and references:
    /// https://gamecopyworld.com/games/pc_omikron.shtml
    /// https://forum.ixbt.com/topic.cgi?id=31:3985
    /// </summary>
    public class CDGuard : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // TODO: Investigate the numerous ".guard" sections present in "Randevu.exe" in Redump entry 97142.

            // Get the export directory table
            if (pex.Model.ExportTable?.ExportDirectoryTable != null)
            {
                // Found in "cdguard.dll" in Redump entry 97142 and IA item "pahgeby-he3hakomkou".
                bool match = pex.Model.ExportTable.ExportDirectoryTable.Name?.Equals("cdguard.dll", StringComparison.OrdinalIgnoreCase) == true;
                if (match)
                    return "CD-Guard Copy Protection System";
            }

            // Get the import directory table
            if (pex.Model.ImportTable?.ImportDirectoryTable != null)
            {
                // Found in "Randevu.exe" in Redump entry 97142.
                bool match = pex.Model.ImportTable.ImportDirectoryTable.Any(idte => idte?.Name != null && idte.Name.Equals("cdguard.dll", StringComparison.OrdinalIgnoreCase));
                if (match)
                      return "CD-Guard Copy Protection System";
            }

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Redump entry 97142.
                new PathMatchSet(new PathMatch("cdguard.dll", useEndsWith: true), "CD-Guard Copy Protection System"),
            };

            return MatchUtil.GetAllMatches(files ?? System.Array.Empty<string>(), matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Found in Redump entry 97142.
                new PathMatchSet(new PathMatch("cdguard.dll", useEndsWith: true), "CD-Guard Copy Protection System"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
