using System;
using System.Collections.Generic;
using System.Linq;

namespace BinaryObjectScanner.Matching
{
    /// <summary>
    /// A set of path matches that work together
    /// </summary>
    public class PathMatchSet : MatchSet<PathMatch, string>
    {
        /// <summary>
        /// Function to get a path version for this Matcher
        /// </summary>
        /// <remarks>
        /// A path version method takes the matched path and an enumerable of files
        /// and returns a single string. That string is either a version string,
        /// in which case it will be appended to the protection name, or `null`,
        /// in which case it will cause the protection to be omitted.
        /// </remarks>
#if NET48
        public Func<string, IEnumerable<string>, string> GetVersion { get; private set; }
#else
        public Func<string, IEnumerable<string>, string>? GetVersion { get; init; }
#endif

        #region Constructors

        public PathMatchSet(string needle, string protectionName)
            : this(new List<string> { needle }, null, protectionName) { }

        public PathMatchSet(List<string> needles, string protectionName)
            : this(needles, null, protectionName) { }

#if NET48
        public PathMatchSet(string needle, Func<string, IEnumerable<string>, string> getVersion, string protectionName)
            : this(new List<string> { needle }, getVersion, protectionName) { }

        public PathMatchSet(List<string> needles, Func<string, IEnumerable<string>, string> getVersion, string protectionName)
            : this(needles.Select(n => new PathMatch(n)).ToList(), getVersion, protectionName) { }
#else
        public PathMatchSet(string needle, Func<string, IEnumerable<string>, string>? getVersion, string protectionName)
            : this(new List<string> { needle }, getVersion, protectionName) { }

        public PathMatchSet(List<string> needles, Func<string, IEnumerable<string>, string>? getVersion, string protectionName)
            : this(needles.Select(n => new PathMatch(n)).ToList(), getVersion, protectionName) { }
#endif

        public PathMatchSet(PathMatch needle, string protectionName)
            : this(new List<PathMatch>() { needle }, null, protectionName) { }

        public PathMatchSet(List<PathMatch> needles, string protectionName)
            : this(needles, null, protectionName) { }

#if NET48
        public PathMatchSet(PathMatch needle, Func<string, IEnumerable<string>, string> getVersion, string protectionName)
            : this(new List<PathMatch>() { needle }, getVersion, protectionName) { }

        public PathMatchSet(List<PathMatch> needles, Func<string, IEnumerable<string>, string> getVersion, string protectionName)
        {
            Matchers = needles;
            GetVersion = getVersion;
            ProtectionName = protectionName;
        }
#else
        public PathMatchSet(PathMatch needle, Func<string, IEnumerable<string>, string>? getVersion, string protectionName)
            : this(new List<PathMatch>() { needle }, getVersion, protectionName) { }

        public PathMatchSet(List<PathMatch> needles, Func<string, IEnumerable<string>, string>? getVersion, string protectionName)
        {
            Matchers = needles;
            GetVersion = getVersion;
            ProtectionName = protectionName;
        }
#endif

        #endregion

        #region Matching

        /// <summary>
        /// Determine whether all path matches pass
        /// </summary>
        /// <param name="stack">List of strings to try to match</param>
        /// <returns>Tuple of passing status and matching values</returns>
        public (bool, List<string>) MatchesAll(IEnumerable<string> stack)
        {
            // If no path matches are defined, we fail out
            if (Matchers == null || !Matchers.Any())
                return (false, new List<string>());

            // Initialize the value list
            List<string> values = new List<string>();

            // Loop through all path matches and make sure all pass
            foreach (var pathMatch in Matchers)
            {
#if NET48
                (bool match, string value) = pathMatch.Match(stack);
#else
                (bool match, string? value) = pathMatch.Match(stack);
#endif
                if (!match || value == null)
                    return (false, new List<string>());
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
#if NET48
        public (bool, string) MatchesAny(IEnumerable<string> stack)
#else
        public (bool, string?) MatchesAny(IEnumerable<string> stack)
#endif
        {
            // If no path matches are defined, we fail out
            if (Matchers == null || !Matchers.Any())
                return (false, null);

            // Loop through all path matches and make sure all pass
            foreach (var pathMatch in Matchers)
            {
#if NET48
                (bool match, string value) = pathMatch.Match(stack);
#else
                (bool match, string? value) = pathMatch.Match(stack);
#endif
                if (match)
                    return (true, value);
            }

            return (false, null);
        }

        #endregion
    }
}