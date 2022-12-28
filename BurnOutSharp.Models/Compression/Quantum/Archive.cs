using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.Compression.Quantum
{
    /// <summary>
    /// Quantum archive file structure
    /// </summary>
    /// <see href="https://handwiki.org/wiki/Software:Quantum_compression"/>
    [StructLayout(LayoutKind.Sequential)]
    public class Archive
    {
        /// <summary>
        /// Quantum signature: 0x44 0x53
        /// </summary>
        public ushort Signature;

        /// <summary>
        /// Quantum major version number
        /// </summary>
        public byte MajorVersion;

        /// <summary>
        /// Quantum minor version number
        /// </summary>
        public byte MinorVersion;

        /// <summary>
        /// Number of files within this archive
        /// </summary>
        public ushort FileCount;

        /// <summary>
        /// Table size required for decompression
        /// </summary>
        public byte TableSize;

        /// <summary>
        /// Compression flags
        /// </summary>
        public byte CompressionFlags;

        /// <summary>
        /// This is immediately followed by the list of files
        /// </summary>
        public FileDescriptor[] FileList;

        // Immediately following the list of files is the compressed data. 
    }
}