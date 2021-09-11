using System;
using System.IO;
using System.Text;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Entries
{
    /// <summary>
    /// The resource directory string area consists of Unicode strings, which are word-aligned.
    /// These strings are stored together after the last Resource Directory entry and before the first Resource Data entry.
    /// This minimizes the impact of these variable-length strings on the alignment of the fixed-size directory entries.
    /// </summary>
    public class ResourceDirectoryString
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
            if (rds.Length + stream.Position > stream.Length)
                return null;

            rds.UnicodeString = new string(stream.ReadChars(rds.Length, Encoding.Unicode));

            return rds;
        }

        public static ResourceDirectoryString Deserialize(byte[] content, ref int offset)
        {
            var rds = new ResourceDirectoryString();

            rds.Length = content.ReadUInt16(ref offset);
            if (rds.Length + offset > content.Length)
                return null;

            rds.UnicodeString = Encoding.Unicode.GetString(content, offset, rds.Length); offset += rds.Length;

            return rds;
        }
    }
}
