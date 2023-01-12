namespace BurnOutSharp.Models.AACS
{
    /// <see href="https://aacsla.com/wp-content/uploads/2019/02/AACS_Spec_Common_Final_0953.pdf"/>
    public sealed class ExplicitSubsetDifferenceRecord : Record
    {
        /// <summary>
        /// In this record, each subset-difference is encoded with 5 bytes.
        /// </summary>
        public SubsetDifference[] SubsetDifferences;
    }
}