using System.Runtime.InteropServices;

namespace BinaryObjectScanner.Models.MicrosoftCabinet
{
    /// <summary>
    /// Each CFFILE structure contains information about one of the files stored (or at least partially
    /// stored) in this cabinet, as shown in the following packet diagram.The first CFFILE structure entry in
    /// each cabinet is found at the absolute offset CFHEADER.coffFiles field. CFHEADER.cFiles field
    /// indicates how many of these entries are in the cabinet. The CFFILE structure entries in a cabinet
    /// are ordered by iFolder field value, and then by the uoffFolderStart field value.Entries for files
    /// continued from the previous cabinet will be first, and entries for files continued to the next cabinet
    /// will be last.
    /// </summary>
    /// <see href="http://download.microsoft.com/download/5/0/1/501ED102-E53F-4CE0-AA6B-B0F93629DDC6/Exchange/%5BMS-CAB%5D.pdf"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class CFFILE
    {
        /// <summary>
        /// Specifies the uncompressed size of this file, in bytes.
        /// </summary>
        public uint FileSize;

        /// <summary>
        /// Specifies the uncompressed offset, in bytes, of the start of this file's data. For the
        /// first file in each folder, this value will usually be zero. Subsequent files in the folder will have offsets
        /// that are typically the running sum of the cbFile field values.
        /// </summary>
        public uint FolderStartOffset;

        /// <summary>
        /// Index of the folder that contains this file's data.
        /// </summary>
        public FolderIndex FolderIndex;

        /// <summary>
        /// Date of this file, in the format ((year–1980) << 9)+(month << 5)+(day), where
        /// month={1..12} and day = { 1..31 }. This "date" is typically considered the "last modified" date in local
        /// time, but the actual definition is application-defined.
        /// </summary>
        public ushort Date;

        /// <summary>
        /// Time of this file, in the format (hour << 11)+(minute << 5)+(seconds/2), where
        /// hour={0..23}. This "time" is typically considered the "last modified" time in local time, but the
        /// actual definition is application-defined.
        /// </summary>
        public ushort Time;

        /// <summary>
        /// Attributes of this file; can be used in any combination.
        /// </summary>
        public FileAttributes Attributes;

        /// <summary>
        /// The null-terminated name of this file. Note that this string can include path
        /// separator characters.The string can contain up to 256 bytes, plus the null byte. When the
        /// _A_NAME_IS_UTF attribute is set, this string can be converted directly to Unicode, avoiding
        /// locale-specific dependencies. When the _A_NAME_IS_UTF attribute is not set, this string is subject
        /// to interpretation depending on locale. When a string that contains Unicode characters larger than
        /// 0x007F is encoded in the szName field, the _A_NAME_IS_UTF attribute SHOULD be included in
        /// the file's attributes. When no characters larger than 0x007F are in the name, the
        /// _A_NAME_IS_UTF attribute SHOULD NOT be set. If byte values larger than 0x7F are found in
        /// CFFILE.szName field, but the _A_NAME_IS_UTF attribute is not set, the characters SHOULD be
        /// interpreted according to the current location.
        /// </summary>
        public string Name;
    }
}
