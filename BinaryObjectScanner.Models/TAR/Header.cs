namespace BinaryObjectScanner.Models.TAR
{
    public sealed class Header
    {
        /// <summary>
        /// File name
        /// </summary>
        public string FileName;

        /// <summary>
        /// File mode
        /// </summary>
        public Mode Mode;

         /// <summary>
        /// Owner's numeric user ID
        /// </summary>
        public uint UID;

         /// <summary>
        /// Owner's numeric user ID
        /// </summary>
        public uint GID;

        /// <summary>
        /// File size in bytes
        /// </summary>
        public ulong Size;

        /// <summary>
        /// Last modification time in numeric Unix time format
        /// </summary>
        public ulong ModifiedTime;

        /// <summary>
        /// Checksum for header record
        /// </summary>
        public ushort Checksum;

        /// <summary>
        /// Link indicator (file type) / Type flag
        /// </summary>
        public TypeFlag TypeFlag;

        /// <summary>
        /// Name of linked file
        /// </summary>
        public string LinkName;

        /// <summary>
        /// UStar indicator, "ustar", then NUL
        /// </summary>
        public string Magic;

        /// <summary>
        /// UStar version, "00"
        /// </summary>
        public string Version;

        /// <summary>
        /// Owner user name
        /// </summary>
        public string UserName;

        /// <summary>
        /// Owner group name
        /// </summary>
        public string GroupName;

        /// <summary>
        /// Device major number
        /// </summary>
        public string DevMajor;

        /// <summary>
        /// Device minor number
        /// </summary>
        public string DevMinor;

        /// <summary>
        /// Filename prefix
        /// </summary>
        public string Prefix;
    }
}