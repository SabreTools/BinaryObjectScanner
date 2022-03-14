using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.PE.Tables
{
    /// <summary>
    /// The export symbol information begins with the export directory table, which describes the remainder of the export symbol information.
    /// The export directory table contains address information that is used to resolve imports to the entry points within this image.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#export-directory-table</remarks>
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
        /// The major version number. The major and minor version numbers can be set by the user.
        /// </summary>
        public ushort MajorVersion;

        /// <summary>
        /// The minor version number.
        /// </summary>
        public ushort MinorVersion;

        /// <summary>
        /// The address of the ASCII string that contains the name of the DLL.
        /// This address is relative to the image base.
        /// </summary>
        public uint NameRVA; // TODO: Read this into a separate field

        /// <summary>
        /// The starting ordinal number for exports in this image.
        /// This field specifies the starting ordinal number for the export address table.
        /// It is usually set to 1.
        /// </summary>
        public uint OrdinalBase;

        /// <summary>
        /// The number of entries in the export address table.
        /// </summary>
        public uint AddressTableEntries;

        /// <summary>
        /// The number of entries in the name pointer table.
        /// This is also the number of entries in the ordinal table.
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

        public static ExportDirectoryTable Deserialize(Stream stream)
        {
            var edt = new ExportDirectoryTable();

            edt.ExportFlags = stream.ReadUInt32();
            edt.TimeDateStamp = stream.ReadUInt32();
            edt.MajorVersion = stream.ReadUInt16();
            edt.MinorVersion = stream.ReadUInt16();
            edt.NameRVA = stream.ReadUInt32();
            edt.OrdinalBase = stream.ReadUInt32();
            edt.AddressTableEntries = stream.ReadUInt32();
            edt.NumberOfNamePointers = stream.ReadUInt32();
            edt.ExportAddressTableRVA = stream.ReadUInt32();
            edt.NamePointerRVA = stream.ReadUInt32();
            edt.OrdinalTableRVA = stream.ReadUInt32();

            return edt;
        }

        public static ExportDirectoryTable Deserialize(byte[] content, ref int offset)
        {
            var edt = new ExportDirectoryTable();

            edt.ExportFlags = content.ReadUInt32(ref offset);
            edt.TimeDateStamp = content.ReadUInt32(ref offset);
            edt.MajorVersion = content.ReadUInt16(ref offset);
            edt.MinorVersion = content.ReadUInt16(ref offset);
            edt.NameRVA = content.ReadUInt32(ref offset);
            edt.OrdinalBase = content.ReadUInt32(ref offset);
            edt.AddressTableEntries = content.ReadUInt32(ref offset);
            edt.NumberOfNamePointers = content.ReadUInt32(ref offset);
            edt.ExportAddressTableRVA = content.ReadUInt32(ref offset);
            edt.NamePointerRVA = content.ReadUInt32(ref offset);
            edt.OrdinalTableRVA = content.ReadUInt32(ref offset);

            return edt;
        }
    }
}