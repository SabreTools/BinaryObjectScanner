namespace BinaryObjectScanner.Models.N3DS
{
    /// <summary>
    /// ExeFS or Executable Filesystem contains information related to the
    /// executable program, and is the part of the CXI format. 
    /// 
    /// The ExeFS usually contains one or more of the following files:
    /// - .code Contains the code binary, which can be optionally reverse-LZSS compressed via an exheader flag.
    /// - logo Contains distribution licensing Binary data.
    /// - banner Contains the banner which homemenu uses for this CXI.
    /// - icon Contains the icon which homemenu displays for this CXI.
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/ExeFS"/>
    public sealed class ExeFSHeader
    {
        /// <summary>
        /// File headers (10 headers maximum, 16 bytes each)
        /// </summary>
        public ExeFSFileHeader[] FileHeaders;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved;

        /// <summary>
        /// File hashes (10 hashes maximum, 32 bytes each, one for each header)
        /// </summary>
        /// <remarks>SHA-256 hashes</remarks>
        public byte[][] FileHashes;
    }
}
