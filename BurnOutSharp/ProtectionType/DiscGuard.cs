﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class DiscGuard : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new List<PathMatch>
                {
                    new PathMatch("IOSLINK.VXD", useEndsWith: true),
                    new PathMatch("IOSLINK.DLL", useEndsWith: true),
                    new PathMatch("IOSLINK.SYS", useEndsWith: true),
                }, "DiscGuard"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: false);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("IOSLINK.VXD", useEndsWith: true), "DiscGuard"),
                new PathMatchSet(new PathMatch("IOSLINK.DLL", useEndsWith: true), "DiscGuard"),
                new PathMatchSet(new PathMatch("IOSLINK.SYS", useEndsWith: true), "DiscGuard"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
