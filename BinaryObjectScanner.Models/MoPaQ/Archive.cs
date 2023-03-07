namespace BinaryObjectScanner.Models.MoPaQ
{
    /// <summary>
    /// MPQ (MoPaQ) is an archive format developed by Blizzard Entertainment,
    /// purposed for storing data files, images, sounds, music and videos for
    /// their games. The name MoPaQ comes from the author of the format,
    /// Mike O'Brien (Mike O'brien PaCK).
    /// </summary>
    /// <see href="http://zezula.net/en/mpq/mpqformat.html"/>
    public sealed class Archive
    {
        // TODO: Data before archive, ignored

        /// <summary>
        /// MPQ User Data (optional)
        /// </summary>
        public UserData UserData { get; set; }

        /// <summary>
        /// MPQ Header (required)
        /// </summary>
        public ArchiveHeader ArchiveHeader { get; set; }

        // TODO: Files (optional)
        // TODO: Special files (optional)

        /// <summary>
        /// HET Table (optional)
        /// </summary>
        public HetTable HetTable { get; set; }

        /// <summary>
        /// BET Table (optional)
        /// </summary>
        public BetTable BetTable { get; set; }

        /// <summary>
        /// Hash Table (optional)
        /// </summary>
        public HashEntry[] HashTable { get; set; }

        /// <summary>
        /// Block Table (optional)
        /// </summary>
        public BlockEntry[] BlockTable { get; set; }

        /// <summary>
        /// Hi-Block Table (optional)
        /// </summary>
        /// <remarks>
        /// Since World of Warcraft - The Burning Crusade, Blizzard extended
        /// the MPQ format to support archives larger than 4GB. The hi-block
        /// table holds the higher 16-bits of the file position in the MPQ.
        /// Hi-block table is plain array of 16-bit values. This table is
        /// not encrypted.
        /// </remarks>
        public short[] HiBlockTable { get; set; }

        // TODO: Strong digital signature
    }
}
