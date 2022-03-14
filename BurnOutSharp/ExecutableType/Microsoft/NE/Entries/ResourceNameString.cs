using System.IO;
using System.Text;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.NE.Entries
{
    /// <summary>
    /// Resource type and name strings
    /// </summary>
    public class ResourceNameString
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

        public static ResourceNameString Deserialize(Stream stream)
        {
            var rns = new ResourceNameString();

            rns.Length = stream.ReadByteValue();
            rns.Value = stream.ReadChars(rns.Length, Encoding.ASCII);

            return rns;
        }

        public static ResourceNameString Deserialize(byte[] content, ref int offset)
        {
            var rns = new ResourceNameString();

            rns.Length = content.ReadByte(ref offset);
            rns.Value = Encoding.ASCII.GetChars(content, offset, rns.Length); offset += rns.Length;

            return rns;
        }
    }
}
