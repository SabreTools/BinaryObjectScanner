using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class HexalockAutoLock : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("Start_Here.exe", useEndsWith: true), "Hexalock AutoLock"),
                new PathMatchSet(new PathMatch("HCPSMng.exe", useEndsWith: true), "Hexalock AutoLock"),
                new PathMatchSet(new PathMatch("MFINT.DLL", useEndsWith: true), "Hexalock AutoLock"),
                new PathMatchSet(new PathMatch("MFIMP.DLL", useEndsWith: true), "Hexalock AutoLock"),
            };

            return MatchUtil.GetAllMatches(files, matchers, any: true);
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            var matchers = new List<PathMatchSet>
            {
                new PathMatchSet(new PathMatch("Start_Here.exe", useEndsWith: true), "Hexalock AutoLock"),
                new PathMatchSet(new PathMatch("HCPSMng.exe", useEndsWith: true), "Hexalock AutoLock"),
                new PathMatchSet(new PathMatch("MFINT.DLL", useEndsWith: true), "Hexalock AutoLock"),
                new PathMatchSet(new PathMatch("MFIMP.DLL", useEndsWith: true), "Hexalock AutoLock"),
            };

            return MatchUtil.GetFirstMatch(path, matchers, any: true);
        }
    }
}
