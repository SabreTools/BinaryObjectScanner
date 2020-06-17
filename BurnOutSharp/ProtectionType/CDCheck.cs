namespace BurnOutSharp.ProtectionType
{
    public class CDCheck
    {
        public static string CheckContents(byte[] fileContent)
        {
            // "GetDriveType"
            byte[] check = new byte[] { 0x47, 0x65, 0x74, 0x44, 0x72, 0x69, 0x76, 0x65, 0x54, 0x79, 0x70, 0x65 };
            if (fileContent.Contains(check, out int position))
                return $"CD Check (Index {position})";

            // "GetVolumeInformation"
            check = new byte[] { 0x47, 0x65, 0x74, 0x56, 0x6F, 0x6C, 0x75, 0x6D, 0x65, 0x49, 0x6E, 0x66, 0x6F, 0x72, 0x6D, 0x61, 0x74, 0x69, 0x6F, 0x6E };
            if (fileContent.Contains(check, out position))
                return $"CD Check (Index {position})";

            return null;
        }
    }
}
