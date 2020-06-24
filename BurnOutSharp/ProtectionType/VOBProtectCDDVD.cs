using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class VOBProtectCDDVD
    {
        public static string CheckContents(string file, byte[] fileContent)
        {
            // "VOB ProtectCD"
            byte[] check = new byte[] { 0x56, 0x4F, 0x42, 0x20, 0x50, 0x72, 0x6F, 0x74, 0x65, 0x63, 0x74, 0x43, 0x44 };
            if (fileContent.Contains(check, out int position))
                return $"VOB ProtectCD/DVD {GetOldVersion(fileContent, --position)} (Index {position})"; // TODO: Verify this subtract

            // "DCP-BOV" + (char)0x00 + (char)0x00
            check = new byte[] { 0x44, 0x43, 0x50, 0x2D, 0x42, 0x4F, 0x56, 0x00, 0x00 };
            if (fileContent.Contains(check, out position))
            {
                string version = GetVersion(fileContent, --position); // TODO: Verify this subtract
                if (version.Length > 0)
                    return $"VOB ProtectCD/DVD {version} (Index {position})";

                version = EVORE.SearchProtectDiscVersion(file, fileContent);
                if (version.Length > 0)
                {
                    if (version.StartsWith("2"))
                        version = $"6{version.Substring(1)}";

                    return $"VOB ProtectCD/DVD {version} (Index {position})";
                }

                return $"VOB ProtectCD/DVD 5.9-6.0 {GetBuild(fileContent, position)} (Index {position})";
            }

            // ".vob.pcd"
            check = new byte[] { 0x2E, 0x76, 0x6F, 0x62, 0x2E, 0x70, 0x63, 0x64 };
            if (fileContent.Contains(check, out position))
                return $"VOB ProtectCD (Index {position})";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("VOB-PCD.KEY", StringComparison.OrdinalIgnoreCase)))
                    return "VOB ProtectCD/DVD";
            }
            else
            {
                if (Path.GetFileName(path).Equals("VOB-PCD.KEY", StringComparison.OrdinalIgnoreCase))
                    return "VOB ProtectCD/DVD";
            }

            return null;
        }

        private static string GetBuild(byte[] fileContent, int position)
        {
            if (!char.IsNumber((char)fileContent[position - 13]))
                return ""; //Build info removed

            int build = BitConverter.ToInt16(fileContent, position - 4); // Check if this is supposed to be a 4-byte read
            return $" (Build {build})";
        }

        private static string GetOldVersion(byte[] fileContent, int position)
        {
            char[] version = new ArraySegment<byte>(fileContent, position + 16, 4).Select(b => (char)b).ToArray(); // Begin reading after "VOB ProtectCD"
            if (char.IsNumber(version[0]) && char.IsNumber(version[2]) && char.IsNumber(version[3]))
                return $"{version[0]}.{version[2]}{version[3]}";

            return "old";
        }

        private static string GetVersion(byte[] fileContent, int position)
        {
            if (fileContent[position - 2] == 5)
            {
                int index = position - 4;
                byte subsubVersion = (byte)((fileContent[index] & 0xF0) >> 4);
                index++;
                byte subVersion = (byte)((fileContent[index] & 0xF0) >> 4);
                return $"5.{subVersion}.{subsubVersion}";
            }

            return "";
        }
    }
}
