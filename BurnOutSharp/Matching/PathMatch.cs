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

        public PathMatch(string needle)
        {
            Needle = needle;
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
                if (stackItem.Contains(Needle))
                    return (true, stackItem);
            }

            return (false, null);
        }
    
        #endregion
    }
}