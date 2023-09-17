using System.Collections.Generic;
using System.Linq;

namespace BinaryObjectScanner.Matching
{
    /// <summary>
    /// Path matching criteria
    /// </summary>
    public class PathMatch : IMatch<string>
    {
        /// <summary>
        /// String to match
        /// </summary>
#if NET48
        public string Needle { get; set; }
#else
        public string? Needle { get; init; }
#endif

        /// <summary>
        /// Match exact casing instead of invariant
        /// </summary>
#if NET48
        public bool MatchExact { get; private set; }
#else
        public bool MatchExact { get; init; }
#endif

        /// <summary>
        /// Match that values end with the needle and not just contains
        /// </summary>
#if NET48
        public bool UseEndsWith { get; private set; }
#else
        public bool UseEndsWith { get; init; }
#endif

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="needle">String representing the search</param>
        /// <param name="matchExact">True to match exact casing, false otherwise</param>
        /// <param name="useEndsWith">True to match the end only, false for all contents</param>
#if NET48
        public PathMatch(string needle, bool matchExact = false, bool useEndsWith = false)
#else
        public PathMatch(string? needle, bool matchExact = false, bool useEndsWith = false)
#endif
        {
            this.Needle = needle;
            this.MatchExact = matchExact;
            this.UseEndsWith = useEndsWith;
        }

        #region Matching

        /// <summary>
        /// Get if this match can be found in a stack
        /// </summary>
        /// <param name="stack">List of strings to search for the given content</param>
        /// <returns>Tuple of success and matched item</returns>
#if NET48
        public (bool, string) Match(IEnumerable<string> stack)
#else
        public (bool, string?) Match(IEnumerable<string>? stack)
#endif
        {
            // If either array is null or empty, we can't do anything
            if (stack == null || !stack.Any() || this.Needle == null || this.Needle.Length == 0)
                return (false, null);

            // Preprocess the needle, if necessary
            string procNeedle = this.MatchExact ? this.Needle : this.Needle.ToLowerInvariant();

            foreach (string stackItem in stack)
            {
                // Preprocess the stack item, if necessary
                string procStackItem = this.MatchExact ? stackItem : stackItem.ToLowerInvariant();

                if (this.UseEndsWith && procStackItem.EndsWith(procNeedle))
                    return (true, stackItem);
                else if (!this.UseEndsWith && procStackItem.Contains(procNeedle))
                    return (true, stackItem);
            }

            return (false, null);
        }
    
        #endregion
    }
}