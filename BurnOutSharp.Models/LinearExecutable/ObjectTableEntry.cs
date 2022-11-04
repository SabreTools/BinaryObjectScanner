using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.LinearExecutable
{
    /// <summary>
    /// The object table contains information that describes each segment in
    /// an executable file. This information includes segment length, segment
    /// type, and segment-relocation data. The following list summarizes the
    /// values found in in the segment table (the locations are relative to
    /// the beginning of each entry):
    /// </summary>
    /// <see href="https://faydoc.tripod.com/formats/exe-LE.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public class ObjectTableEntry
    {
        /// <summary>
        /// Virtual segment size in bytes
        /// </summary>
        public uint VirtualSegmentSize;

        /// <summary>
        /// Relocation base address
        /// </summary>
        public uint RelocationBaseAddress;

        /// <summary>
        /// Object flags
        /// </summary>
        /// <remarks>No flags are documented properly</remarks>
        public uint ObjectFlags;

        /// <summary>
        /// Page map index
        /// </summary>
        public uint PageMapIndex;

        /// <summary>
        /// Page map entries
        /// </summary>
        public uint PageMapEntries;

        /// <summary>
        /// ???
        /// </summary>
        public uint Reserved;
    }
}
