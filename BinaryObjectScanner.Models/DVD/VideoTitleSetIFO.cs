namespace BinaryObjectScanner.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo.html"/>
    public sealed class VideoTitleSetIFO
    {
        /// <summary>
        /// "DVDVIDEO-VTS"
        /// </summary>
        public string Signature;

        /// <summary>
        /// Last sector of title set (last sector of BUP)
        /// </summary>
        public uint LastTitleSetSector;

        /// <summary>
        /// Last sector of IFO
        /// </summary>
        public uint LastIFOSector;

        /// <summary>
        /// Version number
        /// - Byte 0 - Reserved, should be 0
        /// - Byte 1, Bits 7-4 - Major version number
        /// - Byte 1, Bits 3-0 - Minor version number
        /// </summary>
        public ushort VersionNumber;

        /// <summary>
        /// VTS category
        /// </summary>
        /// <remarks>0=unspecified, 1=Karaoke</remarks>
        public uint VMGCategory;

        /// <summary>
        /// Number of volumes
        /// </summary>
        public ushort NumberOfVolumes;

        /// <summary>
        /// Volume number
        /// </summary>
        public ushort VolumeNumber;

        /// <summary>
        /// Side ID
        /// </summary>
        public byte SideID;

        /// <summary>
        /// Number of title sets
        /// </summary>
        public ushort NumberOfTitleSets;

        /// <summary>
        /// Provider ID
        /// </summary>
        public byte[] ProviderID;

        /// <summary>
        /// VMG POS
        /// </summary>
        public ulong VMGPOS;

        /// <summary>
        /// End byte address of VTS_MAT
        /// </summary>
        public uint InformationManagementTableEndByteAddress;

        /// <summary>
        /// Start address of FP_PGC (First Play program chain)
        /// </summary>
        public uint FirstPlayProgramChainStartAddress;

        /// <summary>
        /// Start sector of Menu VOB
        /// </summary>
        public uint MenuVOBStartSector;

        /// <summary>
        /// Start sector of Title VOB
        /// </summary>
        public uint TitleVOBStartSector;

        /// <summary>
        /// Sector pointer to VTS_PTT_SRPT (table of Titles and Chapters)
        /// </summary>
        public uint TableOfTitlesAndChaptersSectorPointer;

        /// <summary>
        /// Sector pointer to VTS_PGCI (Title Program Chain table)
        /// </summary>
        public uint TitleProgramChainTableSectorPointer;

        /// <summary>
        /// Sector pointer to VTSM_PGCI_UT (Menu Program Chain table)
        /// </summary>
        public uint MenuProgramChainTableSectorPointer;

        /// <summary>
        /// Sector pointer to VTS_TMAPTI (time map)
        /// </summary>
        public uint TimeMapSectorPointer;

        /// <summary>
        /// Sector pointer to VTSM_C_ADT (menu cell address table)
        /// </summary>
        public uint MenuCellAddressTableSectorPointer;

        /// <summary>
        /// Sector pointer to VTSM_VOBU_ADMAP (menu VOBU address map)
        /// </summary>
        public uint MenuVOBUAddressMapSectorPointer;

        /// <summary>
        /// Sector pointer to VTS_C_ADT (title set cell address table)
        /// </summary>
        public uint TitleSetCellAddressTableSectorPointer;

        /// <summary>
        /// Sector pointer to VTS_VOBU_ADMAP (title set VOBU address map)
        /// </summary>
        public uint TitleSetVOBUAddressMapSectorPointer;

        /// <summary>
        /// Video attributes of VTSM_VOBS
        /// </summary>
        public byte[] VideoAttributes;

        /// <summary>
        /// Number of audio streams in VTSM_VOBS
        /// </summary>
        public ushort NumberOfAudioStreams;

        /// <summary>
        /// Audio attributes of VTSM_VOBS
        /// </summary>
        public byte[][] AudioAttributes;

        /// <summary>
        /// Unknown
        /// </summary>
        public byte[] Unknown;

        /// <summary>
        /// Number of subpicture streams in VTSM_VOBS (0 or 1)
        /// </summary>
        public ushort NumberOfSubpictureStreams;

        /// <summary>
        /// Subpicture attributes of VTSM_VOBS
        /// </summary>
        public byte[] SubpictureAttributes;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved;
    }
}