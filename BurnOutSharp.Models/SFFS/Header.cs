using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.SFFS
{
    /// <summary>
    /// Header
    /// </summary>
    /// <see cref="https://forum.xentax.com/viewtopic.php?f=21&t=2084"/>
    [StructLayout(LayoutKind.Sequential)]
    public class Header
    {
        /// <summary>
        /// "SFFS"
        /// </summary>
        public uint Magic;

        /// <summary>
        /// Version (0x00000001)
        /// </summary>
        public uint Version;

        /// <summary>
        /// Number of files in the container (encrypted with application key).
        /// Minimal number here is usually 65h, so its not real file count
        /// in some cases, more like index size
        /// </summary>
        public ulong FileCount;
    }
}
