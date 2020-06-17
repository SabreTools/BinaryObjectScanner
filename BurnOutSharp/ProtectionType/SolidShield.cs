using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SolidShield
    {
        public static string CheckContents(string file, string fileContent)
        {
            string check = "D" + (char)0x00 + "V" + (char)0x00 + "M" + (char)0x00 + " " + (char)0x00 + "L" + (char)0x00 + "i" + (char)0x00 + "b" + (char)0x00 + "r" + (char)0x00 + "a" + (char)0x00 + "r" + (char)0x00 + "y";
            if (fileContent.Contains(check))
                return $"SolidShield {Utilities.GetFileVersion(file)} (Index {fileContent.IndexOf(check)})";

            check = "S" + (char)0x00 + "o" + (char)0x00 + "l" + (char)0x00 + "i" + (char)0x00 + "d" + (char)0x00 + "s" + (char)0x00 + "h" + (char)0x00 + "i" + (char)0x00 + "e" + (char)0x00 + "l" + (char)0x00 + "d" + (char)0x00 + " " + (char)0x00 + "L" + (char)0x00 + "i" + (char)0x00 + "b" + (char)0x00 + "r" + (char)0x00 + "a" + (char)0x00 + "r" + (char)0x00 + "y";
            if (fileContent.Contains(check))
            {
                string companyName = string.Empty;
                if (file != null)
                    companyName = FileVersionInfo.GetVersionInfo(file).CompanyName.ToLower();

                if (companyName.Contains("solidshield") || companyName.Contains("tages"))
                    return $"SolidShield Core.dll {Utilities.GetFileVersion(file)} (Index {fileContent.IndexOf(check)})";
            }

            check = "S" + (char)0x00 + "o" + (char)0x00 + "l" + (char)0x00 + "i" + (char)0x00 + "d" + (char)0x00 + "s" + (char)0x00 + "h" + (char)0x00 + "i" + (char)0x00 + "e" + (char)0x00 + "l" + (char)0x00 + "d" + (char)0x00 + " " + (char)0x00 + "A" + (char)0x00 + "c" + (char)0x00 + "t" + (char)0x00 + "i" + (char)0x00 + "v" + (char)0x00 + "a" + (char)0x00 + "t" + (char)0x00 + "i" + (char)0x00 + "o" + (char)0x00 + "n" + (char)0x00 + " " + (char)0x00 + "L" + (char)0x00 + "i" + (char)0x00 + "b" + (char)0x00 + "r" + (char)0x00 + "a" + (char)0x00 + "r" + (char)0x00 + "y";
            if (fileContent.Contains(check))
            {
                string companyName = string.Empty;
                if (file != null)
                    companyName = FileVersionInfo.GetVersionInfo(file).CompanyName.ToLower();

                if (companyName.Contains("solidshield") || companyName.Contains("tages"))
                    return $"SolidShield Core.dll {Utilities.GetFileVersion(file)} (Index {fileContent.IndexOf(check)})";
            }

            check = "" + (char)0xEF + (char)0xBE + (char)0xAD + (char)0xDE;
            int position = fileContent.IndexOf(check);
            if (position > -1)
            {
                var id1 = fileContent.Substring(position + 5, 3);
                var id2 = fileContent.Substring(position + 16, 4);

                if (id1 == "" + (char)0x00 + (char)0x00 + (char)0x00 && id2 == "" + (char)0x00 + (char)0x10 + (char)0x00 + (char)0x00)
                    return $"SolidShield 1 (SolidShield EXE Wrapper) (Index {position})";
                else if (id1 == ".o&" && id2 == "ÛÅ›¹")
                    return $"SolidShield 2 (SolidShield v2 EXE Wrapper) (Index {position})"; // TODO: Verify against other SolidShield 2 discs
            }

            check = "A" + (char)0x00 + "c" + (char)0x00 + "t" + (char)0x00 + "i" + (char)0x00 + "v" + (char)0x00 + "a" + (char)0x00 + "t" + (char)0x00 + "i" + (char)0x00 + "o" + (char)0x00 + "n" + (char)0x00 + " " + (char)0x00 + "M" + (char)0x00 + "a" + (char)0x00 + "n" + (char)0x00 + "a" + (char)0x00 + "g" + (char)0x00 + "e" + (char)0x00 + "r";
            if (fileContent.Contains(check))
            {
                string companyName = string.Empty;
                if (file != null)
                    companyName = FileVersionInfo.GetVersionInfo(file).CompanyName.ToLower();

                if (companyName.Contains("solidshield") || companyName.Contains("tages"))
                    return $"SolidShield Activation Manager Module {Utilities.GetFileVersion(file)} (Index {fileContent.IndexOf(check)})";
            }

            check = "" + (char)0xAD + (char)0xDE + (char)0xFE + (char)0xCA;
            position = fileContent.IndexOf(check);
            if (position > -1)
            {
                if ((fileContent[position + 3] == (char)0x04 || fileContent[position + 3] == (char)0x05)
                    && fileContent.Substring(position + 4, 3) == "" + (char)0x00 + (char)0x00 + (char)0x00
                    && fileContent.Substring(position + 15, 4) == "" + (char)0x00 + (char)0x10 + (char)0x00 + (char)0x00)
                {
                    return $"SolidShield 2 (SolidShield v2 EXE Wrapper) (Index {position})";
                }
                else if (fileContent.Substring(position + 4, 3) == "" + (char)0x00 + (char)0x00 + (char)0x00
                    && fileContent.Substring(position + 15, 4) == "" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00)
                {
                    string check2 = "T" + (char)0x00 + "a" + (char)0x00 + "g" + (char)0x00 + "e" + (char)0x00 + "s" + (char)0x00 + "S" + (char)0x00 + "e" + (char)0x00 + "t" + (char)0x00 + "u" + (char)0x00 + "p" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "0" + (char)0x00 + (char)0x8 + (char)0x00 + (char)0x1 + (char)0x0 + "F" + (char)0x00 + "i" + (char)0x00 + "l" + (char)0x00 + "e" + (char)0x00 + "V" + (char)0x00 + "e" + (char)0x00 + "r" + (char)0x00 + "s" + (char)0x00 + "i" + (char)0x00 + "o" + (char)0x00 + "n" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00;
                    position = fileContent.IndexOf(check2);
                    if (position > -1)
                    {
                        position--; // TODO: Verify this subtract
                        return $"SolidShield 2 + Tagès {fileContent.Substring(position + 0x38, 1)}.{fileContent.Substring(position + 0x38 + 4, 1)}.{fileContent.Substring(position + 0x38 + 8, 1)}.{fileContent.Substring(position + 0x38 + 12, 1)} (Index { fileContent.IndexOf(check)}, {position})";
                    }
                    else
                    {
                        return $"SolidShield 2 (SolidShield v2 EXE Wrapper) (Index {fileContent.IndexOf(check)})";
                    }
                }
            }

            check = "Solidshield";
            position = fileContent.IndexOf(check);
            if (position > -1)
                return $"SolidShield {GetVersion(file, position)} (Index {position})";

            check = "B" + (char)0x00 + "I" + (char)0x00 + "N" + (char)0x00 + (char)0x7 + (char)0x00 + "I" + (char)0x00 + "D" + (char)0x00 + "R" + (char)0x00 + "_" + (char)0x00 + "S" + (char)0x00 + "G" + (char)0x00 + "T" + (char)0x0;
            if (fileContent.Contains(check))
                return $"SolidShield (Index {position})";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetFileName(f).Equals("dvm.dll", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("hc.dll", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("solidshield-cd.dll", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("c11prot.dll", StringComparison.OrdinalIgnoreCase)))
                {
                    return "SolidShield";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("dvm.dll", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("hc.dll", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("solidshield-cd.dll", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("c11prot.dll", StringComparison.OrdinalIgnoreCase))
                {
                    return "SolidShield";
                }
            }

            return null;
        }

        private static string GetVersion(string file, int position)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                br.BaseStream.Seek(position + 12, SeekOrigin.Begin); // Begin reading after "Solidshield"
                char version = br.ReadChar();
                br.ReadByte();
                char subVersion = br.ReadChar();
                br.ReadByte();
                char subsubVersion = br.ReadChar();
                br.ReadByte();
                char subsubsubVersion = br.ReadChar();
                return version + "." + subVersion + "." + subsubVersion + "." + subsubsubVersion;
            }
        }
    }
}
