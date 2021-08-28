using System;
using System.IO;
using System.Runtime.InteropServices;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Entries
{
    /// <summary>
    /// A table of resources for this type
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class NEResourceTableEntry
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

        public static NEResourceTableEntry Deserialize(byte[] contents, int offset)
        {
            var ni = new NEResourceTableEntry();

            ni.Offset = BitConverter.ToUInt16(contents, offset); offset += 2;
            ni.Length = BitConverter.ToUInt16(contents, offset); offset += 2;
            ni.Flags = (ResourceTableEntryFlags)BitConverter.ToUInt16(contents, offset); offset += 2;
            ni.ResourceID = BitConverter.ToUInt16(contents, offset); offset += 2;
            ni.Handle = BitConverter.ToUInt16(contents, offset); offset += 2;
            ni.Usage = BitConverter.ToUInt16(contents, offset); offset += 2;

            return ni;
        }
    }
}