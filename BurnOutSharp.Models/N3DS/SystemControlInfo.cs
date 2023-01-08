namespace BurnOutSharp.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCCH/Extended_Header#System_Control_Info"/>
    public sealed class SystemControlInfo
    {
        /// <summary>
        /// Application title (default is "CtrApp")
        /// </summary>
        public string ApplicationTitle;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved1;

        /// <summary>
        /// Flag (bit 0: CompressExefsCode, bit 1: SDApplication)
        /// </summary>
        public byte Flag;

        /// <summary>
        /// Remaster version
        /// </summary>
        public ushort RemasterVersion;

        /// <summary>
        /// Text code set info
        /// </summary>
        public CodeSetInfo TextCodeSetInfo;

        /// <summary>
        /// Stack size
        /// </summary>
        public uint StackSize;

        /// <summary>
        /// Read-only code set info
        /// </summary>
        public CodeSetInfo ReadOnlyCodeSetInfo;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved2;

        /// <summary>
        /// Data code set info
        /// </summary>
        public CodeSetInfo DataCodeSetInfo;

        /// <summary>
        /// BSS size
        /// </summary>
        public uint BSSSize;

        /// <summary>
        /// Dependency module (program ID) list
        /// </summary>
        public ulong[] DependencyModuleList;

        /// <summary>
        /// SystemInfo
        /// </summary>
        public SystemInfo SystemInfo;
    }
}
