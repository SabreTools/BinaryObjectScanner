namespace BurnOutSharp.Models.Compression.LZX
{
    /// <summary>
    /// The fields of a verbatim block that follow the generic block header
    /// </summary>
    /// <see href="https://interoperability.blob.core.windows.net/files/MS-PATCH/%5bMS-PATCH%5d.pdf"/>
    public class VerbatimBlock
    {
        /// <summary>
        /// Generic block header
        /// </summary>
        public BlockHeader Header;

        /// <summary>
        /// Pretree for first 256 elements of main tree
        /// </summary>
        /// <remarks>20 elements, 4 bits each</remarks>
        public byte[] PretreeFirst256;

        /// <summary>
        /// Path lengths of first 256 elements of main tree
        /// </summary>
        /// <remarks>Encoded using pretree</remarks>
        public int[] PathLengthsFirst256;

        /// <summary>
        /// Pretree for remainder of main tree
        /// </summary>
        /// <remarks>20 elements, 4 bits each</remarks>
        public byte[] PretreeRemainder;

        /// <summary>
        /// Path lengths of remaining elements of main tree
        /// </summary>
        /// <remarks>Encoded using pretree</remarks>
        public int[] PathLengthsRemainder;

        /// <summary>
        /// Pretree for length tree
        /// </summary>
        /// <remarks>20 elements, 4 bits each</remarks>
        public byte[] PretreeLengthTree;

        /// <summary>
        /// Path lengths of elements in length tree
        /// </summary>
        /// <remarks>Encoded using pretree</remarks>
        public int[] PathLengthsLengthTree;

        // Entry                                    Comments                    Size
        // ---------------------------------------------------------------------------------------
        // Token sequence (matches and literals)    Specified in section 2.6    Variable
    }
}