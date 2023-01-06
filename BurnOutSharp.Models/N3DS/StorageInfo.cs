namespace BurnOutSharp.Models.N3DS
{
    /// <summary>
    /// Used in FSReg:Register.
    /// </summary>
    /// <see href="https://www.3dbrew.org/wiki/NCCH/Extended_Header#Storage_Info"/>
    public sealed class StorageInfo
    {
        /// <summary>
        /// Extdata ID
        /// </summary>
        public byte[] ExtdataID;

        /// <summary>
        /// System savedata IDs
        /// </summary>
        public byte[] SystemSavedataIDs;

        /// <summary>
        /// Storage accessible unique IDs
        /// </summary>
        public byte[] StorageAccessibleUniqueIDs;

        /// <summary>
        /// Filesystem access info
        /// </summary>
        public byte[] FilesystemAccessInfo;

        /// <summary>
        /// Other attributes
        /// </summary>
        public StorageInfoOtherAttributes OtherAttributes;
    }
}
