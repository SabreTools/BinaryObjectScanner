using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Matching;

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
    public class ByteShield : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("Byteshield.dll", useEndsWith: true), "ByteShield (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch(".bbz", useEndsWith: true), "ByteShield (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("Byteshield.dll", useEndsWith: true), "ByteShield (Unconfirmed - Please report to us on Github)"),
                new PathMatchSet(new PathMatch(".bbz", useEndsWith: true), "ByteShield (Unconfirmed - Please report to us on Github)"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
