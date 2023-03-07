using System;

namespace BinaryObjectScanner.Models.GCF
{
    [Flags]
    public enum HL_GCF_FLAG : uint
    {
        /// <summary>
        /// Folder
        /// </summary>
        HL_GCF_FLAG_FOLDER = 0x00000000,

        /// <summary>
        /// Don't overwrite the item if copying it to the disk and the item already exists.
        /// </summary>
        HL_GCF_FLAG_COPY_LOCAL_NO_OVERWRITE = 0x00000001,

        /// <summary>
        /// The item is to be copied to the disk.
        /// </summary>
        HL_GCF_FLAG_COPY_LOCAL = 0x0000000A,

        /// <summary>
        /// Backup the item before overwriting it.
        /// </summary>
        HL_GCF_FLAG_BACKUP_LOCAL = 0x00000040,

        /// <summary>
        /// The item is encrypted.
        /// </summary>
        HL_GCF_FLAG_ENCRYPTED = 0x00000100,

        /// <summary>
        /// The item is a file.
        /// </summary>
        HL_GCF_FLAG_FILE = 0x00004000,
    }
}