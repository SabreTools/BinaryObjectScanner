namespace BurnOutSharp.Models.InstallShieldCabinet
{
    public static class Constants
    {
        public static readonly byte[] SignatureBytes = new byte[] { 0x49, 0x53, 0x63, 0x28 };

        public const string SignatureString = "ISc(";

        public const uint SignatureUInt32 = 0x28635349;

        public const int COMMON_HEADER_SIZE = 20;

        public const int VOLUME_HEADER_SIZE_V5 = 40;
        
        public const int VOLUME_HEADER_SIZE_V6 = 64;

        // TODO: Determine how the value "71" was chosen here
        public const int MAX_FILE_GROUP_COUNT = 71;

        // TODO: Determine how the value "71" was chosen here
        public const int MAX_COMPONENT_COUNT = 71;
    }
}