﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class Key2AudioXS : IPathCheck
    {
        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("SDKHM.EXE", useEndsWith: true), "key2AudioXS"),
                new PathMatchSet(new PathMatch("SDKHM.DLL", useEndsWith: true), "key2AudioXS"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("SDKHM.EXE", useEndsWith: true), "key2AudioXS"),
                new PathMatchSet(new PathMatch("SDKHM.DLL", useEndsWith: true), "key2AudioXS"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
