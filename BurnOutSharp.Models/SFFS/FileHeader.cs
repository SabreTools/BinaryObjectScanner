using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.SFFS
{
    /// <see cref="https://forum.xentax.com/viewtopic.php?f=21&t=2084"/>
    [StructLayout(LayoutKind.Sequential)]
    public class FileHeader
    {
        /// <summary>
        /// Start of file content (encrypted with filename)
        /// </summary>
        public ulong FileContentStart;

        /// <summary>
        /// File info (timestamps, size, data position, encrypted)
        /// </summary>
        /// <remarks>Unknown format</remarks>
        public byte[] FileInfo;
    }
}
