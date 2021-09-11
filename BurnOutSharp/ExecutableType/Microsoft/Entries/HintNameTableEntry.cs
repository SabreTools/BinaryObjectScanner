using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Entries
{
    /// <summary>
    /// Each entry in the hint/name table has the following format
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#hintname-table</remarks>
    public class HintNameTableEntry
    {
        /// <summary>
        /// An index into the export name pointer table.
        /// A match is attempted first with this value.
        /// If it fails, a binary search is performed on the DLL's export name pointer table.
        /// </summary>
        public ushort Hint;

        /// <summary>
        /// An ASCII string that contains the name to import.
        /// This is the string that must be matched to the public name in the DLL.
        /// This string is case sensitive and terminated by a null byte.
        /// </summary>
        public string Name;

        /// <summary>
        /// A trailing zero-pad byte that appears after the trailing null byte, if necessary, to align the next entry on an even boundary.
        /// </summary>
        public byte Pad;

        public static HintNameTableEntry Deserialize(Stream stream)
        {
            var hnte = new HintNameTableEntry();

            hnte.Hint = stream.ReadUInt16();
            hnte.Name = string.Empty;
            while (true)
            {
                char c = stream.ReadChar();
                if (c == (char)0x00)
                    break;
                
                hnte.Name += c;
            }

            // If the name length is not even, read and pad
            if (hnte.Name.Length % 2 != 0)
            {
                stream.ReadByte();
                hnte.Pad = 1;
            }
            else
            {
                hnte.Pad = 0;
            }

            return hnte;
        }

        public static HintNameTableEntry Deserialize(byte[] content, ref int offset)
        {
            var hnte = new HintNameTableEntry();

            hnte.Hint = content.ReadUInt16(ref offset);
            hnte.Name = string.Empty;
            while (true)
            {
                char c = (char)content[offset]; offset += 1;
                if (c == (char)0x00)
                    break;
                
                hnte.Name += c;
            }

            // If the name length is not even, read and pad
            if (hnte.Name.Length % 2 != 0)
            {
                offset += 1;
                hnte.Pad = 1;
            }
            else
            {
                hnte.Pad = 0;
            }

            return hnte;
        }
    }
}