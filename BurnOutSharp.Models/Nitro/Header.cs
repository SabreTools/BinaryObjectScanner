namespace BurnOutSharp.Models.Nitro
{
    /// <summary>
    /// Nintendo DS / DSi Cartridge Header
    /// </summary>
    /// <see href="https://dsibrew.org/wiki/DSi_cartridge_header"/>
    public sealed class Header
    {
        #region Common

        /// <summary>
        /// Game Title
        /// </summary>
        public string GameTitle;

        /// <summary>
        /// Gamecode
        /// </summary>
        public uint GameCode;

        /// <summary>
        /// Makercode
        /// </summary>
        public string MakerCode;

        /// <summary>
        /// Unitcode
        /// </summary>
        public Unitcode UnitCode;

        /// <summary>
        /// Encryption seed select (device code. 0 = normal)
        /// </summary>
        public byte EncryptionSeedSelect;

        /// <summary>
        /// Devicecapacity
        /// </summary>
        public byte DeviceCapacity;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved1;

        /// <summary>
        /// Game Revision (used by DSi titles)
        /// </summary>
        public ushort GameRevision;

        /// <summary>
        /// ROM Version
        /// </summary>
        public byte RomVersion;

        /// <summary>
        /// Internal flags, (Bit2: Autostart)
        /// </summary>
        public byte InternalFlags;

        /// <summary>
        /// ARM9 rom offset
        /// </summary>
        public uint ARM9RomOffset;

        /// <summary>
        /// ARM9 entry address
        /// </summary>
        public uint ARM9EntryAddress;

        /// <summary>
        /// ARM9 load address
        /// </summary>
        public uint ARM9LoadAddress;

        /// <summary>
        /// ARM9 size
        /// </summary>
        public uint ARM9Size;

        /// <summary>
        /// ARM7 rom offset
        /// </summary>
        public uint ARM7RomOffset;

        /// <summary>
        /// ARM7 entry address
        /// </summary>
        public uint ARM7EntryAddress;

        /// <summary>
        /// ARM7 load address
        /// </summary>
        public uint ARM7LoadAddress;

        /// <summary>
        /// ARM7 size
        /// </summary>
        public uint ARM7Size;

        /// <summary>
        /// File Name Table (FNT) offset
        /// </summary>
        public uint FileNameTableOffset;

        /// <summary>
        /// File Name Table (FNT) length
        /// </summary>
        public uint FileNameTableLength;

        /// <summary>
        /// File Allocation Table (FNT) offset
        /// </summary>
        public uint FileAllocationTableOffset;

        /// <summary>
        /// File Allocation Table (FNT) length
        /// </summary>
        public uint FileAllocationTableLength;

        /// <summary>
        /// File Name Table (FNT) offset
        /// </summary>
        public uint ARM9OverlayOffset;

        /// <summary>
        /// File Name Table (FNT) length
        /// </summary>
        public uint ARM9OverlayLength;

        /// <summary>
        /// File Name Table (FNT) offset
        /// </summary>
        public uint ARM7OverlayOffset;

        /// <summary>
        /// File Name Table (FNT) length
        /// </summary>
        public uint ARM7OverlayLength;

        /// <summary>
        /// Normal card control register settings (0x00416657 for OneTimePROM)
        /// </summary>
        public uint NormalCardControlRegisterSettings;

        /// <summary>
        /// Secure card control register settings (0x081808F8 for OneTimePROM)
        /// </summary>
        public uint SecureCardControlRegisterSettings;

        /// <summary>
        /// Icon Banner offset (NDSi same as NDS, but with new extra entries)
        /// </summary>
        public uint IconBannerOffset;

        /// <summary>
        /// Secure area (2K) CRC
        /// </summary>
        public ushort SecureAreaCRC;

        /// <summary>
        /// Secure transfer timeout (0x0D7E for OneTimePROM)
        /// </summary>
        public ushort SecureTransferTimeout;

        /// <summary>
        /// ARM9 autoload
        /// </summary>
        public uint ARM9Autoload;

        /// <summary>
        /// ARM7 autoload
        /// </summary>
        public uint ARM7Autoload;

        /// <summary>
        /// Secure disable
        /// </summary>
        public byte[] SecureDisable;

        /// <summary>
        /// NTR region ROM size (excluding DSi area)
        /// </summary>
        public uint NTRRegionRomSize;

        /// <summary>
        /// Header size
        /// </summary>
        public uint HeaderSize;

        /// <summary>
        ///Reserved (0x88, 0x8C, 0x90 = Unknown, used by DSi)
        /// </summary>
        public byte[] Reserved2;

        /// <summary>
        /// Nintendo Logo
        /// </summary>
        public byte[] NintendoLogo;

        /// <summary>
        /// Nintendo Logo CRC
        /// </summary>
        public ushort NintendoLogoCRC;

        /// <summary>
        /// Header CRC
        /// </summary>
        public ushort HeaderCRC;

        /// <summary>
        /// Debugger reserved
        /// </summary>
        public byte[] DebuggerReserved;

        #endregion

        // TODO: Should this be separated into a distinct file?
        #region Extended DSi

        /// <summary>
        /// Global MBK1..MBK5 Settings
        /// </summary>
        public uint[] GlobalMBK15Settings;

        /// <summary>
        ///	Local MBK6..MBK8 Settings for ARM9
        /// </summary>
        public uint[] LocalMBK68SettingsARM9;

        /// <summary>
        /// Local MBK6..MBK8 Settings for ARM7
        /// </summary>
        public uint[] LocalMBK68SettingsARM7;

        /// <summary>
        /// Global MBK9 Setting
        /// </summary>
        public uint GlobalMBK9Setting;

        /// <summary>
        /// Region Flags
        /// </summary>
        public uint RegionFlags;

        /// <summary>
        /// Access control
        /// </summary>
        public uint AccessControl;

        /// <summary>
        /// ARM7 SCFG EXT mask (controls which devices to enable)
        /// </summary>
        public uint ARM7SCFGEXTMask;

        /// <summary>
        /// Reserved/flags? When bit2 of byte 0x1bf is set, usage of banner.sav from the title data dir is enabled.(additional banner data)
        /// </summary>
        public uint ReservedFlags;

        /// <summary>
        /// ARM9i rom offset
        /// </summary>
        public uint ARM9iRomOffset;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved3;

        /// <summary>
        /// ARM9i load address
        /// </summary>
        public uint ARM9iLoadAddress;

        /// <summary>
        /// ARM9i size;
        /// </summary>
        public uint ARM9iSize;

        /// <summary>
        /// ARM7i rom offset
        /// </summary>
        public uint ARM7iRomOffset;

        /// <summary>
        /// Pointer to base address where various structures and parameters are passed to the title - what is that???
        /// </summary>
        public uint Reserved4;

        /// <summary>
        /// ARM7i load address
        /// </summary>
        public uint ARM7iLoadAddress;

        /// <summary>
        /// ARM7i size;
        /// </summary>
        public uint ARM7iSize;

        /// <summary>
        /// Digest NTR region offset
        /// </summary>
        public uint DigestNTRRegionOffset;

        /// <summary>
        /// Digest NTR region length
        /// </summary>
        public uint DigestNTRRegionLength;

        // <summary>
        /// Digest TWL region offset
        /// </summary>
        public uint DigestTWLRegionOffset;

        /// <summary>
        /// Digest TWL region length
        /// </summary>
        public uint DigestTWLRegionLength;

        // <summary>
        /// Digest Sector Hashtable region offset
        /// </summary>
        public uint DigestSectorHashtableRegionOffset;

        /// <summary>
        /// Digest Sector Hashtable region length
        /// </summary>
        public uint DigestSectorHashtableRegionLength;

        // <summary>
        /// Digest Block Hashtable region offset
        /// </summary>
        public uint DigestBlockHashtableRegionOffset;

        /// <summary>
        /// Digest Block Hashtable region length
        /// </summary>
        public uint DigestBlockHashtableRegionLength;

        /// <summary>
        /// Digest Sector size
        /// </summary>
        public uint DigestSectorSize;

        /// <summary>
        /// Digeset Block Sectorount
        /// </summary>
        public uint DigestBlockSectorCount;

        /// <summary>
        /// Icon Banner Size (usually 0x23C0)
        /// </summary>
        public uint IconBannerSize;

        /// <summary>
        /// Unknown (used by DSi)
        /// </summary>
        public uint Unknown1;

        /// <summary>
        /// NTR+TWL region ROM size (total size including DSi area)
        /// </summary>
        public uint NTRTWLRegionRomSize;

        /// <summary>
        /// Unknown (used by DSi)
        /// </summary>
        public byte[] Unknown2;

        /// <summary>
        /// Modcrypt area 1 offset
        /// </summary>
        public uint ModcryptArea1Offset;

        /// <summary>
        /// Modcrypt area 1 size
        /// </summary>
        public uint ModcryptArea1Size;

        /// <summary>
        /// Modcrypt area 2 offset
        /// </summary>
        public uint ModcryptArea2Offset;

        /// <summary>
        /// Modcrypt area 2 size
        /// </summary>
        public uint ModcryptArea2Size;

        /// <summary>
        /// Title ID
        /// </summary>
        public byte[] TitleID;

        /// <summary>
        /// DSiWare: "public.sav" size
        /// </summary>
        public uint DSiWarePublicSavSize;

        /// <summary>
        /// DSiWare: "private.sav" size
        /// </summary>
        public uint DSiWarePrivateSavSize;

        /// <summary>
        /// Reserved (zero)
        /// </summary>
        public byte[] ReservedZero;

        /// <summary>
        /// Unknown (used by DSi)
        /// </summary>
        public byte[] Unknown3;

        /// <summary>
        /// ARM9 (with encrypted secure area) SHA1 HMAC hash
        /// </summary>
        public byte[] ARM9WithSecureAreaSHA1HMACHash;

        /// <summary>
        /// ARM7 SHA1 HMAC hash
        /// </summary>
        public byte[] ARM7SHA1HMACHash;

        /// <summary>
        /// Digest master SHA1 HMAC hash
        /// </summary>
        public byte[] DigestMasterSHA1HMACHash;

        /// <summary>
        /// Banner SHA1 HMAC hash
        /// </summary>
        public byte[] BannerSHA1HMACHash;

        /// <summary>
        /// ARM9i (decrypted) SHA1 HMAC hash
        /// </summary>
        public byte[] ARM9iDecryptedSHA1HMACHash;

        /// <summary>
        /// ARM7i (decrypted) SHA1 HMAC hash
        /// </summary>
        public byte[] ARM7iDecryptedSHA1HMACHash;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved5;

        /// <summary>
        /// ARM9 (without secure area) SHA1 HMAC hash
        /// </summary>
        public byte[] ARM9NoSecureAreaSHA1HMACHash;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved6;

        /// <summary>
        /// Reserved and unchecked region, always zero. Used for passing arguments in debug environment.
        /// </summary>
        public byte[] ReservedAndUnchecked;

        /// <summary>
        /// RSA signature (the first 0xE00 bytes of the header are signed with an 1024-bit RSA signature).
        /// </summary>
        public byte[] RSASignature;

        #endregion
    }
}