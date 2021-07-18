using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BurnOutSharp.Matching
{
    /// <summary>
    /// Helper class for matching
    /// </summary>
    internal static class MatchUtil
    {
        #region Content Matching

        /// <summary>
        /// Get all content matches for a given list of matchers
        /// </summary>
        /// <param name="file">File to check for matches</param>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <param name="matchers">Enumerable of ContentMatchSets to be run on the file</param>
        /// <param name="includePosition">True to include positional data, false otherwise</param>
        /// <returns>List of strings representing the matched protections, null or empty otherwise</returns>
        public static ConcurrentQueue<string> GetAllMatches(
            string file,
            byte[] fileContent,
            IEnumerable<ContentMatchSet> matchers,
            bool includePosition = false)
        {
            return FindAllMatches(file, fileContent, matchers, includePosition, false);
        }

        /// <summary>
        /// Get first content match for a given list of matchers
        /// </summary>
        /// <param name="file">File to check for matches</param>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <param name="matchers">Enumerable of ContentMatchSets to be run on the file</param>
        /// <param name="includePosition">True to include positional data, false otherwise</param>
        /// <returns>String representing the matched protection, null otherwise</returns>
        public static string GetFirstMatch(
            string file,
            byte[] fileContent,
            IEnumerable<ContentMatchSet> matchers,
            bool includePosition = false)
        {
            var contentMatches = FindAllMatches(file, fileContent, matchers, includePosition, true);
            if (contentMatches == null || !contentMatches.Any())
                return null;
            
            return contentMatches.First();
        }

        /// <summary>
        /// Get the required set of content matches on a per Matcher basis
        /// </summary>
        /// <param name="file">File to check for matches</param>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <param name="matchers">Enumerable of ContentMatchSets to be run on the file</param>
        /// <param name="includePosition">True to include positional data, false otherwise</param>
        /// <param name="stopAfterFirst">True to stop after the first match, false otherwise</param>
        /// <returns>List of strings representing the matched protections, null or empty otherwise</returns>        
        private static ConcurrentQueue<string> FindAllMatches(
            string file,
            byte[] fileContent,
            IEnumerable<ContentMatchSet> matchers,
            bool includePosition,
            bool stopAfterFirst)
        {
            // If there's no mappings, we can't match
            if (matchers == null || !matchers.Any())
                return null;

            // Initialize the queue of matched protections
            var matchedProtections = new ConcurrentQueue<string>();

            // Loop through and try everything otherwise
            foreach (var matcher in matchers)
            {
                // Determine if the matcher passes
                (bool passes, List<int> positions) = matcher.MatchesAll(fileContent);
                if (!passes)
                    continue;

                // Format the list of all positions found
                string positionsString = string.Join(", ", positions);

                // If we there is no version method, just return the protection name
                if (matcher.GetVersion == null)
                {
                    matchedProtections.Enqueue((matcher.ProtectionName ?? "Unknown Protection") + (includePosition ? $" (Index {positionsString})" : string.Empty));
                }

                // Otherwise, invoke the version method
                else
                {
                    // A null version returned means the check didn't pass at the version step
                    string version = matcher.GetVersion(file, fileContent, positions);
                    if (version == null)
                        continue;

                    matchedProtections.Enqueue($"{matcher.ProtectionName ?? "Unknown Protection"} {version}".TrimEnd() + (includePosition ? $" (Index {positionsString})" : string.Empty));
                }

                // If we're stopping after the first protection, bail out here
                if (stopAfterFirst)
                    return matchedProtections;
            }

            return matchedProtections;
        }

        #endregion

        #region Path Matching

        /// <summary>
        /// Get all path matches for a given list of matchers
        /// </summary>
        /// <param name="file">File path to check for matches</param>
        /// <param name="matchers">Enumerable of PathMatchSets to be run on the file</param>
        /// <param name="any">True if any path match is a success, false if all have to match</param>
        /// <returns>List of strings representing the matched protections, null or empty otherwise</returns>
        public static ConcurrentQueue<string> GetAllMatches(string file, IEnumerable<PathMatchSet> matchers, bool any = false)
        {
            return FindAllMatches(new List<string> { file }, matchers, any, false);
        }

        // <summary>
        /// Get all path matches for a given list of matchers
        /// </summary>
        /// <param name="files">File paths to check for matches</param>
        /// <param name="matchers">Enumerable of PathMatchSets to be run on the file</param>
        /// <param name="any">True if any path match is a success, false if all have to match</param>
        /// <returns>List of strings representing the matched protections, null or empty otherwise</returns>
        public static ConcurrentQueue<string> GetAllMatches(IEnumerable<string> files, IEnumerable<PathMatchSet> matchers, bool any = false)
        {
            return FindAllMatches(files, matchers, any, false);
        }

        /// <summary>
        /// Get first path match for a given list of matchers
        /// </summary>
        /// <param name="file">File path to check for matches</param>
        /// <param name="matchers">Enumerable of PathMatchSets to be run on the file</param>
        /// <param name="any">True if any path match is a success, false if all have to match</param>
        /// <returns>String representing the matched protection, null otherwise</returns>
        public static string GetFirstMatch(string file, IEnumerable<PathMatchSet> matchers, bool any = false)
        {
            var contentMatches = FindAllMatches(new List<string> { file }, matchers, any, true);
            if (contentMatches == null || !contentMatches.Any())
                return null;
            
            return contentMatches.First();
        }

        /// <summary>
        /// Get first path match for a given list of matchers
        /// </summary>
        /// <param name="files">File paths to check for matches</param>
        /// <param name="matchers">Enumerable of PathMatchSets to be run on the file</param>
        /// <param name="any">True if any path match is a success, false if all have to match</param>
        /// <returns>String representing the matched protection, null otherwise</returns>
        public static string GetFirstMatch(IEnumerable<string> files, IEnumerable<PathMatchSet> matchers, bool any = false)
        {
            var contentMatches = FindAllMatches(files, matchers, any, true);
            if (contentMatches == null || !contentMatches.Any())
                return null;
            
            return contentMatches.First();
        }

        /// <summary>
        /// Get the required set of path matches on a per Matcher basis
        /// </summary>
        /// <param name="files">File paths to check for matches</param>
        /// <param name="matchers">Enumerable of PathMatchSets to be run on the file</param>
        /// <param name="any">True if any path match is a success, false if all have to match</param>
        /// <param name="stopAfterFirst">True to stop after the first match, false otherwise</param>
        /// <returns>List of strings representing the matched protections, null or empty otherwise</returns>        
        private static ConcurrentQueue<string> FindAllMatches(IEnumerable<string> files, IEnumerable<PathMatchSet> matchers, bool any, bool stopAfterFirst)
        {
            // If there's no mappings, we can't match
            if (matchers == null || !matchers.Any())
                return new ConcurrentQueue<string>();

            // Initialize the list of matched protections
            var matchedProtections = new ConcurrentQueue<string>();

            // Loop through and try everything otherwise
            foreach (var matcher in matchers)
            {
                // Determine if the matcher passes
                bool passes;
                string firstMatchedString;
                if (any)
                {
                    (bool anyPasses, string matchedString) = matcher.MatchesAny(files);
                    passes = anyPasses;
                    firstMatchedString = matchedString;
                }
                else
                {
                    (bool allPasses, List<string> matchedStrings) = matcher.MatchesAll(files);
                    passes = allPasses;
                    firstMatchedString = matchedStrings.FirstOrDefault();
                }
                
                // If we don't have a pass, just continue
                if (!passes)
                    continue;

                // If we there is no version method, just return the protection name
                if (matcher.GetVersion == null)
                {
                    matchedProtections.Enqueue(matcher.ProtectionName ?? "Unknown Protection");
                }

                // Otherwise, invoke the version method
                else
                {
                    // A null version returned means the check didn't pass at the version step
                    string version = matcher.GetVersion(firstMatchedString, files);
                    if (version == null)
                        continue;

                    matchedProtections.Enqueue($"{matcher.ProtectionName ?? "Unknown Protection"} {version}".TrimEnd());
                }

                // If we're stopping after the first protection, bail out here
                if (stopAfterFirst)
                    return matchedProtections;
            }

            return matchedProtections;
        }
    
        #endregion
    }
}