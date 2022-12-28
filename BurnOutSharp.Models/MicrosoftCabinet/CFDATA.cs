using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.MicrosoftCabinet
{
    /// <summary>
    /// Each CFDATA structure describes some amount of compressed data, as shown in the following
    /// packet diagram. The first CFDATA structure entry for each folder is located by using the
    /// <see cref="CFFOLDER.CabStartOffset"/> field. Subsequent CFDATA structure records for this folder are
    /// contiguous.
    /// </summary>
    /// <see href="http://download.microsoft.com/download/5/0/1/501ED102-E53F-4CE0-AA6B-B0F93629DDC6/Exchange/%5BMS-CAB%5D.pdf"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class CFDATA
    {
        /// <summary>
        /// Checksum of this CFDATA structure, from the <see cref="CompressedSize"/> through the
        /// <see cref="CompressedData"/> fields. It can be set to 0 (zero) if the checksum is not supplied.
        /// </summary>
        public uint Checksum;

        /// <summary>
        /// Number of bytes of compressed data in this CFDATA structure record. When the
        /// <see cref="UncompressedSize"/> field is zero, this field indicates only the number of bytes that fit into this cabinet file.
        /// </summary>
        public ushort CompressedSize;

        /// <summary>
        /// The uncompressed size of the data in this CFDATA structure entry in bytes. When this
        /// CFDATA structure entry is continued in the next cabinet file, the <see cref="UncompressedSize"/> field will be zero, and
        /// the <see cref="UncompressedSize"/> field in the first CFDATA structure entry in the next cabinet file will report the total
        /// uncompressed size of the data from both CFDATA structure blocks.
        /// </summary>
        public ushort UncompressedSize;

        /// <summary>
        /// If the <see cref="HeaderFlags.RESERVE_PRESENT"/> flag is set
        /// and the <see cref="CFHEADER.DataReservedSize"/> field value is non-zero, this field contains per-datablock application information.
        /// This field is defined by the application, and it is used for application-defined purposes.
        /// </summary>
        public byte[] ReservedData;

        /// <summary>
        /// The compressed data bytes, compressed by using the <see cref="CFFOLDER.CompressionType"/>
        /// method. When the <see cref="UncompressedSize"/> field value is zero, these data bytes MUST be combined with the data
        /// bytes from the next cabinet's first CFDATA structure entry before decompression. When the
        ///<see cref="CFFOLDER.CompressionType"/> field indicates that the data is not compressed, this field contains the
        /// uncompressed data bytes. In this case, the <see cref="CompressedSize"/> and <see cref="UncompressedSize"/> field values will be equal unless
        /// this CFDATA structure entry crosses a cabinet file boundary.
        /// </summary>
        public byte[] CompressedData;
    }
}
