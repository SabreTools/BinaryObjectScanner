using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.BFPK
{
    /// <summary>
    /// Header
    /// </summary>
    /// <see cref="https://forum.xentax.com/viewtopic.php?t=5102"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Header
    {
        /// <summary>
        /// "BFPK"
        /// </summary>
        public string Magic;

        /// <summary>
        /// Version
        /// </summary>
        public int Version;

        /// <summary>
        /// Files
        /// </summary>
        public int Files;
    }
}
