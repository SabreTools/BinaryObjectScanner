namespace BinaryObjectScanner.Models.PortableExecutable
{
    /// <summary>
    /// Contains information about an individual font in a font resource group. The structure definition
    /// provided here is for explanation only; it is not present in any standard header file.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/menurc/fontdirentry"/>
    public sealed class FontDirEntry
    {
        /// <summary>
        /// A user-defined version number for the resource data that tools can use to read and write
        /// resource files.
        /// </summary>
        public ushort Version;

        /// <summary>
        /// The size of the file, in bytes.
        /// </summary>
        public uint Size;

        /// <summary>
        /// The font supplier's copyright information.
        /// </summary>
        /// <remarks>60 characters</remarks>
        public byte[] Copyright;

        /// <summary>
        /// The type of font file.
        /// </summary>
        public ushort Type;

        /// <summary>
        /// The point size at which this character set looks best.
        /// </summary>
        public ushort Points;

        /// <summary>
        /// The vertical resolution, in dots per inch, at which this character set was digitized.
        /// </summary>
        public ushort VertRes;

        /// <summary>
        /// The horizontal resolution, in dots per inch, at which this character set was digitized.
        /// </summary>
        public ushort HorizRes;

        /// <summary>
        /// The distance from the top of a character definition cell to the baseline of the typographical
        /// font.
        /// </summary>
        public ushort Ascent;

        /// <summary>
        /// The amount of leading inside the bounds set by the PixHeight member. Accent marks and other
        /// diacritical characters can occur in this area.
        /// </summary>
        public ushort InternalLeading;

        /// <summary>
        /// The amount of extra leading that the application adds between rows.
        /// </summary>
        public ushort ExternalLeading;

        /// <summary>
        /// An italic font if not equal to zero.
        /// </summary>
        public byte Italic;

        /// <summary>
        /// An underlined font if not equal to zero.
        /// </summary>
        public byte Underline;

        /// <summary>
        /// A strikeout font if not equal to zero.
        /// </summary>
        public byte StrikeOut;

        /// <summary>
        /// The weight of the font in the range 0 through 1000. For example, 400 is roman and 700 is bold.
        /// If this value is zero, a default weight is used. For additional defined values, see the
        /// description of the LOGFONT structure.
        /// </summary>
        public ushort Weight;

        /// <summary>
        /// The character set of the font. For predefined values, see the description of the LOGFONT
        /// structure.
        /// </summary>
        public byte CharSet;

        /// <summary>
        /// The width of the grid on which a vector font was digitized. For raster fonts, if the member
        /// is not equal to zero, it represents the width for all the characters in the bitmap. If the
        /// member is equal to zero, the font has variable-width characters.
        /// </summary>
        public ushort PixWidth;

        /// <summary>
        /// The height of the character bitmap for raster fonts or the height of the grid on which a
        /// vector font was digitized.
        /// </summary>
        public ushort PixHeight;

        /// <summary>
        /// The pitch and the family of the font. For additional information, see the description of
        /// the LOGFONT structure.
        /// </summary>
        public byte PitchAndFamily;

        /// <summary>
        /// The average width of characters in the font (generally defined as the width of the letter x).
        /// This value does not include the overhang required for bold or italic characters.
        /// </summary>
        public ushort AvgWidth;

        /// <summary>
        /// The width of the widest character in the font.
        /// </summary>
        public ushort MaxWidth;

        /// <summary>
        /// The first character code defined in the font.
        /// </summary>
        public byte FirstChar;

        /// <summary>
        /// The last character code defined in the font.
        /// </summary>
        public byte LastChar;

        /// <summary>
        /// The character to substitute for characters not in the font.
        /// </summary>
        public byte DefaultChar;

        /// <summary>
        /// The character that will be used to define word breaks for text justification.
        /// </summary>
        public byte BreakChar;

        /// <summary>
        /// The number of bytes in each row of the bitmap. This value is always even so that the rows
        /// start on word boundaries. For vector fonts, this member has no meaning.
        /// </summary>
        public ushort WidthBytes;

        /// <summary>
        /// The offset in the file to a null-terminated string that specifies a device name. For a
        /// generic font, this value is zero.
        /// </summary>
        public uint Device;

        /// <summary>
        /// The offset in the file to a null-terminated string that names the typeface.
        /// </summary>
        public uint Face;

        /// <summary>
        /// This member is reserved.
        /// </summary>
        public uint Reserved;

        /// <summary>
        /// The name of the device if this font file is designated for a specific device.
        /// </summary>
        public string DeviceName;

        /// <summary>
        /// The typeface name of the font.
        /// </summary>
        public string FaceName;
    }
}
