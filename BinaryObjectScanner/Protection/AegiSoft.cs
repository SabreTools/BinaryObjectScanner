using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// AegiSoft License Manager was made AegiSoft, which was later bought by Real Networks, the makes of RealArcade (https://www.crunchbase.com/organization/aegisoft).
    /// It allowed publishers to give users a time-based free trial of software.
    /// Based on "Asc005.dll" and "Asc006.exe", AegiSoft License Manager may also have been referred to as "Software-On-Demand License Manager", or it may just be a distinct component of the larger product.
    /// The single sample investigated was only able to run on Windows 9x (Redump entry 73521/IA item "Nova_HoyleCasino99USA").
    /// Based on the packaging from IA item "Nova_HoyleCasino99USA", it seems that additional software from "www.1-800-software.com" is likely to be protected with AegiSoft License Manager or other DRM.
    /// References and further information:
    /// https://pitchbook.com/profiles/company/118805-59
    /// https://web.archive.org/web/19990417191351/http://www.aegisoft.com:80/
    /// </summary>
    public class AegiSoft : IPathCheck, IPortableExecutableCheck
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

            // There are possibly identifying Product Names for some files used in AegiSoft License Manager, but they were deemed too overmatching to use for the time being (Found in Redump entry 73521/IA item "Nova_HoyleCasino99USA")..
            // "Asc001.dll" has the Product Name "Install Dynamic Link Library".
            // "Asc002.dll" has the Product Name "Transact Dynamic Link Library".
            // "Asc003.dll" has the Product Name "Uninstall Dynamic Link Library".
            // "Asc005.dll" has the Product Name "OrderWizard Dynamic Link Library".
            // "Asc006.exe" has the Product Name "AGENT Application".

            // These are possibly identifying export name table strings
            // "Asc001.dll" has the strings "AscCheck" and "AscInstall"
            // "Asc002.dll" has the string "AscActivate"

            // Get string table resources
            var resource = pex.FindStringTableByEntry("AegiSoft License Manager");
            if (resource.Any())
                return "AegiSoft License Manager";

            // Get the .data/DATA section, if it exists
            var dataSectionRaw = pex.GetFirstSectionData(".data") ?? pex.GetFirstSectionData("DATA");
            if (dataSectionRaw != null)
            {
                var matchers = new List<ContentMatchSet>
                {
                    // Found in "Asc001.dll", "Asc002.dll", "Asc003.dll", "Asc005.dll", "Asc006.exe", and "AscLM.cpl" (Redump entry 73521/IA item "Nova_HoyleCasino99USA").
                    // ÿÿÿÿ\\.\ASCLM
                    new ContentMatchSet(new byte?[]
                    {
                        0xFF, 0xFF, 0xFF, 0xFF, 0x5C, 0x5C, 0x2E, 0x5C, 
                        0x41, 0x53, 0x43, 0x4C, 0x4D
                    }, "AegiSoft License Manager"),
                };

                var match = MatchUtil.GetFirstMatch(file, dataSectionRaw, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

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
                // Found in Redump entry 73521/IA item "Nova_HoyleCasino99USA".
                new PathMatchSet(new PathMatch("AscLM.cpl", useEndsWith: true), "AegiSoft License Manager"),
                new PathMatchSet(new PathMatch("AscLM.vxd", useEndsWith: true), "AegiSoft License Manager"),
                new PathMatchSet(new PathMatch("AscLMd.vxd", useEndsWith: true), "AegiSoft License Manager"),

                // There are a few other files present, but the file names on their own may be too overmatching. Due to the small sample size, it's not sure if these files are always present together.
                // These files are "Asc001.dll", "Asc002.dll", "Asc003.dll", "Asc005.dll", and "Asc006.exe" (Found in Redump entry 73521/IA item "Nova_HoyleCasino99USA").
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
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
                // Found in Redump entry 73521/IA item "Nova_HoyleCasino99USA".
                new PathMatchSet(new PathMatch("AscLM.cpl", useEndsWith: true), "AegiSoft License Manager"),
                new PathMatchSet(new PathMatch("AscLM.vxd", useEndsWith: true), "AegiSoft License Manager"),
                new PathMatchSet(new PathMatch("AscLMd.vxd", useEndsWith: true), "AegiSoft License Manager"),

                // There are a few other files present, but the file names on their own may be too overmatching. Due to the small sample size, it's not sure if these files are always present together.
                // These files are "Asc001.dll", "Asc002.dll", "Asc003.dll", "Asc005.dll", and "Asc006.exe" (Found in Redump entry 73521/IA item "Nova_HoyleCasino99USA").
            
                // The "DATA.TAG" file in the "AgeiSoft" folder is an INI that includes:
                // Company=AegiSoft Corporation
                // Application=AegiSoft License Manager
                // Version=2.1
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
