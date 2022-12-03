namespace BurnOutSharp.Wrappers
{
    /// <summary>
    /// Location that the data originated from
    /// </summary>
    public enum DataSource
    {
        /// <summary>
        /// Unknown origin / testing
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// Byte array with offset
        /// </summary>
        ByteArray = 1,

        /// <summary>
        /// Stream
        /// </summary>
        Stream = 2,
    }
}