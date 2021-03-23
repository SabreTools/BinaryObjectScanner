namespace BurnOutSharp.Matching
{
    /// <summary>
    /// Content matching criteria
    /// </summary>
    internal class ContentMatch : IMatch<byte?[]>
    {
        /// <summary>
        /// Content to match
        /// </summary>
        public byte?[] Needle { get; set; }

        /// <summary>
        /// Starting index for matching
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Ending index for matching
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="needle">Byte array representing the search</param>
        /// <param name="start">Optional starting index</param>
        /// <param name="end">Optional ending index</param>
        public ContentMatch(byte?[] needle, int start = -1, int end = -1)
        {
            Needle = needle;
            Start = start;
            End = end;
        }

        #region Matching

        /// <summary>
        /// Get if this match can be found in a stack
        /// </summary>
        /// <param name="stack">Array to search for the given content</param>
        /// <param name="reverse">True to search from the end of the array, false from the start</param>
        /// <returns>Tuple of success and found position</returns>
        public (bool success, int position) Match(byte[] stack, bool reverse = false)
        {
            // If either array is null or empty, we can't do anything
            if (stack == null || stack.Length == 0 || Needle == null || Needle.Length == 0)
                return (false, -1);

            // If the needle array is larger than the stack array, it can't be contained within
            if (Needle.Length > stack.Length)
                return (false, -1);

            // If start or end are not set properly, set them to defaults
            if (Start < 0)
                Start = 0;
            if (End < 0)
                End = stack.Length - Needle.Length;

            for (int i = reverse ? End : Start; reverse ? i > Start : i < End; i += reverse ? -1 : 1)
            {
                if (EqualAt(stack, i))
                    return (true, i);
            }

            return (false, -1);
        }

        /// <summary>
        /// Get if a stack at a certain index is equal to a needle
        /// </summary>
        /// <param name="stack">Array to search for the given content</param>
        /// <param name="index">Starting index to check equality</param>
        /// <returns>True if the needle matches the stack at a given index</returns>
        private bool EqualAt(byte[] stack, int index)
        {
            // If the index is invalid, we can't do anything
            if (index < 0)
                return false;

            // If we're too close to the end of the stack, return false
            if (Needle.Length >= stack.Length - index)
                return false;

            for (int i = 0; i < Needle.Length; i++)
            {
                // A null value is a wildcard
                if (Needle[i] == null)
                    continue;
                else if (stack[i + index] != Needle[i])
                    return false;
            }

            return true;
        }
    
        #endregion
    }
}