namespace BurnOutSharp.Models.AACS
{
    /// <summary>
    /// A Media Key Block is formatted as a sequence of contiguous Records.
    /// </summary>
    /// <see href="http://web.archive.org/web/20180718234519/https://aacsla.com/jp/marketplace/evaluating/aacs_technical_overview_040721.pdf"/>
    public sealed class MediaKeyBlock
    {
        /// <summary>
        /// Records
        /// </summary>
        public Record[] Records { get; set; }
    }
}