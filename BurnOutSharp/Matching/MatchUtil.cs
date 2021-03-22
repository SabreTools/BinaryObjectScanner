using System.Collections.Generic;
using System.Linq;

namespace BurnOutSharp.Matching
{
    /// <summary>
    /// Helper class for content matching
    /// </summary>
    internal static class MatchUtil
    {
        /// <summary>
        /// Get all content matches for a given list of matchers
        /// </summary>
        /// <param name="file">File to check for matches</param>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <param name="matchers">Enumerable of matchers to be run on the file</param>
        /// <param name="includePosition">True to include positional data, false otherwise</param>
        /// <returns>List of strings representing the matched protections, null or empty otherwise</returns>
        public static List<string> GetAllContentMatches(
            string file,
            byte[] fileContent,
            IEnumerable<Matcher> matchers,
            bool includePosition = false)
        {
            return FindAllContentMatches(file, fileContent, matchers, includePosition, false);
        }

        /// <summary>
        /// Get first content match for a given list of matchers
        /// </summary>
        /// <param name="file">File to check for matches</param>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <param name="matchers">Enumerable of matchers to be run on the file</param>
        /// <param name="includePosition">True to include positional data, false otherwise</param>
        /// <returns>String representing the matched protection, null otherwise</returns>
        public static string GetFirstContentMatch(
            string file,
            byte[] fileContent,
            IEnumerable<Matcher> matchers,
            bool includePosition = false)
        {
            var contentMatches = FindAllContentMatches(file, fileContent, matchers, includePosition, true);
            if (contentMatches == null || !contentMatches.Any())
                return null;
            
            return contentMatches.First();
        }

        /// <summary>
        /// Get the required set of content matches on a per Matcher basis
        /// </summary>
        /// <param name="file">File to check for matches</param>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <param name="matchers">Enumerable of matchers to be run on the file</param>
        /// <param name="includePosition">True to include positional data, false otherwise</param>
        /// <param name="stopAfterFirst">True to stop after the first match, false otherwise</param>
        /// <returns>List of strings representing the matched protections, null or empty otherwise</returns>        
        private static List<string> FindAllContentMatches(
            string file,
            byte[] fileContent,
            IEnumerable<Matcher> matchers,
            bool includePosition,
            bool stopAfterFirst)
        {
            // If there's no mappings, we can't match
            if (matchers == null || !matchers.Any())
                return null;

            // Initialize the list of matched protections
            List<string> matchedProtections = new List<string>();

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
                    matchedProtections.Add((matcher.ProtectionName ?? "Unknown Protection") + (includePosition ? $" (Index {positionsString})" : string.Empty));
                }

                // Otherwise, invoke the version method
                // TODO: Pass all positions to the version finding method
                else
                {
                    string version = matcher.GetVersion(file, fileContent, positions[0]) ?? "Unknown Version";
                    matchedProtections.Add($"{matcher.ProtectionName ?? "Unknown Protection"} {version}" + (includePosition ? $" (Index {positionsString})" : string.Empty));
                }

                // If we're stopping after the first protection, bail out here
                if (stopAfterFirst)
                    return matchedProtections;
            }

            return matchedProtections;
        }
    }
}