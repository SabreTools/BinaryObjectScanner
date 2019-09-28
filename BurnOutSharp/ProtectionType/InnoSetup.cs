using System.IO;

namespace BurnOutSharp.ProtectionType
{
    public class InnoSetup
    {
        public static string CheckContents(string file, string fileContent)
        {
            if (fileContent.IndexOf("Inno") == 0x30)
            {
                // TOOO: Add Inno Setup extraction
                return "Inno Setup " + GetVersion(file);
            }

            return null;
        }

        private static string GetVersion(string file)
        {
            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                br.BaseStream.Seek(0x30, SeekOrigin.Begin);
                string signature = new string(br.ReadChars(12));

                if (signature == "rDlPtS02" + (char)0x87 + "eVx")
                    return "1.2.10";
                else if (signature == "rDlPtS04" + (char)0x87 + "eVx")
                    return "4.0.0";
                else if (signature == "rDlPtS05" + (char)0x87 + "eVx")
                    return "4.0.3";
                else if (signature == "rDlPtS06" + (char)0x87 + "eVx")
                    return "4.0.10";
                else if (signature == "rDlPtS07" + (char)0x87 + "eVx")
                    return "4.1.6";
                else if (signature == "rDlPtS" + (char)0xcd + (char)0xe6 + (char)0xd7 + "{" + (char)0x0b + "*")
                    return "5.1.5";
                else if (signature == "nS5W7dT" + (char)0x83 + (char)0xaa + (char)0x1b + (char)0x0f + "j")
                    return "5.1.5";

                return string.Empty;
            }
        }
    }
}
