using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SecuROM
    {
        public static string CheckContents(string file, byte[] fileContent)
        {
            // "AddD" + (char)0x03 + (char)0x00 + (char)0x00 + (char)0x00
            byte[] check = new byte[] { 0x41, 0x64, 0x64, 0x44, 0x03, 0x00, 0x00, 0x00 };
            if (fileContent.Contains(check, out int position))
                return $"SecuROM {GetV4Version(file, position)} (Index {position})";

            // (char)0xCA + (char)0xDD + (char)0xDD + (char)0xAC + (char)0x03
            check = new byte[] { 0xCA, 0xDD, 0xDD, 0xAC, 0x03 };
            if (fileContent.Contains(check, out position))
                return $"SecuROM {GetV5Version(file, position)} (Index {position})";

            // ".securom" + (char)0xE0 + (char)0xC0
            check = new byte[] { 0x2E, 0x73, 0x65, 0x63, 0x75, 0x72, 0x6F, 0x6D, 0xE0, 0xC0 };
            if (fileContent.Contains(check, out position))
                return $"SecuROM {GetV7Version(file)} (Index {position}";

            // ".securom"
            check = new byte[] { 0x2E, 0x73, 0x65, 0x63, 0x75, 0x72, 0x6F, 0x6D };
            if (fileContent.Contains(check, out position))
                return $"SecuROM {GetV7Version(file)} (Index {position}";

            // "_and_play.dll" + (char)0x00 + "drm_pagui_doit"
            check = new byte[] { 0x5F, 0x61, 0x6E, 0x64, 0x5F, 0x70, 0x6C, 0x61, 0x79, 0x2E, 0x64, 0x6C, 0x6C, 0x00, 0x64, 0x72, 0x6D, 0x5F, 0x70, 0x61, 0x67, 0x75, 0x69, 0x5F, 0x64, 0x6F, 0x69, 0x74 };
            if (fileContent.Contains(check, out position))
                return $"SecuROM Product Activation {Utilities.GetFileVersion(file)} (Index {position}";

            // ".cms_t" + (char)0x00
            check = new byte[] { 0x2E, 0x63, 0x6D, 0x73, 0x5F, 0x74, 0x00 };
            if (fileContent.Contains(check, out position))
                return $"SecuROM 1-3 (Index {position}";

            // ".cms_d" + (char)0x00
            check = new byte[] { 0x2E, 0x63, 0x6D, 0x73, 0x5F, 0x64, 0x00 };
            if (fileContent.Contains(check, out position))
                return $"SecuROM 1-3 (Index {position}";

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetFileName(f).Equals("CMS16.DLL", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("CMS_95.DLL", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("CMS_NT.DLL", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("CMS32_95.DLL", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("CMS32_NT.DLL", StringComparison.OrdinalIgnoreCase)))
                {
                    return "SecuROM";
                }
                else if (files.Any(f => Path.GetFileName(f).Equals("SINTF32.DLL", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("SINTF16.DLL", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("SINTFNT.DLL", StringComparison.OrdinalIgnoreCase)))
                {
                    return "SecuROM New";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("CMS16.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("CMS_95.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("CMS_NT.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("CMS32_95.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("CMS32_NT.DLL", StringComparison.OrdinalIgnoreCase))
                {
                    return "SecuROM";
                }
                else if (Path.GetFileName(path).Equals("SINTF32.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("SINTF16.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("SINTFNT.DLL", StringComparison.OrdinalIgnoreCase))
                {
                    return "SecuROM New";
                }
            }

            return null;
        }

        private static string GetV4Version(string file, int position)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                br.BaseStream.Seek(position + 8, SeekOrigin.Begin); // Begin reading after "AddD"
                char version = br.ReadChar();
                br.ReadByte();
                char subVersion1 = br.ReadChar();
                char subVersion2 = br.ReadChar();
                br.ReadByte();
                char subsubVersion1 = br.ReadChar();
                char subsubVersion2 = br.ReadChar();
                br.ReadByte();
                char subsubsubVersion1 = br.ReadChar();
                char subsubsubVersion2 = br.ReadChar();
                char subsubsubVersion3 = br.ReadChar();
                char subsubsubVersion4 = br.ReadChar();

                return version + "." + subVersion1 + subVersion2 + "." + subsubVersion1 + subsubVersion2 + "." + subsubsubVersion1 + subsubsubVersion2 + subsubsubVersion3 + subsubsubVersion4;
            }
        }

        private static string GetV5Version(string file, int position)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                br.BaseStream.Seek(position + 8, SeekOrigin.Begin); // Begin reading after "ÊÝÝ¬"
                byte version = (byte)(br.ReadByte() & 0xF);
                br.ReadByte();
                byte subVersion1 = (byte)(br.ReadByte() ^ 36);
                byte subVersion2 = (byte)(br.ReadByte() ^ 28);
                br.ReadByte();
                byte subsubVersion1 = (byte)(br.ReadByte() ^ 42);
                byte subsubVersion2 = (byte)(br.ReadByte() ^ 8);
                br.ReadByte();
                byte subsubsubVersion1 = (byte)(br.ReadByte() ^ 16);
                byte subsubsubVersion2 = (byte)(br.ReadByte() ^ 116);
                byte subsubsubVersion3 = (byte)(br.ReadByte() ^ 34);
                byte subsubsubVersion4 = (byte)(br.ReadByte() ^ 22);

                if (version == 0 || version > 9)
                    return "";

                return version + "." + subVersion1 + subVersion2 + "." + subsubVersion1 + subsubVersion2 + "." + subsubsubVersion1 + subsubsubVersion2 + subsubsubVersion3 + subsubsubVersion4;
            }
        }

        private static string GetV7Version(string file)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                br.BaseStream.Seek(236, SeekOrigin.Begin);
                byte[] bytes = br.ReadBytes(4);
                // if (bytes[0] == 0xED && bytes[3] == 0x5C {
                if (bytes[3] == 0x5C)
                {
                    //SecuROM 7 new and 8
                    return (bytes[0] ^ 0xEA).ToString() + "." + (bytes[1] ^ 0x2C).ToString("00") + "." + (bytes[2] ^ 0x8).ToString("0000");
                }
                else // SecuROM 7 old
                {
                    br.BaseStream.Seek(122, SeekOrigin.Begin);
                    bytes = br.ReadBytes(2);
                    return "7." + (bytes[0] ^ 0x10).ToString("00") + "." + (bytes[1] ^ 0x10).ToString("0000");
                    //return "7.01-7.10"
                }
            }
        }
    }
}
