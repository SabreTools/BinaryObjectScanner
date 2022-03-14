using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.NE.Entries;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.NE.Tables
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
    public class ResourceTable
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
        public ResourceNameString[] TypeAndNameStrings;

        public static ResourceTable Deserialize(Stream stream)
        {
            var rt = new ResourceTable();

            rt.AlignmentShiftCount = stream.ReadUInt16();
            var typeInformationBlocks = new List<ResourceTypeInformationBlock>();
            while (true)
            {
                var block = ResourceTypeInformationBlock.Deserialize(stream);
                if (block.TypeID == 0)
                    break;
                
                typeInformationBlocks.Add(block);
            }

            rt.TypeInformationBlocks = typeInformationBlocks.ToArray();

            var typeAndNameStrings = new List<ResourceNameString>();
            while (true)
            {
                var str = ResourceNameString.Deserialize(stream);
                if (str.Length == 0)
                    break;
                
                typeAndNameStrings.Add(str);
            }

            rt.TypeAndNameStrings = typeAndNameStrings.ToArray();

            return rt;
        }

        public static ResourceTable Deserialize(byte[] content, ref int offset)
        {
            var rt = new ResourceTable();

            rt.AlignmentShiftCount = content.ReadUInt16(ref offset);
            var typeInformationBlocks = new List<ResourceTypeInformationBlock>();
            while (true)
            {
                var block = ResourceTypeInformationBlock.Deserialize(content, ref offset);
                if (block.TypeID == 0)
                    break;

                typeInformationBlocks.Add(block);
            }

            rt.TypeInformationBlocks = typeInformationBlocks.ToArray();

            var typeAndNameStrings = new List<ResourceNameString>();
            while (true)
            {
                var str = ResourceNameString.Deserialize(content, ref offset);
                if (str.Length == 0)
                    break;
                    
                typeAndNameStrings.Add(str);
            }

            rt.TypeAndNameStrings = typeAndNameStrings.ToArray();

            return rt;
        }
    }
}