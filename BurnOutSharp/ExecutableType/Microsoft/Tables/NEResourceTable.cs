using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using BurnOutSharp.ExecutableType.Microsoft.Entries;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Tables
{
    /// <summary>
    /// The resource table follows the segment table and contains entries for
    /// each resource in the executable file. The resource table consists of
    /// an alignment shift count, followed by a table of resource records. The
    /// resource records define the type ID for a set of resources. Each
    /// resource record contains a table of resource entries of the defined
    /// type. The resource entry defines the resource ID or name ID for the
    /// resource. It also defines the location and size of the resource.
    /// </summary>
    /// <remarks>http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm</remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal class NEResourceTable
    {
        /// <summary>
        /// Alignment shift count for resource data.
        /// </summary>
        public ushort AlignmentShiftCount;

        /// <summary>
        /// A table of resource type information blocks.
        /// </summary>
        public ResourceTypeInformationBlock[] TypeInformationBlocks;

        /// <summary>
        /// Resource type and name strings are stored at the end of the
        /// resource table. Note that these strings are NOT null terminated and
        /// are case sensitive.
        /// </summary>
        public NEResourceNameString[] TypeAndNameStrings;

        public static NEResourceTable Deserialize(Stream stream)
        {
            var nrt = new NEResourceTable();

            nrt.AlignmentShiftCount = stream.ReadUInt16();
            var typeInformationBlocks = new List<ResourceTypeInformationBlock>();
            while (true)
            {
                var block = ResourceTypeInformationBlock.Deserialize(stream);
                if (block.TypeID == 0)
                    break;
                
                typeInformationBlocks.Add(block);
            }

            nrt.TypeInformationBlocks = typeInformationBlocks.ToArray();

            var typeAndNameStrings = new List<NEResourceNameString>();
            while (true)
            {
                var str = NEResourceNameString.Deserialize(stream);
                if (str.Length == 0)
                    break;
                
                typeAndNameStrings.Add(str);
            }

            nrt.TypeAndNameStrings = typeAndNameStrings.ToArray();

            return nrt;
        }

        public static NEResourceTable Deserialize(byte[] contents, int offset)
        {
            var nrt = new NEResourceTable();

            nrt.AlignmentShiftCount = BitConverter.ToUInt16(contents, offset); offset += 2;
            var typeInformationBlocks = new List<ResourceTypeInformationBlock>();
            while (true)
            {
                unsafe
                {
                    var block = ResourceTypeInformationBlock.Deserialize(contents, offset); offset += Marshal.SizeOf(block);
                    if (block.TypeID == 0)
                        break;
                    
                    typeInformationBlocks.Add(block);
                }
            }

            nrt.TypeInformationBlocks = typeInformationBlocks.ToArray();

            var typeAndNameStrings = new List<NEResourceNameString>();
            while (true)
            {
                unsafe
                {
                    var str = NEResourceNameString.Deserialize(contents, offset); offset += Marshal.SizeOf(str);
                    if (str.Length == 0)
                        break;
                    
                    typeAndNameStrings.Add(str);
                }
            }

            nrt.TypeAndNameStrings = typeAndNameStrings.ToArray();

            return nrt;
        }
    }
}