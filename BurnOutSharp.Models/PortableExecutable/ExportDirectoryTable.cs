using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// The export symbol information begins with the export directory table,
    /// which describes the remainder of the export symbol information. The
    /// export directory table contains address information that is used to resolve
    /// imports to the entry points within this image.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    [StructLayout(LayoutKind.Sequential)]
    public class ExportDirectoryTable
    {
        /// <summary>
        /// Reserved, must be 0.
        /// </summary>
        public uint ExportFlags;

        /// <summary>
        /// The time and date that the export data was created.
        /// </summary>
        public uint TimeDateStamp;

        /// <summary>
        /// The major version number. The major and minor version numbers can be set
        /// by the user.
        /// </summary>
        public ushort MajorVersion;

        /// <summary>
        /// The minor version number.
        /// </summary>
        public ushort MinorVersion;

        /// <summary>
        /// The address of the ASCII string that contains the name of the DLL. This
        /// address is relative to the image base.
        /// </summary>
        public uint NameRVA;

        /// <summary>
        /// The starting ordinal number for exports in this image. This field specifies
        /// the starting ordinal number for the export address table. It is usually set
        /// to 1.
        /// </summary>
        public uint OrdinalBase;

        /// <summary>
        /// The number of entries in the export address table.
        /// </summary>
        public uint AddressTableEntries;

        /// <summary>
        /// The number of entries in the name pointer table. This is also the number of
        /// entries in the ordinal table.
        /// </summary>
        public uint NumberOfNamePointers;

        /// <summary>
        /// The address of the export address table, relative to the image base.
        /// </summary>
        public uint ExportAddressTableRVA;

        /// <summary>
        /// The address of the export name pointer table, relative to the image base.
        /// The table size is given by the Number of Name Pointers field.
        /// </summary>
        public uint NamePointerRVA;

        /// <summary>
        /// The address of the ordinal table, relative to the image base.
        /// </summary>
        public uint OrdinalTableRVA;
    }
}
