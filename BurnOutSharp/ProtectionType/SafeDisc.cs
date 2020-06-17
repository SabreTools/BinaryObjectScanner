using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SafeDisc
    {
        public static string CheckContents(string file, string fileContent)
        {
            string check = "BoG_ *90.0&!!  Yy>";
            int position = fileContent.IndexOf(check);
            if (position > -1)
            {
                string check2 = "product activation library";
                if (fileContent.Contains(check2))
                    return $"SafeCast {GetVersion(file, position)} (Index {position}, {fileContent.IndexOf(check2)}";
                else
                    return $"SafeDisc {GetVersion(file, position)} (Index {position})";
            }

            check = (char)0x00 + (char)0x00 + "BoG_";
            if (fileContent.Contains(check))
            {
                string version = EVORE.SearchSafeDiscVersion(file);
                if (version.Length > 0)
                    return $"SafeDisc {version} (Index {fileContent.IndexOf(check)})";

                return $"SafeDisc 3.20-4.xx (version removed) (Index {fileContent.IndexOf(check)})";
            }

            check = "stxt774";
            if (fileContent.Contains(check))
            {
                string version = EVORE.SearchSafeDiscVersion(file);
                if (version.Length > 0)
                    return $"SafeDisc {version} (Index {fileContent.IndexOf(check)})";

                return $"SafeDisc 3.20-4.xx (version removed) (Index {fileContent.IndexOf(check)})";
            }

            check = "stxt371";
            if (fileContent.Contains(check))
            {
                string version = EVORE.SearchSafeDiscVersion(file);
                if (version.Length > 0)
                    return $"SafeDisc {version} (Index {fileContent.IndexOf(check)})";

                return $"SafeDisc 3.20-4.xx (version removed) (Index {fileContent.IndexOf(check)})";
            }

            return null;
        }

        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: These are all cop-outs that don't check the existence of the other files
                if (files.Any(f => Path.GetFileName(f).Equals("DPLAYERX.DLL", StringComparison.OrdinalIgnoreCase)))
                {
                    string file = files.First(f => Path.GetFileName(f).Equals("DPLAYERX.DLL", StringComparison.OrdinalIgnoreCase));
                    return GetDPlayerXVersion(file);
                }
                else if (files.Any(f => Path.GetFileName(f).Equals("drvmgt.dll", StringComparison.OrdinalIgnoreCase)))
                {
                    string file = files.First(f => Path.GetFileName(f).Equals("drvmgt.dll", StringComparison.OrdinalIgnoreCase));
                    return GetDrvmgtVersion(file);
                }
                else if (files.Any(f => Path.GetFileName(f).Equals("secdrv.sys", StringComparison.OrdinalIgnoreCase)))
                {
                    string file = files.First(f => Path.GetFileName(f).Equals("secdrv.sys", StringComparison.OrdinalIgnoreCase));
                    return GetSecdrvVersion(file);
                }
                else if (path.EndsWith(".SafeDiscDVD.bundle", StringComparison.OrdinalIgnoreCase))
                {
                    return "SafeDisc for Macintosh";
                }
            }
            else
            {
                // V1
                if (Path.GetFileName(path).Equals("CLCD16.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("CLCD32.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("CLOKSPL.EXE", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(path).Trim('.').Equals("icd", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(path).Trim('.').Equals("016", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(path).Trim('.').Equals("256", StringComparison.OrdinalIgnoreCase))
                {
                    return "SafeDisc 1";
                }

                // V1 or greater
                else if (Path.GetFileName(path).Equals("00000001.TMP", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("CLCD32.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("CLOKSPL.EXE", StringComparison.OrdinalIgnoreCase))
                {
                    return "SafeDisc 1 or greater";
                }

                // V2 or greater
                else if (Path.GetFileName(path).Equals("00000002.TMP", StringComparison.OrdinalIgnoreCase))
                {
                    return "SafeDisc 2 or greater";
                }

                // Specific Versions
                else if (Path.GetFileName(path).Equals("DPLAYERX.DLL", StringComparison.OrdinalIgnoreCase))
                {
                    return GetDPlayerXVersion(path);
                }
                else if (Path.GetFileName(path).Equals("drvmgt.dll", StringComparison.OrdinalIgnoreCase))
                {
                    return GetDrvmgtVersion(path);
                }
                else if (Path.GetFileName(path).Equals("secdrv.sys", StringComparison.OrdinalIgnoreCase))
                {
                    return GetSecdrvVersion(path);
                }
            }

            return null;
        }

        private static string GetDPlayerXVersion(string file)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            FileInfo fi = new FileInfo(file);
            if (fi.Length == 81408)
                return "SafeDisc 1.0x";
            else if (fi.Length == 155648)
                return "SafeDisc 1.1x";
            else if (fi.Length == 156160)
                return "SafeDisc 1.1x-1.2x";
            else if (fi.Length == 163328)
                return "SafeDisc 1.3x";
            else if (fi.Length == 165888)
                return "SafeDisc 1.35";
            else if (fi.Length == 172544)
                return "SafeDisc 1.40";
            else if (fi.Length == 173568)
                return "SafeDisc 1.4x";
            else if (fi.Length == 136704)
                return "SafeDisc 1.4x";
            else if (fi.Length == 138752)
                return "SafeDisc 1.5x";
            else
                return "SafeDisc 1";
        }

        private static string GetDrvmgtVersion(string file)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            FileInfo fi = new FileInfo(file);
            if (fi.Length == 34816)
                return "SafeDisc 1.0x";
            else if (fi.Length == 32256)
                return "SafeDisc 1.1x-1.3x";
            else if (fi.Length == 31744)
                return "SafeDisc 1.4x";
            else if (fi.Length == 34304)
                return "SafeDisc 1.5x-2.40";
            else if (fi.Length == 35840)
                return "SafeDisc 2.51-2.60";
            else if (fi.Length == 40960)
                return "SafeDisc 2.70";
            else if (fi.Length == 23552)
                return "SafeDisc 2.80";
            else if (fi.Length == 41472)
                return "SafeDisc 2.90-3.10";
            else if (fi.Length == 24064)
                return "SafeDisc 3.15-3.20";
            else
                return "SafeDisc 1 or greater";
        }

        private static string GetSecdrvVersion(string file)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            FileInfo fi = new FileInfo(file);
            if (fi.Length == 20128)
                return "SafeDisc 2.10";
            else if (fi.Length == 27440)
                return "SafeDisc 2.30";
            else if (fi.Length == 28624)
                return "SafeDisc 2.40";
            else if (fi.Length == 18768)
                return "SafeDisc 2.50";
            else if (fi.Length == 28400)
                return "SafeDisc 2.51";
            else if (fi.Length == 29392)
                return "SafeDisc 2.60";
            else if (fi.Length == 11376)
                return "SafeDisc 2.70";
            else if (fi.Length == 12464)
                return "SafeDisc 2.80";
            else if (fi.Length == 12400)
                return "SafeDisc 2.90";
            else if (fi.Length == 12528)
                return "SafeDisc 3.10";
            else if (fi.Length == 12528)
                return "SafeDisc 3.15";
            else if (fi.Length == 11973)
                return "SafeDisc 3.20";
            else
                return "SafeDisc 2 or greater";
        }

        private static string GetVersion(string file, int position)
        {
            if (file == null || !File.Exists(file))
                return string.Empty;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                br.BaseStream.Seek(position + 20, SeekOrigin.Begin); // Begin reading after "BoG_ *90.0&!!  Yy>" for old SafeDisc
                int version = br.ReadInt32();
                int subVersion = br.ReadInt32();
                int subsubVersion = br.ReadInt32();

                if (version != 0)
                    return version + "." + subVersion.ToString("00") + "." + subsubVersion.ToString("000");

                br.BaseStream.Seek(position + 18 + 14, SeekOrigin.Begin); // Begin reading after "BoG_ *90.0&!!  Yy>" for newer SafeDisc
                version = br.ReadInt32();
                subVersion = br.ReadInt32();
                subsubVersion = br.ReadInt32();

                if (version == 0)
                    return "";

                return version + "." + subVersion.ToString("00") + "." + subsubVersion.ToString("000");
            }
        }
    }
}
