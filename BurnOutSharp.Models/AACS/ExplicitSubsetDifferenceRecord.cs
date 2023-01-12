namespace BurnOutSharp.Models.AACS
{
    /// <see href="http://web.archive.org/web/20180718234519/https://aacsla.com/jp/marketplace/evaluating/aacs_technical_overview_040721.pdf"/>
    public sealed class ExplicitSubsetDifferenceRecord : Record
    {
        /// <summary>
        /// In this record, each subset-difference is encoded with 5 bytes.
        /// </summary>
        public SubsetDifference[] SubsetDifferences;
    }
}