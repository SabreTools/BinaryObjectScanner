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
#if NET48
        public string GameTitle => this.Model.CommonHeader.GameTitle;
#else
        public string? GameTitle => this.Model.CommonHeader?.GameTitle;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.GameCode"/>
#if NET48
        public uint GameCode => this.Model.CommonHeader.GameCode;
#else
        public uint? GameCode => this.Model.CommonHeader?.GameCode;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.MakerCode"/>
#if NET48
        public string MakerCode => this.Model.CommonHeader.MakerCode;
#else
        public string? MakerCode => this.Model.CommonHeader?.MakerCode;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.UnitCode"/>
#if NET48
        public SabreTools.Models.Nitro.Unitcode UnitCode => this.Model.CommonHeader.UnitCode;
#else
        public SabreTools.Models.Nitro.Unitcode? UnitCode => this.Model.CommonHeader?.UnitCode;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.EncryptionSeedSelect"/>
#if NET48
        public byte EncryptionSeedSelect => this.Model.CommonHeader.EncryptionSeedSelect;
#else
        public byte? EncryptionSeedSelect => this.Model.CommonHeader?.EncryptionSeedSelect;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.DeviceCapacity"/>
#if NET48
        public byte DeviceCapacity => this.Model.CommonHeader.DeviceCapacity;
#else
        public byte? DeviceCapacity => this.Model.CommonHeader?.DeviceCapacity;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.Reserved1"/>
#if NET48
        public byte[] Reserved1 => this.Model.CommonHeader.Reserved1;
#else
        public byte[]? Reserved1 => this.Model.CommonHeader?.Reserved1;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.GameRevision"/>
#if NET48
        public ushort GameRevision => this.Model.CommonHeader.GameRevision;
#else
        public ushort? GameRevision => this.Model.CommonHeader?.GameRevision;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.RomVersion"/>
#if NET48
        public byte RomVersion => this.Model.CommonHeader.RomVersion;
#else
        public byte? RomVersion => this.Model.CommonHeader?.RomVersion;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.InternalFlags"/>
#if NET48
        public byte InternalFlags => this.Model.CommonHeader.InternalFlags;
#else
        public byte? InternalFlags => this.Model.CommonHeader?.InternalFlags;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9RomOffset"/>
#if NET48
        public uint ARM9RomOffset => this.Model.CommonHeader.ARM9RomOffset;
#else
        public uint? ARM9RomOffset => this.Model.CommonHeader?.ARM9RomOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9EntryAddress"/>
#if NET48
        public uint ARM9EntryAddress => this.Model.CommonHeader.ARM9EntryAddress;
#else
        public uint? ARM9EntryAddress => this.Model.CommonHeader?.ARM9EntryAddress;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9LoadAddress"/>
#if NET48
        public uint ARM9LoadAddress => this.Model.CommonHeader.ARM9LoadAddress;
#else
        public uint? ARM9LoadAddress => this.Model.CommonHeader?.ARM9LoadAddress;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9Size"/>
#if NET48
        public uint ARM9Size => this.Model.CommonHeader.ARM9Size;
#else
        public uint? ARM9Size => this.Model.CommonHeader?.ARM9Size;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7RomOffset"/>
#if NET48
        public uint ARM7RomOffset => this.Model.CommonHeader.ARM7RomOffset;
#else
        public uint? ARM7RomOffset => this.Model.CommonHeader?.ARM7RomOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7EntryAddress"/>
#if NET48
        public uint ARM7EntryAddress => this.Model.CommonHeader.ARM7EntryAddress;
#else
        public uint? ARM7EntryAddress => this.Model.CommonHeader?.ARM7EntryAddress;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7LoadAddress"/>
#if NET48
        public uint ARM7LoadAddress => this.Model.CommonHeader.ARM7LoadAddress;
#else
        public uint? ARM7LoadAddress => this.Model.CommonHeader?.ARM7LoadAddress;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7Size"/>
#if NET48
        public uint ARM7Size => this.Model.CommonHeader.ARM7Size;
#else
        public uint? ARM7Size => this.Model.CommonHeader?.ARM7Size;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.FileNameTableOffset"/>
#if NET48
        public uint FileNameTableOffset => this.Model.CommonHeader.FileNameTableOffset;
#else
        public uint? FileNameTableOffset => this.Model.CommonHeader?.FileNameTableOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.FileNameTableLength"/>
#if NET48
        public uint FileNameTableLength => this.Model.CommonHeader.FileNameTableLength;
#else
        public uint? FileNameTableLength => this.Model.CommonHeader?.FileNameTableLength;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.FileAllocationTableOffset"/>
#if NET48
        public uint FileAllocationTableOffset => this.Model.CommonHeader.FileAllocationTableOffset;
#else
        public uint? FileAllocationTableOffset => this.Model.CommonHeader?.FileAllocationTableOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.FileAllocationTableLength"/>
#if NET48
        public uint FileAllocationTableLength => this.Model.CommonHeader.FileAllocationTableLength;
#else
        public uint? FileAllocationTableLength => this.Model.CommonHeader?.FileAllocationTableLength;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9OverlayOffset"/>
#if NET48
        public uint ARM9OverlayOffset => this.Model.CommonHeader.ARM9OverlayOffset;
#else
        public uint? ARM9OverlayOffset => this.Model.CommonHeader?.ARM9OverlayOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9OverlayLength"/>
#if NET48
        public uint ARM9OverlayLength => this.Model.CommonHeader.ARM9OverlayLength;
#else
        public uint? ARM9OverlayLength => this.Model.CommonHeader?.ARM9OverlayLength;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7OverlayOffset"/>
#if NET48
        public uint ARM7OverlayOffset => this.Model.CommonHeader.ARM7OverlayOffset;
#else
        public uint? ARM7OverlayOffset => this.Model.CommonHeader?.ARM7OverlayOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7OverlayLength"/>
#if NET48
        public uint ARM7OverlayLength => this.Model.CommonHeader.ARM7OverlayLength;
#else
        public uint? ARM7OverlayLength => this.Model.CommonHeader?.ARM7OverlayLength;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.NormalCardControlRegisterSettings"/>
#if NET48
        public uint NormalCardControlRegisterSettings => this.Model.CommonHeader.NormalCardControlRegisterSettings;
#else
        public uint? NormalCardControlRegisterSettings => this.Model.CommonHeader?.NormalCardControlRegisterSettings;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.SecureCardControlRegisterSettings"/>
#if NET48
        public uint SecureCardControlRegisterSettings => this.Model.CommonHeader.SecureCardControlRegisterSettings;
#else
        public uint? SecureCardControlRegisterSettings => this.Model.CommonHeader?.SecureCardControlRegisterSettings;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.IconBannerOffset"/>
#if NET48
        public uint IconBannerOffset => this.Model.CommonHeader.IconBannerOffset;
#else
        public uint? IconBannerOffset => this.Model.CommonHeader?.IconBannerOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.SecureAreaCRC"/>
#if NET48
        public ushort SecureAreaCRC => this.Model.CommonHeader.SecureAreaCRC;
#else
        public ushort? SecureAreaCRC => this.Model.CommonHeader?.SecureAreaCRC;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.SecureTransferTimeout"/>
#if NET48
        public ushort SecureTransferTimeout => this.Model.CommonHeader.SecureTransferTimeout;
#else
        public ushort? SecureTransferTimeout => this.Model.CommonHeader?.SecureTransferTimeout;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9Autoload"/>
#if NET48
        public uint ARM9Autoload => this.Model.CommonHeader.ARM9Autoload;
#else
        public uint? ARM9Autoload => this.Model.CommonHeader?.ARM9Autoload;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7Autoload"/>
#if NET48
        public uint ARM7Autoload => this.Model.CommonHeader.ARM7Autoload;
#else
        public uint? ARM7Autoload => this.Model.CommonHeader?.ARM7Autoload;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.SecureDisable"/>
#if NET48
        public byte[] SecureDisable => this.Model.CommonHeader.SecureDisable;
#else
        public byte[]? SecureDisable => this.Model.CommonHeader?.SecureDisable;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.NTRRegionRomSize"/>
#if NET48
        public uint NTRRegionRomSize => this.Model.CommonHeader.NTRRegionRomSize;
#else
        public uint? NTRRegionRomSize => this.Model.CommonHeader?.NTRRegionRomSize;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.HeaderSize"/>
#if NET48
        public uint HeaderSize => this.Model.CommonHeader.HeaderSize;
#else
        public uint? HeaderSize => this.Model.CommonHeader?.HeaderSize;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.Reserved2"/>
#if NET48
        public byte[] Reserved2 => this.Model.CommonHeader.Reserved2;
#else
        public byte[]? Reserved2 => this.Model.CommonHeader?.Reserved2;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.NintendoLogo"/>
#if NET48
        public byte[] NintendoLogo => this.Model.CommonHeader.NintendoLogo;
#else
        public byte[]? NintendoLogo => this.Model.CommonHeader?.NintendoLogo;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.NintendoLogoCRC"/>
#if NET48
        public ushort NintendoLogoCRC => this.Model.CommonHeader.NintendoLogoCRC;
#else
        public ushort? NintendoLogoCRC => this.Model.CommonHeader?.NintendoLogoCRC;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.HeaderCRC"/>
#if NET48
        public ushort HeaderCRC => this.Model.CommonHeader.HeaderCRC;
#else
        public ushort? HeaderCRC => this.Model.CommonHeader?.HeaderCRC;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.DebuggerReserved"/>
#if NET48
        public byte[] DebuggerReserved => this.Model.CommonHeader.DebuggerReserved;
#else
        public byte[]? DebuggerReserved => this.Model.CommonHeader?.DebuggerReserved;
#endif

        #endregion

        #region Extended DSi Header

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.GlobalMBK15Settings"/>
#if NET48
        public uint[] GlobalMBK15Settings => this.Model.ExtendedDSiHeader?.GlobalMBK15Settings;
#else
        public uint[]? GlobalMBK15Settings => this.Model.ExtendedDSiHeader?.GlobalMBK15Settings;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.LocalMBK68SettingsARM9"/>
#if NET48
        public uint[] LocalMBK68SettingsARM9 => this.Model.ExtendedDSiHeader?.LocalMBK68SettingsARM9;
#else
        public uint[]? LocalMBK68SettingsARM9 => this.Model.ExtendedDSiHeader?.LocalMBK68SettingsARM9;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.LocalMBK68SettingsARM7"/>
#if NET48
        public uint[] LocalMBK68SettingsARM7 => this.Model.ExtendedDSiHeader?.LocalMBK68SettingsARM7;
#else
        public uint[]? LocalMBK68SettingsARM7 => this.Model.ExtendedDSiHeader?.LocalMBK68SettingsARM7;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.GlobalMBK9Setting"/>
        public uint? GlobalMBK9Setting => this.Model.ExtendedDSiHeader?.GlobalMBK9Setting;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.RegionFlags"/>
        public uint? RegionFlags => this.Model.ExtendedDSiHeader?.RegionFlags;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.AccessControl"/>
        public uint? AccessControl => this.Model.ExtendedDSiHeader?.AccessControl;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7SCFGEXTMask"/>
        public uint? ARM7SCFGEXTMask => this.Model.ExtendedDSiHeader?.ARM7SCFGEXTMask;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ReservedFlags"/>
        public uint? ReservedFlags => this.Model.ExtendedDSiHeader?.ReservedFlags;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9iRomOffset"/>
        public uint? ARM9iRomOffset => this.Model.ExtendedDSiHeader?.ARM9iRomOffset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Reserved3"/>
        public uint? Reserved3 => this.Model.ExtendedDSiHeader?.Reserved3;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9iLoadAddress"/>
        public uint? ARM9iLoadAddress => this.Model.ExtendedDSiHeader?.ARM9iLoadAddress;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9iSize"/>
        public uint? ARM9iSize => this.Model.ExtendedDSiHeader?.ARM9iSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7iRomOffset"/>
        public uint? ARM7iRomOffset => this.Model.ExtendedDSiHeader?.ARM7iRomOffset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Reserved4"/>
        public uint? Reserved4 => this.Model.ExtendedDSiHeader?.Reserved4;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7iLoadAddress"/>
        public uint? ARM7iLoadAddress => this.Model.ExtendedDSiHeader?.ARM7iLoadAddress;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7iSize"/>
        public uint? ARM7iSize => this.Model.ExtendedDSiHeader?.ARM7iSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestNTRRegionOffset"/>
        public uint? DigestNTRRegionOffset => this.Model.ExtendedDSiHeader?.DigestNTRRegionOffset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestNTRRegionLength"/>
        public uint? DigestNTRRegionLength => this.Model.ExtendedDSiHeader?.DigestNTRRegionLength;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestTWLRegionOffset"/>
        public uint? DigestTWLRegionOffset => this.Model.ExtendedDSiHeader?.DigestTWLRegionOffset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestTWLRegionLength"/>
        public uint? DigestTWLRegionLength => this.Model.ExtendedDSiHeader?.DigestTWLRegionLength;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestSectorHashtableRegionOffset"/>
        public uint? DigestSectorHashtableRegionOffset => this.Model.ExtendedDSiHeader?.DigestSectorHashtableRegionOffset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestSectorHashtableRegionLength"/>
        public uint? DigestSectorHashtableRegionLength => this.Model.ExtendedDSiHeader?.DigestSectorHashtableRegionLength;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestBlockHashtableRegionOffset"/>
        public uint? DigestBlockHashtableRegionOffset => this.Model.ExtendedDSiHeader?.DigestBlockHashtableRegionOffset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestBlockHashtableRegionLength"/>
        public uint? DigestBlockHashtableRegionLength => this.Model.ExtendedDSiHeader?.DigestBlockHashtableRegionLength;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestSectorSize"/>
        public uint? DigestSectorSize => this.Model.ExtendedDSiHeader?.DigestSectorSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestBlockSectorCount"/>
        public uint? DigestBlockSectorCount => this.Model.ExtendedDSiHeader?.DigestBlockSectorCount;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.IconBannerSize"/>
        public uint? IconBannerSize => this.Model.ExtendedDSiHeader?.IconBannerSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Unknown1"/>
        public uint? Unknown1 => this.Model.ExtendedDSiHeader?.Unknown1;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ModcryptArea1Offset"/>
        public uint? ModcryptArea1Offset => this.Model.ExtendedDSiHeader?.ModcryptArea1Offset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ModcryptArea1Size"/>
        public uint? ModcryptArea1Size => this.Model.ExtendedDSiHeader?.ModcryptArea1Size;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ModcryptArea2Offset"/>
        public uint? ModcryptArea2Offset => this.Model.ExtendedDSiHeader?.ModcryptArea2Offset;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ModcryptArea2Size"/>
        public uint? ModcryptArea2Size => this.Model.ExtendedDSiHeader?.ModcryptArea2Size;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.TitleID"/>
#if NET48
        public byte[] TitleID => this.Model.ExtendedDSiHeader?.TitleID;
#else
        public byte[]? TitleID => this.Model.ExtendedDSiHeader?.TitleID;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DSiWarePublicSavSize"/>
        public uint? DSiWarePublicSavSize => this.Model.ExtendedDSiHeader?.DSiWarePublicSavSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DSiWarePrivateSavSize"/>
        public uint? DSiWarePrivateSavSize => this.Model.ExtendedDSiHeader?.DSiWarePrivateSavSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ReservedZero"/>
#if NET48
        public byte[] ReservedZero => this.Model.ExtendedDSiHeader?.ReservedZero;
#else
        public byte[]? ReservedZero => this.Model.ExtendedDSiHeader?.ReservedZero;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Unknown2"/>
#if NET48
        public byte[] Unknown2 => this.Model.ExtendedDSiHeader?.Unknown2;
#else
        public byte[]? Unknown2 => this.Model.ExtendedDSiHeader?.Unknown2;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9WithSecureAreaSHA1HMACHash"/>
#if NET48
        public byte[] ARM9WithSecureAreaSHA1HMACHash => this.Model.ExtendedDSiHeader?.ARM9WithSecureAreaSHA1HMACHash;
#else
        public byte[]? ARM9WithSecureAreaSHA1HMACHash => this.Model.ExtendedDSiHeader?.ARM9WithSecureAreaSHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7SHA1HMACHash"/>
#if NET48
        public byte[] ARM7SHA1HMACHash => this.Model.ExtendedDSiHeader?.ARM7SHA1HMACHash;
#else
        public byte[]? ARM7SHA1HMACHash => this.Model.ExtendedDSiHeader?.ARM7SHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestMasterSHA1HMACHash"/>
#if NET48
        public byte[] DigestMasterSHA1HMACHash => this.Model.ExtendedDSiHeader?.DigestMasterSHA1HMACHash;
#else
        public byte[]? DigestMasterSHA1HMACHash => this.Model.ExtendedDSiHeader?.DigestMasterSHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.BannerSHA1HMACHash"/>
#if NET48
        public byte[] BannerSHA1HMACHash => this.Model.ExtendedDSiHeader?.BannerSHA1HMACHash;
#else
        public byte[]? BannerSHA1HMACHash => this.Model.ExtendedDSiHeader?.BannerSHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9iDecryptedSHA1HMACHash"/>
#if NET48
        public byte[] ARM9iDecryptedSHA1HMACHash => this.Model.ExtendedDSiHeader?.ARM9iDecryptedSHA1HMACHash;
#else
        public byte[]? ARM9iDecryptedSHA1HMACHash => this.Model.ExtendedDSiHeader?.ARM9iDecryptedSHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7iDecryptedSHA1HMACHash"/>
#if NET48
        public byte[] ARM7iDecryptedSHA1HMACHash => this.Model.ExtendedDSiHeader?.ARM7iDecryptedSHA1HMACHash;
#else
        public byte[]? ARM7iDecryptedSHA1HMACHash => this.Model.ExtendedDSiHeader?.ARM7iDecryptedSHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Reserved5"/>
#if NET48
        public byte[] Reserved5 => this.Model.ExtendedDSiHeader?.Reserved5;
#else
        public byte[]? Reserved5 => this.Model.ExtendedDSiHeader?.Reserved5;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9NoSecureAreaSHA1HMACHash"/>
#if NET48
        public byte[] ARM9NoSecureAreaSHA1HMACHash => this.Model.ExtendedDSiHeader?.ARM9NoSecureAreaSHA1HMACHash;
#else
        public byte[]? ARM9NoSecureAreaSHA1HMACHash => this.Model.ExtendedDSiHeader?.ARM9NoSecureAreaSHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Reserved6"/>
#if NET48
        public byte[] Reserved6 => this.Model.ExtendedDSiHeader?.Reserved6;
#else
        public byte[]? Reserved6 => this.Model.ExtendedDSiHeader?.Reserved6;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ReservedAndUnchecked"/>
#if NET48
        public byte[] ReservedAndUnchecked => this.Model.ExtendedDSiHeader?.ReservedAndUnchecked;
#else
        public byte[]? ReservedAndUnchecked => this.Model.ExtendedDSiHeader?.ReservedAndUnchecked;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.RSASignature"/>
#if NET48
        public byte[] RSASignature => this.Model.ExtendedDSiHeader?.RSASignature;
#else
        public byte[]? RSASignature => this.Model.ExtendedDSiHeader?.RSASignature;
#endif

        #endregion

        #region Secure Area

        /// <inheritdoc cref="Models.Nitro.Cart.SecureArea"/>
#if NET48
        public byte[] SecureArea => this.Model.SecureArea;
#else
        public byte[]? SecureArea => this.Model.SecureArea;
#endif

        #endregion

        #region Name Table

        /// <inheritdoc cref="Models.Nitro.NameTable.FolderAllocationTable"/>
#if NET48
        public SabreTools.Models.Nitro.FolderAllocationTableEntry[] FolderAllocationTable => this.Model.NameTable.FolderAllocationTable;
#else
        public SabreTools.Models.Nitro.FolderAllocationTableEntry?[]? FolderAllocationTable => this.Model.NameTable?.FolderAllocationTable;
#endif

        /// <inheritdoc cref="Models.Nitro.NameTable.NameList"/>
#if NET48
        public SabreTools.Models.Nitro.NameListEntry[] NameList => this.Model.NameTable.NameList;
#else
        public SabreTools.Models.Nitro.NameListEntry?[]? NameList => this.Model.NameTable?.NameList;
#endif

        #endregion

        #region File Allocation Table

        /// <inheritdoc cref="Models.Nitro.Cart.FileAllocationTable"/>
#if NET48
        public SabreTools.Models.Nitro.FileAllocationTableEntry[] FileAllocationTable => this.Model.FileAllocationTable;
#else
        public SabreTools.Models.Nitro.FileAllocationTableEntry?[]? FileAllocationTable => this.Model.FileAllocationTable;
#endif

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
#if NET48
        public static Nitro Create(byte[] data, int offset)
#else
        public static Nitro? Create(byte[]? data, int offset)
#endif
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
#if NET48
        public static Nitro Create(Stream data)
#else
        public static Nitro? Create(Stream? data)
#endif
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
            Printing.Nitro.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}