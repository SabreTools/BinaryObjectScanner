namespace BurnOutSharp.Models.PlayJ
{
    /// <summary>
    /// Data referred to by <see cref="AudioHeaderV1.UnknownOffset3"/>
    /// </summary>
    public sealed class UnknownBlock3
    {
        /// <summary>
        /// Unknown data
        /// </summary>
        public byte[] Data;

        // Notes about Data:
        // - This may be where the encrypted audio samples live
        // - It is also possible that it's where the ad data lives and samples follow
        //      + See V2 for example of why this would be the case
    }
}