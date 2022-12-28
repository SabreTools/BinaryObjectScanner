using System.Collections.Generic;

namespace BurnOutSharp.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/internal.h"/>
    public sealed class Header
    {
        // TODO: Move to wrapper, when exists
        public int MajorVersion
        {
            get
            {
                uint majorVersion = CommonHeader.Version;
                if (majorVersion >> 24 == 1)
                {
                    majorVersion = (majorVersion >> 12) & 0x0F;
                }
                else if (majorVersion >> 24 == 2 || majorVersion >> 24 == 4)
                {
                    majorVersion = majorVersion & 0xFFFF;
                    if (majorVersion != 0)
                        majorVersion /= 100;
                }

                return (int)majorVersion;
            }
        }

        #region Headers

        public CommonHeader CommonHeader { get; set; }

        public CabDescriptor CabinetDescriptor { get; set; }

        #endregion

        #region File Descriptors

        public uint[] FileDescriptorOffsets { get; set; }

        public FileDescriptor[] DirectoryDescriptors { get; set; }

        public FileDescriptor[] FileDescriptors { get; set; }

        #endregion

        #region File Groups

        public Dictionary<long, OffsetList> FileGroupOffsets { get; set; }

        public FileGroup[] FileGroups { get; set; }

        #endregion

        #region Components

        public Dictionary<long, OffsetList> ComponentOffsets { get; set; }

        public Component[] Components { get; set; }

        #endregion
    }
}