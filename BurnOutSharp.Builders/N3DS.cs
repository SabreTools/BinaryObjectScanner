using System.IO;
using System.Text;
using BurnOutSharp.Models.N3DS;
using BurnOutSharp.Utilities;
using static BurnOutSharp.Models.N3DS.Constants;

namespace BurnOutSharp.Builders
{
    public class N3DS
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a 3DS cart image
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
        /// Parse a Stream into a 3DS cart image
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
            var header = ParseNCSDHeader(data);
            if (header == null)
                return null;

            // Set the cart image header
            cart.Header = header;

            #endregion

            #region Partitions

            // Create the partition table
            cart.Partitions = new NCCHHeader[8];

            // Iterate and build the partitions
            for (int i = 0; i < 8; i++)
            {
                cart.Partitions[i] = ParseNCCHHeader(data);
            }

            #endregion

            return cart;
        }

        /// <summary>
        /// Parse a Stream into an NCSD header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="development">Indicates if the cart is development or not</param>
        /// <returns>Filled NCSD header on success, null on error</returns>
        private static NCSDHeader ParseNCSDHeader(Stream data, bool development = false)
        {
            // TODO: Use marshalling here instead of building
            NCSDHeader header = new NCSDHeader();

            header.RSA2048Signature = data.ReadBytes(0x100);
            byte[] magicNumber = data.ReadBytes(4);
            header.MagicNumber = Encoding.ASCII.GetString(magicNumber);
            if (header.MagicNumber != NCSDMagicNumber)
                return null;

            header.ImageSizeInMediaUnits = data.ReadUInt32();
            header.MediaId = data.ReadBytes(8);
            header.PartitionsFSType = (FilesystemType)data.ReadUInt64();
            header.PartitionsCryptType = data.ReadBytes(8);

            header.PartitionsTable = new PartitionTableEntry[8];
            for (int i = 0; i < 8; i++)
            {
                header.PartitionsTable[i] = ParsePartitionTableEntry(data);
            }

            if (header.PartitionsFSType == FilesystemType.Normal || header.PartitionsFSType == FilesystemType.None)
            {
                header.ExheaderHash = data.ReadBytes(0x20);
                header.AdditionalHeaderSize = data.ReadUInt32();
                header.SectorZeroOffset = data.ReadUInt32();
                header.PartitionFlags = data.ReadBytes(8);

                header.PartitionIdTable = new byte[8][];
                for (int i = 0; i < 8; i++)
                {
                    header.PartitionIdTable[i] = data.ReadBytes(8);
                }

                header.Reserved1 = data.ReadBytes(0x20);
                header.Reserved2 = data.ReadBytes(0xE);
                header.FirmUpdateByte1 = data.ReadByteValue();
                header.FirmUpdateByte2 = data.ReadByteValue();

                header.CARD2WritableAddressMediaUnits = data.ReadBytes(4);
                header.CardInfoBytemask = data.ReadBytes(4);
                header.Reserved3 = data.ReadBytes(0x108);
                header.TitleVersion = data.ReadUInt16();
                header.CardRevision = data.ReadUInt16();
                header.Reserved4 = data.ReadBytes(0xCEC); // Incorrectly documented as 0xCEE
                header.CardSeedKeyY = data.ReadBytes(0x10);
                header.EncryptedCardSeed = data.ReadBytes(0x10);
                header.CardSeedAESMAC = data.ReadBytes(0x10);
                header.CardSeedNonce = data.ReadBytes(0xC);
                header.Reserved5 = data.ReadBytes(0xC4);
                header.BackupHeader = ParseNCCHHeader(data, true);

                if (development)
                {
                    header.CardDeviceReserved1 = data.ReadBytes(0x200);
                    header.TitleKey = data.ReadBytes(0x10);
                    header.CardDeviceReserved2 = data.ReadBytes(0xF0);
                }
            }
            else if (header.PartitionsFSType == FilesystemType.FIRM)
            {
                header.Unknown = data.ReadBytes(0x5E);
                header.EncryptedMBR = data.ReadBytes(0x42);
            }

            return header;
        }

        /// <summary>
        /// Parse a Stream into a partition table entry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled partition table entry on success, null on error</returns>
        private static PartitionTableEntry ParsePartitionTableEntry(Stream data)
        {
            // TODO: Use marshalling here instead of building
            PartitionTableEntry partitionTableEntry = new PartitionTableEntry();

            partitionTableEntry.Offset = data.ReadUInt32();
            partitionTableEntry.Length = data.ReadUInt32();

            return partitionTableEntry;
        }

        /// <summary>
        /// Parse a Stream into an NCCH header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="skipSignature">Indicates if the signature should be skipped</param>
        /// <returns>Filled NCCH header on success, null on error</returns>
        private static NCCHHeader ParseNCCHHeader(Stream data, bool skipSignature = false)
        {
            // TODO: Use marshalling here instead of building
            NCCHHeader header = new NCCHHeader();

            if (!skipSignature)
                header.RSA2048Signature = data.ReadBytes(0x100);

            byte[] magicId = data.ReadBytes(4);
            header.MagicID = Encoding.ASCII.GetString(magicId);
            if (header.MagicID != NCCHMagicNumber)
                return null;

            header.ContentSizeInMediaUnits = data.ReadUInt32();
            header.PartitionId = data.ReadUInt64();
            header.MakerCode = data.ReadUInt16();
            header.Version = data.ReadUInt16();
            header.VerificationHash = data.ReadUInt32();
            header.ProgramId = data.ReadBytes(8);
            header.Reserved1 = data.ReadBytes(0x10);
            header.LogoRegionHash = data.ReadBytes(0x20);
            header.ProductCode = data.ReadBytes(0x10);
            header.ExtendedHeaderHash = data.ReadBytes(0x20);
            header.ExtendedHeaderSizeInBytes = data.ReadUInt32();
            header.Reserved2 = data.ReadBytes(4);
            header.Flags = ParseNCCHHeaderFlags(data);
            header.PlainRegionOffsetInMediaUnits = data.ReadUInt32();
            header.PlainRegionSizeInMediaUnits = data.ReadUInt32();
            header.LogoRegionOffsetInMediaUnits = data.ReadUInt32();
            header.LogoRegionSizeInMediaUnits = data.ReadUInt32();
            header.ExeFSOffsetInMediaUnits = data.ReadUInt32();
            header.ExeFSSizeInMediaUnits = data.ReadUInt32();
            header.ExeFSHashRegionSizeInMediaUnits = data.ReadUInt32();
            header.Reserved3 = data.ReadBytes(4);
            header.RomFSOffsetInMediaUnits = data.ReadUInt32();
            header.RomFSSizeInMediaUnits = data.ReadUInt32();
            header.RomFSHashRegionSizeInMediaUnits = data.ReadUInt32();
            header.Reserved4 = data.ReadBytes(4);
            header.ExeFSSuperblockHash = data.ReadBytes(0x20);
            header.RomFSSuperblockHash = data.ReadBytes(0x20);

            return header;
        }

        /// <summary>
        /// Parse a Stream into an NCCH header flags
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled NCCH header flags on success, null on error</returns>
        private static NCCHHeaderFlags ParseNCCHHeaderFlags(Stream data)
        {
            // TODO: Use marshalling here instead of building
            NCCHHeaderFlags headerFlags = new NCCHHeaderFlags();

            headerFlags.Reserved0 = data.ReadByteValue();
            headerFlags.Reserved1 = data.ReadByteValue();
            headerFlags.Reserved2 = data.ReadByteValue();
            headerFlags.CryptoMethod = (CryptoMethod)data.ReadByteValue();
            headerFlags.ContentPlatform = (ContentPlatform)data.ReadByteValue();
            headerFlags.MediaPlatformIndex = (ContentType)data.ReadByteValue();
            headerFlags.ContentUnitSize = data.ReadByteValue();
            headerFlags.BitMasks = (BitMasks)data.ReadByteValue();

            return headerFlags;
        }

        #endregion
    }
}
