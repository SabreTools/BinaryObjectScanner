using System.Collections.Generic;

namespace BurnOutSharp.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/internal.h"/>
    public sealed class Cabinet
    {
        #region Headers

        /// <summary>
        /// Common header
        /// </summary>
        public CommonHeader CommonHeader { get; set; }

        /// <summary>
        /// Volume header
        /// </summary>
        public VolumeHeader VolumeHeader { get; set; }

        /// <summary>
        /// Cabinet descriptor
        /// </summary>
        public CabDescriptor CabinetDescriptor { get; set; }

        #endregion

        #region File Descriptors

        /// <summary>
        /// Offsets to all file descriptors
        /// </summary>
        public uint[] FileDescriptorOffsets { get; set; }

        /// <summary>
        /// Directory file descriptors
        /// </summary>
        public FileDescriptor[] DirectoryDescriptors { get; set; }

        /// <summary>
        /// Standard file descriptors
        /// </summary>
        public FileDescriptor[] FileDescriptors { get; set; }

        #endregion

        #region File Groups

        /// <summary>
        /// File group offset to offset list mapping
        /// </summary>
        public Dictionary<long, OffsetList> FileGroupOffsets { get; set; }

        /// <summary>
        /// File groups
        /// </summary>
        public FileGroup[] FileGroups { get; set; }

        #endregion

        #region Components

        /// <summary>
        /// Component offset to offset list mapping
        /// </summary>
        public Dictionary<long, OffsetList> ComponentOffsets { get; set; }

        /// <summary>
        /// Components
        /// </summary>
        public Component[] Components { get; set; }

        #endregion
    }
}