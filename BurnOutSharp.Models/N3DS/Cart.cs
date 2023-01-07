namespace BurnOutSharp.Models.N3DS
{
    /// <summary>
    /// Represents a 3DS cart image
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// 3DS cart header
        /// </summary>
        public NCSDHeader Header { get; set; }

        /// <summary>
        /// NCCH partitions
        /// </summary>
        public NCCHHeader[] Partitions { get; set; }
    }
}