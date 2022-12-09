namespace psxt001z
{
    public static class Common
    {
        public const int ZERO = 0;

        public const string VERSION = "v0.21 beta 1";

        /// <summary>
        /// BCD to u_char
        /// </summary>
        public static byte btoi(byte b) => (byte)(((b) / 16 * 10 + (b) % 16));

        /// <summary>
        /// u_char to BCD
        /// </summary>
        public static byte itob(byte i) => (byte)(((i) / 10 * 16 + (i) % 10));
    }
}
