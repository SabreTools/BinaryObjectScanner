using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Entries
{
    /// <summary>
    /// Each import directory entry has the following format
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#import-directory-table</remarks>
    public class ImportDirectoryTableEntry
    {
        /// <summary>
        /// The RVA of the import lookup table.
        /// This table contains a name or ordinal for each import.
        /// (The name "Characteristics" is used in Winnt.h, but no longer describes this field.)
        /// </summary>
        public uint ImportLookupTableRVA;

        /// <summary>
        /// The stamp that is set to zero until the image is bound.
        /// After the image is bound, this field is set to the time/data stamp of the DLL.
        /// </summary>
        public uint TimeDateStamp;

        /// <summary>
        /// The index of the first forwarder reference.
        /// </summary>
        public uint ForwarderChain;

        /// <summary>
        /// The address of an ASCII string that contains the name of the DLL.
        /// This address is relative to the image base.
        /// </summary>
        public uint NameRVA;

        /// <summary>
        /// The RVA of the import address table.
        /// The contents of this table are identical to the contents of the import lookup table until the image is bound.
        /// </summary>
        public uint ImportAddressTableRVA;

        /// <summary>
        /// Determine if the entry is null or not
        /// This indicates the last entry in a table
        /// </summary>
        public bool IsNull()
        {
            return ImportLookupTableRVA == 0
                && TimeDateStamp == 0
                && ForwarderChain == 0
                && NameRVA == 0
                && ImportAddressTableRVA == 0;
        }

        public static ImportDirectoryTableEntry Deserialize(Stream stream)
        {
            var idte = new ImportDirectoryTableEntry();

            idte.ImportLookupTableRVA = stream.ReadUInt32();
            idte.TimeDateStamp = stream.ReadUInt32();
            idte.ForwarderChain = stream.ReadUInt32();
            idte.NameRVA = stream.ReadUInt32();
            idte.ImportAddressTableRVA = stream.ReadUInt32();

            return idte;
        }

        public static ImportDirectoryTableEntry Deserialize(byte[] content, ref int offset)
        {
            var idte = new ImportDirectoryTableEntry();

            idte.ImportLookupTableRVA = BitConverter.ToUInt32(content, offset); offset += 4;
            idte.TimeDateStamp = BitConverter.ToUInt32(content, offset); offset += 4;
            idte.ForwarderChain = BitConverter.ToUInt32(content, offset); offset += 4;
            idte.NameRVA = BitConverter.ToUInt32(content, offset); offset += 4;
            idte.ImportAddressTableRVA = BitConverter.ToUInt32(content, offset); offset += 4;

            return idte;
        }
    }
}