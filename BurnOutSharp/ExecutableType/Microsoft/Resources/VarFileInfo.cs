using System.IO;

namespace BurnOutSharp.ExecutableType.Microsoft.Resources
{
    public class VarFileInfo : Resource
    {
        /// <summary>
        /// Typically contains a list of languages that the application or DLL supports.
        /// </summary>
        public Var Children;

        public static new VarFileInfo Deserialize(Stream stream)
        {
            VarFileInfo vfi = new VarFileInfo();

            Resource resource = Resource.Deserialize(stream);
            if (resource.Key != "VarFileInfo")
                return null;

            vfi.Length = resource.Length;
            vfi.ValueLength = resource.ValueLength;
            vfi.Type = resource.Type;
            vfi.Key = resource.Key;
            vfi.Children = Var.Deserialize(stream);

            return vfi;
        }

        public static new VarFileInfo Deserialize(byte[] content, ref int offset)
        {
            VarFileInfo vfi = new VarFileInfo();

            Resource resource = Resource.Deserialize(content, ref offset);
            if (resource.Key != "VarFileInfo")
                return null;

            vfi.Length = resource.Length;
            vfi.ValueLength = resource.ValueLength;
            vfi.Type = resource.Type;
            vfi.Key = resource.Key;
            vfi.Children = Var.Deserialize(content, ref offset);

            return vfi;
        }
    }
}