namespace BurnOutSharp.Models.InstallShieldCabinet
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0x49, 0x53, 0x63, 0x28 };

        public const string SignatureString = "ISc(";

        public const uint SignatureUInt32 = 0x28635349;

        // TODO: Determine how the value "71" was chosen here
        public const int MAX_FILE_GROUP_COUNT = 71;

        // TODO: Determine how the value "71" was chosen here
        public const int MAX_COMPONENT_COUNT = 71;
    }
}