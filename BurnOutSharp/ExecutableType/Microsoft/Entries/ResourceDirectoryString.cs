using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Entries
{
    /// <summary>
    /// The resource directory string area consists of Unicode strings, which are word-aligned.
    /// These strings are stored together after the last Resource Directory entry and before the first Resource Data entry.
    /// This minimizes the impact of these variable-length strings on the alignment of the fixed-size directory entries.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class ResourceDirectoryString
    {
        /// <summary>
        /// The size of the string, not including length field itself.
        /// </summary>
        public ushort Length;

        /// <summary>
        /// The variable-length Unicode string data, word-aligned.
        /// </summary>
        public string UnicodeString;

        public static ResourceDirectoryString Deserialize(Stream stream)
        {
            var rds = new ResourceDirectoryString();

            rds.Length = stream.ReadUInt16();
            rds.UnicodeString = new string(stream.ReadChars(rds.Length, Encoding.Unicode));

            return rds;
        }

        public static ResourceDirectoryString Deserialize(byte[] content, int offset)
        {
            var rds = new ResourceDirectoryString();

            rds.Length = BitConverter.ToUInt16(content, offset); offset += 2;
            rds.UnicodeString = Encoding.Unicode.GetString(content, offset, rds.Length);

            return rds;
        }
    }
}
