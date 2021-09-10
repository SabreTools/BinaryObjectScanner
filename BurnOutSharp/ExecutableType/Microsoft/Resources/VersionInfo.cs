using System.IO;

namespace BurnOutSharp.ExecutableType.Microsoft.Resources
{
    public class VersionInfo : Resource
    {
        /// <summary>
        /// Arbitrary data associated with this VS_VERSIONINFO structure.
        /// The wValueLength member specifies the length of this member;
        /// if wValueLength is zero, this member does not exist.
        /// </summary>
        public FixedFileInfo Value;

        /// <summary>
        /// An array of zero or one StringFileInfo structures, and zero or one VarFileInfo structures
        /// that are children of the current VS_VERSIONINFO structure.
        /// </summary>
        public StringFileInfo ChildrenStringFileInfo;

        /// <summary>
        /// An array of zero or one StringFileInfo structures, and zero or one VarFileInfo structures
        /// that are children of the current VS_VERSIONINFO structure.
        /// </summary>
        public VarFileInfo ChildrenVarFileInfo;

        public static new VersionInfo Deserialize(Stream stream)
        {
            long originalPosition = stream.Position;

            VersionInfo vi = new VersionInfo();
            Resource resource = Resource.Deserialize(stream);
            if (resource.Key != "VS_VERSION_INFO")
                return null;
            
            vi.Length = resource.Length;
            vi.ValueLength = resource.ValueLength;
            vi.Type = resource.Type;
            vi.Key = resource.Key;

            if (vi.ValueLength > 0)
                vi.Value = FixedFileInfo.Deserialize(stream);

            if (stream.Position - originalPosition > vi.Length)
                return vi;

            long preChildOffset = stream.Position;
            Resource firstChild = Resource.Deserialize(stream);
            if (firstChild.Key == "StringFileInfo")
            {
                stream.Seek(preChildOffset, SeekOrigin.Begin);
                vi.ChildrenStringFileInfo = StringFileInfo.Deserialize(stream);
            }
            else if (firstChild.Key == "VarFileInfo")
            {
                stream.Seek(preChildOffset, SeekOrigin.Begin);
                vi.ChildrenVarFileInfo = VarFileInfo.Deserialize(stream);
            }

            if (stream.Position - originalPosition > vi.Length)
                return vi;

            preChildOffset = stream.Position;
            Resource secondChild = Resource.Deserialize(stream);
            if (secondChild.Key == "StringFileInfo")
            {
                stream.Seek(preChildOffset, SeekOrigin.Begin);
                vi.ChildrenStringFileInfo = StringFileInfo.Deserialize(stream);
            }
            else if (secondChild.Key == "VarFileInfo")
            {
                stream.Seek(preChildOffset, SeekOrigin.Begin);
                vi.ChildrenVarFileInfo = VarFileInfo.Deserialize(stream);
            }

            return vi;
        }

        public static new VersionInfo Deserialize(byte[] content, ref int offset)
        {
            int originalOffset = offset;

            VersionInfo vi = new VersionInfo();
            Resource resource = Resource.Deserialize(content, ref offset);
            if (resource.Key != "VS_VERSION_INFO")
                return null;
            
            vi.Length = resource.Length;
            vi.ValueLength = resource.ValueLength;
            vi.Type = resource.Type;
            vi.Key = resource.Key;
            
            if (vi.ValueLength > 0)
                vi.Value = FixedFileInfo.Deserialize(content, ref offset);

            if (offset - originalOffset > vi.Length)
                return vi;

            int preChildOffset = offset;
            Resource firstChild = Resource.Deserialize(content, ref offset);
            if (firstChild.Key == "StringFileInfo")
            {
                offset = preChildOffset;
                vi.ChildrenStringFileInfo = StringFileInfo.Deserialize(content, ref offset);
            }
            else if (firstChild.Key == "VarFileInfo")
            {
                offset = preChildOffset;
                vi.ChildrenVarFileInfo = VarFileInfo.Deserialize(content, ref offset);
            }

            if (offset - originalOffset > vi.Length)
                return vi;


            preChildOffset = offset;
            Resource secondChild = Resource.Deserialize(content, ref offset);
            if (secondChild.Key == "StringFileInfo")
            {
                offset = preChildOffset;
                vi.ChildrenStringFileInfo = StringFileInfo.Deserialize(content, ref offset);
            }
            else if (secondChild.Key == "VarFileInfo")
            {
                offset = preChildOffset;
                vi.ChildrenVarFileInfo = VarFileInfo.Deserialize(content, ref offset);
            }

            return vi;
        }
    }
}