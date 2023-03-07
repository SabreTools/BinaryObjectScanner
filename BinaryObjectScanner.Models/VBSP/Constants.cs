namespace BinaryObjectScanner.Models.VBSP
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0x56, 0x42, 0x53, 0x50 };

        public const string SignatureString = "VBSP";

        public const uint SignatureUInt32 = 0x50534256;

        /// <summary>
        /// Total number of lumps in the package
        /// </summary>
        public const int HL_VBSP_LUMP_COUNT = 64;

        /// <summary>
        /// Index for the entities lump
        /// </summary>
        public const int HL_VBSP_LUMP_ENTITIES = 0;

        /// <summary>
        /// Index for the pakfile lump
        /// </summary>
        public const int HL_VBSP_LUMP_PAKFILE = 40;
    }
}