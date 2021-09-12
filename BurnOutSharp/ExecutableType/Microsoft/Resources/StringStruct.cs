using System.IO;
using System.Text;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Resources
{
    public class StringStruct : Resource
    {
        /// <summary>
        /// Typically contains a list of languages that the application or DLL supports.
        /// </summary>
        public string Value;

        public StringStruct(Resource resource)
        {
            this.Length = resource?.Length ?? default;
            this.ValueLength = resource?.ValueLength ?? default;
            this.Type = resource?.Type ?? default;
            this.Key = resource?.Key ?? default;
        }

        public static new StringStruct Deserialize(Stream stream)
        {
            Resource resource = Resource.Deserialize(stream);
            StringStruct s = new StringStruct(resource);
            stream.Seek(stream.Position % 4 == 0 ? 0 : 4 - (stream.Position % 4), SeekOrigin.Current);
            s.Value = new string(stream.ReadChars(s.ValueLength));

            return s;
        }

        public static new StringStruct Deserialize(byte[] content, ref int offset)
        {
            Resource resource = Resource.Deserialize(content, ref offset);
            StringStruct s = new StringStruct(resource);
            offset += offset % 4 == 0 ? 0 : 4 - (offset % 4);
            s.Value = Encoding.Unicode.GetString(content, offset, s.ValueLength * 2); offset += s.ValueLength * 2;
            
            return s;
        }
    }
}