namespace BurnOutSharp.ProtectionType
{
    public class ThreePLock
    {
        public static string CheckContents(string fileContent)
        {
            if (fileContent.Contains(".ldr")
                || fileContent.Contains(".ldt"))
                // || fileContent.Contains("Y" + (char)0xC3 + "U" + (char)0x8B + (char)0xEC + (char)0x83 + (char)0xEC + "0SVW")
            {
                return "3PLock";
            }

            return null;
        }
    }
}
