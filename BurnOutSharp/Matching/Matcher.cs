using System;
using System.Collections.Generic;
using System.Linq;

namespace BurnOutSharp.Matching
{
    /// <summary>
    /// Wrapper for a single set of matching criteria
    /// </summary>
    internal class Matcher
    {
        /// <summary>
        /// Set of all content matches
        /// </summary>
        public IEnumerable<ContentMatch> ContentMatches { get; set; }

        /// <summary>
        /// Set of all path matches
        /// </summary>
        public IEnumerable<PathMatch> PathMatches { get; set; }

        /// <summary>
        /// Function to get a version for this Matcher
        /// </summary>
        /// TODO: Can this be made more generic?
        public Func<string, byte[], int, string> GetVersion { get; set; }

        /// <summary>
        /// Name of the protection to show
        /// </summary>
        public string ProtectionName { get; set; }

        #region ContentMatch Constructors

        public Matcher(byte?[] needle, string protectionName)
            : this(new List<byte?[]> { needle }, null, protectionName) { }

        public Matcher(List<byte?[]> needles, string protectionName)
            : this(needles, null, protectionName) { }

        public Matcher(byte?[] needle, Func<string, byte[], int, string> getVersion, string protectionName)
            : this(new List<byte?[]> { needle }, getVersion, protectionName) { }

        public Matcher(List<byte?[]> needles, Func<string, byte[], int, string> getVersion, string protectionName)
            : this(needles.Select(n => new ContentMatch(n)).ToList(), getVersion, protectionName) { }

        public Matcher(ContentMatch needle, string protectionName)
            : this(new List<ContentMatch>() { needle }, null, protectionName) { }

        public Matcher(List<ContentMatch> needles, string protectionName)
            : this(needles, null, protectionName) { }

        public Matcher(ContentMatch needle, Func<string, byte[], int, string> getVersion, string protectionName)
            : this(new List<ContentMatch>() { needle }, getVersion, protectionName) { }

        public Matcher(List<ContentMatch> needles, Func<string, byte[], int, string> getVersion, string protectionName)
        {
            ContentMatches = needles;
            GetVersion = getVersion;
            ProtectionName = protectionName;
        }

        #endregion

        #region PathMatch Constructors

        public Matcher(string needle, string protectionName)
            : this(new List<string> { needle }, null, protectionName) { }

        public Matcher(List<string> needles, string protectionName)
            : this(needles, null, protectionName) { }

        public Matcher(string needle, Func<string, byte[], int, string> getVersion, string protectionName)
            : this(new List<string> { needle }, getVersion, protectionName) { }

        public Matcher(List<string> needles, Func<string, byte[], int, string> getVersion, string protectionName)
            : this(needles.Select(n => new PathMatch(n)).ToList(), getVersion, protectionName) { }

        public Matcher(PathMatch needle, string protectionName)
            : this(new List<PathMatch>() { needle }, null, protectionName) { }

        public Matcher(List<PathMatch> needles, string protectionName)
            : this(needles, null, protectionName) { }

        public Matcher(PathMatch needle, Func<string, byte[], int, string> getVersion, string protectionName)
            : this(new List<PathMatch>() { needle }, getVersion, protectionName) { }

        public Matcher(List<PathMatch> needles, Func<string, byte[], int, string> getVersion, string protectionName)
        {
            PathMatches = needles;
            GetVersion = getVersion;
            ProtectionName = protectionName;
        }

        #endregion

        #region Matching

        /// <summary>
        /// Determine whether all content matches pass
        /// </summary>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <returns>Tuole of passing status and matching positions</returns>        
        public (bool, List<int>) MatchesAll(byte[] fileContent)
        {
            // If no content matches are defined, we fail out
            if (ContentMatches == null || !ContentMatches.Any())
                return (false, null);

            // Initialize the position list
            List<int> positions = new List<int>();

            // Loop through all content matches and make sure all pass
            foreach (var contentMatch in ContentMatches)
            {
                (bool match, int position) = contentMatch.Match(fileContent);
                if (!match)
                    return (false, null);
                else
                    positions.Add(position);
            }

            return (true, positions);
        }

        /// <summary>
        /// Determine whether all content matches pass
        /// </summary>
        /// <param name="stack">List of strings to try to match</param>
        /// <returns>Tuole of passing status and matching values</returns>        
        public (bool, List<string>) MatchesAll(List<string> stack)
        {
            // If no path matches are defined, we fail out
            if (PathMatches == null || !PathMatches.Any())
                return (false, null);

            // Initialize the value list
            List<string> values = new List<string>();

            // Loop through all path matches and make sure all pass
            foreach (var pathMatch in PathMatches)
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
        /// Determine whether any content matches pass
        /// </summary>
        /// <param name="stack">List of strings to try to match</param>
        /// <returns>Tuole of passing status and matching values</returns>        
        public (bool, string) MatchesAny(List<string> stack)
        {
            // If no path matches are defined, we fail out
            if (PathMatches == null || !PathMatches.Any())
                return (false, null);

            // Loop through all path matches and make sure all pass
            foreach (var pathMatch in PathMatches)
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