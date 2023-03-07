namespace BinaryObjectScanner.Models.N3DS
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
        public ulong ExtdataID;

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
        /// TODO: Create enum for the flag values
        /// TODO: Combine with "other attributes"
        public byte[] FileSystemAccessInfo;

        /// <summary>
        /// Other attributes
        /// </summary>
        public StorageInfoOtherAttributes OtherAttributes;
    }
}
