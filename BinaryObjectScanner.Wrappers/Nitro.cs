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
        public string GameTitle => _model.CommonHeader.GameTitle;
#else
        public string? GameTitle => _model.CommonHeader?.GameTitle;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.GameCode"/>
#if NET48
        public uint GameCode => _model.CommonHeader.GameCode;
#else
        public uint? GameCode => _model.CommonHeader?.GameCode;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.MakerCode"/>
#if NET48
        public string MakerCode => _model.CommonHeader.MakerCode;
#else
        public string? MakerCode => _model.CommonHeader?.MakerCode;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.UnitCode"/>
#if NET48
        public SabreTools.Models.Nitro.Unitcode UnitCode => _model.CommonHeader.UnitCode;
#else
        public SabreTools.Models.Nitro.Unitcode? UnitCode => _model.CommonHeader?.UnitCode;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.EncryptionSeedSelect"/>
#if NET48
        public byte EncryptionSeedSelect => _model.CommonHeader.EncryptionSeedSelect;
#else
        public byte? EncryptionSeedSelect => _model.CommonHeader?.EncryptionSeedSelect;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.DeviceCapacity"/>
#if NET48
        public byte DeviceCapacity => _model.CommonHeader.DeviceCapacity;
#else
        public byte? DeviceCapacity => _model.CommonHeader?.DeviceCapacity;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.Reserved1"/>
#if NET48
        public byte[] Reserved1 => _model.CommonHeader.Reserved1;
#else
        public byte[]? Reserved1 => _model.CommonHeader?.Reserved1;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.GameRevision"/>
#if NET48
        public ushort GameRevision => _model.CommonHeader.GameRevision;
#else
        public ushort? GameRevision => _model.CommonHeader?.GameRevision;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.RomVersion"/>
#if NET48
        public byte RomVersion => _model.CommonHeader.RomVersion;
#else
        public byte? RomVersion => _model.CommonHeader?.RomVersion;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.InternalFlags"/>
#if NET48
        public byte InternalFlags => _model.CommonHeader.InternalFlags;
#else
        public byte? InternalFlags => _model.CommonHeader?.InternalFlags;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9RomOffset"/>
#if NET48
        public uint ARM9RomOffset => _model.CommonHeader.ARM9RomOffset;
#else
        public uint? ARM9RomOffset => _model.CommonHeader?.ARM9RomOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9EntryAddress"/>
#if NET48
        public uint ARM9EntryAddress => _model.CommonHeader.ARM9EntryAddress;
#else
        public uint? ARM9EntryAddress => _model.CommonHeader?.ARM9EntryAddress;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9LoadAddress"/>
#if NET48
        public uint ARM9LoadAddress => _model.CommonHeader.ARM9LoadAddress;
#else
        public uint? ARM9LoadAddress => _model.CommonHeader?.ARM9LoadAddress;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9Size"/>
#if NET48
        public uint ARM9Size => _model.CommonHeader.ARM9Size;
#else
        public uint? ARM9Size => _model.CommonHeader?.ARM9Size;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7RomOffset"/>
#if NET48
        public uint ARM7RomOffset => _model.CommonHeader.ARM7RomOffset;
#else
        public uint? ARM7RomOffset => _model.CommonHeader?.ARM7RomOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7EntryAddress"/>
#if NET48
        public uint ARM7EntryAddress => _model.CommonHeader.ARM7EntryAddress;
#else
        public uint? ARM7EntryAddress => _model.CommonHeader?.ARM7EntryAddress;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7LoadAddress"/>
#if NET48
        public uint ARM7LoadAddress => _model.CommonHeader.ARM7LoadAddress;
#else
        public uint? ARM7LoadAddress => _model.CommonHeader?.ARM7LoadAddress;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7Size"/>
#if NET48
        public uint ARM7Size => _model.CommonHeader.ARM7Size;
#else
        public uint? ARM7Size => _model.CommonHeader?.ARM7Size;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.FileNameTableOffset"/>
#if NET48
        public uint FileNameTableOffset => _model.CommonHeader.FileNameTableOffset;
#else
        public uint? FileNameTableOffset => _model.CommonHeader?.FileNameTableOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.FileNameTableLength"/>
#if NET48
        public uint FileNameTableLength => _model.CommonHeader.FileNameTableLength;
#else
        public uint? FileNameTableLength => _model.CommonHeader?.FileNameTableLength;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.FileAllocationTableOffset"/>
#if NET48
        public uint FileAllocationTableOffset => _model.CommonHeader.FileAllocationTableOffset;
#else
        public uint? FileAllocationTableOffset => _model.CommonHeader?.FileAllocationTableOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.FileAllocationTableLength"/>
#if NET48
        public uint FileAllocationTableLength => _model.CommonHeader.FileAllocationTableLength;
#else
        public uint? FileAllocationTableLength => _model.CommonHeader?.FileAllocationTableLength;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9OverlayOffset"/>
#if NET48
        public uint ARM9OverlayOffset => _model.CommonHeader.ARM9OverlayOffset;
#else
        public uint? ARM9OverlayOffset => _model.CommonHeader?.ARM9OverlayOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9OverlayLength"/>
#if NET48
        public uint ARM9OverlayLength => _model.CommonHeader.ARM9OverlayLength;
#else
        public uint? ARM9OverlayLength => _model.CommonHeader?.ARM9OverlayLength;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7OverlayOffset"/>
#if NET48
        public uint ARM7OverlayOffset => _model.CommonHeader.ARM7OverlayOffset;
#else
        public uint? ARM7OverlayOffset => _model.CommonHeader?.ARM7OverlayOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7OverlayLength"/>
#if NET48
        public uint ARM7OverlayLength => _model.CommonHeader.ARM7OverlayLength;
#else
        public uint? ARM7OverlayLength => _model.CommonHeader?.ARM7OverlayLength;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.NormalCardControlRegisterSettings"/>
#if NET48
        public uint NormalCardControlRegisterSettings => _model.CommonHeader.NormalCardControlRegisterSettings;
#else
        public uint? NormalCardControlRegisterSettings => _model.CommonHeader?.NormalCardControlRegisterSettings;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.SecureCardControlRegisterSettings"/>
#if NET48
        public uint SecureCardControlRegisterSettings => _model.CommonHeader.SecureCardControlRegisterSettings;
#else
        public uint? SecureCardControlRegisterSettings => _model.CommonHeader?.SecureCardControlRegisterSettings;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.IconBannerOffset"/>
#if NET48
        public uint IconBannerOffset => _model.CommonHeader.IconBannerOffset;
#else
        public uint? IconBannerOffset => _model.CommonHeader?.IconBannerOffset;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.SecureAreaCRC"/>
#if NET48
        public ushort SecureAreaCRC => _model.CommonHeader.SecureAreaCRC;
#else
        public ushort? SecureAreaCRC => _model.CommonHeader?.SecureAreaCRC;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.SecureTransferTimeout"/>
#if NET48
        public ushort SecureTransferTimeout => _model.CommonHeader.SecureTransferTimeout;
#else
        public ushort? SecureTransferTimeout => _model.CommonHeader?.SecureTransferTimeout;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM9Autoload"/>
#if NET48
        public uint ARM9Autoload => _model.CommonHeader.ARM9Autoload;
#else
        public uint? ARM9Autoload => _model.CommonHeader?.ARM9Autoload;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.ARM7Autoload"/>
#if NET48
        public uint ARM7Autoload => _model.CommonHeader.ARM7Autoload;
#else
        public uint? ARM7Autoload => _model.CommonHeader?.ARM7Autoload;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.SecureDisable"/>
#if NET48
        public byte[] SecureDisable => _model.CommonHeader.SecureDisable;
#else
        public byte[]? SecureDisable => _model.CommonHeader?.SecureDisable;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.NTRRegionRomSize"/>
#if NET48
        public uint NTRRegionRomSize => _model.CommonHeader.NTRRegionRomSize;
#else
        public uint? NTRRegionRomSize => _model.CommonHeader?.NTRRegionRomSize;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.HeaderSize"/>
#if NET48
        public uint HeaderSize => _model.CommonHeader.HeaderSize;
#else
        public uint? HeaderSize => _model.CommonHeader?.HeaderSize;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.Reserved2"/>
#if NET48
        public byte[] Reserved2 => _model.CommonHeader.Reserved2;
#else
        public byte[]? Reserved2 => _model.CommonHeader?.Reserved2;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.NintendoLogo"/>
#if NET48
        public byte[] NintendoLogo => _model.CommonHeader.NintendoLogo;
#else
        public byte[]? NintendoLogo => _model.CommonHeader?.NintendoLogo;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.NintendoLogoCRC"/>
#if NET48
        public ushort NintendoLogoCRC => _model.CommonHeader.NintendoLogoCRC;
#else
        public ushort? NintendoLogoCRC => _model.CommonHeader?.NintendoLogoCRC;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.HeaderCRC"/>
#if NET48
        public ushort HeaderCRC => _model.CommonHeader.HeaderCRC;
#else
        public ushort? HeaderCRC => _model.CommonHeader?.HeaderCRC;
#endif

        /// <inheritdoc cref="Models.Nitro.CommonHeader.DebuggerReserved"/>
#if NET48
        public byte[] DebuggerReserved => _model.CommonHeader.DebuggerReserved;
#else
        public byte[]? DebuggerReserved => _model.CommonHeader?.DebuggerReserved;
#endif

        #endregion

        #region Extended DSi Header

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.GlobalMBK15Settings"/>
#if NET48
        public uint[] GlobalMBK15Settings => _model.ExtendedDSiHeader?.GlobalMBK15Settings;
#else
        public uint[]? GlobalMBK15Settings => _model.ExtendedDSiHeader?.GlobalMBK15Settings;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.LocalMBK68SettingsARM9"/>
#if NET48
        public uint[] LocalMBK68SettingsARM9 => _model.ExtendedDSiHeader?.LocalMBK68SettingsARM9;
#else
        public uint[]? LocalMBK68SettingsARM9 => _model.ExtendedDSiHeader?.LocalMBK68SettingsARM9;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.LocalMBK68SettingsARM7"/>
#if NET48
        public uint[] LocalMBK68SettingsARM7 => _model.ExtendedDSiHeader?.LocalMBK68SettingsARM7;
#else
        public uint[]? LocalMBK68SettingsARM7 => _model.ExtendedDSiHeader?.LocalMBK68SettingsARM7;
#endif

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
#if NET48
        public byte[] TitleID => _model.ExtendedDSiHeader?.TitleID;
#else
        public byte[]? TitleID => _model.ExtendedDSiHeader?.TitleID;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DSiWarePublicSavSize"/>
        public uint? DSiWarePublicSavSize => _model.ExtendedDSiHeader?.DSiWarePublicSavSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DSiWarePrivateSavSize"/>
        public uint? DSiWarePrivateSavSize => _model.ExtendedDSiHeader?.DSiWarePrivateSavSize;

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ReservedZero"/>
#if NET48
        public byte[] ReservedZero => _model.ExtendedDSiHeader?.ReservedZero;
#else
        public byte[]? ReservedZero => _model.ExtendedDSiHeader?.ReservedZero;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Unknown2"/>
#if NET48
        public byte[] Unknown2 => _model.ExtendedDSiHeader?.Unknown2;
#else
        public byte[]? Unknown2 => _model.ExtendedDSiHeader?.Unknown2;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9WithSecureAreaSHA1HMACHash"/>
#if NET48
        public byte[] ARM9WithSecureAreaSHA1HMACHash => _model.ExtendedDSiHeader?.ARM9WithSecureAreaSHA1HMACHash;
#else
        public byte[]? ARM9WithSecureAreaSHA1HMACHash => _model.ExtendedDSiHeader?.ARM9WithSecureAreaSHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7SHA1HMACHash"/>
#if NET48
        public byte[] ARM7SHA1HMACHash => _model.ExtendedDSiHeader?.ARM7SHA1HMACHash;
#else
        public byte[]? ARM7SHA1HMACHash => _model.ExtendedDSiHeader?.ARM7SHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.DigestMasterSHA1HMACHash"/>
#if NET48
        public byte[] DigestMasterSHA1HMACHash => _model.ExtendedDSiHeader?.DigestMasterSHA1HMACHash;
#else
        public byte[]? DigestMasterSHA1HMACHash => _model.ExtendedDSiHeader?.DigestMasterSHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.BannerSHA1HMACHash"/>
#if NET48
        public byte[] BannerSHA1HMACHash => _model.ExtendedDSiHeader?.BannerSHA1HMACHash;
#else
        public byte[]? BannerSHA1HMACHash => _model.ExtendedDSiHeader?.BannerSHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9iDecryptedSHA1HMACHash"/>
#if NET48
        public byte[] ARM9iDecryptedSHA1HMACHash => _model.ExtendedDSiHeader?.ARM9iDecryptedSHA1HMACHash;
#else
        public byte[]? ARM9iDecryptedSHA1HMACHash => _model.ExtendedDSiHeader?.ARM9iDecryptedSHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM7iDecryptedSHA1HMACHash"/>
#if NET48
        public byte[] ARM7iDecryptedSHA1HMACHash => _model.ExtendedDSiHeader?.ARM7iDecryptedSHA1HMACHash;
#else
        public byte[]? ARM7iDecryptedSHA1HMACHash => _model.ExtendedDSiHeader?.ARM7iDecryptedSHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Reserved5"/>
#if NET48
        public byte[] Reserved5 => _model.ExtendedDSiHeader?.Reserved5;
#else
        public byte[]? Reserved5 => _model.ExtendedDSiHeader?.Reserved5;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ARM9NoSecureAreaSHA1HMACHash"/>
#if NET48
        public byte[] ARM9NoSecureAreaSHA1HMACHash => _model.ExtendedDSiHeader?.ARM9NoSecureAreaSHA1HMACHash;
#else
        public byte[]? ARM9NoSecureAreaSHA1HMACHash => _model.ExtendedDSiHeader?.ARM9NoSecureAreaSHA1HMACHash;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.Reserved6"/>
#if NET48
        public byte[] Reserved6 => _model.ExtendedDSiHeader?.Reserved6;
#else
        public byte[]? Reserved6 => _model.ExtendedDSiHeader?.Reserved6;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.ReservedAndUnchecked"/>
#if NET48
        public byte[] ReservedAndUnchecked => _model.ExtendedDSiHeader?.ReservedAndUnchecked;
#else
        public byte[]? ReservedAndUnchecked => _model.ExtendedDSiHeader?.ReservedAndUnchecked;
#endif

        /// <inheritdoc cref="Models.Nitro.ExtendedDSiHeader.RSASignature"/>
#if NET48
        public byte[] RSASignature => _model.ExtendedDSiHeader?.RSASignature;
#else
        public byte[]? RSASignature => _model.ExtendedDSiHeader?.RSASignature;
#endif

        #endregion

        #region Secure Area

        /// <inheritdoc cref="Models.Nitro.Cart.SecureArea"/>
#if NET48
        public byte[] SecureArea => _model.SecureArea;
#else
        public byte[]? SecureArea => _model.SecureArea;
#endif

        #endregion

        #region Name Table

        /// <inheritdoc cref="Models.Nitro.NameTable.FolderAllocationTable"/>
#if NET48
        public SabreTools.Models.Nitro.FolderAllocationTableEntry[] FolderAllocationTable => _model.NameTable.FolderAllocationTable;
#else
        public SabreTools.Models.Nitro.FolderAllocationTableEntry?[]? FolderAllocationTable => _model.NameTable?.FolderAllocationTable;
#endif

        /// <inheritdoc cref="Models.Nitro.NameTable.NameList"/>
#if NET48
        public SabreTools.Models.Nitro.NameListEntry[] NameList => _model.NameTable.NameList;
#else
        public SabreTools.Models.Nitro.NameListEntry?[]? NameList => _model.NameTable?.NameList;
#endif

        #endregion

        #region File Allocation Table

        /// <inheritdoc cref="Models.Nitro.Cart.FileAllocationTable"/>
#if NET48
        public SabreTools.Models.Nitro.FileAllocationTableEntry[] FileAllocationTable => _model.FileAllocationTable;
#else
        public SabreTools.Models.Nitro.FileAllocationTableEntry?[]? FileAllocationTable => _model.FileAllocationTable;
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
            Printing.Nitro.Print(builder, _model);
            return builder;
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}