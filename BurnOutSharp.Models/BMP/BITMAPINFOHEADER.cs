namespace BurnOutSharp.Models.BMP
{
    /// <summary>
    /// The BITMAPINFOHEADER structure contains information about the dimensions and
    /// color format of a device-independent bitmap (DIB).
    /// </summary>
    public sealed class BITMAPINFOHEADER
    {
        /// <summary>
        /// Specifies the number of bytes required by the structure. This value does
        /// not include the size of the color table or the size of the color masks,
        /// if they are appended to the end of structure.
        /// </summary>
        public uint Size;

        /// <summary>
        /// Specifies the width of the bitmap, in pixels.
        /// </summary>
        public int Width;

        /// <summary>
        /// Specifies the height of the bitmap, in pixels.
        /// - For uncompressed RGB bitmaps, if biHeight is positive, the bitmap is a
        ///   bottom-up DIB with the origin at the lower left corner. If biHeight is
        ///   negative, the bitmap is a top-down DIB with the origin at the upper left
        ///   corner.
        /// - For YUV bitmaps, the bitmap is always top-down, regardless of the sign of
        ///   biHeight. Decoders should offer YUV formats with positive biHeight, but for
        ///   backward compatibility they should accept YUV formats with either positive
        ///   or negative biHeight.
        /// - For compressed formats, biHeight must be positive, regardless of image orientation.
        /// </summary>
        public int Height;

        /// <summary>
        /// Specifies the number of planes for the target device. This value must be set to 1.
        /// </summary>
        public ushort Planes;

        /// <summary>
        /// Specifies the number of bits per pixel (bpp). For uncompressed formats, this value
        /// is the average number of bits per pixel. For compressed formats, this value is the
        /// implied bit depth of the uncompressed image, after the image has been decoded.
        /// </summary>
        public ushort BitCount;

        /// <summary>
        /// For compressed video and YUV formats, this member is a FOURCC code, specified as a
        /// DWORD in little-endian order. For example, YUYV video has the FOURCC 'VYUY' or
        /// 0x56595559. For more information, see FOURCC Codes.
        /// 
        /// For uncompressed RGB formats, the following values are possible:
        /// - BI_RGB: Uncompressed RGB.
        /// - BI_BITFIELDS: Uncompressed RGB with color masks. Valid for 16-bpp and 32-bpp bitmaps.
        ///
        /// Note that BI_JPG and BI_PNG are not valid video formats.
        /// 
        /// For 16-bpp bitmaps, if biCompression equals BI_RGB, the format is always RGB 555.
        /// If biCompression equals BI_BITFIELDS, the format is either RGB 555 or RGB 565. Use
        /// the subtype GUID in the AM_MEDIA_TYPE structure to determine the specific RGB type.
        /// </summary>
        public uint Compression;

        /// <summary>
        /// Specifies the size, in bytes, of the image. This can be set to 0 for uncompressed
        /// RGB bitmaps.
        /// </summary>
        public uint SizeImage;

        /// <summary>
        /// Specifies the horizontal resolution, in pixels per meter, of the target device for
        /// the bitmap.
        /// </summary>
        public int XPelsPerMeter;

        /// <summary>
        /// Specifies the vertical resolution, in pixels per meter, of the target device for
        /// the bitmap.
        /// </summary>
        public int YPelsPerMeter;

        /// <summary>
        /// Specifies the number of color indices in the color table that are actually used by
        /// the bitmap.
        /// </summary>
        public uint ClrUsed;

        /// <summary>
        /// Specifies the number of color indices that are considered important for displaying
        /// the bitmap. If this value is zero, all colors are important.
        /// </summary>
        public uint ClrImportant;
    }
}
