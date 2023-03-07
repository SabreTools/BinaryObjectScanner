namespace BinaryObjectScanner.Models.Nitro
{
    /// <summary>
    /// Nintendo DS / DSi cartridge header
    /// </summary>
    /// <see href="https://dsibrew.org/wiki/DSi_cartridge_header"/>
    public sealed class CommonHeader
    {
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
    }
}