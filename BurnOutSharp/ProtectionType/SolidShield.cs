using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SolidShield
    {
        public static string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "D" + (char)0x00 + "V" + (char)0x00 + "M" + (char)0x00 + " " + (char)0x00 + "L" + (char)0x00 + "i" + (char)0x00 + "b" + (char)0x00 + "r" + (char)0x00 + "a" + (char)0x00 + "r" + (char)0x00 + "y"
            byte[] check = new byte[] { 0x44, 0x00, 0x56, 0x00, 0x4D, 0x00, 0x20, 0x00, 0x4C, 0x00, 0x69, 0x00, 0x62, 0x00, 0x72, 0x00, 0x61, 0x00, 0x72, 0x00, 0x79 };
            if (fileContent.Contains(check, out int position))
                return $"SolidShield {Utilities.GetFileVersion(file)}" + (includePosition ? $" (Index {position})" : string.Empty);

            // "S" + (char)0x00 + "o" + (char)0x00 + "l" + (char)0x00 + "i" + (char)0x00 + "d" + (char)0x00 + "s" + (char)0x00 + "h" + (char)0x00 + "i" + (char)0x00 + "e" + (char)0x00 + "l" + (char)0x00 + "d" + (char)0x00 + " " + (char)0x00 + "L" + (char)0x00 + "i" + (char)0x00 + "b" + (char)0x00 + "r" + (char)0x00 + "a" + (char)0x00 + "r" + (char)0x00 + "y"
            check = new byte[] { 0x53, 0x00, 0x6F, 0x00, 0x6C, 0x00, 0x69, 0x00, 0x64, 0x00, 0x73, 0x00, 0x68, 0x00, 0x69, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x64, 0x00, 0x20, 0x00, 0x4C, 0x00, 0x69, 0x00, 0x62, 0x00, 0x72, 0x00, 0x61, 0x00, 0x72, 0x00, 0x79 };
            if (fileContent.Contains(check, out position))
            {
                string companyName = string.Empty;
                if (file != null)
                    companyName = FileVersionInfo.GetVersionInfo(file).CompanyName.ToLower();

                if (companyName.Contains("solidshield") || companyName.Contains("tages"))
                    return $"SolidShield Core.dll {Utilities.GetFileVersion(file)}" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            // "S" + (char)0x00 + "o" + (char)0x00 + "l" + (char)0x00 + "i" + (char)0x00 + "d" + (char)0x00 + "s" + (char)0x00 + "h" + (char)0x00 + "i" + (char)0x00 + "e" + (char)0x00 + "l" + (char)0x00 + "d" + (char)0x00 + " " + (char)0x00 + "A" + (char)0x00 + "c" + (char)0x00 + "t" + (char)0x00 + "i" + (char)0x00 + "v" + (char)0x00 + "a" + (char)0x00 + "t" + (char)0x00 + "i" + (char)0x00 + "o" + (char)0x00 + "n" + (char)0x00 + " " + (char)0x00 + "L" + (char)0x00 + "i" + (char)0x00 + "b" + (char)0x00 + "r" + (char)0x00 + "a" + (char)0x00 + "r" + (char)0x00 + "y"
            check = new byte[] { 0x53, 0x00, 0x6F, 0x00, 0x6C, 0x00, 0x69, 0x00, 0x64, 0x00, 0x73, 0x00, 0x68, 0x00, 0x69, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x64, 0x00, 0x20, 0x00, 0x41, 0x00, 0x63, 0x00, 0x74, 0x00, 0x69, 0x00, 0x76, 0x00, 0x61, 0x00, 0x74, 0x00, 0x69, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x20, 0x00, 0x4C, 0x00, 0x69, 0x00, 0x62, 0x00, 0x72, 0x00, 0x61, 0x00, 0x72, 0x00, 0x79 };
            if (fileContent.Contains(check, out position))
            {
                string companyName = string.Empty;
                if (file != null)
                    companyName = FileVersionInfo.GetVersionInfo(file).CompanyName.ToLower();

                if (companyName.Contains("solidshield") || companyName.Contains("tages"))
                    return $"SolidShield Core.dll {Utilities.GetFileVersion(file)}" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            // (char)0xEF + (char)0xBE + (char)0xAD + (char)0xDE
            check = new byte[] { };
            if (fileContent.Contains(check, out position))
            {
                var id1 = new ArraySegment<byte>(fileContent, position + 5, 3);
                var id2 = new ArraySegment<byte>(fileContent, position + 16, 4);

                if (id1.SequenceEqual(new byte[] { 0x00, 0x00, 0x00 }) && id2.SequenceEqual(new byte[] { 0x00, 0x10, 0x00, 0x00 }))
                    return "SolidShield 1 (SolidShield EXE Wrapper)" + (includePosition ? $" (Index {position})" : string.Empty);
                else if (id1.SequenceEqual(new byte[] { 0x2E, 0x6F, 0x26 }) && id2.SequenceEqual(new byte[] { 0xDB, 0xC5, 0x20, 0x3A, 0xB9 }))
                    return "SolidShield 2 (SolidShield v2 EXE Wrapper)" + (includePosition ? $" (Index {position})" : string.Empty); // TODO: Verify against other SolidShield 2 discs
            }

            // "A" + (char)0x00 + "c" + (char)0x00 + "t" + (char)0x00 + "i" + (char)0x00 + "v" + (char)0x00 + "a" + (char)0x00 + "t" + (char)0x00 + "i" + (char)0x00 + "o" + (char)0x00 + "n" + (char)0x00 + " " + (char)0x00 + "M" + (char)0x00 + "a" + (char)0x00 + "n" + (char)0x00 + "a" + (char)0x00 + "g" + (char)0x00 + "e" + (char)0x00 + "r"
            check = new byte[] { 0x41, 0x00, 0x63, 0x00, 0x74, 0x00, 0x69, 0x00, 0x76, 0x00, 0x61, 0x00, 0x74, 0x00, 0x69, 0x00, 0x6f, 0x00, 0x6e, 0x00, 0x20, 0x00, 0x4d, 0x00, 0x61, 0x00, 0x6e, 0x00, 0x61, 0x00, 0x67, 0x00, 0x65, 0x00, 0x72 };
            if (fileContent.Contains(check, out position))
            {
                string companyName = string.Empty;
                if (file != null)
                    companyName = FileVersionInfo.GetVersionInfo(file).CompanyName.ToLower();

                if (companyName.Contains("solidshield") || companyName.Contains("tages"))
                    return $"SolidShield Activation Manager Module {Utilities.GetFileVersion(file)}" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            // (char)0xAD + (char)0xDE + (char)0xFE + (char)0xCA
            check = new byte[] { 0xAD, 0xDE, 0xFE, 0xCA };
            if (fileContent.Contains(check, out position))
            {
                var id1 = new ArraySegment<byte>(fileContent, position + 4, 3);
                var id2 = new ArraySegment<byte>(fileContent, position + 15, 4);

                if ((fileContent[position + 3] == 0x04 || fileContent[position + 3] == 0x05)
                    && id1.SequenceEqual(new byte[] { 0x00, 0x00, 0x00 })
                    && id2.SequenceEqual(new byte[] { 0x00, 0x10, 0x00, 0x00 }))
                {
                    return "SolidShield 2 (SolidShield v2 EXE Wrapper)" + (includePosition ? $" (Index {position})" : string.Empty);
                }
                else if (id1.SequenceEqual(new byte[] { 0x00, 0x00, 0x00 })
                    && id2.SequenceEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 }))
                {
                    // "T" + (char)0x00 + "a" + (char)0x00 + "g" + (char)0x00 + "e" + (char)0x00 + "s" + (char)0x00 + "S" + (char)0x00 + "e" + (char)0x00 + "t" + (char)0x00 + "u" + (char)0x00 + "p" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "0" + (char)0x00 + (char)0x8 + (char)0x00 + (char)0x1 + (char)0x0 + "F" + (char)0x00 + "i" + (char)0x00 + "l" + (char)0x00 + "e" + (char)0x00 + "V" + (char)0x00 + "e" + (char)0x00 + "r" + (char)0x00 + "s" + (char)0x00 + "i" + (char)0x00 + "o" + (char)0x00 + "n" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00
                    byte[] check2 = new byte[] { 0x54, 0x61, 0x67, 0x65, 0x73, 0x53, 0x65, 0x74, 0x75, 0x70, 0x30, 0x08, 0x01, 0x00, 0x46, 0x69, 0x6C, 0x65, 0x56, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x00, 0x00, 0x00, 0x00 };
                    if (fileContent.Contains(check2, out int position2))
                    {
                        position2--; // TODO: Verify this subtract
                        return $"SolidShield 2 + Tagès {fileContent[position2 + 0x38]}.{fileContent[position2 + 0x38 + 4]}.{fileContent[position2 + 0x38 + 8]}.{fileContent[position + 0x38 + 12]}" + (includePosition ? $" (Index {position}, {position2})" : string.Empty);
                    }
                    else
                    {
                        return "SolidShield 2 (SolidShield v2 EXE Wrapper)" + (includePosition ? $" (Index {position})" : string.Empty);
                    }
                }
            }

            // "Solidshield"
            check = new byte[] { 0x53, 0x6F, 0x6C, 0x69, 0x64, 0x73, 0x68, 0x69, 0x65, 0x6C, 0x64 };
            if (fileContent.Contains(check, out position))
                return $"SolidShield {GetVersion(fileContent, position)}" + (includePosition ? $" (Index {position})" : string.Empty);

            // "B" + (char)0x00 + "I" + (char)0x00 + "N" + (char)0x00 + (char)0x7 + (char)0x00 + "I" + (char)0x00 + "D" + (char)0x00 + "R" + (char)0x00 + "_" + (char)0x00 + "S" + (char)0x00 + "G" + (char)0x00 + "T" + (char)0x0
            check = new byte[] { 0x42, 0x00, 0x49, 0x00, 0x4E, 0x00, 0x07, 0x00, 0x49, 0x00, 0x44, 0x00, 0x52, 0x00, 0x5F, 0x00, 0x53, 0x00, 0x47, 0x00, 0x54, 0x00 };
            if (fileContent.Contains(check, out position))
                return "SolidShield" + (includePosition ? $" (Index {position})" : string.Empty);

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

        private static string GetVersion(byte[] fileContent, int position)
        {
            int index = position + 12; // Begin reading after "Solidshield"
            char version = (char)fileContent[index];
            index++;
            index++;
            char subVersion = (char)fileContent[index];
            index++;
            index++;
            char subSubVersion = (char)fileContent[index];
            index++;
            index++;
            char subSubSubVersion = (char)fileContent[index];
            
            return $"{version}.{subVersion}.{subSubVersion}.{subSubSubVersion}";
        }
    }
}
