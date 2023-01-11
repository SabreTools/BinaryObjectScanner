using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.Models.LinearExecutable;
using BurnOutSharp.Utilities;
using static BurnOutSharp.Models.LinearExecutable.Constants;

namespace BurnOutSharp.Builders
{
    public static class LinearExecutable
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a Linear Executable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled executable on success, null on error</returns>
        public static Executable ParseExecutable(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseExecutable(dataStream);
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a Linear Executable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled executable on success, null on error</returns>
        public static Executable ParseExecutable(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new executable to fill
            var executable = new Executable();

            #region MS-DOS Stub

            // Parse the MS-DOS stub
            var stub = MSDOS.ParseExecutable(data);
            if (stub?.Header == null || stub.Header.NewExeHeaderAddr == 0)
                return null;

            // Set the MS-DOS stub
            executable.Stub = stub;

            #endregion

            #region Information Block

            // Try to parse the executable header
            data.Seek(initialOffset + stub.Header.NewExeHeaderAddr, SeekOrigin.Begin);
            var informationBlock = ParseInformationBlock(data);
            if (informationBlock == null)
                return null;

            // Set the executable header
            executable.InformationBlock = informationBlock;

            #endregion

            #region Object Table

            // Get the object table offset
            long objectTableOffset = informationBlock.ObjectTableOffset + stub.Header.NewExeHeaderAddr;
            if (objectTableOffset < 0 || objectTableOffset >= data.Length)
                return null;

            // Seek to the object table
            data.Seek(objectTableOffset, SeekOrigin.Begin);

            // Create the object table
            executable.ObjectTable = new ObjectTableEntry[informationBlock.ObjectTableCount];

            // Try to parse the object table
            for (int i = 0; i < executable.ObjectTable.Length; i++)
            {
                var objectTableEntry = ParseObjectTableEntry(data);
                if (objectTableEntry == null)
                    return null;

                executable.ObjectTable[i] = objectTableEntry;
            }

            #endregion

            // TODO: Implement LE/LX parsing

            return executable;
        }

        /// <summary>
        /// Parse a Stream into an information block
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled information block on success, null on error</returns>
        private static InformationBlock ParseInformationBlock(Stream data)
        {
            // TODO: Use marshalling here instead of building
            InformationBlock informationBlock = new InformationBlock();

            byte[] magic = data.ReadBytes(2);
            informationBlock.Signature = Encoding.ASCII.GetString(magic);
            if (informationBlock.Signature != LESignatureString && informationBlock.Signature != LXSignatureString)
                return null;

            informationBlock.ByteOrder = (ByteOrder)data.ReadByteValue();
            informationBlock.WordOrder = (WordOrder)data.ReadByteValue();
            informationBlock.ExecutableFormatLevel = data.ReadUInt32();
            informationBlock.CPUType = (CPUType)data.ReadUInt16();
            informationBlock.ModuleOS = (OperatingSystem)data.ReadUInt16();
            informationBlock.ModuleVersion = data.ReadUInt32();
            informationBlock.ModuleTypeFlags = (ModuleFlags)data.ReadUInt32();
            informationBlock.ModuleNumberPages = data.ReadUInt32();
            informationBlock.InitialObjectCS = data.ReadUInt32();
            informationBlock.InitialEIP = data.ReadUInt32();
            informationBlock.InitialObjectSS = data.ReadUInt32();
            informationBlock.InitialESP = data.ReadUInt32();
            informationBlock.MemoryPageSize = data.ReadUInt32();
            informationBlock.BytesOnLastPage = data.ReadUInt32();
            informationBlock.FixupSectionSize = data.ReadUInt32();
            informationBlock.FixupSectionChecksum = data.ReadUInt32();
            informationBlock.LoaderSectionSize = data.ReadUInt32();
            informationBlock.LoaderSectionChecksum = data.ReadUInt32();
            informationBlock.ObjectTableOffset = data.ReadUInt32();
            informationBlock.ObjectTableCount = data.ReadUInt32();
            informationBlock.ObjectPageMapOffset = data.ReadUInt32();
            informationBlock.ObjectIterateDataMapOffset = data.ReadUInt32();
            informationBlock.ResourceTableOffset = data.ReadUInt32();
            informationBlock.ResourceTableCount = data.ReadUInt32();
            informationBlock.ResidentNamesTableOffset = data.ReadUInt32();
            informationBlock.EntryTableOffset = data.ReadUInt32();
            informationBlock.ModuleDirectivesTableOffset = data.ReadUInt32();
            informationBlock.ModuleDirectivesCount = data.ReadUInt32();
            informationBlock.FixupPageTableOffset = data.ReadUInt32();
            informationBlock.FixupRecordTableOffset = data.ReadUInt32();
            informationBlock.ImportedModulesNameTableOffset = data.ReadUInt32();
            informationBlock.ImportedModulesCount = data.ReadUInt32();
            informationBlock.ImportProcedureNameTableOffset = data.ReadUInt32();
            informationBlock.PerPageChecksumTableOffset = data.ReadUInt32();
            informationBlock.DataPagesOffset = data.ReadUInt32();
            informationBlock.PreloadPageCount = data.ReadUInt32();
            informationBlock.NonResidentNamesTableOffset = data.ReadUInt32();
            informationBlock.NonResidentNamesTableLength = data.ReadUInt32();
            informationBlock.NonResidentNamesTableChecksum = data.ReadUInt32();
            informationBlock.AutomaticDataObject = data.ReadUInt32();
            informationBlock.DebugInformationOffset = data.ReadUInt32();
            informationBlock.DebugInformationLength = data.ReadUInt32();
            informationBlock.PreloadInstancePagesNumber = data.ReadUInt32();
            informationBlock.DemandInstancePagesNumber = data.ReadUInt32();
            informationBlock.ExtraHeapAllocation = data.ReadUInt32();

            return informationBlock;
        }

        /// <summary>
        /// Parse a Stream into an object table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled object table entry on success, null on error</returns>
        private static ObjectTableEntry ParseObjectTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ObjectTableEntry objectTableEntry = new ObjectTableEntry();

            objectTableEntry.VirtualSegmentSize = data.ReadUInt32();
            objectTableEntry.RelocationBaseAddress = data.ReadUInt32();
            objectTableEntry.ObjectFlags = (ObjectFlags)data.ReadUInt16();
            objectTableEntry.PageTableIndex = data.ReadUInt32();
            objectTableEntry.PageTableEntries = data.ReadUInt32();
            objectTableEntry.Reserved = data.ReadUInt32();

            return objectTableEntry;
        }

        #endregion
    }
}