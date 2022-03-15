using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.PE.Entries
{
    /// <summary>
    /// The base relocation table is divided into blocks.
    /// Each block represents the base relocations for a 4K page.
    /// Each block must start on a 32-bit boundary.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#base-relocation-block</remarks>
    public class BaseRelocationBlock
    {
        /// <summary>
        /// The image base plus the page RVA is added to each offset to create the VA where the base relocation must be applied.
        /// </summary>
        public uint PageRVA;

        /// <summary>
        /// The total number of bytes in the base relocation block, including the Page RVA and Block Size fields and the Type/Offset fields that follow.
        /// </summary>
        public uint BlockSize;

        public static BaseRelocationBlock Deserialize(Stream stream)
        {
            var brb = new BaseRelocationBlock();

            brb.PageRVA = stream.ReadUInt32();
            brb.BlockSize = stream.ReadUInt32();

            // TODO: Read in the type/offset field entries

            return brb;
        }

        public static BaseRelocationBlock Deserialize(byte[] content, ref int offset)
        {
            var brb = new BaseRelocationBlock();

            brb.PageRVA = content.ReadUInt32(ref offset);
            brb.BlockSize = content.ReadUInt32(ref offset);

            // TODO: Read in the type/offset field entries

            return brb;
        }
    }
}