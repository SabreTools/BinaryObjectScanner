namespace BurnOutSharp.Models.AACS
{
    /// <summary>
    /// Devices, except for recording devices which are writing Media Key Block
    /// Extensions, may ignore this record. Recording devices shall verify the
    /// signature (see End of Media Key Block record) and use the Version Number
    /// in this record to determine if a new Media Key BLock Extension is, in
    /// fact, more recent than the Media Key Block Extension that is currently
    /// on the media.
    /// </summary>
    /// <see href="https://aacsla.com/wp-content/uploads/2019/02/AACS_Spec_Common_Final_0953.pdf"/>
    public sealed class TypeAndVersionRecord : Record
    {
        /// <summary>
        /// For AACS applications, the MKBType field is one of three values.
        /// It is not an error for a Type 3 Media Key Block to be used for
        /// controlling access to AACS Content on pre- recorded media. In
        /// this case, the device shall not use the KCD.
        /// </summary>
        public MediaKeyBlockType MediaKeyBlockType;

        /// <summary>
        /// The Version Number is a 32-bit unsigned integer. Each time the
        /// licensing agency changes the revocation, it increments the version
        /// number and inserts the new value in subsequent Media Key Blocks.
        /// Thus, larger values indicate more recent Media Key Blocks. The
        /// Version Numbers begin at 1; 0 is a special value used for test
        /// Media Key Blocks.
        /// </summary>
        public uint VersionNumber;
    }
}