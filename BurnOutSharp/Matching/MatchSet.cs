using System.Collections.Generic;

namespace BurnOutSharp.Matching
{
    /// <summary>
    /// Wrapper for a single set of matching criteria
    /// </summary>
    public abstract class MatchSet<T, U> where T : IMatch<U>
    {
        /// <summary>
        /// Set of all matchers
        /// </summary>
        public IEnumerable<T> Matchers { get; set; }

        /// <summary>
        /// Name of the protection to show
        /// </summary>
        public string ProtectionName { get; set; }
    }
}