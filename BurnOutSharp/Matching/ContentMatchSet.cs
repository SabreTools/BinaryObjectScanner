using System;
using System.Collections.Generic;
using System.Linq;

namespace BurnOutSharp.Matching
{
    /// <summary>
    /// A set of content matches that work together
    /// </summary>
    internal class ContentMatchSet : MatchSet<ContentMatch, byte?[]>
    {
        /// <summary>
        /// Function to get a content version
        /// </summary>
        /// <remarks>
        /// A content version method takes the file path, the file contents,
        /// and a list of found positions and returns a single string. That
        /// string is either a version string, in which case it will be appended
        /// to the protection name, or `null`, in which case it will cause
        /// the protection to be omitted.
        /// </remarks>
        public Func<string, byte[], List<int>, string> GetVersion { get; set; }

        #region Constructors

        public ContentMatchSet(byte?[] needle, string protectionName)
            : this(new List<byte?[]> { needle }, null, protectionName) { }

        public ContentMatchSet(List<byte?[]> needles, string protectionName)
            : this(needles, null, protectionName) { }

        public ContentMatchSet(byte?[] needle, Func<string, byte[], List<int>, string> getVersion, string protectionName)
            : this(new List<byte?[]> { needle }, getVersion, protectionName) { }

        public ContentMatchSet(List<byte?[]> needles, Func<string, byte[], List<int>, string> getVersion, string protectionName)
            : this(needles.Select(n => new ContentMatch(n)).ToList(), getVersion, protectionName) { }

        public ContentMatchSet(ContentMatch needle, string protectionName)
            : this(new List<ContentMatch>() { needle }, null, protectionName) { }

        public ContentMatchSet(List<ContentMatch> needles, string protectionName)
            : this(needles, null, protectionName) { }

        public ContentMatchSet(ContentMatch needle, Func<string, byte[], List<int>, string> getVersion, string protectionName)
            : this(new List<ContentMatch>() { needle }, getVersion, protectionName) { }

        public ContentMatchSet(List<ContentMatch> needles, Func<string, byte[], List<int>, string> getVersion, string protectionName)
        {
            Matchers = needles;
            GetVersion = getVersion;
            ProtectionName = protectionName;
        }

        #endregion

        #region Matching

        /// <summary>
        /// Determine whether all content matches pass
        /// </summary>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <returns>Tuple of passing status and matching positions</returns>        
        public (bool, List<int>) MatchesAll(byte[] fileContent)
        {
            // If no content matches are defined, we fail out
            if (Matchers == null || !Matchers.Any())
                return (false, null);

            // Initialize the position list
            List<int> positions = new List<int>();

            // Loop through all content matches and make sure all pass
            foreach (var contentMatch in Matchers)
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
        /// Determine whether any content matches pass
        /// </summary>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <returns>Tuple of passing status and first matching position</returns>        
        public (bool, int) MatchesAny(byte[] fileContent)
        {
            // If no content matches are defined, we fail out
            if (Matchers == null || !Matchers.Any())
                return (false, -1);

            // Loop through all content matches and make sure all pass
            foreach (var contentMatch in Matchers)
            {
                (bool match, int position) = contentMatch.Match(fileContent);
                if (match)
                    return (true, position);
            }

            return (false, -1);
        }

        #endregion
    }
}