namespace BurnOutSharp.ProtectionType
{
    public class CDCheck
    {
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // MGS CDCheck
            byte[] check = new byte[] { 0x4D, 0x47, 0x53, 0x20, 0x43, 0x44, 0x43, 0x68, 0x65, 0x63, 0x6B };
            if (fileContent.Contains(check, out int position))
                return "Microsoft Game Studios CD Check" + (includePosition ? $" (Index {position})" : string.Empty);
            
            // CDCheck
            check = new byte[] { 0x43, 0x44, 0x43, 0x68, 0x65, 0x63, 0x6B };
            if (fileContent.Contains(check, out position))
                return "Executable-Based CD Check" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        // These content checks are too broad to be useful
        private static string CheckContentsBroad(byte[] fileContent, bool includePosition = false)
        {
            // GetDriveType
            byte[] check = new byte[] { 0x47, 0x65, 0x74, 0x44, 0x72, 0x69, 0x76, 0x65, 0x54, 0x79, 0x70, 0x65 };
            if (fileContent.Contains(check, out int position))
                return "CD Check" + (includePosition ? $" (Index {position})" : string.Empty);

            // GetVolumeInformation
            check = new byte[] { 0x47, 0x65, 0x74, 0x56, 0x6F, 0x6C, 0x75, 0x6D, 0x65, 0x49, 0x6E, 0x66, 0x6F, 0x72, 0x6D, 0x61, 0x74, 0x69, 0x6F, 0x6E };
            if (fileContent.Contains(check, out position))
                return "CD Check" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
