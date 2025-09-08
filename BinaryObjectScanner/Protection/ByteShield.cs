using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Paths;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
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
    public class ByteShield : IExecutableCheck<PortableExecutable>, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Found in "LineRider2.exe" in Redump entry 6236
            var name = exe.FileDescription;
            if (name.OptionalEquals("ByteShield Client"))
                return $"ByteShield Activation Client {exe.GetInternalVersion()}";

            // Found in "LineRider2.exe" in Redump entry 6236
            name = exe.InternalName;
            if (name.OptionalEquals("ByteShield"))
                return $"ByteShield Activation Client {exe.GetInternalVersion()}";

            // Found in "LineRider2.exe" in Redump entry 6236
            name = exe.OriginalFilename;
            if (name.OptionalEquals("ByteShield.EXE"))
                return $"ByteShield Activation Client {exe.GetInternalVersion()}";

            // Found in "LineRider2.exe" in Redump entry 6236
            name = exe.ProductName;
            if (name.OptionalEquals("ByteShield Client"))
                return $"ByteShield Activation Client {exe.GetInternalVersion()}";

            // Found in "ByteShield.dll" in Redump entry 6236
            name = exe.ExportTable?.ExportDirectoryTable?.Name;
            if (name.OptionalEquals("ByteShield Client"))
                return "ByteShield Component Module";

            // Found in "LineRider2.exe" in Redump entry 6236
            if (exe.FindStringTableByEntry("ByteShield").Count > 0)
                return $"ByteShield Activation Client {exe.GetInternalVersion()}";

            // Found in "LineRider2.exe" in Redump entry 6236
            if (exe.FindDialogByTitle("About ByteShield").Count > 0)
                return "ByteShield";

            // TODO: See if the version number is anywhere else
            // TODO: Parse the version number out of the dialog box item
            // Found in "LineRider2.exe" in Redump entry 6236
            if (exe.FindDialogBoxByItemTitle("ByteShield Version 1.0").Count > 0)
                return "ByteShield";

            // Get the .data/DATA section strings, if they exist
            var strs = exe.GetFirstSectionStrings(".data") ?? exe.GetFirstSectionStrings("DATA");
            if (strs != null)
            {
                // Found in "LineRider2.exe" in Redump entry 6236
                if (strs.Exists(s => s.OptionalContains("ByteShield")))
                    return "ByteShield";
            }

            // Get the .rdata section strings, if they exist
            strs = exe.GetFirstSectionStrings(".rdata");
            if (strs != null)
            {
                // Found in "ByteShield.dll" in Redump entry 6236
                if (strs.Exists(s => s.OptionalContains("Byte|Shield")))
                    return "ByteShield Component Module";

                // Found in "ByteShield.dll" in Redump entry 6236
                else if (strs.Exists(s => s.OptionalContains("Byteshield0")))
                    return "ByteShield Component Module";

                // Found in "ByteShield.dll" in Redump entry 6236
                else if (strs.Exists(s => s.OptionalContains("ByteShieldLoader")))
                    return "ByteShield Component Module";
            }

            // Get the .ret section strings, if they exist
            strs = exe.GetFirstSectionStrings(".ret");
            if (strs != null)
            {
                // TODO: Figure out if this specifically indicates if the file is encrypted
                // Found in "LineRider2.bbz" in Redump entry 6236
                if (strs.Exists(s => s.OptionalContains("ByteShield")))
                    return "ByteShield";
            }

            return null;
        }

        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, List<string>? files)
        {
            // TODO: Investigate reference to "bbz650.tmp" in "Byteshield.dll" (Redump entry 6236)
            // Files with the ".bbz" extension are associated with ByteShield, but the extenstion is known to be used in other places as well.
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("Byteshield.dll"), "ByteShield Component Module"),
                new(new FilePathMatch("Byteshield.ini"), "ByteShield"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            // TODO: Investigate reference to "bbz650.tmp" in "Byteshield.dll" (Redump entry 6236)
            // Files with the ".bbz" extension are associated with ByteShield, but the extenstion is known to be used in other places as well.
            var matchers = new List<PathMatchSet>
            {
                new(new FilePathMatch("Byteshield.dll"), "ByteShield Component Module"),
                new(new FilePathMatch("Byteshield.ini"), "ByteShield"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
