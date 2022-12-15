using System.Collections.Generic;
using System.Linq;

namespace BurnOutSharp.Matching
{
    public static class Extensions
    {
        /// <summary>
        /// Find all positions of one array in another, if possible, if possible
        /// </summary>
        public static List<int> FindAllPositions(this byte[] stack, byte?[] needle, int start = 0, int end = -1)
        {
            // Get the outgoing list
            List<int> positions = new List<int>();

            // Initialize the loop variables
            bool found = true;
            int lastPosition = start;
            var matcher = new ContentMatch(needle, end: end);

            // Loop over and get all positions
            while (found)
            {
                matcher.Start = lastPosition;
                (found, lastPosition) = matcher.Match(stack, false);
                if (found)
                    positions.Add(lastPosition);
            }

            return positions;
        }

        /// <summary>
        /// Find the first position of one array in another, if possible
        /// </summary>
        public static bool FirstPosition(this byte[] stack, byte[] needle, out int position, int start = 0, int end = -1)
        {
            byte?[] nullableNeedle = needle != null ? needle.Select(b => (byte?)b).ToArray() : null;
            return stack.FirstPosition(nullableNeedle, out position, start, end);
        }

        /// <summary>
        /// Find the first position of one array in another, if possible
        /// </summary>
        public static bool FirstPosition(this byte[] stack, byte?[] needle, out int position, int start = 0, int end = -1)
        {
            var matcher = new ContentMatch(needle, start, end);
            (bool found, int foundPosition) = matcher.Match(stack, false);
            position = foundPosition;
            return found;
        }

        /// <summary>
        /// Find the last position of one array in another, if possible
        /// </summary>
        public static bool LastPosition(this byte[] stack, byte?[] needle, out int position, int start = 0, int end = -1)
        {
            var matcher = new ContentMatch(needle, start, end);
            (bool found, int foundPosition) = matcher.Match(stack, true);
            position = foundPosition;
            return found;
        }

        /// <summary>
        /// See if a byte array starts with another
        /// </summary>
        public static bool StartsWith(this byte[] stack, byte?[] needle)
        {
            return stack.FirstPosition(needle, out int _, start: 0, end: 1);
        }

        /// <summary>
        /// See if a byte array ends with another
        /// </summary>
        public static bool EndsWith(this byte[] stack, byte?[] needle)
        {
            return stack.FirstPosition(needle, out int _, start: stack.Length - needle.Length);
        }
    }
}