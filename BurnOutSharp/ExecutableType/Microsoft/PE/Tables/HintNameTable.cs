using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.PE.Entries;

namespace BurnOutSharp.ExecutableType.Microsoft.PE.Tables
{
    /// <summary>
    /// One hint/name table suffices for the entire import section.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#hintname-table</remarks>
    public class HintNameTable
    {
        /// <remarks>Number of entries is defined externally</remarks>
        public HintNameTableEntry[] Entries;

        public static HintNameTable Deserialize(Stream stream, int count)
        {
            var hnt = new HintNameTable();

            hnt.Entries = new HintNameTableEntry[count];
            for (int i = 0; i < count; i++)
            {
                hnt.Entries[i] = HintNameTableEntry.Deserialize(stream);
            }

            return hnt;
        }

        public static HintNameTable Deserialize(byte[] content, ref int offset, int count)
        {
            var hnt = new HintNameTable();

            hnt.Entries = new HintNameTableEntry[count];
            for (int i = 0; i < count; i++)
            {
                hnt.Entries[i] = HintNameTableEntry.Deserialize(content, ref offset);
                offset += 2 + hnt.Entries[i].Name.Length + hnt.Entries[i].Pad;
            }

            return hnt;
        }
    }
}