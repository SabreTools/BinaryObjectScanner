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

        public static new StringTable Deserialize(Stream stream)
        {
            StringTable st = new StringTable();

            Resource resource = Resource.Deserialize(stream);
            if (resource.Key.Length != 8)
                return null;

            st.Length = resource.Length;
            st.ValueLength = resource.ValueLength;
            st.Type = resource.Type;
            st.Key = resource.Key;

            // TODO: Deserialize the Value array
            stream.Seek(st.Length - 6 - (st.Key.Length * 2), SeekOrigin.Begin);

            return st;
        }

        public static new StringTable Deserialize(byte[] content, ref int offset)
        {
            int originalPosition = offset;
            StringTable st = new StringTable();

            Resource resource = Resource.Deserialize(content, ref offset);
            if (resource.Key.Length != 8)
                return null;

            st.Length = resource.Length;
            st.ValueLength = resource.ValueLength;
            st.Type = resource.Type;
            st.Key = resource.Key;
        
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