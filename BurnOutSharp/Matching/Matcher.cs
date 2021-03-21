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
        /// Function to get a version for this Matcher
        /// </summary>
        public Func<string, byte[], int, string> GetVersion { get; set; }

        /// <summary>
        /// Name of the protection to show
        /// </summary>
        public string ProtectionName { get; set; }

        #region Constructors

        public Matcher(byte?[] needle, string protectionName)
            : this(new List<byte?[]>() { needle }, null, protectionName) { }

        public Matcher(List<byte?[]> needles, string protectionName)
            : this(needles, null, protectionName) { }

        public Matcher(byte?[] needle, Func<string, byte[], int, string> getVersion, string protectionName)
            : this(new List<byte?[]>() { needle }, getVersion, protectionName) { }

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
    }
}