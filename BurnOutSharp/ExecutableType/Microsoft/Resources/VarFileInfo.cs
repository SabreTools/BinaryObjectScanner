using System.IO;

namespace BurnOutSharp.ExecutableType.Microsoft.Resources
{
    public class VarFileInfo : Resource
    {
        /// <summary>
        /// Typically contains a list of languages that the application or DLL supports.
        /// </summary>
        public Var Children;

        public VarFileInfo(Resource resource)
        {
            this.Length = resource?.Length ?? default;
            this.ValueLength = resource?.ValueLength ?? default;
            this.Type = resource?.Type ?? default;
            this.Key = resource?.Key ?? default;
        }

        public static new VarFileInfo Deserialize(Stream stream)
        {
            Resource resource = Resource.Deserialize(stream);
            if (resource.Key != "VarFileInfo")
                return null;

            VarFileInfo vfi = new VarFileInfo(resource);
            vfi.Children = Var.Deserialize(stream);

            return vfi;
        }

        public static new VarFileInfo Deserialize(byte[] content, ref int offset)
        {
            Resource resource = Resource.Deserialize(content, ref offset);
            if (resource.Key != "VarFileInfo")
                return null;

            VarFileInfo vfi = new VarFileInfo(resource);
            vfi.Children = Var.Deserialize(content, ref offset);

            return vfi;
        }
    }
}