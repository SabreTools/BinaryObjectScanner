using System;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class ProtectDisc
    {
        public static string CheckContents(string file, byte[] fileContent)
        {
            // "HúMETINF"
            byte[] check = new byte[] { 0x48, 0xFA, 0x4D, 0x45, 0x54, 0x49, 0x4E, 0x46 };
            if (fileContent.Contains(check, out int position))
            {
                string version = EVORE.SearchProtectDiscVersion(file, fileContent);
                if (version.Length > 0)
                {
                    string[] astrVersionArray = version.Split('.');
                    if (astrVersionArray[0] == "9")
                    {
                        if (GetVersionBuild76till10(fileContent, position, out int ibuild).Length > 0)
                            return $"ProtectDisc {astrVersionArray[0]}.{astrVersionArray[1]}{astrVersionArray[2]}.{astrVersionArray[3]} (Build {ibuild}) (Index {position})";
                    }
                    else
                    {
                        return $"ProtectDisc {astrVersionArray[0]}.{astrVersionArray[1]}.{astrVersionArray[2]} (Build {astrVersionArray[3]}) (Index {position})";
                    }
                }
            }

            // "ACE-PCD"
            check = new byte[] { 0x41, 0x43, 0x45, 0x2D, 0x50, 0x43, 0x44 };
            if (fileContent.Contains(check, out position))
            {
                string version = EVORE.SearchProtectDiscVersion(file, fileContent);
                if (version.Length > 0)
                {
                    string[] astrVersionArray = version.Split('.');
                    return $"ProtectDisc {astrVersionArray[0]}.{astrVersionArray[1]}.{astrVersionArray[2]} (Build {astrVersionArray[3]}) (Index {position})";
                }

                return $"ProtectDisc {GetVersionBuild6till8(fileContent, position)} (Index {position})";
            }

            return null;
        }

        private static string GetVersionBuild6till8(byte[] fileContent, int position)
        {
            string version;
            string strBuild;

            int index = position - 12;
            if (new ArraySegment<byte>(fileContent, index, 4).SequenceEqual(new byte[] { 0x0A, 0x0D, 0x0A, 0x0D })) // ProtectDisc 6-7 with Build Number in plain text
            {
                index = position - 12 - 6;

                // ProtectDisc 7
                if (new string(new ArraySegment<byte>(fileContent, index, 6).Select(b => (char)b).ToArray()) == "Henrik")
                {
                    version = "7.1-7.5";
                    index = position - 12 - 6 - 6;
                }

                // ProtectDisc 6
                else
                {
                    version = "6";
                    index = position - 12 - 10;
                    while (true) //search for e.g. "Build 050913 -  September 2005"
                    {
                        if (Char.IsNumber((char)fileContent[index]))
                            break;

                        index -= 1; //search upwards
                    }

                    index -= 5;
                }
            }
            else
            {
                index = position + 28;
                if (fileContent[index] == 0xFB)
                    return "7.6-7.x";
                else
                    return "8.0";
            }

            strBuild = new string(new ArraySegment<byte>(fileContent, index, 5).Select(b => (char)b).ToArray());
            return $"{version} (Build {strBuild})";
        }

        private static string GetVersionBuild76till10(byte[] fileContent, int position, out int irefBuild)
        {
            int index = position + 37;
            byte subversion = fileContent[index];
            index++;
            index++;
            byte version = fileContent[index];
            index = position + 49;
            irefBuild = BitConverter.ToInt32(fileContent, index);
            index = position + 53;
            byte versionindicatorPD9 = fileContent[index];
            index = position + 0x40;
            byte subsubversionPD9x = fileContent[index];
            index++;
            byte subversionPD9x2 = fileContent[index];
            index++;
            byte subversionPD9x1 = fileContent[index];

            // version 7
            if (version == 0xAC)
                return $"7.{subversion ^ 0x43} (Build {irefBuild})";

            // version 8
            else if (version == 0xA2)
            {
                if (subversion == 0x46)
                {
                    if ((irefBuild & 0x3A00) == 0x3A00)
                        return $"8.2 (Build {irefBuild})";
                    else
                        return $"8.1 (Build {irefBuild})";
                }

                return $"8.{subversion ^ 0x47} (Build {irefBuild})";
            }

            // version 9
            else if (version == 0xA3)
            {
                // version removed or not given
                if ((subversionPD9x2 == 0x5F && subversionPD9x1 == 0x61) || (subversionPD9x1 == 0 && subversionPD9x2 == 0))
                {
                    if (versionindicatorPD9 == 0xB)
                    {
                        return $"9.0-9.4 (Build {irefBuild})";
                    }
                    else if (versionindicatorPD9 == 0xC)
                    {
                        if (subversionPD9x2 == 0x5F && subversionPD9x1 == 0x61)
                            return $"9.5-9.11 (Build {irefBuild})";
                        else if (subversionPD9x1 == 0 && subversionPD9x2 == 0)
                            return $"9.11-9.20 (Build {irefBuild})";
                    }
                    else
                    {
                        return $"9.{subversionPD9x1}{subversionPD9x2}.{subsubversionPD9x} (Build {irefBuild})";
                    }
                }
            }

            // version 10
            else if (version == 0xA0)
            {
                // version removed
                if (subversionPD9x1 != 0 || subversionPD9x2 != 0)
                    return $"10.{subversionPD9x1}.{subsubversionPD9x} (Build {irefBuild})";
                else
                    return $"10.x (Build {irefBuild})";
            }

            // unknown version
            else
            {
                return $"7.6-10.x (Build {irefBuild})";
            }

            return "";
        }
    }
}
