using System.IO;

namespace BurnOutSharp.Models.Compression.LZ
{
    public sealed class State
    {
        /// <summary>
        /// Internal backing stream
        /// </summary>
        public Stream Source { get; set; }

        /// <summary>
        /// The last char of the filename for replacement
        /// </summary>
        public char LastChar { get; set; }

        /// <summary>
        /// Decompressed length of the file
        /// </summary>
        public uint RealLength { get; set; }

        /// <summary>
        /// Position the decompressor currently is
        /// </summary>
        public uint RealCurrent { get; set; }

        /// <summary>
        /// Position the user wants to read from
        /// </summary>
        public uint RealWanted { get; set; }

        /// <summary>
        /// The rotating LZ table
        /// </summary>
        public byte[] Table { get; set; }

        /// <summary>
        /// CURrent TABle ENTry
        /// </summary>
        public uint CurrentTableEntry { get; set; }

        /// <summary>
        /// Length and position of current string
        /// </summary>
        public byte StringLength { get; set; }

        /// <summary>
        /// From stringtable
        /// </summary>
        public uint StringPosition { get; set; }

        /// <summary>
        /// Bitmask within blocks
        /// </summary>
        public ushort ByteType { get; set; }

        /// <summary>
        /// GETLEN bytes
        /// </summary>
        public byte[] Window { get; set; }

        /// <summary>
        /// Current read
        /// </summary>
        public uint WindowCurrent { get; set; }

        /// <summary>
        /// Length last got
        /// </summary>
        public uint WindowLength { get; set; }
    }
}