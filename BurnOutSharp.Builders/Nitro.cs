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
            var header = ParseHeader(data);
            if (header == null)
                return null;

            // Set the cart image header
            cart.Header = header;

            #endregion

            #region Secure Area

            // Try to get the secure area offset
            int secureAreaOffset = 0x4000;
            if (secureAreaOffset > data.Length)
                return null;

            // Seek to the secure area
            data.Seek(secureAreaOffset, SeekOrigin.Begin);

            // Read the secure area without processing
            cart.SecureArea = data.ReadBytes(0x800);

            #endregion

            // TODO: Read and optionally parse out the other areas
            // Look for offsets and lengths in the header pieces

            return cart;
        }

        /// <summary>
        /// Parse a Stream into a header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled header on success, null on error</returns>
        private static Header ParseHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            Header header = new Header();

            byte[] gameTitle = data.ReadBytes(0x0C);
            header.GameTitle = Encoding.ASCII.GetString(gameTitle);
            header.GameCode = data.ReadUInt32();
            byte[] makerCode = data.ReadBytes(2);
            header.MakerCode = Encoding.ASCII.GetString(bytes: makerCode);
            header.UnitCode = (Unitcode)data.ReadByteValue();
            header.EncryptionSeedSelect = data.ReadByteValue();
            header.DeviceCapacity = data.ReadByteValue();
            header.Reserved1 = data.ReadBytes(7);
            header.GameRevision = data.ReadUInt16();
            header.RomVersion = data.ReadByteValue();
            header.InternalFlags = data.ReadByteValue();
            header.ARM9RomOffset = data.ReadUInt32();
            header.ARM9EntryAddress = data.ReadUInt32();
            header.ARM9LoadAddress = data.ReadUInt32();
            header.ARM9Size = data.ReadUInt32();
            header.ARM7RomOffset = data.ReadUInt32();
            header.ARM7EntryAddress = data.ReadUInt32();
            header.ARM7LoadAddress = data.ReadUInt32();
            header.ARM7Size = data.ReadUInt32();
            header.FileNameTableOffset = data.ReadUInt32();
            header.FileNameTableLength = data.ReadUInt32();
            header.FileAllocationTableOffset = data.ReadUInt32();
            header.FileAllocationTableLength = data.ReadUInt32();
            header.ARM9OverlayOffset = data.ReadUInt32();
            header.ARM9OverlayLength = data.ReadUInt32();
            header.ARM7OverlayOffset = data.ReadUInt32();
            header.ARM7OverlayLength = data.ReadUInt32();
            header.NormalCardControlRegisterSettings = data.ReadUInt32();
            header.SecureCardControlRegisterSettings = data.ReadUInt32();
            header.IconBannerOffset = data.ReadUInt32();
            header.SecureAreaCRC = data.ReadUInt16();
            header.SecureTransferTimeout = data.ReadUInt16();
            header.ARM9Autoload = data.ReadUInt32();
            header.ARM7Autoload = data.ReadUInt32();
            header.SecureDisable = data.ReadBytes(8);
            header.NTRRegionRomSize = data.ReadUInt32();
            header.HeaderSize = data.ReadUInt32();
            header.Reserved2 = data.ReadBytes(56);
            header.NintendoLogo = data.ReadBytes(156);
            header.NintendoLogoCRC = data.ReadUInt16();
            header.HeaderCRC = data.ReadUInt16();
            header.DebuggerReserved = data.ReadBytes(0x20);

            // If we have a DSi compatible title
            if (header.UnitCode == Unitcode.NDSPlusDSi || header.UnitCode == Unitcode.DSi)
            {
                header.GlobalMBK15Settings = new uint[5];
                for (int i = 0; i < 5; i++)
                {
                    header.GlobalMBK15Settings[i] = data.ReadUInt32();
                }
                header.LocalMBK68SettingsARM9 = new uint[3];
                for (int i = 0; i < 3; i++)
                {
                    header.LocalMBK68SettingsARM9[i] = data.ReadUInt32();
                }
                header.LocalMBK68SettingsARM7 = new uint[3];
                for (int i = 0; i < 3; i++)
                {
                    header.LocalMBK68SettingsARM7[i] = data.ReadUInt32();
                }
                header.GlobalMBK9Setting = data.ReadUInt32();
                header.RegionFlags = data.ReadUInt32();
                header.AccessControl = data.ReadUInt32();
                header.ARM7SCFGEXTMask = data.ReadUInt32();
                header.ReservedFlags = data.ReadUInt32();
                header.ARM9iRomOffset = data.ReadUInt32();
                header.Reserved3 = data.ReadUInt32();
                header.ARM9iLoadAddress = data.ReadUInt32();
                header.ARM9iSize = data.ReadUInt32();
                header.ARM7iRomOffset = data.ReadUInt32();
                header.Reserved4 = data.ReadUInt32();
                header.ARM7iLoadAddress = data.ReadUInt32();
                header.ARM7iSize = data.ReadUInt32();
                header.DigestNTRRegionOffset = data.ReadUInt32();
                header.DigestNTRRegionLength = data.ReadUInt32();
                header.DigestTWLRegionOffset = data.ReadUInt32();
                header.DigestTWLRegionLength = data.ReadUInt32();
                header.DigestSectorHashtableRegionOffset = data.ReadUInt32();
                header.DigestSectorHashtableRegionLength = data.ReadUInt32();
                header.DigestBlockHashtableRegionOffset = data.ReadUInt32();
                header.DigestBlockHashtableRegionLength = data.ReadUInt32();
                header.DigestSectorSize = data.ReadUInt32();
                header.DigestBlockSectorCount = data.ReadUInt32();
                header.IconBannerSize = data.ReadUInt32();
                header.Unknown1 = data.ReadUInt32();
                header.ModcryptArea1Offset = data.ReadUInt32();
                header.ModcryptArea1Size = data.ReadUInt32();
                header.ModcryptArea2Offset = data.ReadUInt32();
                header.ModcryptArea2Size = data.ReadUInt32();
                header.TitleID = data.ReadBytes(8);
                header.DSiWarePublicSavSize = data.ReadUInt32();
                header.DSiWarePrivateSavSize = data.ReadUInt32();
                header.ReservedZero = data.ReadBytes(176);
                header.Unknown2 = data.ReadBytes(0x10);
                header.ARM9WithSecureAreaSHA1HMACHash = data.ReadBytes(20);
                header.ARM7SHA1HMACHash = data.ReadBytes(20);
                header.DigestMasterSHA1HMACHash = data.ReadBytes(20);
                header.BannerSHA1HMACHash = data.ReadBytes(20);
                header.ARM9iDecryptedSHA1HMACHash = data.ReadBytes(20);
                header.ARM7iDecryptedSHA1HMACHash = data.ReadBytes(20);
                header.Reserved5 = data.ReadBytes(40);
                header.ARM9NoSecureAreaSHA1HMACHash = data.ReadBytes(20);
                header.Reserved6 = data.ReadBytes(2636);
                header.ReservedAndUnchecked = data.ReadBytes(0x180);
                header.RSASignature = data.ReadBytes(0x80);
            }

            return header;
        }

        #endregion
    }
}
