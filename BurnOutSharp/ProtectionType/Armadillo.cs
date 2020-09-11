namespace BurnOutSharp.ProtectionType
{
    public class Armadillo
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // ".nicode" + (char)0x00
            byte[] check = new byte[] { 0x2E, 0x6E, 0x69, 0x63, 0x6F, 0x64, 0x65, 0x00 };
            if (fileContent.Contains(check, out int position))
                return $"Armadillo" + (includePosition ? $" (Index {position})" : string.Empty);

            // "ARMDEBUG"
            check = new byte[] { 0x41, 0x52, 0x4D, 0x44, 0x45, 0x42, 0x55, 0x47 };
            if (fileContent.Contains(check, out position))
                return $"Armadillo" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
