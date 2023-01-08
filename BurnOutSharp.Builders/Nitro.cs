using System.Collections.Generic;
using System.IO;
using System.Text;
using BurnOutSharp.Models.Nitro;
using BurnOutSharp.Utilities;

namespace BurnOutSharp.Builders
{
    public class Nitro
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a NDS cart image
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled cart image on success, null on error</returns>
        public static Cart ParseCart(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseCart(dataStream);
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a NDS cart image
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled cart image on success, null on error</returns>
        public static Cart ParseCart(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new cart image to fill
            var cart = new Cart();

            #region Header

            // Try to parse the header
            var header = ParseCommonHeader(data);
            if (header == null)
                return null;

            // Set the cart image header
            cart.CommonHeader = header;

            #endregion

            #region Extended DSi Header

            // If we have a DSi-compatible cartridge
            if (header.UnitCode == Unitcode.NDSPlusDSi || header.UnitCode == Unitcode.DSi)
            {
                var extendedDSiHeader = ParseExtendedDSiHeader(data);
                if (extendedDSiHeader == null)
                    return null;

                cart.ExtendedDSiHeader = extendedDSiHeader;
            }

            #endregion

            #region Secure Area

            // Try to get the secure area offset
            long secureAreaOffset = 0x4000;
            if (secureAreaOffset > data.Length)
                return null;

            // Seek to the secure area
            data.Seek(secureAreaOffset, SeekOrigin.Begin);

            // Read the secure area without processing
            cart.SecureArea = data.ReadBytes(0x800);

            #endregion

            #region Name Table

            // Try to get the name table offset
            long nameTableOffset = header.FileNameTableOffset;
            if (nameTableOffset < 0 || nameTableOffset > data.Length)
                return null;

            // Seek to the name table
            data.Seek(nameTableOffset, SeekOrigin.Begin);

            // Try to parse the name table
            var nameTable = ParseNameTable(data);
            if (nameTable == null)
                return null;

            // Set the name table
            cart.NameTable = nameTable;

            #endregion

            // TODO: Parse file allocation table

            // TODO: Read and optionally parse out the other areas
            // Look for offsets and lengths in the header pieces

            return cart;
        }

        /// <summary>
        /// Parse a Stream into a common header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled common header on success, null on error</returns>
        private static CommonHeader ParseCommonHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            CommonHeader commonHeader = new CommonHeader();

            byte[] gameTitle = data.ReadBytes(12);
            commonHeader.GameTitle = Encoding.ASCII.GetString(gameTitle).TrimEnd('\0');
            commonHeader.GameCode = data.ReadUInt32();
            byte[] makerCode = data.ReadBytes(2);
            commonHeader.MakerCode = Encoding.ASCII.GetString(bytes: makerCode).TrimEnd('\0');
            commonHeader.UnitCode = (Unitcode)data.ReadByteValue();
            commonHeader.EncryptionSeedSelect = data.ReadByteValue();
            commonHeader.DeviceCapacity = data.ReadByteValue();
            commonHeader.Reserved1 = data.ReadBytes(7);
            commonHeader.GameRevision = data.ReadUInt16();
            commonHeader.RomVersion = data.ReadByteValue();
            commonHeader.InternalFlags = data.ReadByteValue();
            commonHeader.ARM9RomOffset = data.ReadUInt32();
            commonHeader.ARM9EntryAddress = data.ReadUInt32();
            commonHeader.ARM9LoadAddress = data.ReadUInt32();
            commonHeader.ARM9Size = data.ReadUInt32();
            commonHeader.ARM7RomOffset = data.ReadUInt32();
            commonHeader.ARM7EntryAddress = data.ReadUInt32();
            commonHeader.ARM7LoadAddress = data.ReadUInt32();
            commonHeader.ARM7Size = data.ReadUInt32();
            commonHeader.FileNameTableOffset = data.ReadUInt32();
            commonHeader.FileNameTableLength = data.ReadUInt32();
            commonHeader.FileAllocationTableOffset = data.ReadUInt32();
            commonHeader.FileAllocationTableLength = data.ReadUInt32();
            commonHeader.ARM9OverlayOffset = data.ReadUInt32();
            commonHeader.ARM9OverlayLength = data.ReadUInt32();
            commonHeader.ARM7OverlayOffset = data.ReadUInt32();
            commonHeader.ARM7OverlayLength = data.ReadUInt32();
            commonHeader.NormalCardControlRegisterSettings = data.ReadUInt32();
            commonHeader.SecureCardControlRegisterSettings = data.ReadUInt32();
            commonHeader.IconBannerOffset = data.ReadUInt32();
            commonHeader.SecureAreaCRC = data.ReadUInt16();
            commonHeader.SecureTransferTimeout = data.ReadUInt16();
            commonHeader.ARM9Autoload = data.ReadUInt32();
            commonHeader.ARM7Autoload = data.ReadUInt32();
            commonHeader.SecureDisable = data.ReadBytes(8);
            commonHeader.NTRRegionRomSize = data.ReadUInt32();
            commonHeader.HeaderSize = data.ReadUInt32();
            commonHeader.Reserved2 = data.ReadBytes(56);
            commonHeader.NintendoLogo = data.ReadBytes(156);
            commonHeader.NintendoLogoCRC = data.ReadUInt16();
            commonHeader.HeaderCRC = data.ReadUInt16();
            commonHeader.DebuggerReserved = data.ReadBytes(0x20);

            return commonHeader;
        }

        /// <summary>
        /// Parse a Stream into an extended DSi header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled extended DSi header on success, null on error</returns>
        private static ExtendedDSiHeader ParseExtendedDSiHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            ExtendedDSiHeader extendedDSiHeader = new ExtendedDSiHeader();

            extendedDSiHeader.GlobalMBK15Settings = new uint[5];
            for (int i = 0; i < 5; i++)
            {
                extendedDSiHeader.GlobalMBK15Settings[i] = data.ReadUInt32();
            }
            extendedDSiHeader.LocalMBK68SettingsARM9 = new uint[3];
            for (int i = 0; i < 3; i++)
            {
                extendedDSiHeader.LocalMBK68SettingsARM9[i] = data.ReadUInt32();
            }
            extendedDSiHeader.LocalMBK68SettingsARM7 = new uint[3];
            for (int i = 0; i < 3; i++)
            {
                extendedDSiHeader.LocalMBK68SettingsARM7[i] = data.ReadUInt32();
            }
            extendedDSiHeader.GlobalMBK9Setting = data.ReadUInt32();
            extendedDSiHeader.RegionFlags = data.ReadUInt32();
            extendedDSiHeader.AccessControl = data.ReadUInt32();
            extendedDSiHeader.ARM7SCFGEXTMask = data.ReadUInt32();
            extendedDSiHeader.ReservedFlags = data.ReadUInt32();
            extendedDSiHeader.ARM9iRomOffset = data.ReadUInt32();
            extendedDSiHeader.Reserved3 = data.ReadUInt32();
            extendedDSiHeader.ARM9iLoadAddress = data.ReadUInt32();
            extendedDSiHeader.ARM9iSize = data.ReadUInt32();
            extendedDSiHeader.ARM7iRomOffset = data.ReadUInt32();
            extendedDSiHeader.Reserved4 = data.ReadUInt32();
            extendedDSiHeader.ARM7iLoadAddress = data.ReadUInt32();
            extendedDSiHeader.ARM7iSize = data.ReadUInt32();
            extendedDSiHeader.DigestNTRRegionOffset = data.ReadUInt32();
            extendedDSiHeader.DigestNTRRegionLength = data.ReadUInt32();
            extendedDSiHeader.DigestTWLRegionOffset = data.ReadUInt32();
            extendedDSiHeader.DigestTWLRegionLength = data.ReadUInt32();
            extendedDSiHeader.DigestSectorHashtableRegionOffset = data.ReadUInt32();
            extendedDSiHeader.DigestSectorHashtableRegionLength = data.ReadUInt32();
            extendedDSiHeader.DigestBlockHashtableRegionOffset = data.ReadUInt32();
            extendedDSiHeader.DigestBlockHashtableRegionLength = data.ReadUInt32();
            extendedDSiHeader.DigestSectorSize = data.ReadUInt32();
            extendedDSiHeader.DigestBlockSectorCount = data.ReadUInt32();
            extendedDSiHeader.IconBannerSize = data.ReadUInt32();
            extendedDSiHeader.Unknown1 = data.ReadUInt32();
            extendedDSiHeader.ModcryptArea1Offset = data.ReadUInt32();
            extendedDSiHeader.ModcryptArea1Size = data.ReadUInt32();
            extendedDSiHeader.ModcryptArea2Offset = data.ReadUInt32();
            extendedDSiHeader.ModcryptArea2Size = data.ReadUInt32();
            extendedDSiHeader.TitleID = data.ReadBytes(8);
            extendedDSiHeader.DSiWarePublicSavSize = data.ReadUInt32();
            extendedDSiHeader.DSiWarePrivateSavSize = data.ReadUInt32();
            extendedDSiHeader.ReservedZero = data.ReadBytes(176);
            extendedDSiHeader.Unknown2 = data.ReadBytes(0x10);
            extendedDSiHeader.ARM9WithSecureAreaSHA1HMACHash = data.ReadBytes(20);
            extendedDSiHeader.ARM7SHA1HMACHash = data.ReadBytes(20);
            extendedDSiHeader.DigestMasterSHA1HMACHash = data.ReadBytes(20);
            extendedDSiHeader.BannerSHA1HMACHash = data.ReadBytes(20);
            extendedDSiHeader.ARM9iDecryptedSHA1HMACHash = data.ReadBytes(20);
            extendedDSiHeader.ARM7iDecryptedSHA1HMACHash = data.ReadBytes(20);
            extendedDSiHeader.Reserved5 = data.ReadBytes(40);
            extendedDSiHeader.ARM9NoSecureAreaSHA1HMACHash = data.ReadBytes(20);
            extendedDSiHeader.Reserved6 = data.ReadBytes(2636);
            extendedDSiHeader.ReservedAndUnchecked = data.ReadBytes(0x180);
            extendedDSiHeader.RSASignature = data.ReadBytes(0x80);

            return extendedDSiHeader;
        }

        /// <summary>
        /// Parse a Stream into a name table
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled name table on success, null on error</returns>
        private static NameTable ParseNameTable(Stream data)
        {
            // TODO: Use marshalling here instead of building
            NameTable nameTable = new NameTable();

            // Create a variable-length table
            var folderAllocationTable = new List<FolderAllocationTableEntry>();
            int entryCount = int.MaxValue;
            while (entryCount > 0)
            {
                var entry = ParseFolderAllocationTableEntry(data);
                folderAllocationTable.Add(entry);

                // If we have the root entry
                if (entryCount == int.MaxValue)
                    entryCount = (entry.Unknown << 8) | entry.ParentFolderIndex;

                // Decrement the entry count
                entryCount--;
            }

            // Assign the folder allocation table
            nameTable.FolderAllocationTable = folderAllocationTable.ToArray();

            // Create a variable-length table
            var nameList = new List<NameListEntry>();
            while (true)
            {
                var entry = ParseNameListEntry(data);
                if (entry == null)
                    break;

                nameList.Add(entry);
            }

            // Assign the name list
            nameTable.NameList = nameList.ToArray();

            return nameTable;
        }

        /// <summary>
        /// Parse a Stream into a folder allocation table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled folder allocation table entry on success, null on error</returns>
        private static FolderAllocationTableEntry ParseFolderAllocationTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            FolderAllocationTableEntry entry = new FolderAllocationTableEntry();

            entry.StartOffset = data.ReadUInt32();
            entry.FirstFileIndex = data.ReadUInt16();
            entry.ParentFolderIndex = data.ReadByteValue();
            entry.Unknown = data.ReadByteValue();

            return entry;
        }

        /// <summary>
        /// Parse a Stream into a name list entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled name list entry on success, null on error</returns>
        private static NameListEntry ParseNameListEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            NameListEntry entry = new NameListEntry();

            byte flagAndSize = data.ReadByteValue();
            if (flagAndSize == 0xFF)
                return null;

            entry.Folder = (flagAndSize & 0x80) != 0;

            byte size = (byte)(flagAndSize & ~0x80);
            if (size > 0)
            {
                byte[] name = data.ReadBytes(size);
                entry.Name = Encoding.UTF8.GetString(name);
            }

            if (entry.Folder)
                entry.Index = data.ReadUInt16();

            return entry;
        }

        #endregion
    }
}
