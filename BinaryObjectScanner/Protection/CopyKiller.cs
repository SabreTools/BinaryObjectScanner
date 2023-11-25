﻿#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;

namespace BinaryObjectScanner.Protection
{
    public class CopyKiller : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string? CheckContents(string file, byte[] fileContent, bool includeDebug)
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            if (includeDebug)
            {
                var contentMatchSets = new List<ContentMatchSet>
                {
                    // Tom Commander
                    new(new byte?[]
                    {
                        0x54, 0x6F, 0x6D, 0x20, 0x43, 0x6F, 0x6D, 0x6D,
                        0x61, 0x6E, 0x64, 0x65, 0x72
                    }, "CopyKiller"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
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
            // TODO: The following checks are overly broad and should be refined
            // TODO: Look into .PFF files as an indicator. At least one disc has those oversized files
            var matchers = new List<PathMatchSet>
            {
                //new(new PathMatch("Autorun.dat", useEndsWith: true), "CopyKiller"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            // TODO: The following checks are overly broad and should be refined
            // TODO: Look into .PFF files as an indicator. At least one disc has those oversized files
            var matchers = new List<PathMatchSet>
            {
                //new(new PathMatch("Autorun.dat", useEndsWith: true), "CopyKiller"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
