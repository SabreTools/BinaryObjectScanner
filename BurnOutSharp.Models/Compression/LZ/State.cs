using System.IO;

namespace BurnOutSharp.Models.Compression.LZ
{
    public sealed class State
    {
        /// <summary>
        /// The real filedescriptor
        /// </summary>
        public Stream RealFD { get; set; }

        /// <summary>
        /// The last char of the filename
        /// </summary>
        public char LastChar { get; set; }

        /// <summary>
        /// The decompressed length of the file
        /// </summary>
        public uint RealLength { get; set; }

        /// <summary>
        /// The position the decompressor currently is
        /// </summary>
        public uint RealCurrent { get; set; }

        /// <summary>
        /// The position the user wants to read from
        /// </summary>
        public uint RealWanted { get; set; }

        /// <summary>
        /// The rotating LZ table
        /// </summary>
        public byte[] Table { get; set; }

        /// <summary>
        /// CURrent TABle ENTry
        /// </summary>
        public uint CurTabEnt { get; set; }

        /// <summary>
        /// Length and position of current string
        /// </summary>
        public byte StringLen { get; set; }

        /// <summary>
        /// From stringtable
        /// </summary>
        public uint StringPos { get; set; }

        /// <summary>
        /// Bitmask within blocks
        /// </summary>
        public ushort ByteType { get; set; }

        /// <summary>
        /// GETLEN bytes
        /// </summary>
        public byte[] Get { get; set; }

        /// <summary>
        /// Current read
        /// </summary>
        public uint GetCur { get; set; }

        /// <summary>
        /// Length last got
        /// </summary>
        public uint GetLen { get; set; }
    }
}