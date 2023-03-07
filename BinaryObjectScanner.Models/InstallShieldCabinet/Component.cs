using System;

namespace BinaryObjectScanner.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/libunshield.h"/>
    public sealed class Component
    {
        /// <summary>
        /// Offset to the component identifier
        /// </summary>
        public uint IdentifierOffset;

        /// <summary>
        /// Component identifier
        /// </summary>
        public string Identifier;

        /// <summary>
        /// Offset to the component descriptor
        /// </summary>
        public uint DescriptorOffset;

        /// <summary>
        /// Offset to the display name
        /// </summary>
        public uint DisplayNameOffset;

        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved0;

        /// <summary>
        /// Reserved offset
        /// </summary>
        public uint ReservedOffset0;

        /// <summary>
        /// Reserved offset
        /// </summary>
        public uint ReservedOffset1;

        /// <summary>
        /// Component index
        /// </summary>
        public ushort ComponentIndex;

        /// <summary>
        /// Offset to the component name
        /// </summary>
        public uint NameOffset;

        /// <summary>
        /// Component name
        /// </summary>
        public string Name;

        /// <summary>
        /// Reserved offset
        /// </summary>
        public uint ReservedOffset2;

        /// <summary>
        /// Reserved offset
        /// </summary>
        public uint ReservedOffset3;

        /// <summary>
        /// Reserved offset
        /// </summary>
        public uint ReservedOffset4;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved1;

        /// <summary>
        /// Offset to the component CLSID
        /// </summary>
        public uint CLSIDOffset;

        /// <summary>
        /// Component CLSID
        /// </summary>
        public Guid CLSID;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved2;

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved3;

        /// <summary>
        /// Number of depends(?)
        /// </summary>
        public ushort DependsCount;

        /// <summary>
        /// Offset to depends(?)
        /// </summary>
        public uint DependsOffset;

        /// <summary>
        /// Number of file groups
        /// </summary>
        public uint FileGroupCount;

        /// <summary>
        /// Offset to the file group names
        /// </summary>
        public uint FileGroupNamesOffset;

        /// <summary>
        /// File group names
        /// </summary>
        public string[] FileGroupNames;

        /// <summary>
        /// Number of X3(?)
        /// </summary>
        public ushort X3Count;

        /// <summary>
        /// Offset to X3(?)
        /// </summary>
        public uint X3Offset;

        /// <summary>
        /// Number of sub-components
        /// </summary>
        public ushort SubComponentsCount;

        /// <summary>
        /// Offset to the sub-components
        /// </summary>
        public uint SubComponentsOffset;

        /// <summary>
        /// Offset to the next component
        /// </summary>
        public uint NextComponentOffset;

        /// <summary>
        /// Reserved offset
        /// </summary>
        public uint ReservedOffset5;

        /// <summary>
        /// Reserved offset
        /// </summary>
        public uint ReservedOffset6;

        /// <summary>
        /// Reserved offset
        /// </summary>
        public uint ReservedOffset7;

        /// <summary>
        /// Reserved offset
        /// </summary>
        public uint ReservedOffset8;
    }
}