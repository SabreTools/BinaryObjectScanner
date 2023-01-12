namespace BurnOutSharp.Models.AACS
{
    /// <summary>
    /// A properly formatted type 3 or type 4 Media Key Block shall have exactly
    /// one Host Revocation List Record as its second record. This record provides
    /// a list of hosts that have been revoked by the AACS LA. The AACS specification
    /// is applicable to PC-based system where a Licensed Drive and PC Host act
    /// together as the Recording Device and/or Playback Device for AACS Content.
    /// AACS uses a drive-host authentication protocol for the host to verify the
    /// integrity of the data received from the Licensed Drive, and for the Licensed
    /// Drive to check the validity of the host application. The Type and Version
    /// Record and the Host Revocation List Record are guaranteed to be the first two
    /// records of a Media Key Block, to make it easier for Licensed Drives to extract
    /// this data from an arbitrary Media Key Block.
    /// </summary>
    /// <see href="https://aacsla.com/wp-content/uploads/2019/02/AACS_Spec_Common_Final_0953.pdf"/>
    public sealed class HostRevocationListRecord : Record
    {
        /// <summary>
        /// The total number of Host Revocation List Entry fields that follow.
        /// </summary>
        public uint TotalNumberOfEntries;

        /// <summary>
        /// Revocation list entries
        /// </summary>
        public HostRevocationSignatureBlock[] SignatureBlocks;
    }
}