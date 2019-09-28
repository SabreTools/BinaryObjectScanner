using System.IO;

namespace BurnOutSharp.ProtectionType
{
    public class JoWooDXProt
    {
        public static string CheckContents(string file, string fileContent)
        {
            int position;
            if (fileContent.Contains(".ext    "))
            {
                if ((position = fileContent.IndexOf("kernel32.dll" + (char)0x00 + (char)0x00 + (char)0x00 + "VirtualProtect")) > -1)
                {
                    return "JoWooD X-Prot " + GetVersion(file, --position);
                }
                else
                {
                    return "JoWooD X-Prot v1";
                }
            }

            if (fileContent.Contains("@HC09    "))
                return "JoWooD X-Prot v2";

            return null;
        }

        private static string GetVersion(string file, int position)
        {
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
