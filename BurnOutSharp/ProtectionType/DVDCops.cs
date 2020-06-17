using System.IO;

namespace BurnOutSharp.ProtectionType
{
    public class DVDCops
    {
        public static string CheckContents(string file, byte[] fileContent)
        {
            // "DVD-Cops,  ver. "
            byte[] check = new byte[] { 0x44, 0x56, 0x44, 0x2D, 0x43, 0x6F, 0x70, 0x73, 0x2C, 0x20, 0x20, 0x76, 0x65, 0x72, 0x2E, 0x20 };
            if (fileContent.Contains(check, out int position))
                return $"DVD-Cops {GetVersion(file, position)} (Index {position})";

            return null;
        }

        private static string GetVersion(string file, int position)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                br.BaseStream.Seek(position + 15, SeekOrigin.Begin); // Begin reading after "CD-Cops,  ver."
                char[] version = br.ReadChars(4);
                if (version[0] == 0x00)
                    return "";

                return new string(version);
            }
        }
    }
}
