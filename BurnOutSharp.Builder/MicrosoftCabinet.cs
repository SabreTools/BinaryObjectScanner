using System.Collections.Generic;
using System.IO;
using System.Text;
using BurnOutSharp.Models.MicrosoftCabinet;
using BurnOutSharp.Utilities;

namespace BurnOutSharp.Builder
{
    // TODO: Add multi-cabinet reading
    // TODO: Make Stream Data rely on Byte Data
    public class MicrosoftCabinet
    {
        #region Constants

        /// <summary>
        /// Human-readable signature
        /// </summary>
        public static readonly string SignatureString = "MSCF";

        /// <summary>
        /// Signature as an unsigned Int32 value
        /// </summary>
        public const uint SignatureValue = 0x4643534D;

        /// <summary>
        /// Signature as a byte array
        /// </summary>
        public static readonly byte[] SignatureBytes = new byte[] { 0x4D, 0x53, 0x43, 0x46 };

        /// <summary>
        /// A maximum uncompressed size of an input file to store in CAB
        /// </summary>
        public const uint MaximumUncompressedFileSize = 0x7FFF8000;

        /// <summary>
        /// A maximum file COUNT
        /// </summary>
        public const ushort MaximumFileCount = 0xFFFF;

        /// <summary>
        /// A maximum size of a created CAB (compressed)
        /// </summary>
        public const uint MaximumCabSize = 0x7FFFFFFF;

        /// <summary>
        /// A maximum CAB-folder COUNT
        /// </summary>
        public const ushort MaximumFolderCount = 0xFFFF;

        /// <summary>
        /// A maximum uncompressed data size in a CAB-folder
        /// </summary>
        public const uint MaximumUncompressedFolderSize = 0x7FFF8000;

        #endregion

        #region Byte Data

        /// <summary>
        /// Parse a byte array into a Microsoft Cabinet file
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled cabinet on success, null on error</returns>
        public static Cabinet ParseCabinet(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = offset;

            // Create a new cabinet to fill
            var cabinet = new Cabinet();

            #region Cabinet Header

            // Try to parse the cabinet header
            var cabinetHeader = ParseCabinetHeader(data, ref offset);
            if (cabinetHeader == null)
                return null;

            // Set the cabinet header
            cabinet.Header = cabinetHeader;

            #endregion

            #region Folders

            // Set the folder array
            cabinet.Folders = new CFFOLDER[cabinetHeader.FolderCount];

            // Try to parse each folder, if we have any
            for (int i = 0; i < cabinetHeader.FolderCount; i++)
            {
                var folder = ParseFolder(data, ref offset, cabinetHeader, initialOffset);
                if (folder == null)
                    return null;

                // Set the folder
                cabinet.Folders[i] = folder;
            }

            #endregion

            #region Files

            // Get the files offset
            int filesOffset = (int)cabinetHeader.FilesOffset + initialOffset;
            if (filesOffset > data.Length)
                return null;

            // Set the file array
            cabinet.Files = new CFFILE[cabinetHeader.FileCount];

            // Try to parse each file, if we have any
            for (int i = 0; i < cabinetHeader.FileCount; i++)
            {
                var file = ParseFile(data, ref filesOffset);
                if (file == null)
                    return null;

                // Set the file
                cabinet.Files[i] = file;
            }

            #endregion

            return cabinet;
        }

        /// <summary>
        /// Parse a byte array into a cabinet header
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled cabinet header on success, null on error</returns>
        private static CFHEADER ParseCabinetHeader(byte[] data, ref int offset)
        {
            // TODO: Use marshalling here instead of building
            CFHEADER header = new CFHEADER();

            header.Signature = data.ReadUInt32(ref offset);
            if (header.Signature != SignatureValue)
                return null;

            header.Reserved1 = data.ReadUInt32(ref offset);
            if (header.Reserved1 != 0x00000000)
                return null;

            header.CabinetSize = data.ReadUInt32(ref offset);
            if (header.CabinetSize > MaximumCabSize)
                return null;

            header.Reserved2 = data.ReadUInt32(ref offset);
            if (header.Reserved2 != 0x00000000)
                return null;

            header.FilesOffset = data.ReadUInt32(ref offset);

            header.Reserved3 = data.ReadUInt32(ref offset);
            if (header.Reserved3 != 0x00000000)
                return null;

            header.VersionMinor = data.ReadByte(ref offset);
            header.VersionMajor = data.ReadByte(ref offset);
            if (header.VersionMajor != 0x00000001 || header.VersionMinor != 0x00000003)
                return null;

            header.FolderCount = data.ReadUInt16(ref offset);
            if (header.FolderCount > MaximumFolderCount)
                return null;

            header.FileCount = data.ReadUInt16(ref offset);
            if (header.FileCount > MaximumFileCount)
                return null;

            header.Flags = (HeaderFlags)data.ReadUInt16(ref offset);
            header.SetID = data.ReadUInt16(ref offset);
            header.CabinetIndex = data.ReadUInt16(ref offset);

            if (header.Flags.HasFlag(HeaderFlags.RESERVE_PRESENT))
            {
                header.HeaderReservedSize = data.ReadUInt16(ref offset);
                if (header.HeaderReservedSize > 60_000)
                    return null;

                header.FolderReservedSize = data.ReadByte(ref offset);
                header.DataReservedSize = data.ReadByte(ref offset);

                if (header.HeaderReservedSize > 0)
                    header.ReservedData = data.ReadBytes(ref offset, header.HeaderReservedSize);
            }

            if (header.Flags.HasFlag(HeaderFlags.PREV_CABINET))
            {
                header.CabinetPrev = data.ReadString(ref offset, Encoding.ASCII);
                header.DiskPrev = data.ReadString(ref offset, Encoding.ASCII);
            }

            if (header.Flags.HasFlag(HeaderFlags.NEXT_CABINET))
            {
                header.CabinetNext = data.ReadString(ref offset, Encoding.ASCII);
                header.DiskNext = data.ReadString(ref offset, Encoding.ASCII);
            }

            return header;
        }

        /// <summary>
        /// Parse a byte array into a folder
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="header">Cabinet header to get flags and sizes from</param>
        /// <param name="initialOffset">Initial offset for calculations</param>
        /// <returns>Filled folder on success, null on error</returns>
        private static CFFOLDER ParseFolder(byte[] data, ref int offset, CFHEADER header, int initialOffset)
        {
            // TODO: Use marshalling here instead of building
            CFFOLDER folder = new CFFOLDER();

            folder.CabStartOffset = data.ReadUInt32(ref offset);
            folder.DataCount = data.ReadUInt16(ref offset);
            folder.CompressionType = (CompressionType)data.ReadUInt16(ref offset);

            if (header.FolderReservedSize > 0)
                folder.ReservedData = data.ReadBytes(ref offset, header.FolderReservedSize);

            if (folder.CabStartOffset > 0)
            {
                int blockPtr = initialOffset + (int)folder.CabStartOffset;

                folder.DataBlocks = new Dictionary<int, CFDATA>();
                for (int i = 0; i < folder.DataCount; i++)
                {
                    int dataStart = blockPtr;
                    CFDATA dataBlock = ParseDataBlock(data, ref blockPtr, header.DataReservedSize);
                    folder.DataBlocks[dataStart] = dataBlock;
                }
            }

            return folder;
        }

        /// <summary>
        /// Parse a byte array into a data block
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <param name="dataReservedSize">Reserved byte size for data blocks</param>
        /// <returns>Filled folder on success, null on error</returns>
        private static CFDATA ParseDataBlock(byte[] data, ref int offset, byte dataReservedSize)
        {
            // TODO: Use marshalling here instead of building
            CFDATA dataBlock = new CFDATA();

            dataBlock.Checksum = data.ReadUInt32(ref offset);
            dataBlock.CompressedSize = data.ReadUInt16(ref offset);
            dataBlock.UncompressedSize = data.ReadUInt16(ref offset);

            if (dataBlock.UncompressedSize != 0 && dataBlock.CompressedSize > dataBlock.UncompressedSize)
                return null;

            if (dataReservedSize > 0)
                dataBlock.ReservedData = data.ReadBytes(ref offset, dataReservedSize);

            if (dataBlock.CompressedSize > 0)
                dataBlock.CompressedData = data.ReadBytes(ref offset, dataBlock.CompressedSize);

            return dataBlock;
        }

        /// <summary>
        /// Parse a byte array into a file
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled file on success, null on error</returns>
        private static CFFILE ParseFile(byte[] data, ref int offset)
        {
            // TODO: Use marshalling here instead of building
            CFFILE file = new CFFILE();

            file.FileSize = data.ReadUInt32(ref offset);
            file.FolderStartOffset = data.ReadUInt32(ref offset);
            file.FolderIndex = (FolderIndex)data.ReadUInt16(ref offset);
            file.Date = data.ReadUInt16(ref offset);
            file.Time = data.ReadUInt16(ref offset);
            file.Attributes = (Models.MicrosoftCabinet.FileAttributes)data.ReadUInt16(ref offset);

            if (file.Attributes.HasFlag(Models.MicrosoftCabinet.FileAttributes.NAME_IS_UTF))
                file.Name = data.ReadString(ref offset, Encoding.Unicode);
            else
                file.Name = data.ReadString(ref offset, Encoding.ASCII);

            return file;
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a Microsoft Cabinet file
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled cabinet on success, null on error</returns>
        public static Cabinet ParseCabinet(Stream data)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new cabinet to fill
            var cabinet = new Cabinet();

            #region Cabinet Header

            // Try to parse the cabinet header
            var cabinetHeader = ParseCabinetHeader(data);
            if (cabinetHeader == null)
                return null;

            // Set the cabinet header
            cabinet.Header = cabinetHeader;

            #endregion

            #region Folders

            // Set the folder array
            cabinet.Folders = new CFFOLDER[cabinetHeader.FolderCount];

            // Try to parse each folder, if we have any
            for (int i = 0; i < cabinetHeader.FolderCount; i++)
            {
                var folder = ParseFolder(data, cabinetHeader);
                if (folder == null)
                    return null;

                // Set the folder
                cabinet.Folders[i] = folder;
            }

            #endregion

            #region Files

            // Get the files offset
            int filesOffset = (int)cabinetHeader.FilesOffset + initialOffset;
            if (filesOffset > data.Length)
                return null;

            // Seek to the offset
            data.Seek(filesOffset, SeekOrigin.Begin);

            // Set the file array
            cabinet.Files = new CFFILE[cabinetHeader.FileCount];

            // Try to parse each file, if we have any
            for (int i = 0; i < cabinetHeader.FileCount; i++)
            {
                var file = ParseFile(data);
                if (file == null)
                    return null;

                // Set the file
                cabinet.Files[i] = file;
            }

            #endregion

            return cabinet;
        }

        /// <summary>
        /// Parse a Stream into a cabinet header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled cabinet header on success, null on error</returns>
        private static CFHEADER ParseCabinetHeader(Stream data)
        {
            CFHEADER header = new CFHEADER();

            header.Signature = data.ReadUInt32();
            if (header.Signature != SignatureValue)
                return null;

            header.Reserved1 = data.ReadUInt32();
            if (header.Reserved1 != 0x00000000)
                return null;

            header.CabinetSize = data.ReadUInt32();
            if (header.CabinetSize > MaximumCabSize)
                return null;

            header.Reserved2 = data.ReadUInt32();
            if (header.Reserved2 != 0x00000000)
                return null;

            header.FilesOffset = data.ReadUInt32();

            header.Reserved3 = data.ReadUInt32();
            if (header.Reserved3 != 0x00000000)
                return null;

            header.VersionMinor = data.ReadByteValue();
            header.VersionMajor = data.ReadByteValue();
            if (header.VersionMajor != 0x00000001 || header.VersionMinor != 0x00000003)
                return null;

            header.FolderCount = data.ReadUInt16();
            if (header.FolderCount > MaximumFolderCount)
                return null;

            header.FileCount = data.ReadUInt16();
            if (header.FileCount > MaximumFileCount)
                return null;

            header.Flags = (HeaderFlags)data.ReadUInt16();
            header.SetID = data.ReadUInt16();
            header.CabinetIndex = data.ReadUInt16();

            if (header.Flags.HasFlag(HeaderFlags.RESERVE_PRESENT))
            {
                header.HeaderReservedSize = data.ReadUInt16();
                if (header.HeaderReservedSize > 60_000)
                    return null;

                header.FolderReservedSize = data.ReadByteValue();
                header.DataReservedSize = data.ReadByteValue();

                if (header.HeaderReservedSize > 0)
                    header.ReservedData = data.ReadBytes(header.HeaderReservedSize);
            }

            if (header.Flags.HasFlag(HeaderFlags.PREV_CABINET))
            {
                header.CabinetPrev = data.ReadString(Encoding.ASCII);
                header.DiskPrev = data.ReadString(Encoding.ASCII);
            }

            if (header.Flags.HasFlag(HeaderFlags.NEXT_CABINET))
            {
                header.CabinetNext = data.ReadString(Encoding.ASCII);
                header.DiskNext = data.ReadString(Encoding.ASCII);
            }

            return header;
        }

        /// <summary>
        /// Parse a Stream into a folder
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="header">Cabinet header to get flags and sizes from</param>
        /// <returns>Filled folder on success, null on error</returns>
        private static CFFOLDER ParseFolder(Stream data, CFHEADER header)
        {
            CFFOLDER folder = new CFFOLDER();

            folder.CabStartOffset = data.ReadUInt32();
            folder.DataCount = data.ReadUInt16();
            folder.CompressionType = (CompressionType)data.ReadUInt16();

            if (header.FolderReservedSize > 0)
                folder.ReservedData = data.ReadBytes(header.FolderReservedSize);

            if (folder.CabStartOffset > 0)
            {
                long currentPosition = data.Position;
                data.Seek(folder.CabStartOffset, SeekOrigin.Begin);

                folder.DataBlocks = new Dictionary<int, CFDATA>();
                for (int i = 0; i < folder.DataCount; i++)
                {
                    CFDATA dataBlock = ParseDataBlock(data, header.DataReservedSize);
                    folder.DataBlocks[(int)folder.CabStartOffset] = dataBlock;
                }

                data.Seek(currentPosition, SeekOrigin.Begin);
            }

            return folder;
        }

        /// <summary>
        /// Parse a Stream into a data block
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="dataReservedSize">Reserved byte size for data blocks</param>
        /// <returns>Filled folder on success, null on error</returns>
        private static CFDATA ParseDataBlock(Stream data, byte dataReservedSize)
        {
            CFDATA dataBlock = new CFDATA();

            dataBlock.Checksum = data.ReadUInt32();
            dataBlock.CompressedSize = data.ReadUInt16();
            dataBlock.UncompressedSize = data.ReadUInt16();

            if (dataBlock.UncompressedSize != 0 && dataBlock.CompressedSize > dataBlock.UncompressedSize)
                return null;

            if (dataReservedSize > 0)
                dataBlock.ReservedData = data.ReadBytes(dataReservedSize);

            if (dataBlock.CompressedSize > 0)
                dataBlock.CompressedData = data.ReadBytes(dataBlock.CompressedSize);

            return dataBlock;
        }

        /// <summary>
        /// Parse a Stream into a file
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled file on success, null on error</returns>
        private static CFFILE ParseFile(Stream data)
        {
            CFFILE file = new CFFILE();

            file.FileSize = data.ReadUInt32();
            file.FolderStartOffset = data.ReadUInt32();
            file.FolderIndex = (FolderIndex)data.ReadUInt16();
            file.Date = data.ReadUInt16();
            file.Time = data.ReadUInt16();
            file.Attributes = (Models.MicrosoftCabinet.FileAttributes)data.ReadUInt16();

            if (file.Attributes.HasFlag(Models.MicrosoftCabinet.FileAttributes.NAME_IS_UTF))
                file.Name = data.ReadString(Encoding.Unicode);
            else
                file.Name = data.ReadString(Encoding.ASCII);

            return file;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// The computation and verification of checksums found in CFDATA structure entries cabinet files is
        /// done by using a function described by the following mathematical notation. When checksums are
        /// not supplied by the cabinet file creating application, the checksum field is set to 0 (zero). Cabinet
        /// extracting applications do not compute or verify the checksum if the field is set to 0 (zero).
        /// </summary>
        public static class Checksum
        {
            public static uint ChecksumData(byte[] data)
            {
                uint[] C = new uint[4]
                {
                S(data, 1, data.Length),
                S(data, 2, data.Length),
                S(data, 3, data.Length),
                S(data, 4, data.Length),
                };

                return C[0] ^ C[1] ^ C[2] ^ C[3];
            }

            private static uint S(byte[] a, int b, int x)
            {
                int n = a.Length;

                if (x < 4 && b > n % 4)
                    return 0;
                else if (x < 4 && b <= n % 4)
                    return a[n - b + 1];
                else // if (x >= 4)
                    return a[n - x + b] ^ S(a, b, x - 4);
            }
        }

        #endregion
    }
}
