namespace BurnOutSharp.Models.AACS
{
    /// <summary>
    /// A properly formatted type 3 or type 4 Media Key Block contains exactly
    /// one Drive Revocation List Record. It follows the Host Revocation List
    /// Record, although it may not immediately follow it.
    /// 
    /// The Drive Revocation List Record is identical to the Host Revocation
    /// List Record, except it has type 2016, and it contains Drive Revocation
    /// List Entries, not Host Revocation List Entries. The Drive Revocation List
    /// Entries refer to Drive IDs in the Drive Certificates.
    /// </summary>
    /// <see href="https://aacsla.com/wp-content/uploads/2019/02/AACS_Spec_Common_Final_0953.pdf"/>
    public sealed class DriveRevocationListRecord : Record
    {
        /// <summary>
        /// The total number of Drive Revocation List Entry fields that follow.
        /// </summary>
        public uint TotalNumberOfEntries;

        /// <summary>
        /// Revocation list entries
        /// </summary>
        public DriveRevocationSignatureBlock[] SignatureBlocks;
    }
}