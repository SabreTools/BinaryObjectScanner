using System.IO;

namespace BurnOutSharp.ProtectionType
{
    public class Sysiphus
    {
        public static string CheckContents(string file, string fileContent)
        {
            int position;
            if ((position = fileContent.IndexOf("V SUHPISYSDVD")) > -1)
                return "Sysiphus DVD " + GetVersion(file, position);

            if ((position = fileContent.IndexOf("V SUHPISYS")) > -1)
                return "Sysiphus " + GetVersion(file, position);

            return null;
        }

        private static string GetVersion(string file, int position)
        {
            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                br.BaseStream.Seek(position - 3, SeekOrigin.Begin);
                char subVersion = br.ReadChar();
                br.ReadChar();
                char version = br.ReadChar();

                if (char.IsNumber(version) && char.IsNumber(subVersion))
                    return version + "." + subVersion;
                else
                    return "";
            }
        }
    }
}
