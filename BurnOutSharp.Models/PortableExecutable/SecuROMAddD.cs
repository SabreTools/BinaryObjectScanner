using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.PortableExecutable
{
    /// <summary>
    /// Overlay data associated with SecuROM executables
    /// </summary>
    /// <remarks>
    /// All information in this file has been researched in a clean room
    /// environment by using sample from legally obtained software that
    /// is protected by SecuROM.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public class SecuROMAddD
    {
        /// <summary>
        /// "AddD", Identifier?
        /// </summary>
        public uint Signature;

        /// <summary>
        /// Unknown (Entry count?)
        /// </summary>
        public uint EntryCount;

        /// <summary>
        /// Version
        /// </summary>
        public string Version;

        /// <summary>
        /// Unknown (Build? Formatted as a string)
        /// </summary>
        public char[] Build;

        /// <summary>
        /// Unknown (0x14)
        /// </summary>
        /// <remarks>498211340 [6668, 3762] in the sample</remarks>
        public uint Unknown14h;

        /// <summary>
        /// Unknown (0x18), possibly entry 1 date/time?
        /// </summary>
        /// <remarks>783556112 [7696, 11956] / 1994-10-30 22:28:32 in the sample</remarks>
        public uint Unknown18h;

        /// <summary>
        /// Unknown (0x1C), possibly entry 2 date/time?
        /// </summary>
        /// <remarks>861548360 [12104, 13146] / 1997-04-20 14:59:20 in the sample</remarks>
        public uint Unknown1Ch;

        /// <summary>
        /// Unknown (0x20), possibly entry 3 date/time?
        /// </summary>
        /// <remarks>879113110 [13206, 13414] / 1997-11-09 22:05:10 in the sample</remarks>
        public uint Unknown20h;

        /// <summary>
        /// Unknown (0x24)
        /// </summary>
        /// <remarks>4212741 [18437, 64] in the sample</remarks>
        public uint Unknown24h;

        /// <summary>
        /// Unknown (0x28)
        /// </summary>
        /// <remarks>1364015180 [14412, 20813] in the sample</remarks>
        public uint Unknown28h;

        /// <summary>
        /// Unknown (0x2C)
        /// </summary>
        /// <remarks>2 [2, 0] in the sample</remarks>
        public uint Unknown2Ch;

        /// <summary>
        /// Unknown (0x30)
        /// </summary>
        /// <remarks>4270144 [10304, 65] in the sample</remarks>
        public uint Unknown30h;

        /// <summary>
        /// Unknown (0x34)
        /// </summary>
        /// <remarks>132 [132, 0] in the sample</remarks>
        public uint Unknown34h;

        /// <summary>
        /// Unknown (0x38)
        /// </summary>
        /// <remarks>3147176 [1448, 48] in the sample</remarks>
        public uint Unknown38h;

        /// <summary>
        /// Unknown (0x3C)
        /// </summary>
        /// <remarks>1244976 [65328, 18] in the sample</remarks>
        public uint Unknown3Ch;

        /// <summary>
        /// Entry table
        /// </summary>
        public SecuROMAddDEntry[] Entries;
    }
}
