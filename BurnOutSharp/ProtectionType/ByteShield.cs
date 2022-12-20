﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;
using BurnOutSharp.Wrappers;

namespace BurnOutSharp.ProtectionType
{
    /// <summary>
    /// ByteShield, Inc. (https://web.archive.org/web/20070216191623/http://www.byteshield.net/) was founded in 2004 (https://www.apollo.io/companies/ByteShield--Inc-/54a1357069702d4494ab9b00).
    /// There is a website seemingly belonging to them that's been archived as early as 2004, but there doesn't appear to be anything useful on it (https://web.archive.org/web/20040615001350/http://byteshield.com/).
    /// The ByteShield DRM itself is online activation based, using randomly generated activation codes it refers to as DACs (https://web.archive.org/web/20080921231346/http://www.byteshield.net/byteshield_whitepaper_0005.pdf).
    /// It appears that ByteShield advertised itself around online web forums (https://gamedev.net/forums/topic/508082-net-copy-protection-c/508082/ and https://cboard.cprogramming.com/tech-board/106642-how-add-copy-protection-software.html).
    /// Patent relating to ByteShield: https://patentimages.storage.googleapis.com/ed/76/c7/d98a56aeeca2e9/US7716474.pdf and https://patents.google.com/patent/US20100212028.
    /// 
    /// Games known to use it: 
    /// Line Rider 2: Unbound (https://fileforums.com/showthread.php?t=86909).
    /// Football Manager 2011 (https://community.sigames.com/forums/topic/189163-for-those-of-you-struggling-with-byteshield-activation-issues/).
    /// 
    /// Publishers known to use it:
    /// PAN Vision (http://web.archive.org/web/20120413160122/http://www.byteshield.net/ -> Company -> Milestones).
    /// JIAN (http://web.archive.org/web/20120413160122/http://www.byteshield.net/ -> Company -> Milestones).
    /// GamersGate (http://web.archive.org/web/20120413160122/http://www.byteshield.net/ -> Company -> Milestones).
    /// Akella (http://web.archive.org/web/20120413160122/http://www.byteshield.net/ -> Customers).
    /// All Interactive Distributuion (http://web.archive.org/web/20120413160122/http://www.byteshield.net/ -> Customers).
    /// Beowulf (http://web.archive.org/web/20120413160122/http://www.byteshield.net/ -> Customers).
    /// CroVortex (http://web.archive.org/web/20120413160122/http://www.byteshield.net/ -> Customers).
    /// N3V Games (http://web.archive.org/web/20120413160122/http://www.byteshield.net/ -> Customers).
    /// OnePlayS (http://web.archive.org/web/20120413160122/http://www.byteshield.net/ -> Customers).
    /// YAWMA (http://web.archive.org/web/20120413160122/http://www.byteshield.net/ -> Customers).
    /// 
    /// Further links and resources:
    /// https://www.cdmediaworld.com/hardware/cdrom/cd_protections_byteshield.shtml
    /// https://www.bcs.org/articles-opinion-and-research/is-there-anything-like-acceptable-drm/
    /// https://forums.auran.com/trainz/showthread.php?106673-Trainz-and-DRM/page22
    /// https://www.auran.com/planetauran/byteshield_drm.php
    /// https://www.ftc.gov/sites/default/files/documents/public_comments/ftc-town-hall-address-digital-rights-management-technologies-event-takes-place-wednesday-march-25/539814-00707.pdf
    /// https://www.gamesindustry.biz/byteshield-drm-system-now-protecting-over-200-games
    /// </summary>
    public class ByteShield : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // TODO: Add check for "ByteShield" in "LineRider2.exe" that includes version parsing.

            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            var strings = pex.GetFirstSectionStrings(".ret");
            if (strings != null && strings.Any())
            {
                // TODO: Figure out if this specifically indicates if the file is a protected exe.
                // Found in "LineRider2.bbz" in IA item (Redump entry 6236).
                if (strings.Any(s => s?.Contains("ByteShield") == true))
                    return "ByteShield";
            };

            strings = pex.GetFirstSectionStrings(".rdata");
            if (strings != null && strings.Any())
            {
                // Found in "ByteShield.dll" in IA item (Redump entry 6236).
                if (strings.Any(s => s?.Contains("Byteshield0") == true))
                    return "ByteShield";

                // Found in "ByteShield.dll" in IA item (Redump entry 6236).
                if (strings.Any(s => s?.Contains("Byte|Shield") == true))
                    return "ByteShield";

                // Found in "ByteShield.dll" in IA item (Redump entry 6236).
                if (strings.Any(s => s?.Contains("bbz650.tmp") == true))
                    return "ByteShield";
            };

            string name = pex.FileDescription;

            // Found in "LineRider2.exe" (Redump entry 6236).
            if (name?.StartsWith("ByteShield Client", StringComparison.OrdinalIgnoreCase) == true)
                return $"ByteShield Activation Client";

            name = pex.ProductName;

            // Found in "LineRider2.exe" (Redump entry 6236).
            if (name?.StartsWith("ByteShield Client", StringComparison.OrdinalIgnoreCase) == true)
                return $"ByteShield Activation Client";

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Investigate reference to "bbz650.tmp" in "Byteshield.dll" (Redump entry 6236).
            var matchers = new List<PathMatchSet>
            {
                // Files with the ".bbz" extension are associated with ByteShield, but the extenstion is known to be used in other places as well.
                new PathMatchSet(new PathMatch("Byteshield.dll", useEndsWith: true), "ByteShield"),
                new PathMatchSet(new PathMatch("Byteshield.ini", useEndsWith: true), "ByteShield"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Files with the ".bbz" extension are associated with ByteShield, but the extenstion is known to be used in other places as well.
                new PathMatchSet(new PathMatch("Byteshield.dll", useEndsWith: true), "ByteShield"),
                new PathMatchSet(new PathMatch("Byteshield.ini", useEndsWith: true), "ByteShield"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
