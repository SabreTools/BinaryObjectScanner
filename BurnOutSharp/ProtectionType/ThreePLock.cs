namespace BurnOutSharp.ProtectionType
{
    public class ThreePLock
    {
        public static string CheckContents(string fileContent)
        {
            string check = ".ldr";
            if (fileContent.Contains(check))
                return $"3PLock (Index {fileContent.IndexOf(check)})";

            check = ".ldt";
            if (fileContent.Contains(check))
                return $"3PLock (Index {fileContent.IndexOf(check)})";

            //check = "Y" + (char)0xC3 + "U" + (char)0x8B + (char)0xEC + (char)0x83 + (char)0xEC + "0SVW";
            //if (fileContent.Contains(check))
            //    return $"3PLock (Index {check})";

            return null;
        }
    }
}
