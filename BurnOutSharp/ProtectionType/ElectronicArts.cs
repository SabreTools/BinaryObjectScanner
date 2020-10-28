namespace BurnOutSharp.ProtectionType
{
    public class ElectronicArts
    {
        // TODO: Verify this doesn't over-match
        public static string CheckContents(byte[] fileContent, bool includePosition = false)
        {
            // EASTL
            byte[] check = new byte[] { 0x45, 0x41, 0x53, 0x54, 0x4C };
            if (fileContent.Contains(check, out int position))
                return "Cucko (EA Custom)" + (includePosition ? $" (Index {position})" : string.Empty);

            // GenericEA + (char)0x00 + (char)0x00 + (char)0x00 + Activation
            check = new byte[] { 0x47, 0x65, 0x6E, 0x65, 0x72, 0x69, 0x63, 0x45, 0x41, 0x00, 0x00, 0x00, 0x41, 0x63, 0x74, 0x69, 0x76, 0x61, 0x74, 0x69, 0x6F, 0x6E };
            if (fileContent.Contains(check, out position))
                return "EA DRM Protection" + (includePosition ? $" (Index {position})" : string.Empty);

            // E + (char)0x00 + A + (char)0x00 +   + (char)0x00 + D + (char)0x00 + R + (char)0x00 + M + (char)0x00 +   + (char)0x00 + H + (char)0x00 + e + (char)0x00 + l + (char)0x00 + p + (char)0x00 + e + (char)0x00 + r
            check = new byte[] { 0x45, 0x00, 0x41, 0x00, 0x20, 0x00, 0x44, 0x00, 0x52, 0x00, 0x4D, 0x00, 0x20, 0x00, 0x48, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x70, 0x00, 0x65, 0x00, 0x72 };
            if (fileContent.Contains(check, out position))
                return "EA DRM Protection" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }
    }
}
