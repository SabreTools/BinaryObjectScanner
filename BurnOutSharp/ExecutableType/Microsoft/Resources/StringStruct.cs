using System;
using System.IO;
using System.Text;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Resources
{
    internal class StringStruct : Resource
    {
        /// <summary>
        /// Typically contains a list of languages that the application or DLL supports.
        /// </summary>
        public string Value;

        public static new StringStruct Deserialize(Stream stream)
        {
            StringStruct s = new StringStruct();

            Resource resource = Resource.Deserialize(stream);

            s.Length = resource.Length;
            s.ValueLength = resource.ValueLength;
            s.Type = resource.Type;
            s.Key = resource.Key;
            stream.Seek(stream.Position % 4 == 0 ? 0 : 4 - (stream.Position % 4), SeekOrigin.Current);
            s.Value = new string(stream.ReadChars(s.ValueLength));

            return s;
        }

        public static new StringStruct Deserialize(byte[] content, ref int offset)
        {
            StringStruct s = new StringStruct();

            Resource resource = Resource.Deserialize(content, ref offset);

            s.Length = resource.Length;
            s.ValueLength = resource.ValueLength;
            s.Type = resource.Type;
            s.Key = resource.Key;
            offset += offset % 4 == 0 ? 0 : 4 - (offset % 4);
            s.Value = Encoding.Unicode.GetString(content, offset, s.ValueLength * 2); offset += s.ValueLength * 2;
            
            return s;
        }
    }
}