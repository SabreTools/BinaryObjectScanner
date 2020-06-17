using System.IO;

namespace BurnOutSharp.ProtectionType
{
    public class JoWooDXProt
    {
        public static string CheckContents(string file, string fileContent)
        {
            string check = ".ext    ";
            if (fileContent.Contains(check))
            {
                string check2 = "kernel32.dll" + (char)0x00 + (char)0x00 + (char)0x00 + "VirtualProtect";
                int position = fileContent.IndexOf(check2);
                if (position > -1)
                    return $"JoWooD X-Prot {GetVersion(file, --position)} (Index {fileContent.IndexOf(check)})";
                else
                    return $"JoWooD X-Prot v1 (Index {fileContent.IndexOf(check)})";
            }

            check = "@HC09    ";
            if (fileContent.Contains(check))
                return $"JoWooD X-Prot v2 (Index {fileContent.IndexOf(check)})";

            return null;
        }

        private static string GetVersion(string file, int position)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                char[] version = new char[5];
                br.BaseStream.Seek(position + 67, SeekOrigin.Begin);
                version[0] = br.ReadChar();
                br.ReadByte();
                version[1] = br.ReadChar();
                br.ReadByte();
                version[2] = br.ReadChar();
                br.ReadByte();
                version[3] = br.ReadChar();
                version[4] = br.ReadChar();

                return version[0] + "." + version[1] + "." + version[2] + "." + version[3] + version[4];
            }
        }
    }
}
