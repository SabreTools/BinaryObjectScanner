namespace BurnOutSharp.Matching
{
    /// <summary>
    /// Single matching criteria
    /// </summary>
    internal class ContentMatch
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

        public ContentMatch(byte?[] needle, int start = -1, int end = -1)
        {
            Needle = needle;
            Start = start;
            End = end;
        }
    }
}