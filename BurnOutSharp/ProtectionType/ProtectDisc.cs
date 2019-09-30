using System;
using System.IO;

namespace BurnOutSharp.ProtectionType
{
    public class ProtectDisc
    {
        public static string CheckContents(string file, string fileContent)
        {
            int position;
            if ((position = fileContent.IndexOf("HúMETINF")) > -1)
            {
                string version = EVORE.SearchProtectDiscVersion(file);
                if (version.Length > 0)
                {
                    string[] astrVersionArray = version.Split('.');
                    if (astrVersionArray[0] == "9")
                    {
                        if (GetVersionBuild76till10(file, position, out int ibuild).Length > 0)
                            return "ProtectDisc " + astrVersionArray[0] + "." + astrVersionArray[1] + astrVersionArray[2] + "." + astrVersionArray[3] + " (Build " + ibuild + ")";
                    }
                    else
                        return "ProtectDisc " + astrVersionArray[0] + "." + astrVersionArray[1] + "." + astrVersionArray[2] + " (Build " + astrVersionArray[3] + ")";
                }
            }

            if ((position = fileContent.IndexOf("ACE-PCD")) > -1)
            {
                string version = EVORE.SearchProtectDiscVersion(file);
                if (version.Length > 0)
                {
                    string[] astrVersionArray = version.Split('.');
                    return "ProtectDisc " + astrVersionArray[0] + "." + astrVersionArray[1] + "." + astrVersionArray[2] + " (Build " + astrVersionArray[3] + ")";
                }

                return "ProtectDisc " + GetVersionBuild6till8(file, position);
            }

            return null;
        }

        private static string GetVersionBuild6till8(string file, int position)
        {
            if (file == null)
                return string.Empty;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                string version;
                string strBuild;

                br.BaseStream.Seek(position - 12, SeekOrigin.Begin);
                if (br.ReadByte() == 0xA && br.ReadByte() == 0xD && br.ReadByte() == 0xA && br.ReadByte() == 0xD) // ProtectDisc 6-7 with Build Number in plain text
                {
                    br.BaseStream.Seek(position - 12 - 6, SeekOrigin.Begin);
                    if (new string(br.ReadChars(6)) == "Henrik") // ProtectDisc 7
                    {
                        version = "7.1-7.5";
                        br.BaseStream.Seek(position - 12 - 6 - 6, SeekOrigin.Begin);
                    }
                    else // ProtectDisc 6
                    {
                        version = "6";
                        br.BaseStream.Seek(position - 12 - 10, SeekOrigin.Begin);
                        while (true) //search for e.g. "Build 050913 -  September 2005"
                        {
                            if (Char.IsNumber(br.ReadChar()))
                                break;
                            br.BaseStream.Seek(-2, SeekOrigin.Current); //search upwards
                        }

                        br.BaseStream.Seek(-5, SeekOrigin.Current);
                    }
                }
                else
                {
                    br.BaseStream.Seek(position + 28, SeekOrigin.Begin);
                    if (br.ReadByte() == 0xFB)
                    {
                        return "7.6-7.x";
                    }
                    else
                    {
                        return "8.0";
                    }
                }
                strBuild = "" + br.ReadChar() + br.ReadChar() + br.ReadChar() + br.ReadChar() + br.ReadChar();
                return version + " (Build " + strBuild + ")";
            }
        }

        private static string GetVersionBuild76till10(string file, int position, out int irefBuild)
        {
            if (file == null)
                return string.Empty;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var br = new BinaryReader(fs))
            {
                br.BaseStream.Seek(position + 37, SeekOrigin.Begin);
                byte subversion = br.ReadByte();
                br.ReadByte();
                byte version = br.ReadByte();
                br.BaseStream.Seek(position + 49, SeekOrigin.Begin);
                irefBuild = br.ReadInt32();
                br.BaseStream.Seek(position + 53, SeekOrigin.Begin);
                byte versionindicatorPD9 = br.ReadByte();
                br.BaseStream.Seek(position + 0x40, SeekOrigin.Begin);
                byte subsubversionPD9x = br.ReadByte();
                byte subversionPD9x2 = br.ReadByte();
                byte subversionPD9x1 = br.ReadByte();

                // version 7
                if (version == 0xAC)
                    return "7." + (subversion ^ 0x43) + " (Build " + irefBuild + ")";
                // version 8
                else if (version == 0xA2)
                {
                    if (subversion == 0x46)
                    {
                        if ((irefBuild & 0x3A00) == 0x3A00)
                            return "8.2" + " (Build " + irefBuild + ")";
                        else
                            return "8.1" + " (Build " + irefBuild + ")";
                    }
                    return "8." + (subversion ^ 0x47) + " (Build " + irefBuild + ")";
                }
                // version 9
                else if (version == 0xA3)
                {
                    // version removed or not given
                    if ((subversionPD9x2 == 0x5F && subversionPD9x1 == 0x61) || (subversionPD9x1 == 0 && subversionPD9x2 == 0))
                    {
                        if (versionindicatorPD9 == 0xB)
                            return "9.0-9.4" + " (Build " + irefBuild + ")";
                        else if (versionindicatorPD9 == 0xC)
                        {
                            if (subversionPD9x2 == 0x5F && subversionPD9x1 == 0x61)
                                return "9.5-9.11" + " (Build " + irefBuild + ")";
                            else if (subversionPD9x1 == 0 && subversionPD9x2 == 0)
                                return "9.11-9.20" + " (Build " + irefBuild + ")";
                        }
                        else
                            return "9." + subversionPD9x1 + subversionPD9x2 + "." + subsubversionPD9x + " (Build " + irefBuild + ")";
                    }
                }
                else if (version == 0xA0)
                {
                    // version removed
                    if (subversionPD9x1 != 0 || subversionPD9x2 != 0)
                        return "10." + subversionPD9x1 + "." + subsubversionPD9x + " (Build " + irefBuild + ")";
                    else
                        return "10.x (Build " + irefBuild + ")";
                }
                else
                    return "7.6-10.x (Build " + irefBuild + ")";

                return "";
            }
        }
    }
}
