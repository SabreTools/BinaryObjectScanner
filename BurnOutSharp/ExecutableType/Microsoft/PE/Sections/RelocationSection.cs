using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE.Entries;

namespace BurnOutSharp.ExecutableType.Microsoft.PE.Sections
{
    /// <summary>
    /// The base relocation table contains entries for all base relocations in the image.
    /// The Base Relocation Table field in the optional header data directories gives the number of bytes in the base relocation table.
    /// The base relocation table is divided into blocks.
    /// Each block represents the base relocations for a 4K page.
    /// Each block must start on a 32-bit boundary.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#the-reloc-section-image-only</remarks>
    public class RelocationSection
    {
        /// <summary>
        /// The base relocation table is divided into blocks.
        /// </summary>
        public BaseRelocationBlock[] BaseRelocationTable;

        public static RelocationSection Deserialize(Stream stream, int blockCount)
        {
            long originalPosition = stream.Position;

            var rs = new RelocationSection();
            rs.BaseRelocationTable = new BaseRelocationBlock[blockCount];
            for (int i = 0; i < blockCount; i++)
            {
                rs.BaseRelocationTable[i] = BaseRelocationBlock.Deserialize(stream);
            }

            stream.Seek(originalPosition, SeekOrigin.Begin);
            return rs;
        }

        public static RelocationSection Deserialize(byte[] content, ref int offset, int blockCount)
        {
            int originalPosition = offset;

            var rs = new RelocationSection();
            rs.BaseRelocationTable = new BaseRelocationBlock[blockCount];
            for (int i = 0; i < blockCount; i++)
            {
                rs.BaseRelocationTable[i] = BaseRelocationBlock.Deserialize(content, ref offset);
            }

            offset = originalPosition;
            return rs;
        }
    }
}