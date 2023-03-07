using System.Runtime.InteropServices;

namespace BinaryObjectScanner.Models.MicrosoftCabinet
{
    /// <summary>
    /// The CFHEADER structure shown in the following packet diagram provides information about this
    /// cabinet (.cab) file.
    /// </summary>
    /// <see href="http://download.microsoft.com/download/5/0/1/501ED102-E53F-4CE0-AA6B-B0F93629DDC6/Exchange/%5BMS-CAB%5D.pdf"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class CFHEADER
    {
        /// <summary>
        /// Contains the characters "M", "S", "C", and "F" (bytes 0x4D, 0x53, 0x43,
        /// 0x46). This field is used to ensure that the file is a cabinet (.cab) file.
        /// </summary>
        public string Signature;

        /// <summary>
        /// Reserved field; MUST be set to 0 (zero).
        /// </summary>
        public uint Reserved1;

        /// <summary>
        /// Specifies the total size of the cabinet file, in bytes.
        /// </summary>
        public uint CabinetSize;

        /// <summary>
        /// Reserved field; MUST be set to 0 (zero).
        /// </summary>
        public uint Reserved2;

        /// <summary>
        /// Specifies the absolute file offset, in bytes, of the first CFFILE field entry.
        /// </summary>
        public uint FilesOffset;

        /// <summary>
        /// Reserved field; MUST be set to 0 (zero).
        /// </summary>
        public uint Reserved3;

        /// <summary>
        /// Specifies the minor cabinet file format version. This value MUST be set to 3 (three).
        /// </summary>
        public byte VersionMinor;

        /// <summary>
        /// Specifies the major cabinet file format version. This value MUST be set to 1 (one).
        /// </summary>
        public byte VersionMajor;

        /// <summary>
        /// Specifies the number of CFFOLDER field entries in this cabinet file.
        /// </summary>
        public ushort FolderCount;

        /// <summary>
        /// Specifies the number of CFFILE field entries in this cabinet file.
        /// </summary>
        public ushort FileCount;

        /// <summary>
        /// Specifies bit-mapped values that indicate the presence of optional data.
        /// </summary>
        public HeaderFlags Flags;

        /// <summary>
        /// Specifies an arbitrarily derived (random) value that binds a collection of linked cabinet files
        /// together.All cabinet files in a set will contain the same setID field value.This field is used by
        /// cabinet file extractors to ensure that cabinet files are not inadvertently mixed.This value has no
        /// meaning in a cabinet file that is not in a set.
        /// </summary>
        public ushort SetID;

        /// <summary>
        /// Specifies the sequential number of this cabinet in a multicabinet set. The first cabinet has
        /// iCabinet=0. This field, along with the setID field, is used by cabinet file extractors to ensure that
        /// this cabinet is the correct continuation cabinet when spanning cabinet files.
        /// </summary>
        public ushort CabinetIndex;

        /// <summary>
        /// If the flags.cfhdrRESERVE_PRESENT field is not set, this field is not
        /// present, and the value of cbCFHeader field MUST be zero.Indicates the size, in bytes, of the
        /// abReserve field in this CFHEADER structure.Values for cbCFHeader field MUST be between 0-
        /// 60,000.
        /// </summary>
        public ushort HeaderReservedSize;

        /// <summary>
        /// If the flags.cfhdrRESERVE_PRESENT field is not set, this field is not
        /// present, and the value of cbCFFolder field MUST be zero.Indicates the size, in bytes, of the
        /// abReserve field in each CFFOLDER field entry.Values for fhe cbCFFolder field MUST be between
        /// 0-255.
        /// </summary>
        public byte FolderReservedSize;

        /// <summary>
        /// If the flags.cfhdrRESERVE_PRESENT field is not set, this field is not
        /// present, and the value for the cbCFDATA field MUST be zero.The cbCFDATA field indicates the
        /// size, in bytes, of the abReserve field in each CFDATA field entry. Values for the cbCFDATA field
        /// MUST be between 0 - 255.
        /// </summary>
        public byte DataReservedSize;

        /// <summary>
        /// If the flags.cfhdrRESERVE_PRESENT field is set and the
        /// cbCFHeader field is non-zero, this field contains per-cabinet-file application information. This field
        /// is defined by the application, and is used for application-defined purposes.
        /// </summary>
        public byte[] ReservedData;

        /// <summary>
        /// If the flags.cfhdrPREV_CABINET field is not set, this
        /// field is not present.This is a null-terminated ASCII string that contains the file name of the
        /// logically previous cabinet file. The string can contain up to 255 bytes, plus the null byte. Note that
        /// this gives the name of the most recently preceding cabinet file that contains the initial instance of a
        /// file entry.This might not be the immediately previous cabinet file, when the most recent file spans
        /// multiple cabinet files.If searching in reverse for a specific file entry, or trying to extract a file that is
        /// reported to begin in the "previous cabinet," the szCabinetPrev field would indicate the name of the
        /// cabinet to examine.
        /// </summary>
        public string CabinetPrev;

        /// <summary>
        /// If the flags.cfhdrPREV_CABINET field is not set, then this
        /// field is not present.This is a null-terminated ASCII string that contains a descriptive name for the
        /// media that contains the file named in the szCabinetPrev field, such as the text on the disk label.
        /// This string can be used when prompting the user to insert a disk. The string can contain up to 255
        /// bytes, plus the null byte.
        /// </summary>
        public string DiskPrev;

        /// <summary>
        /// If the flags.cfhdrNEXT_CABINET field is not set, this
        /// field is not present.This is a null-terminated ASCII string that contains the file name of the next
        /// cabinet file in a set. The string can contain up to 255 bytes, plus the null byte. Files that extend
        /// beyond the end of the current cabinet file are continued in the named cabinet file.
        /// </summary>
        public string CabinetNext;

        /// <summary>
        /// If the flags.cfhdrNEXT_CABINET field is not set, this field is
        /// not present.This is a null-terminated ASCII string that contains a descriptive name for the media
        /// that contains the file named in the szCabinetNext field, such as the text on the disk label. The
        /// string can contain up to 255 bytes, plus the null byte. This string can be used when prompting the
        /// user to insert a disk.
        /// </summary>
        public string DiskNext;
    }
}
