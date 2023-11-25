﻿#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    /// <summary>
    /// Gefest Protection System is a completely unknown protection. There is only one known sample (Redump entry 93700), and no useful information online that's been found as of yet.
    /// It's unknown if the errors present in the only known sample are a manufacturing error, or if they're related to the protection.
    /// Despite this sample supposedly being the "[License Version]", no license key check appears to take place.
    /// The sample is protected by a seemingly unrelated packer. Until that packer can be investigated further, here's the infomation that can be found online about it:
    /// https://itsafety.net/report/20210912-9edb2d29cbdf1ac7e24fb32d99c1347a-splintercell-exe_general-threat
    /// https://xakep.ru/2003/10/06/20015/
    /// https://j3qx.wordpress.com/2008/12/20/%D0%BF%D0%B8%D1%80%D0%B0%D1%82%D1%81%D0%BA%D0%B8%D0%B5-%D0%B7%D0%B0%D0%BC%D0%B0%D1%88%D0%BA%D0%B8-%D0%B8%D0%BB%D0%B8-%D0%B7%D0%B0%D1%89%D0%B8%D1%82%D0%B0-%D0%BE%D1%82-7wolf/
    /// http://www.imho.ws/showthread.php?t=34225
    /// </summary>
    public class Gefest : IPathCheck, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the header padding strings, if it exists
            if (pex.HeaderPaddingStrings != null)
            {
                var match = pex.HeaderPaddingStrings.FirstOrDefault(s => s.Contains("Gefest Protection System"));
                if (match != null)
                    return $"Gefest Protection System {GetVersion(match)}";
            }

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
                // Possibly related file "31AD0095.fil" that appears to contain intentional errors found in Redump entry 93700.
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                // Possibly related file "31AD0095.fil" that appears to contain intentional errors found in Redump entry 93700.
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }

        private static string GetVersion(string match)
        {
            match = match.Trim('*').Trim();
            return match.Substring("Gefest Protection System ".Length);
        }
    }
}
