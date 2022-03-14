using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.NE.Entries
{
    /// <summary>
    /// The segment table contains an entry for each segment in the executable
    /// file. The number of segment table entries are defined in the segmented
    /// EXE header. The first entry in the segment table is segment number 1.
    /// The following is the structure of a segment table entry.
    /// </summary>
    public class SegmentTableEntry
    {
        /// <summary>
        /// Logical-sector offset (n byte) to the contents of the segment
        /// data, relative to the beginning of the file. Zero means no
        /// file data.
        /// </summary>
        public ushort StartFileSector;
        
        /// <summary>
        /// Length of the segment in the file, in bytes. Zero means 64K.
        /// </summary>
        public ushort BytesInFile;
        
        /// <summary>
        /// Attribute flags
        /// </summary>
        public SegmentTableEntryFlags Flags;
        
        /// <summary>
        /// Minimum allocation size of the segment, in bytes.
        /// Total size of the segment. Zero means 64K
        /// </summary>
        public ushort MinimumAllocation;

        public static SegmentTableEntry Deserialize(Stream stream)
        {
            var nste = new SegmentTableEntry();

            nste.StartFileSector = stream.ReadUInt16();
            nste.BytesInFile = stream.ReadUInt16();
            nste.Flags = (SegmentTableEntryFlags)stream.ReadUInt16();
            nste.MinimumAllocation = stream.ReadUInt16();

            return nste;
        }

        public static SegmentTableEntry Deserialize(byte[] content, ref int offset)
        {
            var nste = new SegmentTableEntry();

            nste.StartFileSector = content.ReadUInt16(ref offset);
            nste.BytesInFile = content.ReadUInt16(ref offset);
            nste.Flags = (SegmentTableEntryFlags)content.ReadUInt16(ref offset);
            nste.MinimumAllocation = content.ReadUInt16(ref offset);

            return nste;
        }
    }
}
