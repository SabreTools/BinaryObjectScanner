﻿namespace BurnOutSharp.ProtectionType
{
    public class ThreePLock
    {
        public static string CheckContents(byte[] fileContent)
        {
            // ".ldr"
            byte[] check = new byte[] { 0x2E, 0x6C, 0x64, 0x72 };
            if (fileContent.Contains(check, out int position))
                return $"3PLock (Index {position})";

            // ".ldt"
            check = new byte[] { 0x2E, 0x6C, 0x64, 0x74 };
            if (fileContent.Contains(check, out position))
                return $"3PLock (Index {position})";

            // "Y" + (char)0xC3 + "U" + (char)0x8B + (char)0xEC + (char)0x83 + (char)0xEC + "0SVW"
            // check = new byte[] { 0x59, 0xC3, 0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x30, 0x53, 0x56, 0x57 };
            //if (fileContent.Contains(check, out position))
            //    return $"3PLock (Index {position})";

            return null;
        }
    }
}
