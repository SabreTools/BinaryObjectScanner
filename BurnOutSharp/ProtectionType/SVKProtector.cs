﻿namespace BurnOutSharp.ProtectionType
{
    public class SVKProtector
    {
        public static string CheckContents(byte[] fileContent)
        {
            // "?SVKP" + (char)0x00 + (char)0x00
            byte[] check = new byte[] { 0x3F, 0x53, 0x56, 0x4B, 0x50, 0x00, 0x00 };
            if (fileContent.Contains(check, out int position))
                return $"SVK Protector (Index {position})";

            return null;
        }
    }
}
