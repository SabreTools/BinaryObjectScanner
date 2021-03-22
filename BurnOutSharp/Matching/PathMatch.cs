using System.Collections.Generic;

namespace BurnOutSharp.Matching
{
    /// <summary>
    /// Path matching criteria
    /// </summary>
    internal class PathMatch
    {
        /// <summary>
        /// String to match
        /// </summary>
        public string Needle { get; set; }

        /// <summary>
        /// Match that values end with the needle and not just contains
        /// </summary>
        public bool UseEndsWith { get; set; }

        public PathMatch(string needle, bool useEndsWith = false)
        {
            Needle = needle;
            UseEndsWith = useEndsWith;
        }

        #region Matching

        /// <summary>
        /// Get if this match can be found in a stack
        /// </summary>
        /// <param name="stack">List of strings to search for the given content</param>
        public (bool, string) Match(List<string> stack)
        {
            // If either array is null or empty, we can't do anything
            if (stack == null || stack.Count == 0 || Needle == null || Needle.Length == 0)
                return (false, null);

            foreach (string stackItem in stack)
            {
                if (UseEndsWith && stackItem.EndsWith(Needle))
                    return (true, stackItem);
                else if (!UseEndsWith && stackItem.Contains(Needle))
                    return (true, stackItem);
            }

            return (false, null);
        }
    
        #endregion
    }
}