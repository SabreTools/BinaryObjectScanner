﻿namespace BurnOutSharp.ProtectionType
{
    public class Sysiphus
    {
        public static string CheckContents(byte[] fileContent)
        {
            // "V SUHPISYSDVD"
            byte[] check = new byte[] { 0x56, 0x20, 0x53, 0x55, 0x48, 0x50, 0x49, 0x53, 0x59, 0x53, 0x44, 0x56, 0x44 };
            if (fileContent.Contains(check, out int position))
                return $"Sysiphus DVD {GetVersion(fileContent, position)} (Index {position})";

            // "V SUHPISYS"
            check = new byte[] { 0x56, 0x20, 0x53, 0x55, 0x48, 0x50, 0x49, 0x53, 0x59, 0x53 };
            if (fileContent.Contains(check, out position))
                return $"Sysiphus {GetVersion(fileContent, position)} (Index {position})";

            return null;
        }

        private static string GetVersion(byte[] fileContent, int position)
        {
            int index = position - 3;
            char subVersion = (char)fileContent[index];
            index++;
            index++;
            char version = (char)fileContent[index];

            if (char.IsNumber(version) && char.IsNumber(subVersion))
                return $"{version}.{subVersion}";
            else
                return "";
        }
    }
}
