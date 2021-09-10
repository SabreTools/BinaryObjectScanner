using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Resources
{
    internal class Resource
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

            r.Length = stream.ReadUInt16();
            r.ValueLength = stream.ReadUInt16();
            r.Type = stream.ReadUInt16();
            r.Key = stream.ReadString(Encoding.Unicode);

            return r;
        }

        public static Resource Deserialize(byte[] content, ref int offset)
        {
            Resource r = new Resource();

            while ((r.Length = BitConverter.ToUInt16(content, offset)) == 0x0000)
            {
                offset += 2;
            }

            offset += 2;
            r.ValueLength = BitConverter.ToUInt16(content, offset); offset += 2;
            r.Type = BitConverter.ToUInt16(content, offset); offset += 2;

            List<char> keyChars = new List<char>();
            while (BitConverter.ToUInt16(content, offset) != 0x0000)
            {
                keyChars.Add(Encoding.Unicode.GetChars(content, offset, 2)[0]); offset += 2;
            }
            offset += 2;

            r.Key = new string(keyChars.ToArray());

            return r;
        }
    }
}