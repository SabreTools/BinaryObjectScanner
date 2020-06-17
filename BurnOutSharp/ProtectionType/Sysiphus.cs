using System.IO;

namespace BurnOutSharp.ProtectionType
{
    public class Sysiphus
    {
        public static string CheckContents(string file, byte[] fileContent)
        {
            // "V SUHPISYSDVD"
            byte[] check = new byte[] { 0x56, 0x20, 0x53, 0x55, 0x48, 0x50, 0x49, 0x53, 0x59, 0x53, 0x44, 0x56, 0x44 };
            if (fileContent.Contains(check, out int position))
                return $"Sysiphus DVD {GetVersion(file, position)} (Index {position})";

            // "V SUHPISYS"
            check = new byte[] { 0x56, 0x20, 0x53, 0x55, 0x48, 0x50, 0x49, 0x53, 0x59, 0x53 };
            if (fileContent.Contains(check, out position))
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
