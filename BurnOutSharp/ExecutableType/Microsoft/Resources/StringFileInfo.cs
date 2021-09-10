using System.IO;

namespace BurnOutSharp.ExecutableType.Microsoft.Resources
{
    public class StringFileInfo : Resource
    {
        /// <summary>
        /// An array of one or more StringTable structures.
        /// Each StringTable structure's szKey member indicates the appropriate language and code page for displaying the text in that StringTable structure.
        /// </summary>
        public StringTable Children;

        public static new StringFileInfo Deserialize(Stream stream)
        {
            StringFileInfo sfi = new StringFileInfo();

            Resource resource = Resource.Deserialize(stream);
            if (resource.Key != "StringFileInfo")
                return null;

            sfi.Length = resource.Length;
            sfi.ValueLength = resource.ValueLength;
            sfi.Type = resource.Type;
            sfi.Key = resource.Key;
            sfi.Children = StringTable.Deserialize(stream);

            return sfi;
        }

        public static new StringFileInfo Deserialize(byte[] content, ref int offset)
        {
            StringFileInfo sfi = new StringFileInfo();
            
            Resource resource = Resource.Deserialize(content, ref offset);
            if (resource.Key != "StringFileInfo")
                return null;

            sfi.Length = resource.Length;
            sfi.ValueLength = resource.ValueLength;
            sfi.Type = resource.Type;
            sfi.Key = resource.Key;
            sfi.Children = StringTable.Deserialize(content, ref offset);

            return sfi;
        }
    }
}