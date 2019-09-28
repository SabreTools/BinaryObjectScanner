using System.IO;

namespace BurnOutSharp.ProtectionType
{
    public class DVDCops
    {
        public static string CheckContents(string file, string fileContent)
        {
            int position;
            if ((position = fileContent.IndexOf("DVD-Cops,  ver. ")) > -1)
                return "DVD-Cops " + GetVersion(file, position);

            return null;
        }

        private static string GetVersion(string file, int position)
        {
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
