using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinaryObjectScanner.Matching
{
    /// <summary>
    /// A set of content matches that work together
    /// </summary>
    public class ContentMatchSet : MatchSet<ContentMatch, byte?[]>
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
#if NET48
        public Func<string, byte[], List<int>, string> GetArrayVersion { get; private set; }
#else
        public Func<string, byte[], List<int>, string>? GetArrayVersion { get; init; }
#endif

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
#if NET48
        public Func<string, Stream, List<int>, string> GetStreamVersion { get; private set; }
#else
        public Func<string, Stream, List<int>, string>? GetStreamVersion { get; init; }
#endif

        #region Generic Constructors

        public ContentMatchSet(byte?[] needle, string protectionName)
            : this(new List<byte?[]> { needle }, getArrayVersion: null, protectionName) { }

        public ContentMatchSet(List<byte?[]> needles, string protectionName)
            : this(needles, getArrayVersion: null, protectionName) { }

        public ContentMatchSet(ContentMatch needle, string protectionName)
            : this(new List<ContentMatch>() { needle }, getArrayVersion: null, protectionName) { }

        public ContentMatchSet(List<ContentMatch> needles, string protectionName)
            : this(needles, getArrayVersion: null, protectionName) { }

        #endregion

        #region Array Constructors

#if NET48
        public ContentMatchSet(byte?[] needle, Func<string, byte[], List<int>, string> getArrayVersion, string protectionName)
            : this(new List<byte?[]> { needle }, getArrayVersion, protectionName) { }

        public ContentMatchSet(List<byte?[]> needles, Func<string, byte[], List<int>, string> getArrayVersion, string protectionName)
            : this(needles.Select(n => new ContentMatch(n)).ToList(), getArrayVersion, protectionName) { }

        public ContentMatchSet(ContentMatch needle, Func<string, byte[], List<int>, string> getArrayVersion, string protectionName)
            : this(new List<ContentMatch>() { needle }, getArrayVersion, protectionName) { }

        public ContentMatchSet(List<ContentMatch> needles, Func<string, byte[], List<int>, string> getArrayVersion, string protectionName)
        {
            Matchers = needles;
            GetArrayVersion = getArrayVersion;
            ProtectionName = protectionName;
        }
#else
        public ContentMatchSet(byte?[] needle, Func<string, byte[], List<int>, string>? getArrayVersion, string protectionName)
            : this(new List<byte?[]> { needle }, getArrayVersion, protectionName) { }

        public ContentMatchSet(List<byte?[]> needles, Func<string, byte[], List<int>, string>? getArrayVersion, string protectionName)
            : this(needles.Select(n => new ContentMatch(n)).ToList(), getArrayVersion, protectionName) { }

        public ContentMatchSet(ContentMatch needle, Func<string, byte[], List<int>, string>? getArrayVersion, string protectionName)
            : this(new List<ContentMatch>() { needle }, getArrayVersion, protectionName) { }

        public ContentMatchSet(List<ContentMatch> needles, Func<string, byte[], List<int>, string>? getArrayVersion, string protectionName)
        {
            Matchers = needles;
            GetArrayVersion = getArrayVersion;
            ProtectionName = protectionName;
        }
#endif

        #endregion

        #region Stream Constructors

#if NET48
        public ContentMatchSet(byte?[] needle, Func<string, Stream, List<int>, string> getStreamVersion, string protectionName)
            : this(new List<byte?[]> { needle }, getStreamVersion, protectionName) { }

        public ContentMatchSet(List<byte?[]> needles, Func<string, Stream, List<int>, string> getStreamVersion, string protectionName)
            : this(needles.Select(n => new ContentMatch(n)).ToList(), getStreamVersion, protectionName) { }

        public ContentMatchSet(ContentMatch needle, Func<string, Stream, List<int>, string> getStreamVersion, string protectionName)
            : this(new List<ContentMatch>() { needle }, getStreamVersion, protectionName) { }

        public ContentMatchSet(List<ContentMatch> needles, Func<string, Stream, List<int>, string> getStreamVersion, string protectionName)
        {
            Matchers = needles;
            GetStreamVersion = getStreamVersion;
            ProtectionName = protectionName;
        }
#else
        public ContentMatchSet(byte?[] needle, Func<string, Stream, List<int>, string>? getStreamVersion, string protectionName)
            : this(new List<byte?[]> { needle }, getStreamVersion, protectionName) { }

        public ContentMatchSet(List<byte?[]> needles, Func<string, Stream, List<int>, string>? getStreamVersion, string protectionName)
            : this(needles.Select(n => new ContentMatch(n)).ToList(), getStreamVersion, protectionName) { }

        public ContentMatchSet(ContentMatch needle, Func<string, Stream, List<int>, string>? getStreamVersion, string protectionName)
            : this(new List<ContentMatch>() { needle }, getStreamVersion, protectionName) { }

        public ContentMatchSet(List<ContentMatch> needles, Func<string, Stream, List<int>, string>? getStreamVersion, string protectionName)
        {
            Matchers = needles;
            GetStreamVersion = getStreamVersion;
            ProtectionName = protectionName;
        }
#endif

        #endregion

        #region Array Matching

        /// <summary>
        /// Determine whether all content matches pass
        /// </summary>
        /// <param name="stack">Array to search</param>
        /// <returns>Tuple of passing status and matching positions</returns>
        public (bool, List<int>) MatchesAll(byte[] stack)
        {
            // If no content matches are defined, we fail out
            if (Matchers == null || !Matchers.Any())
                return (false, new List<int>());

            // Initialize the position list
            var positions = new List<int>();

            // Loop through all content matches and make sure all pass
            foreach (var contentMatch in Matchers)
            {
                (bool match, int position) = contentMatch.Match(stack);
                if (!match)
                    return (false, new List<int>());
                else
                    positions.Add(position);
            }

            return (true, positions);
        }

        /// <summary>
        /// Determine whether any content matches pass
        /// </summary>
        /// <param name="stack">Array to search</param>
        /// <returns>Tuple of passing status and first matching position</returns>
        public (bool, int) MatchesAny(byte[] stack)
        {
            // If no content matches are defined, we fail out
            if (Matchers == null || !Matchers.Any())
                return (false, -1);

            // Loop through all content matches and make sure all pass
            foreach (var contentMatch in Matchers)
            {
                (bool match, int position) = contentMatch.Match(stack);
                if (match)
                    return (true, position);
            }

            return (false, -1);
        }

        #endregion

        #region Stream Matching

        /// <summary>
        /// Determine whether all content matches pass
        /// </summary>
        /// <param name="stack">Stream to search</param>
        /// <returns>Tuple of passing status and matching positions</returns>
        public (bool, List<int>) MatchesAll(Stream stack)
        {
            // If no content matches are defined, we fail out
            if (Matchers == null || !Matchers.Any())
                return (false, new List<int>());

            // Initialize the position list
            List<int> positions = new List<int>();

            // Loop through all content matches and make sure all pass
            foreach (var contentMatch in Matchers)
            {
                (bool match, int position) = contentMatch.Match(stack);
                if (!match)
                    return (false, new List<int>());
                else
                    positions.Add(position);
            }

            return (true, positions);
        }

        /// <summary>
        /// Determine whether any content matches pass
        /// </summary>
        /// <param name="stack">Stream to search</param>
        /// <returns>Tuple of passing status and first matching position</returns>
        public (bool, int) MatchesAny(Stream stack)
        {
            // If no content matches are defined, we fail out
            if (Matchers == null || !Matchers.Any())
                return (false, -1);

            // Loop through all content matches and make sure all pass
            foreach (var contentMatch in Matchers)
            {
                (bool match, int position) = contentMatch.Match(stack);
                if (match)
                    return (true, position);
            }

            return (false, -1);
        }

        #endregion
    }
}