using System.IO;
using System.Text;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Resources
{
    public class Resource
    {
        /// <summary>
        /// The length, in bytes, of the resource structure.
        /// This length does not include any padding that aligns any subsequent version resource data on a 32-bit boundary.
        /// </summary>
        public ushort Length;

        /// <summary>
        /// The length, in bytes, of the Value member.
        /// This value is zero if there is no Value member associated with the current version structure.
        /// </summary>
        public ushort ValueLength;

        /// <summary>
        /// The type of data in the version resource.
        /// This member is 1 if the version resource contains text data and 0 if the version resource contains binary data.
        /// </summary>
        public ushort Type;

        /// <summary>
        /// A Unicode string representing the key
        /// </summary>
        public string Key;

        public static Resource Deserialize(Stream stream)
        {
            Resource r = new Resource();

            while ((r.Length = stream.ReadUInt16()) == 0x0000);

            r.ValueLength = stream.ReadUInt16();
            r.Type = stream.ReadUInt16();
            r.Key = stream.ReadString(Encoding.Unicode);

            return r;
        }

        public static Resource Deserialize(byte[] content, ref int offset)
        {
            Resource r = new Resource();

            while ((r.Length = content.ReadUInt16(ref offset)) == 0x0000);

            r.ValueLength = content.ReadUInt16(ref offset);
            r.Type = content.ReadUInt16(ref offset);
            r.Key = content.ReadString(ref offset, Encoding.Unicode);

            return r;
        }
    }
}