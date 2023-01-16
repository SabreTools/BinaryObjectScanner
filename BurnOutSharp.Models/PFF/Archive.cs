namespace BurnOutSharp.Models.PFF
{
    /// <summary>
    /// PFF archive
    /// </summary>
    /// <see href="https://devilsclaws.net/download/file-pff-new-bz2"/>
    public sealed class Archive
    {
        /// <summary>
        /// Archive header
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Segments
        /// </summary>
        public Segment[] Segments { get; set; }

        /// <summary>
        /// Footer
        /// </summary>
        public Footer Footer { get; set; }
    }
}