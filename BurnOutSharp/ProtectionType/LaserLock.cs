using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class LaserLock
    {
        public static string CheckContents(string file, string fileContent)
        {
            string check = "Packed by SPEEnc V2 Asterios Parlamentas.PE";
            string check2 = "GetModuleHandleA" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "GetProcAddress" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "LoadLibraryA" + (char)0x00 + (char)0x00 + "KERNEL32.dll" + (char)0x00 + "ëy" + (char)0x01 + "SNIF";
            int position = fileContent.IndexOf(check2);
            if (fileContent.Contains(check))
            {
                if (position > -1)
                    return $"LaserLock {GetVersion(fileContent, position)} {GetBuild(fileContent, true)} (Index {fileContent.IndexOf(check)}, {position})";
                else
                    return $"LaserLock Marathon {GetBuild(fileContent, false)} (Index {fileContent.IndexOf(check)})";
            }
            else if (position > -1)
            {
                return $"LaserLock {GetVersion(fileContent, --position)} {GetBuild(fileContent, false)} (Index {position})";
            }

            check = "NOMOUSE.SP";
            if (file != null && string.Equals(Path.GetFileName(file), check, StringComparison.OrdinalIgnoreCase))
                return $"LaserLock {GetVersion16Bit(file)}";

            check = ":\\LASERLOK\\LASERLOK.IN" + (char)0x00 + "C:\\NOMOUSE.SP";
            if (fileContent.Contains(check))
                return $"LaserLock 3 (Index {fileContent.IndexOf(check)})";

            check = "LASERLOK_INIT" + (char)0xC + "LASERLOK_RUN" + (char)0xE + "LASERLOK_CHECK" + (char)0xF + "LASERLOK_CHECK2" + (char)0xF + "LASERLOK_CHECK3";
            if (fileContent.Contains(check))
                return $"LaserLock 5 (Index {fileContent.IndexOf(check)})";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (Directory.Exists(Path.Combine(path, "LASERLOK")))
                {
                    return "LaserLock";
                }

                // TODO: Verify if these are OR or AND
                else if (files.Any(f => Path.GetFileName(f).Equals("NOMOUSE.SP", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("NOMOUSE.COM", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("l16dll.dll", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("laserlok.in", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("laserlok.o10", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("laserlok.011", StringComparison.OrdinalIgnoreCase)))
                {
                    return "LaserLock";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("NOMOUSE.SP", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("NOMOUSE.COM", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("l16dll.dll", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("laserlok.in", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("laserlok.o10", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("laserlok.011", StringComparison.OrdinalIgnoreCase))
                {
                    return "LaserLock";
                }
            }

            return null;
        }

        private static string GetBuild(string fileContent, bool versionTwo)
        {
            // TODO: Is this supposed to be "Unknown"?
            int position = fileContent.IndexOf("Unkown" + (char)0 + "Unkown");
            string year, month, day;
            if (versionTwo)
            {
                day = fileContent.Substring(position + 14, 2);
                month = fileContent.Substring(position + 14 + 3, 2);
                year = "20" + fileContent.Substring(position + 14 + 6, 2);
            }
            else
            {
                day = fileContent.Substring(position + 13, 2);
                month = fileContent.Substring(position + 13 + 3, 2);
                year = "20" + fileContent.Substring(position + 13 + 6, 2);
            }

            return "(Build " + year + "-" + month + "-" + day + ")";
        }

        private static string GetVersion(string FileContent, int position)
        {
            return FileContent.Substring(position + 76, 4);
        }

        private static string GetVersion16Bit(string file)
        {
            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                char[] version = new char[3];
                br.BaseStream.Seek(71, SeekOrigin.Begin);
                version[0] = br.ReadChar();
                br.ReadByte();
                version[1] = br.ReadChar();
                version[2] = br.ReadChar();

                if (char.IsNumber(version[0]) && char.IsNumber(version[1]) && char.IsNumber(version[2]))
                    return version[0] + "." + version[1] + version[2];

                return "";
            }
        }
    }
}
