namespace BurnOutSharp.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCCH/Extended_Header#ARM11_Local_System_Capabilities"/>
    public sealed class ARM11LocalSystemCapabilities
    {
        /// <summary>
        /// Program ID
        /// </summary>
        public byte[] ProgramID;

        /// <summary>
        /// Core version (The Title ID low of the required FIRM)
        /// </summary>
        public uint CoreVersion;

        /// <summary>
        /// Flag1 (implemented starting from 8.0.0-18).
        /// </summary>
        public ARM11LSCFlag1 Flag1;

        /// <summary>
        /// Flag2 (implemented starting from 8.0.0-18).
        /// </summary>
        public ARM11LSCFlag2 Flag2;

        /// <summary>
        /// Flag0
        /// </summary>
        public ARM11LSCFlag0 Flag0;

        /// <summary>
        /// Priority
        /// </summary>
        public byte Priority;

        /// <summary>
        /// Resource limit descriptors. The first byte here controls the maximum allowed CpuTime.
        /// </summary>
        public byte[][] ResourceLimitDescriptors;

        /// <summary>
        /// Storage info
        /// </summary>
        public StorageInfo StorageInfo;

        /// <summary>
        /// Service access control
        /// </summary>
        public byte[][] ServiceAccessControl;

        /// <summary>
        /// Extended service access control, support for this was implemented with 9.3.0-X.
        /// </summary>
        public byte[][] ExtendedServiceAccessControl;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved;

        /// <summary>
        /// Resource limit category. (0 = APPLICATION, 1 = SYS_APPLET, 2 = LIB_APPLET, 3 = OTHER (sysmodules running under the BASE memregion))
        /// </summary>
        public ResourceLimitCategory ResourceLimitCategory;
    }
}
