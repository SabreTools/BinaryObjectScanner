using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class VOBProtectCDDVD
    {
        public static string CheckContents(string file, string fileContent)
        {
            int position;
            if ((position = fileContent.IndexOf("VOB ProtectCD")) > -1)
                return "VOB ProtectCD/DVD " + GetOldVersion(file, --position); // TODO: Verify this subtract

            if ((position = fileContent.IndexOf("DCP-BOV" + (char)0x00 + (char)0x00)) > -1)
            {
                string version = GetVersion(file, --position); // TODO: Verify this subtract
                if (version.Length > 0)
                {
                    return "VOB ProtectCD/DVD " + version;
                }

                version = EVORE.SearchProtectDiscVersion(file);
                if (version.Length > 0)
                {
                    if (version.StartsWith("2"))
                    {
                        version = "6" + version.Substring(1);
                    }
                    return "VOB ProtectCD/DVD " + version;
                }

                return "VOB ProtectCD/DVD 5.9-6.0" + GetBuild(file, position);
            }

            if (fileContent.Contains(".vob.pcd"))
                return "VOB ProtectCD";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                if (files.Count(f => Path.GetFileName(f).Equals("VOB-PCD.KEY", StringComparison.OrdinalIgnoreCase)) > 0)
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
            if (file == null)
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
            if (file == null)
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
            if (file == null)
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
