using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Entries
{
    /// <summary>
    /// Resource type information block
    /// </summary>
    public class ResourceTypeInformationBlock
    {
        /// <summary>
        /// Type ID. This is an integer type if the high-order bit is
        /// set (8000h); otherwise, it is an offset to the type string,
        /// the offset is relative to the beginning of the resource
        /// table. A zero type ID marks the end of the resource type
        /// information blocks.
        /// </summary>
        public ushort TypeID;
        
        /// <summary>
        /// Number of resources for this type.
        /// </summary>
        public ushort ResourceCount;
        
        /// <summary>
        /// Reserved.
        /// </summary>
        public uint Reserved;

        /// <summary>
        /// Reserved.
        /// </summary>
        public NEResourceTableEntry[] ResourceTable;

        public static ResourceTypeInformationBlock Deserialize(Stream stream)
        {
            var rtib = new ResourceTypeInformationBlock();

            rtib.TypeID = stream.ReadUInt16();
            rtib.ResourceCount = stream.ReadUInt16();
            rtib.Reserved = stream.ReadUInt32();

            rtib.ResourceTable = new NEResourceTableEntry[rtib.ResourceCount];
            for (int i = 0; i < rtib.ResourceCount; i++)
            {
                rtib.ResourceTable[i] = NEResourceTableEntry.Deserialize(stream);
            }

            return rtib;
        }

        public static ResourceTypeInformationBlock Deserialize(byte[] content, ref int offset)
        {
            var rtib = new ResourceTypeInformationBlock();

            rtib.TypeID = BitConverter.ToUInt16(content, offset); offset += 2;
            rtib.ResourceCount = BitConverter.ToUInt16(content, offset); offset += 2;
            rtib.Reserved = BitConverter.ToUInt32(content, offset); offset += 4;

            rtib.ResourceTable = new NEResourceTableEntry[rtib.ResourceCount];
            for (int i = 0; i < rtib.ResourceCount; i++)
            {
                rtib.ResourceTable[i] = NEResourceTableEntry.Deserialize(content, ref offset);
            }

            return rtib;
        }
    }
}