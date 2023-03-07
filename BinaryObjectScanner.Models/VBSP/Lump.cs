namespace BinaryObjectScanner.Models.VBSP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VBSPFile.h"/>
    public sealed class Lump
    {
        public uint Offset;

        public uint Length;

        /// <summary>
        /// Default to zero.
        /// </summary>
        public uint Version;

        /// <summary>
        /// Default to (char)0, (char)0, (char)0, (char)0.
        /// </summary>
        public char[] FourCC;
    }
}
