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
                return $"VOB ProtectCD/DVD {GetOldVersion(file, --position)} (Index {position})"; // TODO: Verify this subtract

            // "DCP-BOV" + (char)0x00 + (char)0x00
            check = new byte[] { 0x44, 0x43, 0x50, 0x2D, 0x42, 0x4F, 0x56, 0x00, 0x00 };
            if (fileContent.Contains(check, out position))
            {
                string version = GetVersion(file, --position); // TODO: Verify this subtract
                if (version.Length > 0)
                    return $"VOB ProtectCD/DVD {version} (Index {position})";

                version = EVORE.SearchProtectDiscVersion(file);
                if (version.Length > 0)
                {
                    if (version.StartsWith("2"))
                        version = $"6{version.Substring(1)}";

                    return $"VOB ProtectCD/DVD {version} (Index {position})";
                }

                return $"VOB ProtectCD/DVD 5.9-6.0 {GetBuild(file, position)} (Index {position})";
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

        private static string GetBuild(string file, int position)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                br.BaseStream.Seek(position - 13, SeekOrigin.Begin);
                if (!char.IsNumber(br.ReadChar()))
                    return ""; //Build info removed

                br.BaseStream.Seek(position - 4, SeekOrigin.Begin);
                int build = br.ReadInt16();
                return " (Build " + build + ")";
            }
        }

        private static string GetOldVersion(string file, int position)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                char[] version = new char[3];
                br.BaseStream.Seek(position + 16, SeekOrigin.Begin); // Begin reading after "VOB ProtectCD"
                version[0] = br.ReadChar();
                br.ReadByte();
                version[1] = br.ReadChar();
                version[2] = br.ReadChar();

                if (char.IsNumber(version[0]) && char.IsNumber(version[1]) && char.IsNumber(version[2]))
                    return version[0] + "." + version[1] + version[2];

                return "old";
            }
        }

        private static string GetVersion(string file, int position)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                br.BaseStream.Seek(position - 2, SeekOrigin.Begin);
                byte version = br.ReadByte();
                if (version == 5)
                {
                    br.BaseStream.Seek(position - 4, SeekOrigin.Begin);
                    byte subsubVersion = (byte)((br.ReadByte() & 0xF0) >> 4);
                    byte subVersion = (byte)((br.ReadByte() & 0xF0) >> 4);
                    return version + "." + subVersion + "." + subsubVersion;
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
