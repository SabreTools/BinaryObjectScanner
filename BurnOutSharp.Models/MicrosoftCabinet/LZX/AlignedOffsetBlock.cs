namespace BurnOutSharp.Models.MicrosoftCabinet.LZX
{
    /// <summary>
    /// An aligned offset block is identical to the verbatim block except for the presence of the aligned offset
    /// tree preceding the other trees.
    /// </summary>
    /// <see href="https://interoperability.blob.core.windows.net/files/MS-PATCH/%5bMS-PATCH%5d.pdf"/>
    public class AlignedOffsetBlock
    {
        /// <summary>
        /// Generic block header
        /// </summary>
        public BlockHeader Header;

        /// <summary>
        /// Aligned offset tree
        /// </summary>
        /// <remarks>8 elements, 3 bits each</remarks>
        public byte[] AlignedOffsetTree;

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