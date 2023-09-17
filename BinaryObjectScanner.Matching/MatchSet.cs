using System.Collections.Generic;

namespace BinaryObjectScanner.Matching
{
    /// <summary>
    /// Wrapper for a single set of matching criteria
    /// </summary>
    public abstract class MatchSet<T, U> where T : IMatch<U>
    {
        /// <summary>
        /// Set of all matchers
        /// </summary>
#if NET48
        public IEnumerable<T> Matchers { get; set; }
#else
        public IEnumerable<T>? Matchers { get; set; }
#endif

        /// <summary>
        /// Name of the protection to show
        /// </summary>
#if NET48
        public string ProtectionName { get; set; }
#else
        public string? ProtectionName { get; set; }
#endif
    }
}