using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// Contains version information for a file. This information is language and
    /// code page independent.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/verrsrc/ns-verrsrc-vs_fixedfileinfo"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class FixedFileInfo
    {
        /// <summary>
        /// Contains the value 0xFEEF04BD. This is used with the szKey member of the VS_VERSIONINFO
        /// structure when searching a file for the FixedFileInfo structure.
        /// </summary>
        public uint Signature;

        /// <summary>
        /// The binary version number of this structure. The high-order word of this member contains
        /// the major version number, and the low-order word contains the minor version number.
        /// </summary>
        public uint StrucVersion;

        /// <summary>
        /// The most significant 32 bits of the file's binary version number. This member is used with
        /// FileVersionLS to form a 64-bit value used for numeric comparisons.
        /// </summary>
        public uint FileVersionMS;

        /// <summary>
        /// The least significant 32 bits of the file's binary version number. This member is used with
        /// FileVersionMS to form a 64-bit value used for numeric comparisons.
        /// </summary>
        public uint FileVersionLS;

        /// <summary>
        /// The most significant 32 bits of the binary version number of the product with which this file
        /// was distributed. This member is used with ProductVersionLS to form a 64-bit value used for
        /// numeric comparisons.
        /// </summary>
        public uint ProductVersionMS;

        /// <summary>
        /// The least significant 32 bits of the binary version number of the product with which this file
        /// was distributed. This member is used with ProductVersionMS to form a 64-bit value used for
        /// numeric comparisons.
        /// </summary>
        public uint ProductVersionLS;

        /// <summary>
        /// Contains a bitmask that specifies the valid bits in FileFlags. A bit is valid only if it was
        /// defined when the file was created.
        /// </summary>
        public uint FileFlagsMask;

        /// <summary>
        /// Contains a bitmask that specifies the Boolean attributes of the file.
        /// </summary>
        public FixedFileInfoFlags FileFlags;

        /// <summary>
        /// The operating system for which this file was designed.
        /// </summary>
        public FixedFileInfoOS FileOS;

        /// <summary>
        /// The general type of file.
        /// </summary>
        public FixedFileInfoFileType FileType;

        /// <summary>
        /// The function of the file. The possible values depend on the value of FileType. For all values
        /// of FileType not described in the following list, FileSubtype is zero.
        /// </summary>
        public FixedFileInfoFileSubtype FileSubtype;

        /// <summary>
        /// The most significant 32 bits of the file's 64-bit binary creation date and time stamp.
        /// </summary>
        public uint FileDateMS;

        /// <summary>
        /// The least significant 32 bits of the file's 64-bit binary creation date and time stamp.
        /// </summary>
        public uint FileDateLS;
    }
}
