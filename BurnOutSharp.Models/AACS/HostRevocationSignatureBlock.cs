namespace BurnOutSharp.Models.AACS
{
    /// <see href="https://aacsla.com/wp-content/uploads/2019/02/AACS_Spec_Common_Final_0953.pdf"/>
    public sealed class HostRevocationSignatureBlock
    {
        /// <summary>
        /// The number of Host Revocation List Entry fields in the signature block.
        /// </summary>
        public uint NumberOfEntries;

        /// <summary>
        /// A list of 8-byte Host Revocation List Entry fields, the length of this
        /// list being equal to the number in the signature block.
        /// </summary>
        public HostRevocationListEntry[] EntryFields;
    }
}