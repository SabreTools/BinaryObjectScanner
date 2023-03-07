namespace BinaryObjectScanner.Models.BMP
{
    /// <summary>
    /// The BITMAPFILEHEADER structure contains information about the type, size,
    /// and layout of a file that contains a DIB.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapfileheader"/>
    public sealed class BITMAPFILEHEADER
    {
        /// <summary>
        /// The file type; must be BM.
        /// </summary>
        public ushort Type;

        /// <summary>
        /// The size, in bytes, of the bitmap file.
        /// </summary>
        public uint Size;

        /// <summary>
        /// Reserved; must be zero.
        /// </summary>
        public ushort Reserved1;

        /// <summary>
        /// Reserved; must be zero.
        /// </summary>
        public ushort Reserved2;

        /// <summary>
        /// The offset, in bytes, from the beginning of the BITMAPFILEHEADER structure to the bitmap bits.
        /// </summary>
        public uint OffBits;
    }
}
