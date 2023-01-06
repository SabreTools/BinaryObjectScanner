namespace BurnOutSharp.Models.Nitro
{
    /// <summary>
    /// Represents a DS cart image
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// DS cart header
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Secure area, may be encrypted or decrypted
        /// </summary>
        public byte[] SecureArea { get; set; }
    }
}