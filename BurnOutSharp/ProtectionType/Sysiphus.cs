using System.IO;

namespace BurnOutSharp.ProtectionType
{
    public class Sysiphus
    {
        public static string CheckContents(string file, string fileContent)
        {
            string check = "V SUHPISYSDVD";
            int position = fileContent.IndexOf(check);
            if (position > -1)
                return $"Sysiphus DVD {GetVersion(file, position)} (Index {position})";

            check = "V SUHPISYS";
            position = fileContent.IndexOf(check);
            if (position > -1)
                return $"Sysiphus {GetVersion(file, position)} (Index {position})";

            return null;
        }

        private static string GetVersion(string file, int position)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

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
