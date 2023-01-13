namespace BurnOutSharp.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo_vmg.html"/>
    public sealed class TitlesTableEntry
    {
        /// <summary>
        /// Title type
        /// </summary>
        public TitleType TitleType;

        /// <summary>
        /// Number of angles
        /// </summary>
        public byte NumberOfAngles;

        /// <summary>
        /// Number of chapters (PTTs)
        /// </summary>
        public ushort NumberOfChapters;

        /// <summary>
        /// Parental management mask
        /// </summary>
        public ushort ParentalManagementMask;

        /// <summary>
        /// Video Title Set number (VTSN)
        /// </summary>
        public byte VideoTitleSetNumber;

        /// <summary>
        /// Title number within VTS (VTS_TTN)
        /// </summary>
        public byte TitleNumberWithinVTS;

        /// <summary>
        /// Start sector for VTS, referenced to whole disk
        /// (video_ts.ifo starts at sector 00000000)
        /// </summary>
        public uint VTSStartSector;
    }
}