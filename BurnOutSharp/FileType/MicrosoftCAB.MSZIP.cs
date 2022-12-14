using System;
using System.Linq;
using ComponentAce.Compression.Libs.zlib;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Each MSZIP block MUST consist of a 2-byte MSZIP signature and one or more RFC 1951 blocks. The
    /// 2-byte MSZIP signature MUST consist of the bytes 0x43 and 0x4B. The MSZIP signature MUST be
    /// the first 2 bytes in the MSZIP block.The MSZIP signature is shown in the following packet diagram.       
    /// </summary>
    internal class MSZIPBlock
    {
        #region Constants

        /// <summary>
        /// Human-readable signature
        /// </summary>
        public static readonly string SignatureString = "CK";

        /// <summary>
        /// Signature as an unsigned Int16 value
        /// </summary>
        public const ushort SignatureValue = 0x4B43;

        /// <summary>
        /// Signature as a byte array
        /// </summary>
        public static readonly byte[] SignatureBytes = new byte[] { 0x43, 0x4B };

        #endregion

        #region Properties

        /// <summary>
        /// 'CB'
        /// </summary>
        public ushort Signature { get; private set; }

        /// <summary>
        /// Each MSZIP block is the result of a single deflate compression operation, as defined in [RFC1951].
        /// The compressor that performs the compression operation MUST generate one or more RFC 1951
        /// blocks, as defined in [RFC1951]. The number, deflation mode, and type of RFC 1951 blocks in each
        /// MSZIP block is determined by the compressor, as defined in [RFC1951]. The last RFC 1951 block in
        /// each MSZIP block MUST be marked as the "end" of the stream(1), as defined by[RFC1951]
        /// section 3.2.3. Decoding trees MUST be discarded after each RFC 1951 block, but the history buffer
        /// MUST be maintained.Each MSZIP block MUST represent no more than 32 KB of uncompressed data.
        /// 
        /// The maximum compressed size of each MSZIP block is 32 KB + 12 bytes.This enables the MSZIP
        /// block to contain 32 KB of data split between two noncompressed RFC 1951 blocks, each of which
        /// has a value of BTYPE = 00.
        /// </summary>
        public byte[] Data { get; private set; }

        #endregion

        #region Static Properties

        public static ZStream DecompressionStream { get; set; } = new ZStream();

        #endregion

        #region Serialization

        public static MSZIPBlock Deserialize(byte[] data)
        {
            if (data == null)
                return null;

            MSZIPBlock block = new MSZIPBlock();
            int dataPtr = 0;

            block.Signature = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
            if (block.Signature != SignatureValue)
                return null;

            block.Data = new byte[data.Length - 2];
            Array.Copy(data, dataPtr, block.Data, 0, data.Length - 2);
            dataPtr += data.Length - 2;

            return block;
        }

        #endregion

        #region Public Functionality

        /// <summary>
        /// Decompress a single block of MS-ZIP data
        /// </summary>
        public byte[] DecompressBlock(int decompressedSize, byte[] previousBytes = null)
        {
            if (Data == null || Data.Length == 0)
                return null;

            try
            {
                // The first block can use DeflateStream since it has no history
                if (previousBytes == null)
                {
                    // Setup the input
                    DecompressionStream = new ZStream();
                    int initErr = DecompressionStream.inflateInit();
                    if (initErr != zlibConst.Z_OK)
                        return null;
                }

                // All n+1 blocks require the previous uncompressed data as a dictionary
                else
                {
                    // TODO: We need to force a dictionary setting - at this point, mode is 8 not 6

                    // Setup the dictionary
                    int dictErr = DecompressionStream.inflateSetDictionary(previousBytes, previousBytes.Length);
                    if (dictErr != zlibConst.Z_OK)
                        return null;
                }

                // Setup the output
                byte[] output = new byte[decompressedSize];
                DecompressionStream.next_out = output;
                DecompressionStream.avail_out = decompressedSize;

                // Inflate the data -- 0x78, 0x9C is needed to trick zlib
                DecompressionStream.next_in = new byte[] { 0x78, 0x9C }.Concat(Data).ToArray();
                DecompressionStream.next_in_index = 0;
                DecompressionStream.avail_in = Data.Length + 2;

                int err = DecompressionStream.inflate(zlibConst.Z_FULL_FLUSH);
                if (err != zlibConst.Z_OK)
                    return null;

                return output;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
