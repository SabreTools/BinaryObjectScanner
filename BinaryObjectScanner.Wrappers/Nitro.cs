using System;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class Nitro : WrapperBase<SabreTools.Models.Nitro.Cart>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Nintendo DS/DSi Cart Image";

        #endregion

        #region Pass-Through Properties

        #region Common Header

        /// <inheritdoc cref="Models.Nitro.CommonHeader.GameTitle"/>
        public string GameTitle => _model.CommonHeader.GameTitle;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.GameCode"/>
        public uint GameCode => _model.CommonHeader.GameCode;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.MakerCode"/>
        public string MakerCode => _model.CommonHeader.MakerCode;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.UnitCode"/>
        public SabreTools.Models.Nitro.Unitcode UnitCode => _model.CommonHeader.UnitCode;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.EncryptionSeedSelect"/>
        public byte EncryptionSeedSelect => _model.CommonHeader.EncryptionSeedSelect;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.DeviceCapacity"/>
        public byte DeviceCapacity => _model.CommonHeader.DeviceCapacity;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.Reserved1"/>
        public byte[] Reserved1 => _model.CommonHeader.Reserved1;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.GameRevision"/>
        public ushort GameRevision => _model.CommonHeader.GameRevision;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.RomVersion"/>
        public byte RomVersion => _model.CommonHeader.RomVersion;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.InternalFlags"/>
        public byte InternalFlags => _model.CommonHeader.InternalFlags;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9RomOffset"/>
        public uint ARM9RomOffset => _model.CommonHeader.ARM9RomOffset;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9EntryAddress"/>
        public uint ARM9EntryAddress => _model.CommonHeader.ARM9EntryAddress;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9LoadAddress"/>
        public uint ARM9LoadAddress => _model.CommonHeader.ARM9LoadAddress;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9Size"/>
        public uint ARM9Size => _model.CommonHeader.ARM9Size;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7RomOffset"/>
        public uint ARM7RomOffset => _model.CommonHeader.ARM7RomOffset;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7EntryAddress"/>
        public uint ARM7EntryAddress => _model.CommonHeader.ARM7EntryAddress;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7LoadAddress"/>
        public uint ARM7LoadAddress => _model.CommonHeader.ARM7LoadAddress;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7Size"/>
        public uint ARM7Size => _model.CommonHeader.ARM7Size;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.FileNameTableOffset"/>
        public uint FileNameTableOffset => _model.CommonHeader.FileNameTableOffset;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.FileNameTableLength"/>
        public uint FileNameTableLength => _model.CommonHeader.FileNameTableLength;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.FileAllocationTableOffset"/>
        public uint FileAllocationTableOffset => _model.CommonHeader.FileAllocationTableOffset;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.FileAllocationTableLength"/>
        public uint FileAllocationTableLength => _model.CommonHeader.FileAllocationTableLength;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9OverlayOffset"/>
        public uint ARM9OverlayOffset => _model.CommonHeader.ARM9OverlayOffset;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9OverlayLength"/>
        public uint ARM9OverlayLength => _model.CommonHeader.ARM9OverlayLength;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7OverlayOffset"/>
        public uint ARM7OverlayOffset => _model.CommonHeader.ARM7OverlayOffset;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7OverlayLength"/>
        public uint ARM7OverlayLength => _model.CommonHeader.ARM7OverlayLength;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.NormalCardControlRegisterSettings"/>
        public uint NormalCardControlRegisterSettings => _model.CommonHeader.NormalCardControlRegisterSettings;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.SecureCardControlRegisterSettings"/>
        public uint SecureCardControlRegisterSettings => _model.CommonHeader.SecureCardControlRegisterSettings;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.IconBannerOffset"/>
        public uint IconBannerOffset => _model.CommonHeader.IconBannerOffset;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.SecureAreaCRC"/>
        public ushort SecureAreaCRC => _model.CommonHeader.SecureAreaCRC;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.SecureTransferTimeout"/>
        public ushort SecureTransferTimeout => _model.CommonHeader.SecureTransferTimeout;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9Autoload"/>
        public uint ARM9Autoload => _model.CommonHeader.ARM9Autoload;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7Autoload"/>
        public uint ARM7Autoload => _model.CommonHeader.ARM7Autoload;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.SecureDisable"/>
        public byte[] SecureDisable => _model.CommonHeader.SecureDisable;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.NTRRegionRomSize"/>
        public uint NTRRegionRomSize => _model.CommonHeader.NTRRegionRomSize;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.HeaderSize"/>
        public uint HeaderSize => _model.CommonHeader.HeaderSize;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.Reserved2"/>
        public byte[] Reserved2 => _model.CommonHeader.Reserved2;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.NintendoLogo"/>
        public byte[] NintendoLogo => _model.CommonHeader.NintendoLogo;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.NintendoLogoCRC"/>
        public ushort NintendoLogoCRC => _model.CommonHeader.NintendoLogoCRC;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.HeaderCRC"/>
        public ushort HeaderCRC => _model.CommonHeader.HeaderCRC;

        /// <inheritdoc cref="Models.Nitro.CommonHeader.DebuggerReserved"/>
        public byte[] DebuggerReserved => _model.CommonHeader.DebuggerReserved;

        #endregion

        #region Extended DSi Header

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.GlobalMBK15Settings"/>
        public uint[] GlobalMBK15Settings => _model.ExtendedDSiHeader?.GlobalMBK15Settings;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.LocalMBK68SettingsARM9"/>
        public uint[] LocalMBK68SettingsARM9 => _model.ExtendedDSiHeader?.LocalMBK68SettingsARM9;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.LocalMBK68SettingsARM7"/>
        public uint[] LocalMBK68SettingsARM7 => _model.ExtendedDSiHeader?.LocalMBK68SettingsARM7;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.GlobalMBK9Setting"/>
        public uint? GlobalMBK9Setting => _model.ExtendedDSiHeader?.GlobalMBK9Setting;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.RegionFlags"/>
        public uint? RegionFlags => _model.ExtendedDSiHeader?.RegionFlags;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.AccessControl"/>
        public uint? AccessControl => _model.ExtendedDSiHeader?.AccessControl;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7SCFGEXTMask"/>
        public uint? ARM7SCFGEXTMask => _model.ExtendedDSiHeader?.ARM7SCFGEXTMask;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ReservedFlags"/>
        public uint? ReservedFlags => _model.ExtendedDSiHeader?.ReservedFlags;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9iRomOffset"/>
        public uint? ARM9iRomOffset => _model.ExtendedDSiHeader?.ARM9iRomOffset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Reserved3"/>
        public uint? Reserved3 => _model.ExtendedDSiHeader?.Reserved3;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9iLoadAddress"/>
        public uint? ARM9iLoadAddress => _model.ExtendedDSiHeader?.ARM9iLoadAddress;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9iSize"/>
        public uint? ARM9iSize => _model.ExtendedDSiHeader?.ARM9iSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7iRomOffset"/>
        public uint? ARM7iRomOffset => _model.ExtendedDSiHeader?.ARM7iRomOffset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Reserved4"/>
        public uint? Reserved4 => _model.ExtendedDSiHeader?.Reserved4;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7iLoadAddress"/>
        public uint? ARM7iLoadAddress => _model.ExtendedDSiHeader?.ARM7iLoadAddress;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7iSize"/>
        public uint? ARM7iSize => _model.ExtendedDSiHeader?.ARM7iSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestNTRRegionOffset"/>
        public uint? DigestNTRRegionOffset => _model.ExtendedDSiHeader?.DigestNTRRegionOffset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestNTRRegionLength"/>
        public uint? DigestNTRRegionLength => _model.ExtendedDSiHeader?.DigestNTRRegionLength;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestTWLRegionOffset"/>
        public uint? DigestTWLRegionOffset => _model.ExtendedDSiHeader?.DigestTWLRegionOffset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestTWLRegionLength"/>
        public uint? DigestTWLRegionLength => _model.ExtendedDSiHeader?.DigestTWLRegionLength;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestSectorHashtableRegionOffset"/>
        public uint? DigestSectorHashtableRegionOffset => _model.ExtendedDSiHeader?.DigestSectorHashtableRegionOffset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestSectorHashtableRegionLength"/>
        public uint? DigestSectorHashtableRegionLength => _model.ExtendedDSiHeader?.DigestSectorHashtableRegionLength;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestBlockHashtableRegionOffset"/>
        public uint? DigestBlockHashtableRegionOffset => _model.ExtendedDSiHeader?.DigestBlockHashtableRegionOffset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestBlockHashtableRegionLength"/>
        public uint? DigestBlockHashtableRegionLength => _model.ExtendedDSiHeader?.DigestBlockHashtableRegionLength;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestSectorSize"/>
        public uint? DigestSectorSize => _model.ExtendedDSiHeader?.DigestSectorSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestBlockSectorCount"/>
        public uint? DigestBlockSectorCount => _model.ExtendedDSiHeader?.DigestBlockSectorCount;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.IconBannerSize"/>
        public uint? IconBannerSize => _model.ExtendedDSiHeader?.IconBannerSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Unknown1"/>
        public uint? Unknown1 => _model.ExtendedDSiHeader?.Unknown1;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ModcryptArea1Offset"/>
        public uint? ModcryptArea1Offset => _model.ExtendedDSiHeader?.ModcryptArea1Offset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ModcryptArea1Size"/>
        public uint? ModcryptArea1Size => _model.ExtendedDSiHeader?.ModcryptArea1Size;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ModcryptArea2Offset"/>
        public uint? ModcryptArea2Offset => _model.ExtendedDSiHeader?.ModcryptArea2Offset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ModcryptArea2Size"/>
        public uint? ModcryptArea2Size => _model.ExtendedDSiHeader?.ModcryptArea2Size;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.TitleID"/>
        public byte[] TitleID => _model.ExtendedDSiHeader?.TitleID;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DSiWarePublicSavSize"/>
        public uint? DSiWarePublicSavSize => _model.ExtendedDSiHeader?.DSiWarePublicSavSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DSiWarePrivateSavSize"/>
        public uint? DSiWarePrivateSavSize => _model.ExtendedDSiHeader?.DSiWarePrivateSavSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ReservedZero"/>
        public byte[] ReservedZero => _model.ExtendedDSiHeader?.ReservedZero;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Unknown2"/>
        public byte[] Unknown2 => _model.ExtendedDSiHeader?.Unknown2;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9WithSecureAreaSHA1HMACHash"/>
        public byte[] ARM9WithSecureAreaSHA1HMACHash => _model.ExtendedDSiHeader?.ARM9WithSecureAreaSHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7SHA1HMACHash"/>
        public byte[] ARM7SHA1HMACHash => _model.ExtendedDSiHeader?.ARM7SHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestMasterSHA1HMACHash"/>
        public byte[] DigestMasterSHA1HMACHash => _model.ExtendedDSiHeader?.DigestMasterSHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.BannerSHA1HMACHash"/>
        public byte[] BannerSHA1HMACHash => _model.ExtendedDSiHeader?.BannerSHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9iDecryptedSHA1HMACHash"/>
        public byte[] ARM9iDecryptedSHA1HMACHash => _model.ExtendedDSiHeader?.ARM9iDecryptedSHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7iDecryptedSHA1HMACHash"/>
        public byte[] ARM7iDecryptedSHA1HMACHash => _model.ExtendedDSiHeader?.ARM7iDecryptedSHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Reserved5"/>
        public byte[] Reserved5 => _model.ExtendedDSiHeader?.Reserved5;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9NoSecureAreaSHA1HMACHash"/>
        public byte[] ARM9NoSecureAreaSHA1HMACHash => _model.ExtendedDSiHeader?.ARM9NoSecureAreaSHA1HMACHash;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Reserved6"/>
        public byte[] Reserved6 => _model.ExtendedDSiHeader?.Reserved6;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ReservedAndUnchecked"/>
        public byte[] ReservedAndUnchecked => _model.ExtendedDSiHeader?.ReservedAndUnchecked;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.RSASignature"/>
        public byte[] RSASignature => _model.ExtendedDSiHeader?.RSASignature;

        #endregion

        #region Secure Area

        /// <inheritdoc cref="Models.Nitro.Cart.SecureArea"/>
        public byte[] SecureArea => _model.SecureArea;

        #endregion

        #region Name Table

        /// <inheritdoc cref="Models.Nitro.NameTable.FolderAllocationTable"/>
        public SabreTools.Models.Nitro.FolderAllocationTableEntry[] FolderAllocationTable => _model.NameTable.FolderAllocationTable;

        /// <inheritdoc cref="Models.Nitro.NameTable.NameList"/>
        public SabreTools.Models.Nitro.NameListEntry[] NameList => _model.NameTable.NameList;

        #endregion

        #region File Allocation Table

        /// <inheritdoc cref="Models.Nitro.Cart.FileAllocationTable"/>
        public SabreTools.Models.Nitro.FileAllocationTableEntry[] FileAllocationTable => _model.FileAllocationTable;

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public Nitro(SabreTools.Models.Nitro.Cart model, byte[] data, int offset)
#else
        public Nitro(SabreTools.Models.Nitro.Cart? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public Nitro(SabreTools.Models.Nitro.Cart model, Stream data)
#else
        public Nitro(SabreTools.Models.Nitro.Cart? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

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

            var archive = new SabreTools.Serialization.Streams.Nitro().Deserialize(data);
            if (archive == null)
                return null;

            try
            {
                return new Nitro(archive, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("NDS Cart Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintCommonHeader(builder);
            PrintExtendedDSiHeader(builder);
            PrintSecureArea(builder);
            PrintNameTable(builder);
            PrintFileAllocationTable(builder);

            return builder;
        }

        /// <summary>
        /// Print common header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintCommonHeader(StringBuilder builder)
        {
            builder.AppendLine("  Common Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Game title: {GameTitle ?? "[NULL]"}");
            builder.AppendLine($"  Game code: {GameCode} (0x{GameCode:X})");
            builder.AppendLine($"  Maker code: {MakerCode ?? "[NULL]"}");
            builder.AppendLine($"  Unit code: {UnitCode} (0x{UnitCode:X})");
            builder.AppendLine($"  Encryption seed select: {EncryptionSeedSelect} (0x{EncryptionSeedSelect:X})");
            builder.AppendLine($"  Device capacity: {DeviceCapacity} (0x{DeviceCapacity:X})");
            builder.AppendLine($"  Reserved 1: {BitConverter.ToString(Reserved1).Replace('-', ' ')}");
            builder.AppendLine($"  Game revision: {GameRevision} (0x{GameRevision:X})");
            builder.AppendLine($"  Rom version: {RomVersion} (0x{RomVersion:X})");
            builder.AppendLine($"  ARM9 rom offset: {ARM9RomOffset} (0x{ARM9RomOffset:X})");
            builder.AppendLine($"  ARM9 entry address: {ARM9EntryAddress} (0x{ARM9EntryAddress:X})");
            builder.AppendLine($"  ARM9 load address: {ARM9LoadAddress} (0x{ARM9LoadAddress:X})");
            builder.AppendLine($"  ARM9 size: {ARM9Size} (0x{ARM9Size:X})");
            builder.AppendLine($"  ARM7 rom offset: {ARM7RomOffset} (0x{ARM7RomOffset:X})");
            builder.AppendLine($"  ARM7 entry address: {ARM7EntryAddress} (0x{ARM7EntryAddress:X})");
            builder.AppendLine($"  ARM7 load address: {ARM7LoadAddress} (0x{ARM7LoadAddress:X})");
            builder.AppendLine($"  ARM7 size: {ARM7Size} (0x{ARM7Size:X})");
            builder.AppendLine($"  File name table offset: {FileNameTableOffset} (0x{FileNameTableOffset:X})");
            builder.AppendLine($"  File name table length: {FileNameTableLength} (0x{FileNameTableLength:X})");
            builder.AppendLine($"  File allocation table offset: {FileAllocationTableOffset} (0x{FileAllocationTableOffset:X})");
            builder.AppendLine($"  File allocation table length: {FileAllocationTableLength} (0x{FileAllocationTableLength:X})");
            builder.AppendLine($"  ARM9 overlay offset: {ARM9OverlayOffset} (0x{ARM9OverlayOffset:X})");
            builder.AppendLine($"  ARM9 overlay length: {ARM9OverlayLength} (0x{ARM9OverlayLength:X})");
            builder.AppendLine($"  ARM7 overlay offset: {ARM7OverlayOffset} (0x{ARM7OverlayOffset:X})");
            builder.AppendLine($"  ARM7 overlay length: {ARM7OverlayLength} (0x{ARM7OverlayLength:X})");
            builder.AppendLine($"  Normal card control register settings: {NormalCardControlRegisterSettings} (0x{NormalCardControlRegisterSettings:X})");
            builder.AppendLine($"  Secure card control register settings: {SecureCardControlRegisterSettings} (0x{SecureCardControlRegisterSettings:X})");
            builder.AppendLine($"  Icon banner offset: {IconBannerOffset} (0x{IconBannerOffset:X})");
            builder.AppendLine($"  Secure area CRC: {SecureAreaCRC} (0x{SecureAreaCRC:X})");
            builder.AppendLine($"  Secure transfer timeout: {SecureTransferTimeout} (0x{SecureTransferTimeout:X})");
            builder.AppendLine($"  ARM9 autoload: {ARM9Autoload} (0x{ARM9Autoload:X})");
            builder.AppendLine($"  ARM7 autoload: {ARM7Autoload} (0x{ARM7Autoload:X})");
            builder.AppendLine($"  Secure disable: {SecureDisable} (0x{SecureDisable:X})");
            builder.AppendLine($"  NTR region rom size: {NTRRegionRomSize} (0x{NTRRegionRomSize:X})");
            builder.AppendLine($"  Header size: {HeaderSize} (0x{HeaderSize:X})");
            builder.AppendLine($"  Reserved 2: {BitConverter.ToString(Reserved2).Replace('-', ' ')}");
            builder.AppendLine($"  Nintendo logo: {BitConverter.ToString(NintendoLogo).Replace('-', ' ')}");
            builder.AppendLine($"  Nintendo logo CRC: {NintendoLogoCRC} (0x{NintendoLogoCRC:X})");
            builder.AppendLine($"  Header CRC: {HeaderCRC} (0x{HeaderCRC:X})");
            builder.AppendLine($"  Debugger reserved: {BitConverter.ToString(DebuggerReserved).Replace('-', ' ')}");
            builder.AppendLine();
        }

        /// <summary>
        /// Print extended DSi header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintExtendedDSiHeader(StringBuilder builder)
        {
            builder.AppendLine("  Extended DSi Header Information:");
            builder.AppendLine("  -------------------------");
            if (_model.ExtendedDSiHeader == null)
            {
                builder.AppendLine("  No extended DSi header");
            }
            else
            {
                builder.AppendLine($"  Global MBK1..MBK5 settings: {string.Join(", ", GlobalMBK15Settings)}");
                builder.AppendLine($"  Local MBK6..MBK8 settings for ARM9: {string.Join(", ", LocalMBK68SettingsARM9)}");
                builder.AppendLine($"  Local MBK6..MBK8 settings for ARM7: {string.Join(", ", LocalMBK68SettingsARM7)}");
                builder.AppendLine($"  Global MBK9 setting: {GlobalMBK9Setting} (0x{GlobalMBK9Setting:X})");
                builder.AppendLine($"  Region flags: {RegionFlags} (0x{RegionFlags:X})");
                builder.AppendLine($"  Access control: {AccessControl} (0x{AccessControl:X})");
                builder.AppendLine($"  ARM7 SCFG EXT mask: {ARM7SCFGEXTMask} (0x{ARM7SCFGEXTMask:X})");
                builder.AppendLine($"  Reserved/flags?: {ReservedFlags} (0x{ReservedFlags:X})");
                builder.AppendLine($"  ARM9i rom offset: {ARM9iRomOffset} (0x{ARM9iRomOffset:X})");
                builder.AppendLine($"  Reserved 3: {Reserved3} (0x{Reserved3:X})");
                builder.AppendLine($"  ARM9i load address: {ARM9iLoadAddress} (0x{ARM9iLoadAddress:X})");
                builder.AppendLine($"  ARM9i size: {ARM9iSize} (0x{ARM9iSize:X})");
                builder.AppendLine($"  ARM7i rom offset: {ARM7iRomOffset} (0x{ARM7iRomOffset:X})");
                builder.AppendLine($"  Reserved 4: {Reserved4} (0x{Reserved4:X})");
                builder.AppendLine($"  ARM7i load address: {ARM7iLoadAddress} (0x{ARM7iLoadAddress:X})");
                builder.AppendLine($"  ARM7i size: {ARM7iSize} (0x{ARM7iSize:X})");
                builder.AppendLine($"  Digest NTR region offset: {DigestNTRRegionOffset} (0x{DigestNTRRegionOffset:X})");
                builder.AppendLine($"  Digest NTR region length: {DigestNTRRegionLength} (0x{DigestNTRRegionLength:X})");
                builder.AppendLine($"  Digest TWL region offset: {DigestTWLRegionOffset} (0x{DigestTWLRegionOffset:X})");
                builder.AppendLine($"  Digest TWL region length: {DigestTWLRegionLength} (0x{DigestTWLRegionLength:X})");
                builder.AppendLine($"  Digest sector hashtable region offset: {DigestSectorHashtableRegionOffset} (0x{DigestSectorHashtableRegionOffset:X})");
                builder.AppendLine($"  Digest sector hashtable region length: {DigestSectorHashtableRegionLength} (0x{DigestSectorHashtableRegionLength:X})");
                builder.AppendLine($"  Digest block hashtable region offset: {DigestBlockHashtableRegionOffset} (0x{DigestBlockHashtableRegionOffset:X})");
                builder.AppendLine($"  Digest block hashtable region length: {DigestBlockHashtableRegionLength} (0x{DigestBlockHashtableRegionLength:X})");
                builder.AppendLine($"  Digest sector size: {DigestSectorSize} (0x{DigestSectorSize:X})");
                builder.AppendLine($"  Digest block sector count: {DigestBlockSectorCount} (0x{DigestBlockSectorCount:X})");
                builder.AppendLine($"  Icon banner size: {IconBannerSize} (0x{IconBannerSize:X})");
                builder.AppendLine($"  Unknown 1: {Unknown1} (0x{Unknown1:X})");
                builder.AppendLine($"  Modcrypt area 1 offset: {ModcryptArea1Offset} (0x{ModcryptArea1Offset:X})");
                builder.AppendLine($"  Modcrypt area 1 size: {ModcryptArea1Size} (0x{ModcryptArea1Size:X})");
                builder.AppendLine($"  Modcrypt area 2 offset: {ModcryptArea2Offset} (0x{ModcryptArea2Offset:X})");
                builder.AppendLine($"  Modcrypt area 2 size: {ModcryptArea2Size} (0x{ModcryptArea2Size:X})");
                builder.AppendLine($"  Title ID: {BitConverter.ToString(TitleID).Replace('-', ' ')}");
                builder.AppendLine($"  DSiWare 'public.sav' size: {DSiWarePublicSavSize} (0x{DSiWarePublicSavSize:X})");
                builder.AppendLine($"  DSiWare 'private.sav' size: {DSiWarePrivateSavSize} (0x{DSiWarePrivateSavSize:X})");
                builder.AppendLine($"  Reserved (zero): {BitConverter.ToString(ReservedZero).Replace('-', ' ')}");
                builder.AppendLine($"  Unknown 2: {BitConverter.ToString(Unknown2).Replace('-', ' ')}");
                builder.AppendLine($"  ARM9 (with encrypted secure area) SHA1 HMAC hash: {BitConverter.ToString(ARM9WithSecureAreaSHA1HMACHash).Replace('-', ' ')}");
                builder.AppendLine($"  ARM7 SHA1 HMAC hash: {BitConverter.ToString(ARM7SHA1HMACHash).Replace('-', ' ')}");
                builder.AppendLine($"  Digest master SHA1 HMAC hash: {BitConverter.ToString(DigestMasterSHA1HMACHash).Replace('-', ' ')}");
                builder.AppendLine($"  Banner SHA1 HMAC hash: {BitConverter.ToString(BannerSHA1HMACHash).Replace('-', ' ')}");
                builder.AppendLine($"  ARM9i (decrypted) SHA1 HMAC hash: {BitConverter.ToString(ARM9iDecryptedSHA1HMACHash).Replace('-', ' ')}");
                builder.AppendLine($"  ARM7i (decrypted) SHA1 HMAC hash: {BitConverter.ToString(ARM7iDecryptedSHA1HMACHash).Replace('-', ' ')}");
                builder.AppendLine($"  Reserved 5: {BitConverter.ToString(Reserved5).Replace('-', ' ')}");
                builder.AppendLine($"  ARM9 (without secure area) SHA1 HMAC hash: {BitConverter.ToString(ARM9NoSecureAreaSHA1HMACHash).Replace('-', ' ')}");
                builder.AppendLine($"  Reserved 6: {BitConverter.ToString(Reserved6).Replace('-', ' ')}");
                builder.AppendLine($"  Reserved and unchecked region: {BitConverter.ToString(ReservedAndUnchecked).Replace('-', ' ')}");
                builder.AppendLine($"  RSA signature: {BitConverter.ToString(RSASignature).Replace('-', ' ')}");
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print secure area information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintSecureArea(StringBuilder builder)
        {
            builder.AppendLine("  Secure Area Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  {BitConverter.ToString(SecureArea).Replace('-', ' ')}");
            builder.AppendLine();
        }

        /// <summary>
        /// Print name table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintNameTable(StringBuilder builder)
        {
            builder.AppendLine("  Name Table Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine();

            PrintFolderAllocationTable(builder);
            PrintNameList(builder);
        }

        /// <summary>
        /// Print folder allocation table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFolderAllocationTable(StringBuilder builder)
        {
            builder.AppendLine($"  Folder Allocation Table:");
            builder.AppendLine("  -------------------------");
            if (FolderAllocationTable == null || FolderAllocationTable.Length == 0)
            {
                builder.AppendLine("  No folder allocation table entries");
            }
            else
            {
                for (int i = 0; i < FolderAllocationTable.Length; i++)
                {
                    var entry = FolderAllocationTable[i];
                    builder.AppendLine($"  Folder Allocation Table Entry {i}");
                    builder.AppendLine($"    Start offset: {entry.StartOffset} (0x{entry.StartOffset:X})");
                    builder.AppendLine($"    First file index: {entry.FirstFileIndex} (0x{entry.FirstFileIndex:X})");
                    if (entry.Unknown == 0xF0)
                    {
                        builder.AppendLine($"    Parent folder index: {entry.ParentFolderIndex} (0x{entry.ParentFolderIndex:X})");
                        builder.AppendLine($"    Unknown: {entry.Unknown} (0x{entry.Unknown:X})");
                    }
                    else
                    {
                        ushort totalEntries = (ushort)((entry.Unknown << 8) | entry.ParentFolderIndex);
                        builder.AppendLine($"    Total entries: {totalEntries} (0x{totalEntries:X})");
                    }
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print folder allocation table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintNameList(StringBuilder builder)
        {
            builder.AppendLine($"  Name List:");
            builder.AppendLine("  -------------------------");
            if (NameList == null || NameList.Length == 0)
            {
                builder.AppendLine("  No name list entries");
            }
            else
            {
                for (int i = 0; i < NameList.Length; i++)
                {
                    var entry = NameList[i];
                    builder.AppendLine($"  Name List Entry {i}");
                    builder.AppendLine($"    Folder: {entry.Folder} (0x{entry.Folder:X})");
                    builder.AppendLine($"    Name: {entry.Name ?? "[NULL]"}");
                    if (entry.Folder)
                        builder.AppendLine($"    Index: {entry.Index} (0x{entry.Index:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print file allocation table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFileAllocationTable(StringBuilder builder)
        {
            builder.AppendLine($"  File Allocation Table:");
            builder.AppendLine("  -------------------------");
            if (FileAllocationTable == null || FileAllocationTable.Length == 0)
            {
                builder.AppendLine("  No file allocation table entries");
            }
            else
            {
                for (int i = 0; i < FileAllocationTable.Length; i++)
                {
                    var entry = FileAllocationTable[i];
                    builder.AppendLine($"  File Allocation Table Entry {i}");
                    builder.AppendLine($"    Start offset: {entry.StartOffset} (0x{entry.StartOffset:X})");
                    builder.AppendLine($"    End offset: {entry.EndOffset} (0x{entry.EndOffset:X})");
                }
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}