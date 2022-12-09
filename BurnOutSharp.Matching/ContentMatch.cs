using System.IO;

namespace BurnOutSharp.Matching
{
    /// <summary>
    /// Content matching criteria
    /// </summary>
    public class ContentMatch : IMatch<byte?[]>
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

        #region Array Matching

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

            // Set the default start and end values
            int start = Start;
            int end = End;

            // If start or end are not set properly, set them to defaults
            if (start < 0)
                start = 0;
            if (end < 0)
                end = stack.Length - Needle.Length;

            for (int i = reverse ? end : start; reverse ? i > start : i < end; i += reverse ? -1 : 1)
            {
                // If we somehow have an invalid end and we haven't matched, return
                if (i > stack.Length)
                    return (false, -1);

                // Check to see if the values are equal
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
            if (Needle.Length > stack.Length - index)
                return false;

            // Loop through and check the value
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

        #region Stream Matching

        /// <summary>
        /// Get if this match can be found in a stack
        /// </summary>
        /// <param name="stack">Stream to search for the given content</param>
        /// <param name="reverse">True to search from the end of the array, false from the start</param>
        /// <returns>Tuple of success and found position</returns>
        public (bool success, int position) Match(Stream stack, bool reverse = false)
        {
            // If either array is null or empty, we can't do anything
            if (stack == null || stack.Length == 0 || Needle == null || Needle.Length == 0)
                return (false, -1);

            // If the needle array is larger than the stack array, it can't be contained within
            if (Needle.Length > stack.Length)
                return (false, -1);

            // Set the default start and end values
            int start = Start;
            int end = End;

            // If start or end are not set properly, set them to defaults
            if (start < 0)
                start = 0;
            if (end < 0)
                end = (int)(stack.Length - Needle.Length);

            for (int i = reverse ? end : start; reverse ? i > start : i < end; i += reverse ? -1 : 1)
            {
                // If we somehow have an invalid end and we haven't matched, return
                if (i > stack.Length)
                    return (false, -1);

                // Check to see if the values are equal
                if (EqualAt(stack, i))
                    return (true, i);
            }

            return (false, -1);
        }

        /// <summary>
        /// Get if a stack at a certain index is equal to a needle
        /// </summary>
        /// <param name="stack">Stream to search for the given content</param>
        /// <param name="index">Starting index to check equality</param>
        /// <returns>True if the needle matches the stack at a given index</returns>
        private bool EqualAt(Stream stack, int index)
        {
            // If the index is invalid, we can't do anything
            if (index < 0)
                return false;

            // If we're too close to the end of the stack, return false
            if (Needle.Length > stack.Length - index)
                return false;

            // Save the current position and move to the index
            long currentPosition = stack.Position;
            stack.Seek(index, SeekOrigin.Begin);

            // Set the return value
            bool matched = true;

            // Loop through and check the value
            for (int i = 0; i < Needle.Length; i++)
            {
                byte stackValue = (byte)stack.ReadByte();

                // A null value is a wildcard
                if (Needle[i] == null)
                {
                    continue;
                }
                else if (stackValue != Needle[i])
                {
                    matched = false;
                    break;
                }
            }

            // Reset the position and return the value
            stack.Seek(currentPosition, SeekOrigin.Begin);
            return matched;
        }

        #endregion
    }
}