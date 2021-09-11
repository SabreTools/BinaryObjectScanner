using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Entries
{
    /// <summary>
    /// A table of resources for this type
    /// </summary>
    public class NEResourceTableEntry
    {
        /// <summary>
        /// File offset to the contents of the resource data,
        /// relative to beginning of file. The offset is in terms
        /// of the alignment shift count value specified at
        /// beginning of the resource table.
        /// </summary>
        public ushort Offset;
        
        /// <summary>
        /// Length of the resource in the file (in bytes).
        /// </summary>
        public ushort Length;
        
        /// <summary>
        /// Resource flags
        /// </summary>
        public ResourceTableEntryFlags Flags;
        
        /// <summary>
        /// This is an integer type if the high-order
        /// bit is set (8000h), otherwise it is the offset to the
        /// resource string, the offset is relative to the
        /// beginning of the resource table.
        /// </summary>
        public ushort ResourceID;
        
        /// <summary>
        /// Reserved.
        /// </summary>
        public ushort Handle;
        
        /// <summary>
        /// Reserved.
        /// </summary>
        public ushort Usage;

        public static NEResourceTableEntry Deserialize(Stream stream)
        {
            var ni = new NEResourceTableEntry();

            ni.Offset = stream.ReadUInt16();
            ni.Length = stream.ReadUInt16();
            ni.Flags = (ResourceTableEntryFlags)stream.ReadUInt16();
            ni.ResourceID = stream.ReadUInt16();
            ni.Handle = stream.ReadUInt16();
            ni.Usage = stream.ReadUInt16();

            return ni;
        }

        public static NEResourceTableEntry Deserialize(byte[] content, ref int offset)
        {
            var ni = new NEResourceTableEntry();

            ni.Offset = content.ReadUInt16(ref offset);
            ni.Length = content.ReadUInt16(ref offset);
            ni.Flags = (ResourceTableEntryFlags)content.ReadUInt16(ref offset);
            ni.ResourceID = content.ReadUInt16(ref offset);
            ni.Handle = content.ReadUInt16(ref offset);
            ni.Usage = content.ReadUInt16(ref offset);

            return ni;
        }
    }
}