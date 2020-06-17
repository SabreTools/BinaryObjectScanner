using System.IO;

namespace BurnOutSharp.ProtectionType
{
    public class JoWooDXProt
    {
        public static string CheckContents(string file, byte[] fileContent)
        {
            // ".ext    "
            byte[] check = new byte[] { 0x2E, 0x65, 0x78, 0x74, 0x20, 0x20, 0x20, 0x20 };
            if (fileContent.Contains(check, out int position))
            {
                // "kernel32.dll" + (char)0x00 + (char)0x00 + (char)0x00 + "VirtualProtect"
                byte[] check2 = new byte[] { 0x6B, 0x65, 0x72, 0x6E, 0x65, 0x6C, 0x33, 0x32, 0x2E, 0x64, 0x6C, 0x6C, 0x00, 0x00, 0x00, 0x56, 0x69, 0x72, 0x74, 0x75, 0x61, 0x6C, 0x50, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74 };
                if (fileContent.Contains(check2, out int position2))
                    return $"JoWooD X-Prot {GetVersion(file, --position2)} (Index {position}, {position2})";
                else
                    return $"JoWooD X-Prot v1 (Index {position})";
            }

            // "@HC09    "
            check = new byte[] { 0x40, 0x48, 0x43, 0x30, 0x39, 0x20, 0x20, 0x20, 0x20 };
            if (fileContent.Contains(check, out position))
                return $"JoWooD X-Prot v2 (Index {position})";

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
