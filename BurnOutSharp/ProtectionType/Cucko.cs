namespace BurnOutSharp.ProtectionType
{
    public class Cucko
    {
        // TODO: Verify this doesn't over-match
        public static string CheckContents(byte[] fileContent)
        {
            // "EASTL"
            byte[] check = new byte[] { 0x45, 0x41, 0x53, 0x54, 0x4C };
            if (fileContent.Contains(check, out int position))
                return $"Cucko (EA Custom) (Index {position})";

            return null;
        }
    }
}
