using System.Collections.Generic;
using System.IO;

namespace BurnOutSharp.ExecutableType.Microsoft.Resources
{
    public class StringTable : Resource
    {
        /// <summary>
        /// An array of one or more String structures.
        /// </summary>
        public StringStruct[] Children;

        public StringTable(Resource resource)
        {
            this.Length = resource?.Length ?? default;
            this.ValueLength = resource?.ValueLength ?? default;
            this.Type = resource?.Type ?? default;
            this.Key = resource?.Key ?? default;
        }

        public static new StringTable Deserialize(Stream stream)
        {
            long originalPosition = stream.Position;
            Resource resource = Resource.Deserialize(stream);
            if (resource.Key.Length != 8)
                return null;

            StringTable st = new StringTable(resource);

            var tempValue = new List<StringStruct>();
            while (stream.Position - originalPosition < st.Length)
            {
                tempValue.Add(StringStruct.Deserialize(stream));
            }

            st.Children = tempValue.ToArray();

            return st;
        }

        public static new StringTable Deserialize(byte[] content, ref int offset)
        {
            int originalPosition = offset;
            Resource resource = Resource.Deserialize(content, ref offset);
            if (resource.Key.Length != 8)
                return null;

            StringTable st = new StringTable(resource);
        
            var tempValue = new List<StringStruct>();
            while (offset - originalPosition < st.Length)
            {
                tempValue.Add(StringStruct.Deserialize(content, ref offset));
            }

            st.Children = tempValue.ToArray();

            return st;
        }
    }
}