using System;
using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class Nitro : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        #region Common

        /// <inheritdoc cref="Models.Nitro.Header.GameTitle"/>
        public string GameTitle => _cart.Header.GameTitle;

        /// <inheritdoc cref="Models.Nitro.Header.GameCode"/>
        public uint GameCode => _cart.Header.GameCode;

        /// <inheritdoc cref="Models.Nitro.Header.MakerCode"/>
        public string MakerCode => _cart.Header.MakerCode;

        /// <inheritdoc cref="Models.Nitro.Header.UnitCode"/>
        public Models.Nitro.Unitcode UnitCode => _cart.Header.UnitCode;

        /// <inheritdoc cref="Models.Nitro.Header.EncryptionSeedSelect"/>
        public byte EncryptionSeedSelect => _cart.Header.EncryptionSeedSelect;

        /// <inheritdoc cref="Models.Nitro.Header.DeviceCapacity"/>
        public byte DeviceCapacity => _cart.Header.DeviceCapacity;

        /// <inheritdoc cref="Models.Nitro.Header.Reserved1"/>
        public byte[] Reserved1 => _cart.Header.Reserved1;

        /// <inheritdoc cref="Models.Nitro.Header.GameRevision"/>
        public ushort GameRevision => _cart.Header.GameRevision;

        /// <inheritdoc cref="Models.Nitro.Header.RomVersion"/>
        public byte RomVersion => _cart.Header.RomVersion;

        /// <inheritdoc cref="Models.Nitro.Header.InternalFlags"/>
        public byte InternalFlags => _cart.Header.InternalFlags;

        /// <inheritdoc cref="Models.Nitro.Header.ARM9RomOffset"/>
        public uint ARM9RomOffset => _cart.Header.ARM9RomOffset;

        /// <inheritdoc cref="Models.Nitro.Header.ARM9EntryAddress"/>
        public uint ARM9EntryAddress => _cart.Header.ARM9EntryAddress;

        /// <inheritdoc cref="Models.Nitro.Header.ARM9LoadAddress"/>
        public uint ARM9LoadAddress => _cart.Header.ARM9LoadAddress;

        /// <inheritdoc cref="Models.Nitro.Header.ARM9Size"/>
        public uint ARM9Size => _cart.Header.ARM9Size;

        /// <inheritdoc cref="Models.Nitro.Header.ARM7RomOffset"/>
        public uint ARM7RomOffset => _cart.Header.ARM7RomOffset;

        /// <inheritdoc cref="Models.Nitro.Header.ARM7EntryAddress"/>
        public uint ARM7EntryAddress => _cart.Header.ARM7EntryAddress;

        /// <inheritdoc cref="Models.Nitro.Header.ARM7LoadAddress"/>
        public uint ARM7LoadAddress => _cart.Header.ARM7LoadAddress;

        /// <inheritdoc cref="Models.Nitro.Header.ARM7Size"/>
        public uint ARM7Size => _cart.Header.ARM7Size;

        /// <inheritdoc cref="Models.Nitro.Header.FileNameTableOffset"/>
        public uint FileNameTableOffset => _cart.Header.FileNameTableOffset;

        /// <inheritdoc cref="Models.Nitro.Header.FileNameTableLength"/>
        public uint FileNameTableLength => _cart.Header.FileNameTableLength;

        /// <inheritdoc cref="Models.Nitro.Header.FileAllocationTableOffset"/>
        public uint FileAllocationTableOffset => _cart.Header.FileAllocationTableOffset;

        /// <inheritdoc cref="Models.Nitro.Header.FileAllocationTableLength"/>
        public uint FileAllocationTableLength => _cart.Header.FileAllocationTableLength;

        /// <inheritdoc cref="Models.Nitro.Header.ARM9OverlayOffset"/>
        public uint ARM9OverlayOffset => _cart.Header.ARM9OverlayOffset;

        /// <inheritdoc cref="Models.Nitro.Header.ARM9OverlayLength"/>
        public uint ARM9OverlayLength => _cart.Header.ARM9OverlayLength;

        /// <inheritdoc cref="Models.Nitro.Header.ARM7OverlayOffset"/>
        public uint ARM7OverlayOffset => _cart.Header.ARM7OverlayOffset;

        /// <inheritdoc cref="Models.Nitro.Header.ARM7OverlayLength"/>
        public uint ARM7OverlayLength => _cart.Header.ARM7OverlayLength;

        /// <inheritdoc cref="Models.Nitro.Header.NormalCardControlRegisterSettings"/>
        public uint NormalCardControlRegisterSettings => _cart.Header.NormalCardControlRegisterSettings;

        /// <inheritdoc cref="Models.Nitro.Header.SecureCardControlRegisterSettings"/>
        public uint SecureCardControlRegisterSettings => _cart.Header.SecureCardControlRegisterSettings;

        /// <inheritdoc cref="Models.Nitro.Header.IconBannerOffset"/>
        public uint IconBannerOffset => _cart.Header.IconBannerOffset;

        /// <inheritdoc cref="Models.Nitro.Header.SecureAreaCRC"/>
        public ushort SecureAreaCRC => _cart.Header.SecureAreaCRC;

        /// <inheritdoc cref="Models.Nitro.Header.SecureTransferTimeout"/>
        public ushort SecureTransferTimeout => _cart.Header.SecureTransferTimeout;

        /// <inheritdoc cref="Models.Nitro.Header.ARM9Autoload"/>
        public uint ARM9Autoload => _cart.Header.ARM9Autoload;

        /// <inheritdoc cref="Models.Nitro.Header.ARM7Autoload"/>
        public uint ARM7Autoload => _cart.Header.ARM7Autoload;

        /// <inheritdoc cref="Models.Nitro.Header.SecureDisable"/>
        public byte[] SecureDisable => _cart.Header.SecureDisable;

        /// <inheritdoc cref="Models.Nitro.Header.NTRRegionRomSize"/>
        public uint NTRRegionRomSize => _cart.Header.NTRRegionRomSize;

        /// <inheritdoc cref="Models.Nitro.Header.HeaderSize"/>
        public uint HeaderSize => _cart.Header.HeaderSize;

        /// <inheritdoc cref="Models.Nitro.Header.Reserved2"/>
        public byte[] Reserved2 => _cart.Header.Reserved2;

        /// <inheritdoc cref="Models.Nitro.Header.NintendoLogo"/>
        public byte[] NintendoLogo => _cart.Header.NintendoLogo;

        /// <inheritdoc cref="Models.Nitro.Header.NintendoLogoCRC"/>
        public ushort NintendoLogoCRC => _cart.Header.NintendoLogoCRC;

        /// <inheritdoc cref="Models.Nitro.Header.HeaderCRC"/>
        public ushort HeaderCRC => _cart.Header.HeaderCRC;

        /// <inheritdoc cref="Models.Nitro.Header.DebuggerReserved"/>
        public byte[] DebuggerReserved => _cart.Header.DebuggerReserved;

        #endregion

        #region Extended DSi

        /// <inheritdoc cref="Models.Nitro.Header.GlobalMBK15Settings"/>
        public uint[] GlobalMBK15Settings => _cart.Header.GlobalMBK15Settings;

        /// <inheritdoc cref="Models.Nitro.Header.LocalMBK68SettingsARM9"/>
        public uint[] LocalMBK68SettingsARM9 => _cart.Header.LocalMBK68SettingsARM9;

        /// <inheritdoc cref="Models.Nitro.Header.LocalMBK68SettingsARM7"/>
        public uint[] LocalMBK68SettingsARM7 => _cart.Header.LocalMBK68SettingsARM7;

        /// <inheritdoc cref="Models.Nitro.Header.GlobalMBK9Setting"/>
        public uint GlobalMBK9Setting => _cart.Header.GlobalMBK9Setting;

        /// <inheritdoc cref="Models.Nitro.Header.RegionFlags"/>
        public uint RegionFlags => _cart.Header.RegionFlags;

        /// <inheritdoc cref="Models.Nitro.Header.AccessControl"/>
        public uint AccessControl => _cart.Header.AccessControl;

        /// <inheritdoc cref="Models.Nitro.Header.ARM7SCFGEXTMask"/>
        public uint ARM7SCFGEXTMask => _cart.Header.ARM7SCFGEXTMask;

        /// <inheritdoc cref="Models.Nitro.Header.ReservedFlags"/>
        public uint ReservedFlags => _cart.Header.ReservedFlags;

        /// <inheritdoc cref="Models.Nitro.Header.ARM9iRomOffset"/>
        public uint ARM9iRomOffset => _cart.Header.ARM9iRomOffset;

        /// <inheritdoc cref="Models.Nitro.Header.Reserved3"/>
        public uint Reserved3 => _cart.Header.Reserved3;

        /// <inheritdoc cref="Models.Nitro.Header.ARM9iLoadAddress"/>
        public uint ARM9iLoadAddress => _cart.Header.ARM9iLoadAddress;

        /// <inheritdoc cref="Models.Nitro.Header.ARM9iSize"/>
        public uint ARM9iSize => _cart.Header.ARM9iSize;

        /// <inheritdoc cref="Models.Nitro.Header.ARM7iRomOffset"/>
        public uint ARM7iRomOffset => _cart.Header.ARM7iRomOffset;

        /// <inheritdoc cref="Models.Nitro.Header.Reserved4"/>
        public uint Reserved4 => _cart.Header.Reserved4;

        /// <inheritdoc cref="Models.Nitro.Header.ARM7iLoadAddress"/>
        public uint ARM7iLoadAddress => _cart.Header.ARM7iLoadAddress;

        /// <inheritdoc cref="Models.Nitro.Header.ARM7iSize"/>
        public uint ARM7iSize => _cart.Header.ARM7iSize;

        /// <inheritdoc cref="Models.Nitro.Header.DigestNTRRegionOffset"/>
        public uint DigestNTRRegionOffset => _cart.Header.DigestNTRRegionOffset;

        /// <inheritdoc cref="Models.Nitro.Header.DigestNTRRegionLength"/>
        public uint DigestNTRRegionLength => _cart.Header.DigestNTRRegionLength;

        /// <inheritdoc cref="Models.Nitro.Header.DigestTWLRegionOffset"/>
        public uint DigestTWLRegionOffset => _cart.Header.DigestTWLRegionOffset;

        /// <inheritdoc cref="Models.Nitro.Header.DigestTWLRegionLength"/>
        public uint DigestTWLRegionLength => _cart.Header.DigestTWLRegionLength;

        /// <inheritdoc cref="Models.Nitro.Header.DigestSectorHashtableRegionOffset"/>
        public uint DigestSectorHashtableRegionOffset => _cart.Header.DigestSectorHashtableRegionOffset;

        /// <inheritdoc cref="Models.Nitro.Header.DigestSectorHashtableRegionLength"/>
        public uint DigestSectorHashtableRegionLength => _cart.Header.DigestSectorHashtableRegionLength;

        /// <inheritdoc cref="Models.Nitro.Header.DigestBlockHashtableRegionOffset"/>
        public uint DigestBlockHashtableRegionOffset => _cart.Header.DigestBlockHashtableRegionOffset;

        /// <inheritdoc cref="Models.Nitro.Header.DigestBlockHashtableRegionLength"/>
        public uint DigestBlockHashtableRegionLength => _cart.Header.DigestBlockHashtableRegionLength;

        /// <inheritdoc cref="Models.Nitro.Header.DigestSectorSize"/>
        public uint DigestSectorSize => _cart.Header.DigestSectorSize;

        /// <inheritdoc cref="Models.Nitro.Header.DigestBlockSectorCount"/>
        public uint DigestBlockSectorCount => _cart.Header.DigestBlockSectorCount;

        /// <inheritdoc cref="Models.Nitro.Header.IconBannerSize"/>
        public uint IconBannerSize => _cart.Header.IconBannerSize;

        /// <inheritdoc cref="Models.Nitro.Header.Unknown1"/>
        public uint Unknown1 => _cart.Header.Unknown1;

        /// <inheritdoc cref="Models.Nitro.Header.ModcryptArea1Offset"/>
        public uint ModcryptArea1Offset => _cart.Header.ModcryptArea1Offset;

        /// <inheritdoc cref="Models.Nitro.Header.ModcryptArea1Size"/>
        public uint ModcryptArea1Size => _cart.Header.ModcryptArea1Size;

        /// <inheritdoc cref="Models.Nitro.Header.ModcryptArea2Offset"/>
        public uint ModcryptArea2Offset => _cart.Header.ModcryptArea2Offset;

        /// <inheritdoc cref="Models.Nitro.Header.ModcryptArea2Size"/>
        public uint ModcryptArea2Size => _cart.Header.ModcryptArea2Size;

        /// <inheritdoc cref="Models.Nitro.Header.TitleID"/>
        public byte[] TitleID => _cart.Header.TitleID;

        /// <inheritdoc cref="Models.Nitro.Header.DSiWarePublicSavSize"/>
        public uint DSiWarePublicSavSize => _cart.Header.DSiWarePublicSavSize;

        /// <inheritdoc cref="Models.Nitro.Header.DSiWarePrivateSavSize"/>
        public uint DSiWarePrivateSavSize => _cart.Header.DSiWarePrivateSavSize;

        /// <inheritdoc cref="Models.Nitro.Header.ReservedZero"/>
        public byte[] ReservedZero => _cart.Header.ReservedZero;

        /// <inheritdoc cref="Models.Nitro.Header.Unknown2"/>
        public byte[] Unknown2 => _cart.Header.Unknown2;

        /// <inheritdoc cref="Models.Nitro.Header.ARM9WithSecureAreaSHA1HMACHash"/>
        public byte[] ARM9WithSecureAreaSHA1HMACHash => _cart.Header.ARM9WithSecureAreaSHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.Header.ARM7SHA1HMACHash"/>
        public byte[] ARM7SHA1HMACHash => _cart.Header.ARM7SHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.Header.DigestMasterSHA1HMACHash"/>
        public byte[] DigestMasterSHA1HMACHash => _cart.Header.DigestMasterSHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.Header.BannerSHA1HMACHash"/>
        public byte[] BannerSHA1HMACHash => _cart.Header.BannerSHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.Header.ARM9iDecryptedSHA1HMACHash"/>
        public byte[] ARM9iDecryptedSHA1HMACHash => _cart.Header.ARM9iDecryptedSHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.Header.ARM7iDecryptedSHA1HMACHash"/>
        public byte[] ARM7iDecryptedSHA1HMACHash => _cart.Header.ARM7iDecryptedSHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.Header.Reserved5"/>
        public byte[] Reserved5 => _cart.Header.Reserved5;

        /// <inheritdoc cref="Models.Nitro.Header.ARM9NoSecureAreaSHA1HMACHash"/>
        public byte[] ARM9NoSecureAreaSHA1HMACHash => _cart.Header.ARM9NoSecureAreaSHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.Header.Reserved6"/>
        public byte[] Reserved6 => _cart.Header.Reserved6;

        /// <inheritdoc cref="Models.Nitro.Header.ReservedAndUnchecked"/>
        public byte[] ReservedAndUnchecked => _cart.Header.ReservedAndUnchecked;

        /// <inheritdoc cref="Models.Nitro.Header.RSASignature"/>
        public byte[] RSASignature => _cart.Header.RSASignature;

        #endregion

        #endregion

        #region Secure Area

        /// <inheritdoc cref="Models.Nitro.Cart.SecureArea"/>
        public byte[] SecureArea => _cart.SecureArea;

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the cart
        /// </summary>
        private Models.Nitro.Cart _cart;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private Nitro() { }

        /// <summary>
        /// Create a NDS cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A NDS cart image wrapper on success, null on failure</returns>
        public static Nitro Create(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a NDS cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A NDS cart image wrapper on success, null on failure</returns>
        public static Nitro Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = Builders.Nitro.ParseCart(data);
            if (archive == null)
                return null;

            var wrapper = new Nitro
            {
                _cart = archive,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override void Print()
        {
            Console.WriteLine("NDS Cart Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            PrintCommonHeader();
            PrintDSiHeader();
            PrintSecureArea();
        }

        /// <summary>
        /// Print common header information
        /// </summary>
        private void PrintCommonHeader()
        {
            Console.WriteLine("  Common Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Game title: {GameTitle ?? "[NULL]"}");
            Console.WriteLine($"  Game code: {GameCode}");
            Console.WriteLine($"  Maker code: {MakerCode ?? "[NULL]"}");
            Console.WriteLine($"  Unit code: {UnitCode}");
            Console.WriteLine($"  Encryption seed select: {EncryptionSeedSelect}");
            Console.WriteLine($"  Device capacity: {DeviceCapacity}");
            Console.WriteLine($"  Reserved 1: {BitConverter.ToString(Reserved1).Replace('-', ' ')}");
            Console.WriteLine($"  Game revision: {GameRevision}");
            Console.WriteLine($"  Rom version: {RomVersion}");
            Console.WriteLine($"  ARM9 rom offset: {ARM9RomOffset}");
            Console.WriteLine($"  ARM9 entry address: {ARM9EntryAddress}");
            Console.WriteLine($"  ARM9 load address: {ARM9LoadAddress}");
            Console.WriteLine($"  ARM9 size: {ARM9Size}");
            Console.WriteLine($"  ARM7 rom offset: {ARM7RomOffset}");
            Console.WriteLine($"  ARM7 entry address: {ARM7EntryAddress}");
            Console.WriteLine($"  ARM7 load address: {ARM7LoadAddress}");
            Console.WriteLine($"  ARM7 size: {ARM7Size}");
            Console.WriteLine($"  File name table offset: {FileNameTableOffset}");
            Console.WriteLine($"  File name table length: {FileNameTableLength}");
            Console.WriteLine($"  File allocation table offset: {FileAllocationTableOffset}");
            Console.WriteLine($"  File allocation table length: {FileAllocationTableLength}");
            Console.WriteLine($"  ARM9 overlay offset: {ARM9OverlayOffset}");
            Console.WriteLine($"  ARM9 overlay length: {ARM9OverlayLength}");
            Console.WriteLine($"  ARM7 overlay offset: {ARM7OverlayOffset}");
            Console.WriteLine($"  ARM7 overlay length: {ARM7OverlayLength}");
            Console.WriteLine($"  Normal card control register settings: {NormalCardControlRegisterSettings}");
            Console.WriteLine($"  Secure card control register settings: {SecureCardControlRegisterSettings}");
            Console.WriteLine($"  Icon banner offset: {IconBannerOffset}");
            Console.WriteLine($"  Secure area CRC: {SecureAreaCRC}");
            Console.WriteLine($"  Secure transfer timeout: {SecureTransferTimeout}");
            Console.WriteLine($"  ARM9 autoload: {ARM9Autoload}");
            Console.WriteLine($"  ARM7 autoload: {ARM7Autoload}");
            Console.WriteLine($"  Secure disable: {SecureDisable}");
            Console.WriteLine($"  NTR region rom size: {NTRRegionRomSize}");
            Console.WriteLine($"  Header size: {HeaderSize}");
            Console.WriteLine($"  Reserved 2: {BitConverter.ToString(Reserved2).Replace('-', ' ')}");
            Console.WriteLine($"  Nintendo logo: {BitConverter.ToString(NintendoLogo).Replace('-', ' ')}");
            Console.WriteLine($"  Nintendo logo CRC: {NintendoLogoCRC}");
            Console.WriteLine($"  Header CRC: {HeaderCRC}");
            Console.WriteLine($"  Debugger reserved: {BitConverter.ToString(DebuggerReserved).Replace('-', ' ')}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print DSi header information
        /// </summary>
        private void PrintDSiHeader()
        {
            Console.WriteLine("  DSi Header Information:");
            Console.WriteLine("  -------------------------");
            if (UnitCode == Models.Nitro.Unitcode.NDSPlusDSi || UnitCode == Models.Nitro.Unitcode.DSi)
            {
                Console.WriteLine("  No DSi header");
            }
            else
            {
                Console.WriteLine($"  Global MBK1..MBK5 settings: {string.Join(", ", GlobalMBK15Settings)}");
                Console.WriteLine($"  Local MBK6..MBK8 settings for ARM9: {string.Join(", ", LocalMBK68SettingsARM9)}");
                Console.WriteLine($"  Local MBK6..MBK8 settings for ARM7: {string.Join(", ", LocalMBK68SettingsARM7)}");
                Console.WriteLine($"  Global MBK9 setting: {GlobalMBK9Setting}");
                Console.WriteLine($"  Region flags: {RegionFlags}");
                Console.WriteLine($"  Access control: {AccessControl}");
                Console.WriteLine($"  ARM7 SCFG EXT mask: {ARM7SCFGEXTMask}");
                Console.WriteLine($"  Reserved/flags?: {ReservedFlags}");
                Console.WriteLine($"  ARM9i rom offset: {ARM9iRomOffset}");
                Console.WriteLine($"  Reserved 3: {Reserved3}");
                Console.WriteLine($"  ARM9i load address: {ARM9iLoadAddress}");
                Console.WriteLine($"  ARM9i size: {ARM9iSize}");
                Console.WriteLine($"  ARM7i rom offset: {ARM7iRomOffset}");
                Console.WriteLine($"  Reserved 4: {Reserved4}");
                Console.WriteLine($"  ARM7i load address: {ARM7iLoadAddress}");
                Console.WriteLine($"  ARM7i size: {ARM7iSize}");
                Console.WriteLine($"  Digest NTR region offset: {DigestNTRRegionOffset}");
                Console.WriteLine($"  Digest NTR region length: {DigestNTRRegionLength}");
                Console.WriteLine($"  Digest TWL region offset: {DigestTWLRegionOffset}");
                Console.WriteLine($"  Digest TWL region length: {DigestTWLRegionLength}");
                Console.WriteLine($"  Digest sector hashtable region offset: {DigestSectorHashtableRegionOffset}");
                Console.WriteLine($"  Digest sector hashtable region length: {DigestSectorHashtableRegionLength}");
                Console.WriteLine($"  Digest block hashtable region offset: {DigestBlockHashtableRegionOffset}");
                Console.WriteLine($"  Digest block hashtable region length: {DigestBlockHashtableRegionLength}");
                Console.WriteLine($"  Digest sector size: {DigestSectorSize}");
                Console.WriteLine($"  Digest block sector count: {DigestBlockSectorCount}");
                Console.WriteLine($"  Icon banner size: {IconBannerSize}");
                Console.WriteLine($"  Unknown 1: {Unknown1}");
                Console.WriteLine($"  Modcrypt area 1 offset: {ModcryptArea1Offset}");
                Console.WriteLine($"  Modcrypt area 1 size: {ModcryptArea1Size}");
                Console.WriteLine($"  Modcrypt area 2 offset: {ModcryptArea2Offset}");
                Console.WriteLine($"  Modcrypt area 2 size: {ModcryptArea2Size}");
                Console.WriteLine($"  Title ID: {BitConverter.ToString(TitleID).Replace('-', ' ')}");
                Console.WriteLine($"  DSiWare 'public.sav' size: {DSiWarePublicSavSize}");
                Console.WriteLine($"  DSiWare 'private.sav' size: {DSiWarePrivateSavSize}");
                Console.WriteLine($"  Reserved (zero): {BitConverter.ToString(ReservedZero).Replace('-', ' ')}");
                Console.WriteLine($"  Unknown 2: {BitConverter.ToString(Unknown2).Replace('-', ' ')}");
                Console.WriteLine($"  ARM9 (with encrypted secure area) SHA1 HMAC hash: {BitConverter.ToString(ARM9WithSecureAreaSHA1HMACHash).Replace('-', ' ')}");
                Console.WriteLine($"  ARM7 SHA1 HMAC hash: {BitConverter.ToString(ARM7SHA1HMACHash).Replace('-', ' ')}");
                Console.WriteLine($"  Digest master SHA1 HMAC hash: {BitConverter.ToString(DigestMasterSHA1HMACHash).Replace('-', ' ')}");
                Console.WriteLine($"  Banner SHA1 HMAC hash: {BitConverter.ToString(BannerSHA1HMACHash).Replace('-', ' ')}");
                Console.WriteLine($"  ARM9i (decrypted) SHA1 HMAC hash: {BitConverter.ToString(ARM9iDecryptedSHA1HMACHash).Replace('-', ' ')}");
                Console.WriteLine($"  ARM7i (decrypted) SHA1 HMAC hash: {BitConverter.ToString(ARM7iDecryptedSHA1HMACHash).Replace('-', ' ')}");
                Console.WriteLine($"  Reserved 5: {BitConverter.ToString(Reserved5).Replace('-', ' ')}");
                Console.WriteLine($"  ARM9 (without secure area) SHA1 HMAC hash: {BitConverter.ToString(ARM9NoSecureAreaSHA1HMACHash).Replace('-', ' ')}");
                Console.WriteLine($"  Reserved 6: {BitConverter.ToString(Reserved6).Replace('-', ' ')}");
                Console.WriteLine($"  Reserved and unchecked region: {BitConverter.ToString(ReservedAndUnchecked).Replace('-', ' ')}");
                Console.WriteLine($"  RSA signature: {BitConverter.ToString(RSASignature).Replace('-', ' ')}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print secure area information
        /// </summary>
        private void PrintSecureArea()
        {
            Console.WriteLine("  Secure Area Information:");
            Console.WriteLine("  -------------------------");
            if (SecureArea == null || SecureArea.Length == 0)
                Console.WriteLine("  No secure area");
            else
                Console.WriteLine($"  {BitConverter.ToString(SecureArea).Replace('-', ' ')}");
            Console.WriteLine();
        }

        #endregion
    }
}