namespace BinaryObjectScanner.Models.CFB
{
    public enum ColorFlag : byte
    {
        Red = 0x00,
        Black = 0x01,
    }

    public enum ObjectType : byte
    {
        Unknown = 0x00,
        StorageObject = 0x01,
        StreamObject = 0x02,
        RootStorageObject = 0x05,
    }

    public enum SectorNumber : uint
    {
        /// <summary>
        /// Regular sector number.
        /// </summary>
        REGSECT = 0x00000000, // 0x00000000 - 0xFFFFFFF9

        /// <summary>
        /// Maximum regular sector number.
        /// </summary>
        MAXREGSECT = 0xFFFFFFFA,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        NotApplicable = 0xFFFFFFFB,

        /// <summary>
        /// Specifies a DIFAT sector in the FAT.
        /// </summary>
        DIFSECT = 0xFFFFFFFC,

        /// <summary>
        /// Specifies a FAT sector in the FAT.
        /// </summary>
        FATSECT = 0xFFFFFFFD,

        /// <summary>
        /// End of a linked chain of sectors.
        /// </summary>
        ENDOFCHAIN = 0xFFFFFFFE,

        /// <summary>
        /// Specifies an unallocated sector in the FAT, Mini FAT, or DIFAT.
        /// </summary>
        FREESECT = 0xFFFFFFFF,
    }

    public enum StreamID : uint
    {
        /// <summary>
        /// Regular stream ID to identify the directory entry.
        /// </summary>
        REGSID = 0x00000000, // 0x00000000 - 0xFFFFFFF9

        /// <summary>
        /// Maximum regular stream ID.
        /// </summary>
        MAXREGSID = 0xFFFFFFFA,

        /// <summary>
        /// Terminator or empty pointer.
        /// </summary>
        NOSTREAM = 0xFFFFFFFF,
    }

    /// <see href="https://learn.microsoft.com/en-us/windows/win32/stg/the-summary-information-property-set"/>
    public enum SummaryInformationProperty : uint
    {
        /// <summary>
        /// Title
        /// </summary>
        PIDSI_TITLE = 0x00000002,

        /// <summary>
        /// Subject
        /// </summary>
        PIDSI_SUBJECT = 0x00000003,

        /// <summary>
        /// Author
        /// </summary>
        PIDSI_AUTHOR = 0x00000004,

        /// <summary>
        /// Keywords
        /// </summary>
        PIDSI_KEYWORDS = 0x00000005,

        /// <summary>
        /// Comments
        /// </summary>
        PIDSI_COMMENTS = 0x00000006,

        /// <summary>
        /// Template
        /// </summary>
        PIDSI_TEMPLATE = 0x00000007,

        /// <summary>
        /// Last Saved By
        /// </summary>
        PIDSI_LASTAUTHOR = 0x00000008,

        /// <summary>
        /// Revision Number
        /// </summary>
        PIDSI_REVNUMBER = 0x00000009,

        /// <summary>
        /// Total Editing Time
        /// </summary>
        PIDSI_EDITTIME = 0x0000000A,

        /// <summary>
        /// Last Printed
        /// </summary>
        PIDSI_LASTPRINTED = 0x0000000B,

        /// <summary>
        /// Create Time/Date
        /// </summary>
        PIDSI_CREATE_DTM = 0x0000000C,

        /// <summary>
        /// Last saved Time/Date
        /// </summary>
        PIDSI_LASTSAVE_DTM = 0x0000000D,

        /// <summary>
        /// Number of Pages
        /// </summary>
        PIDSI_PAGECOUNT = 0x0000000E,

        /// <summary>
        /// Number of Words
        /// </summary>
        PIDSI_WORDCOUNT = 0x0000000F,

        /// <summary>
        /// Number of Characters
        /// </summary>
        PIDSI_CHARCOUNT = 0x00000010,

        /// <summary>
        /// Thumbnail
        /// </summary>
        PIDSI_THUMBNAIL = 0x00000011,

        /// <summary>
        /// Name of Creating Application
        /// </summary>
        PIDSI_APPNAME = 0x00000012,

        /// <summary>
        /// Security
        /// </summary>
        PIDSI_SECURITY = 0x00000013,
    }

    /// <remarks>Also includes the DocumentSummaryInformation set</remarks>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/stg/the-documentsummaryinformation-and-userdefined-property-sets"/>
    public enum UserDefinedProperty : uint
    {
        /// <summary>
        /// Category - A text string typed by the user that indicates what
        /// category the file belongs to (memo, proposal, and so on). It
        /// is useful for finding files of same type.
        /// </summary>
        PIDDSI_CATEGORY = 0x00000002,

        /// <summary>
        /// PresentationTarget - Target format for presentation (35mm,
        /// printer, video, and so on).
        /// </summary>
        PIDDSI_PRESFORMAT = 0x00000003,

        /// <summary>
        /// Bytes - Number of bytes.
        /// </summary>
        PIDDSI_BYTECOUNT = 0x00000004,

        /// <summary>
        /// Lines - Number of lines.
        /// </summary>
        PIDDSI_LINECOUNT = 0x00000005,

        /// <summary>
        /// Paragraphs - Number of paragraphs.
        /// </summary>
        PIDDSI_PARCOUNT = 0x00000006,

        /// <summary>
        /// Slides - Number of slides.
        /// </summary>
        PIDDSI_SLIDECOUNT = 0x00000007,

        /// <summary>
        /// Notes - Number of pages that contain notes.
        /// </summary>
        PIDDSI_NOTECOUNT = 0x00000008,

        /// <summary>
        /// HiddenSlides - Number of slides that are hidden.
        /// </summary>
        PIDDSI_HIDDENCOUNT = 0x00000009,

        /// <summary>
        /// MMClips - Number of sound or video clips.
        /// </summary>
        PIDDSI_MMCLIPCOUNT = 0x0000000A,

        /// <summary>
        /// ScaleCrop - Set to True (-1) when scaling of the thumbnail

        /// is desired. If not set, cropping is desired.
        /// </summary>
        PIDDSI_SCALE = 0x0000000B,

        /// <summary>
        /// HeadingPairs - Internally used property indicating the
        /// grouping of different document parts and the number of
        /// items in each group. The titles of the document parts are
        /// stored in the TitlesofParts property. The HeadingPairs
        /// property is stored as a vector of variants, in repeating
        /// pairs of VT_LPSTR (or VT_LPWSTR) and VT_I4 values. The
        /// VT_LPSTR value represents a heading name, and the VT_I4
        /// value indicates the count of document parts under that heading.
        /// </summary>
        PIDDSI_HEADINGPAIR = 0x0000000C,

        /// <summary>
        /// TitlesofParts - Names of document parts.
        /// </summary>
        PIDDSI_DOCPARTS = 0x0000000D,

        /// <summary>
        /// Manager - Manager of the project.
        /// </summary>
        PIDDSI_MANAGER = 0x0000000E,

        /// <summary>
        /// Company - Company name.
        /// </summary>
        PIDDSI_COMPANY = 0x0000000F,

        /// <summary>
        /// LinksUpToDate - Boolean value to indicate whether the custom
        /// links are hampered by excessive noise, for all applications.
        /// </summary>
        PIDDSI_LINKSDIRTY = 0x00000010,
    }

    /// <see href="https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-oaut/3fe7db9f-5803-4dc4-9d14-5425d3f5461f"/>
    public enum VariantType : ushort
    {
        /// <summary>
        /// The type of the contained field is undefined. When this flag is
        /// specified, the VARIANT MUST NOT contain a data field.
        /// </summary>
        VT_EMPTY = 0x0000,

        /// <summary>
        /// The type of the contained field is NULL. When this flag is
        /// specified, the VARIANT MUST NOT contain a data field.
        /// </summary>
        VT_NULL = 0x0001,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be a 2-byte signed integer.
        /// </summary>
        VT_I2 = 0x0002,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be a 4-byte signed integer.
        /// </summary>
        VT_I4 = 0x0003,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be a 4-byte IEEE floating-point number.
        /// </summary>
        VT_R4 = 0x0004,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be an 8-byte IEEE floating-point number.
        /// </summary>
        VT_R8 = 0x0005,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be CURRENCY.
        /// </summary>
        VT_CY = 0x0006,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be DATE.
        /// </summary>
        VT_DATE = 0x0007,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be BSTR.
        /// </summary>
        VT_BSTR = 0x0008,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be a pointer to IDispatch.
        /// </summary>
        VT_DISPATCH = 0x0009,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be HRESULT.
        /// </summary>
        VT_ERROR = 0x000A,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be VARIANT_BOOL.
        /// </summary>
        VT_BOOL = 0x000B,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be VARIANT. It MUST appear with the bit flag VT_BYREF.
        /// </summary>
        VT_VARIANT = 0x000C,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be a pointer to IUnknown.
        /// </summary>
        VT_UNKNOWN = 0x000D,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be DECIMAL.
        /// </summary>
        VT_DECIMAL = 0x000E,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be a 1-byte integer.
        /// </summary>
        VT_I1 = 0x0010,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be a 1-byte unsigned integer.
        /// </summary>
        VT_UI1 = 0x0011,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be a 2-byte unsigned integer.
        /// </summary>
        VT_UI2 = 0x0012,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be a 4-byte unsigned integer.
        /// </summary>
        VT_UI4 = 0x0013,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be an 8-byte signed integer.
        /// </summary>
        VT_I8 = 0x0014,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be an 8-byte unsigned integer.
        /// </summary>
        VT_UI8 = 0x0015,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be a 4-byte signed integer.
        /// </summary>
        VT_INT = 0x0016,

        /// <summary>
        /// Either the specified type, or the type of the element or contained
        /// field MUST be a 4-byte unsigned integer.
        /// </summary>
        VT_UINT = 0x0017,

        /// <summary>
        /// The specified type MUST be void.
        /// </summary>
        VT_VOID = 0x0018,

        /// <summary>
        /// The specified type MUST be HRESULT.
        /// </summary>
        VT_HRESULT = 0x0019,

        /// <summary>
        /// The specified type MUST be a unique pointer.
        /// </summary>
        VT_PTR = 0x001A,

        /// <summary>
        /// The specified type MUST be SAFEARRAY.
        /// </summary>
        VT_SAFEARRAY = 0x001B,

        /// <summary>
        /// The specified type MUST be a fixed-size array.
        /// </summary>
        VT_CARRAY = 0x001C,

        /// <summary>
        /// The specified type MUST be user defined.
        /// </summary>
        VT_USERDEFINED = 0x001D,

        /// <summary>
        /// The specified type MUST be a NULL-terminated string.
        /// </summary>
        VT_LPSTR = 0x001E,

        /// <summary>
        /// The specified type MUST be a zero-terminated string of
        /// UNICODE characters.
        /// </summary>
        VT_LPWSTR = 0x001F,

        /// <summary>
        /// The type of the element or contained field MUST be a BRECORD.
        /// </summary>
        VT_RECORD = 0x0024,

        /// <summary>
        /// The specified type MUST be either a 4-byte or an 8-byte signed
        /// integer. The size of the integer is platform specific and
        /// determines the system pointer size value.
        /// </summary>
        VT_INT_PTR = 0x0025,

        /// <summary>
        /// The specified type MUST be either a 4 byte or an 8 byte unsigned
        /// integer. The size of the integer is platform specific and
        /// determines the system pointer size value
        /// </summary>
        VT_UINT_PTR = 0x0026,

        /// <summary>
        /// The type of the element or contained field MUST be a SAFEARRAY.
        /// </summary>
        VT_ARRAY = 0x2000,

        /// <summary>
        /// The type of the element or contained field MUST be a pointer to
        /// one of the types listed in the previous rows of this table. If
        /// present, this bit flag MUST appear in a VARIANT discriminant
        /// with one of the previous flags.
        /// </summary>
        VT_BYREF = 0x4000
    }
}