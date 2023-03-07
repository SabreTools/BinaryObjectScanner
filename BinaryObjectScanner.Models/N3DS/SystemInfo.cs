namespace BinaryObjectScanner.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCCH/Extended_Header#System_Info"/>
    public sealed class SystemInfo
    {
        /// <summary>
        /// SaveData Size
        /// </summary>
        public ulong SaveDataSize;

        /// <summary>
        /// Jump ID
        /// </summary>
        public ulong JumpID;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved;
    }
}
