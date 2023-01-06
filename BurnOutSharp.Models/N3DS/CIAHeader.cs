namespace BurnOutSharp.Models.N3DS
{
    /// <summary>
    /// CIA stands for CTR Importable Archive. This format allows the installation of
    /// titles to the 3DS. CIA files and titles on Nintendo's CDN contain identical data.
    /// As a consequence, valid CIA files can be generated from CDN content. This also
    /// means CIA files can contain anything that titles on Nintendo's CDN can contain.
    /// 
    /// Under normal circumstances CIA files are used where downloading a title is
    /// impractical or not possible. Such as distributing a Download Play child, or
    /// installing forced Gamecard updates. Those CIA(s) are stored by the titles in
    /// question, in an auxiliary CFA file.
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/CIA"/>
    public sealed class CIAHeader
    {
        /// <summary>
        /// Archive header size, usually 0x2020 bytes
        /// </summary>
        public int HeaderSize;

        /// <summary>
        /// Type
        /// </summary>
        public ushort Type;

        /// <summary>
        /// Version
        /// </summary>
        public ushort Version;

        /// <summary>
        /// Certificate chain size
        /// </summary>
        public int CertificateChainSize;

        /// <summary>
        /// Ticket size
        /// </summary>
        public int TicketSize;

        /// <summary>
        /// TMD file size
        /// </summary>
        public int TMDFileSize;

        /// <summary>
        /// Meta size (0 if no Meta data is present)
        /// </summary>
        public int MetaSize;

        /// <summary>
        /// Content size
        /// </summary>
        public long ContentSize;

        /// <summary>
        /// Content Index
        /// </summary>
        public byte[] ContentIndex;

        #region Content Index

        /// <summary>
        /// Certificate chain
        /// </summary>
        /// <remarks>
        /// https://www.3dbrew.org/wiki/CIA#Certificate_Chain
        /// </remarks>
        public Certificate[] CertificateChain;

        /// <summary>
        /// Ticket
        /// </summary>
        public Ticket Ticket;

        /// <summary>
        /// TMD file data
        /// </summary>
        public TitleMetadata TMDFileData;

        /// <summary>
        /// Content file data
        /// </summary>
        public NCCHHeader[] Partitions;

        /// <summary>
        /// Meta file data (Not a necessary component)
        /// </summary>
        public MetaFile MetaFileData;

        #endregion
    }
}