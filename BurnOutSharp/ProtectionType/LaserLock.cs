using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class LaserLock
    {
        public static string CheckContents(string file, byte[] fileContent)
        {
            // "Packed by SPEEnc V2 Asterios Parlamentas.PE"
            byte[] check = new byte[] { 0x50, 0x61, 0x63, 0x6B, 0x65, 0x64, 0x20, 0x62, 0x79, 0x20, 0x53, 0x50, 0x45, 0x45, 0x6E, 0x63, 0x20, 0x56, 0x32, 0x20, 0x41, 0x73, 0x74, 0x65, 0x72, 0x69, 0x6F, 0x73, 0x20, 0x50, 0x61, 0x72, 0x6C, 0x61, 0x6D, 0x65, 0x6E, 0x74, 0x61, 0x73, 0x2E, 0x50, 0x45 };
            bool containsCheck = fileContent.Contains(check, out int position);

            // "GetModuleHandleA" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "GetProcAddress" + (char)0x00 + (char)0x00 + (char)0x00 + (char)0x00 + "LoadLibraryA" + (char)0x00 + (char)0x00 + "KERNEL32.dll" + (char)0x00 + "ëy" + (char)0x01 + "SNIF"
            byte[] check2 = { 0x47, 0x65, 0x74, 0x4D, 0x6F, 0x64, 0x75, 0x6C, 0x65, 0x48, 0x61, 0x6E, 0x64, 0x6C, 0x65, 0x41, 0x00, 0x00, 0x00, 0x00, 0x47, 0x65, 0x74, 0x50, 0x72, 0x6F, 0x63, 0x41, 0x64, 0x64, 0x72, 0x65, 0x73, 0x73, 0x00, 0x00, 0x00, 0x00, 0x4C, 0x6F, 0x61, 0x64, 0x4C, 0x69, 0x62, 0x72, 0x61, 0x72, 0x79, 0x41, 0x00, 0x00, 0x4B, 0x45, 0x52, 0x4E, 0x45, 0x4C, 0x33, 0x32, 0x2E, 0x64, 0x6C, 0x6C, 0x00, 0xEB, 0x79, 0x01, 0x53, 0x4E, 0x49, 0x46 };
            bool containsCheck2 = fileContent.Contains(check2, out int position2);

            if (containsCheck && containsCheck2)
                return $"LaserLock {GetVersion(fileContent, position2)} {GetBuild(fileContent, true)} (Index {position}, {position2})";
            else if (containsCheck && !containsCheck2)
                return $"LaserLock Marathon {GetBuild(fileContent, false)} (Index {position})";
            else if (!containsCheck && containsCheck2)
                return $"LaserLock {GetVersion(fileContent, --position2)} {GetBuild(fileContent, false)} (Index {position2})";

            if (file != null && string.Equals(Path.GetFileName(file), "NOMOUSE.SP", StringComparison.OrdinalIgnoreCase))
                return $"LaserLock {GetVersion16Bit(fileContent)} (Index 71)";

            // ":\\LASERLOK\\LASERLOK.IN" + (char)0x00 + "C:\\NOMOUSE.SP"
            check = new byte[] { 0x3A, 0x5C, 0x5C, 0x4C, 0x41, 0x53, 0x45, 0x52, 0x4C, 0x4F, 0x4B, 0x5C, 0x5C, 0x4C, 0x41, 0x53, 0x45, 0x52, 0x4C, 0x4F, 0x4B, 0x2E, 0x49, 0x4E, 0x00, 0x43, 0x3A, 0x5C, 0x5C, 0x4E, 0x4F, 0x4D, 0x4F, 0x55, 0x53, 0x45, 0x2E, 0x53, 0x50 };
            if (fileContent.Contains(check, out position))
                return $"LaserLock 3 (Index {position})";

            // "LASERLOK_INIT" + (char)0xC + "LASERLOK_RUN" + (char)0xE + "LASERLOK_CHECK" + (char)0xF + "LASERLOK_CHECK2" + (char)0xF + "LASERLOK_CHECK3"
            check = new byte[] { 0x4C, 0x41, 0x53, 0x45, 0x52, 0x4C, 0x4F, 0x4B, 0x5F, 0x49, 0x4E, 0x49, 0x54, 0x0C, 0x4C, 0x41, 0x53, 0x45, 0x52, 0x4C, 0x4F, 0x4B, 0x5F, 0x52, 0x55, 0x4E, 0x0E, 0x4C, 0x41, 0x53, 0x45, 0x52, 0x4C, 0x4F, 0x4B, 0x5F, 0x43, 0x48, 0x45, 0x43, 0x4B, 0x0F, 0x4C, 0x41, 0x53, 0x45, 0x52, 0x4C, 0x4F, 0x4B, 0x5F, 0x43, 0x48, 0x45, 0x43, 0x4B, 0x32, 0x0F, 0x4C, 0x41, 0x53, 0x45, 0x52, 0x4C, 0x4F, 0x4B, 0x5F, 0x43, 0x48, 0x45, 0x43, 0x4B, 0x33 };
            if (fileContent.Contains(check, out position))
                return $"LaserLock 5 (Index {position})";

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
                if (path != null && string.Equals(Path.GetFileName(path), "NOMOUSE.SP", StringComparison.OrdinalIgnoreCase))
                    return $"LaserLock {GetVersion16Bit(path)}";

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

        private static string GetBuild(byte[] fileContent, bool versionTwo)
        {
            // "Unkown" + (char)0x00 + "Unkown" // TODO: Is this supposed to be "Unknown"?
            byte[] check = new byte[] { 0x55, 0x6E, 0x6B, 0x6F, 0x77, 0x6E, 0x00, 0x55, 0x6E, 0x6B, 0x6F, 0x77, 0x6E };
            fileContent.Contains(check, out int position);
            string year, month, day;
            if (versionTwo)
            {
                int index = position + 14;
                day = new string(fileContent.Skip(index).Take(2).Select(b => (char)b).ToArray());
                index += 3;
                month = new string(fileContent.Skip(index).Take(2).Select(b => (char)b).ToArray());
                index += 3;
                year = "20" + new string(fileContent.Skip(index).Take(2).Select(b => (char)b).ToArray());
            }
            else
            {
                int index = position + 13;
                day = new string(fileContent.Skip(index).Take(2).Select(b => (char)b).ToArray());
                index += 3;
                month = new string(fileContent.Skip(index).Take(2).Select(b => (char)b).ToArray());
                index += 3;
                year = "20" + new string(fileContent.Skip(index).Take(2).Select(b => (char)b).ToArray());
            }

            return $"(Build {year}-{month}-{day})";
        }

        private static string GetVersion(byte[] fileContent, int position)
        {
            return new string(fileContent.Skip(position + 76).Take(4).Select(b => (char)b).ToArray());
        }

        private static string GetVersion16Bit(string file)
        {
            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                return GetVersion16Bit(br.ReadBytes((int)fs.Length));
            }
        }

        private static string GetVersion16Bit(byte[] fileContent)
        {
            char[] version = fileContent.Skip(71).Take(4).Select(b => (char)b).ToArray();
            if (char.IsNumber(version[0]) && char.IsNumber(version[2]) && char.IsNumber(version[3]))
                return $"{version[0]}.{version[2]}{version[3]}";

            return "";
        }
    }
}
