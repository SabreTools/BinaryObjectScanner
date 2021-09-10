using System.IO;
using System.Text;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Entries
{
    /// <summary>
    /// Resource type and name strings
    /// </summary>
    internal class NEResourceNameString
    {
        /// <summary>
        /// Length of the type or name string that follows. A zero value
        /// indicates the end of the resource type and name string, also
        /// the end of the resource table.
        /// </summary>
        public byte Length;

        /// <summary>
        /// ASCII text of the type or name string.
        /// </summary>
        public char[] Value;

        public static NEResourceNameString Deserialize(Stream stream)
        {
            var rds = new NEResourceNameString();

            rds.Length = stream.ReadByteValue();
            rds.Value = stream.ReadChars(rds.Length, Encoding.ASCII);

            return rds;
        }

        public static NEResourceNameString Deserialize(byte[] content, ref int offset)
        {
            var rds = new NEResourceNameString();

            rds.Length = content[offset++];
            rds.Value = Encoding.ASCII.GetChars(content, offset, rds.Length); offset += rds.Length;

            return rds;
        }
    }
}
