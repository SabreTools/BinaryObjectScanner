namespace BurnOutSharp.Models.MicrosoftCabinet.MSZIP
{
    /// <summary>
    /// Each MSZIP block MUST consist of a 2-byte MSZIP signature and one or more RFC 1951 blocks. The
    /// 2-byte MSZIP signature MUST consist of the bytes 0x43 and 0x4B. The MSZIP signature MUST be
    /// the first 2 bytes in the MSZIP block. The MSZIP signature is shown in the following packet diagram.
    /// 
    /// Each MSZIP block is the result of a single deflate compression operation, as defined in [RFC1951].
    /// The compressor that performs the compression operation MUST generate one or more RFC 1951
    /// blocks, as defined in [RFC1951]. The number, deflation mode, and type of RFC 1951 blocks in each
    /// MSZIP block is determined by the compressor, as defined in [RFC1951]. The last RFC 1951 block in
    /// each MSZIP block MUST be marked as the "end" of the stream(1), as defined by [RFC1951]
    /// section 3.2.3. Decoding trees MUST be discarded after each RFC 1951 block, but the history buffer
    /// MUST be maintained.Each MSZIP block MUST represent no more than 32 KB of uncompressed data.
    /// 
    /// The maximum compressed size of each MSZIP block is 32 KB + 12 bytes. This enables the MSZIP
    /// block to contain 32 KB of data split between two noncompressed RFC 1951 blocks, each of which
    /// has a value of BTYPE = 00.
    /// </summary>
    /// <see href="https://interoperability.blob.core.windows.net/files/MS-MCI/%5bMS-MCI%5d.pdf"/>
    public class BlockHeader
    {
        /// <summary>
        /// 'CK'
        /// </summary>
        public ushort Signature;
    }
}