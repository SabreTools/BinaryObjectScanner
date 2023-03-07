namespace BinaryObjectScanner.Models.N3DS
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
    public class CIA
    {
        /// <summary>
        /// CIA header
        /// </summary>
        public CIAHeader Header { get; set; }

        /// <summary>
        /// Certificate chain
        /// </summary>
        /// <remarks>
        /// https://www.3dbrew.org/wiki/CIA#Certificate_Chain
        /// </remarks>
        public Certificate[] CertificateChain { get; set; }

        /// <summary>
        /// Ticket
        /// </summary>
        public Ticket Ticket { get; set; }

        /// <summary>
        /// TMD file data
        /// </summary>
        public TitleMetadata TMDFileData { get; set; }

        /// <summary>
        /// Content file data
        /// </summary>
        public NCCHHeader[] Partitions { get; set; }

        /// <summary>
        /// Content file data
        /// </summary>
        /// TODO: Parse the content file data
        public byte[] ContentFileData { get; set; }

        /// <summary>
        /// Meta file data (Not a necessary component)
        /// </summary>
        public MetaData MetaData { get; set; }
    }
}