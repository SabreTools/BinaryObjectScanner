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

        public StringFileInfo(Resource resource)
        {
            this.Length = resource?.Length ?? default;
            this.ValueLength = resource?.ValueLength ?? default;
            this.Type = resource?.Type ?? default;
            this.Key = resource?.Key?.TrimStart('\u0001') ?? default;
        }

        public static new StringFileInfo Deserialize(Stream stream)
        {
            Resource resource = Resource.Deserialize(stream);
            if (resource.Key != "StringFileInfo" && resource.Key != "\u0001StringFileInfo")
                return null;

            StringFileInfo sfi = new StringFileInfo(resource);
            sfi.Children = StringTable.Deserialize(stream);

            return sfi;
        }

        public static new StringFileInfo Deserialize(byte[] content, ref int offset)
        {
            Resource resource = Resource.Deserialize(content, ref offset);
            if (resource.Key != "StringFileInfo" && resource.Key != "\u0001StringFileInfo")
                return null;

            StringFileInfo sfi = new StringFileInfo(resource);
            sfi.Children = StringTable.Deserialize(content, ref offset);

            return sfi;
        }
    }
}