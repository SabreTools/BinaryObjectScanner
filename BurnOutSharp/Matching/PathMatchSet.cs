using System;
using System.Collections.Generic;
using System.Linq;

namespace BurnOutSharp.Matching
{
    /// <summary>
    /// A set of path matches that work together
    /// </summary>
    internal class PathMatchSet : MatchSet<PathMatch, string>
    {
        /// <summary>
        /// Function to get a path version for this Matcher
        /// </summary>
        /// <remarks>
        /// A path version method takes the matched path and a list of files
        /// and returns a single string. That string is either a version string,
        /// in which case it will be appended to the protection name, or `null`,
        /// in which case it will cause the protection to be omitted.
        /// </remarks>
        public Func<string, List<string>, string> GetVersion { get; set; }

        #region Constructors

        public PathMatchSet(string needle, string protectionName)
            : this(new List<string> { needle }, null, protectionName) { }

        public PathMatchSet(List<string> needles, string protectionName)
            : this(needles, null, protectionName) { }

        public PathMatchSet(string needle, Func<string, List<string>, string> getVersion, string protectionName)
            : this(new List<string> { needle }, getVersion, protectionName) { }

        public PathMatchSet(List<string> needles, Func<string, List<string>, string> getVersion, string protectionName)
            : this(needles.Select(n => new PathMatch(n)).ToList(), getVersion, protectionName) { }

        public PathMatchSet(PathMatch needle, string protectionName)
            : this(new List<PathMatch>() { needle }, null, protectionName) { }

        public PathMatchSet(List<PathMatch> needles, string protectionName)
            : this(needles, null, protectionName) { }

        public PathMatchSet(PathMatch needle, Func<string, List<string>, string> getVersion, string protectionName)
            : this(new List<PathMatch>() { needle }, getVersion, protectionName) { }

        public PathMatchSet(List<PathMatch> needles, Func<string, List<string>, string> getVersion, string protectionName)
        {
            Matchers = needles;
            GetVersion = getVersion;
            ProtectionName = protectionName;
        }

        #endregion

        #region Matching

        /// <summary>
        /// Determine whether all path matches pass
        /// </summary>
        /// <param name="stack">List of strings to try to match</param>
        /// <returns>Tuple of passing status and matching values</returns>        
        public (bool, List<string>) MatchesAll(List<string> stack)
        {
            // If no path matches are defined, we fail out
            if (Matchers == null || !Matchers.Any())
                return (false, null);

            // Initialize the value list
            List<string> values = new List<string>();

            // Loop through all path matches and make sure all pass
            foreach (var pathMatch in Matchers)
            {
                (bool match, string value) = pathMatch.Match(stack);
                if (!match)
                    return (false, null);
                else
                    values.Add(value);
            }

            return (true, values);
        }

        /// <summary>
        /// Determine whether any path matches pass
        /// </summary>
        /// <param name="stack">List of strings to try to match</param>
        /// <returns>Tuple of passing status and first matching value</returns>        
        public (bool, string) MatchesAny(List<string> stack)
        {
            // If no path matches are defined, we fail out
            if (Matchers == null || !Matchers.Any())
                return (false, null);

            // Loop through all path matches and make sure all pass
            foreach (var pathMatch in Matchers)
            {
                (bool match, string value) = pathMatch.Match(stack);
                if (match)
                    return (true, value);
            }

            return (false, null);
        }

        #endregion
    }
}