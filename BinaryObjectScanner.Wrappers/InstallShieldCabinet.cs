using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public partial class InstallShieldCabinet : WrapperBase<SabreTools.Models.InstallShieldCabinet.Cabinet>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "InstallShield Cabinet";

        #endregion

        #region Pass-Through Properties

        #region Common Header

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.Signature"/>
#if NET48
        public string Signature => this.Model.CommonHeader.Signature;
#else
        public string? Signature => this.Model.CommonHeader?.Signature;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.Version"/>
#if NET48
        public uint Version => this.Model.CommonHeader.Version;
#else
        public uint? Version => this.Model.CommonHeader?.Version;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.VolumeInfo"/>
#if NET48
        public uint VolumeInfo => this.Model.CommonHeader.VolumeInfo;
#else
        public uint? VolumeInfo => this.Model.CommonHeader?.VolumeInfo;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.DescriptorOffset"/>
#if NET48
        public uint DescriptorOffset => this.Model.CommonHeader.DescriptorOffset;
#else
        public uint? DescriptorOffset => this.Model.CommonHeader?.DescriptorOffset;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.CommonHeader.DescriptorSize"/>
#if NET48
        public uint DescriptorSize => this.Model.CommonHeader.DescriptorSize;
#else
        public uint? DescriptorSize => this.Model.CommonHeader?.DescriptorSize;
#endif

        #endregion

        #region Volume Header

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.DataOffset"/>
#if NET48
        public uint DataOffset => this.Model.VolumeHeader.DataOffset;
#else
        public uint? DataOffset => this.Model.VolumeHeader?.DataOffset;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.DataOffsetHigh"/>
#if NET48
        public uint DataOffsetHigh => this.Model.VolumeHeader.DataOffsetHigh;
#else
        public uint? DataOffsetHigh => this.Model.VolumeHeader?.DataOffsetHigh;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileIndex"/>
#if NET48
        public uint FirstFileIndex => this.Model.VolumeHeader.FirstFileIndex;
#else
        public uint? FirstFileIndex => this.Model.VolumeHeader?.FirstFileIndex;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileIndex"/>
#if NET48
        public uint LastFileIndex => this.Model.VolumeHeader.LastFileIndex;
#else
        public uint? LastFileIndex => this.Model.VolumeHeader?.LastFileIndex;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileOffset"/>
#if NET48
        public uint FirstFileOffset => this.Model.VolumeHeader.FirstFileOffset;
#else
        public uint? FirstFileOffset => this.Model.VolumeHeader?.FirstFileOffset;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileOffsetHigh"/>
#if NET48
        public uint FirstFileOffsetHigh => this.Model.VolumeHeader.FirstFileOffsetHigh;
#else
        public uint? FirstFileOffsetHigh => this.Model.VolumeHeader?.FirstFileOffsetHigh;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileSizeExpanded"/>
#if NET48
        public uint FirstFileSizeExpanded => this.Model.VolumeHeader.FirstFileSizeExpanded;
#else
        public uint? FirstFileSizeExpanded => this.Model.VolumeHeader?.FirstFileSizeExpanded;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileSizeExpandedHigh"/>
#if NET48
        public uint FirstFileSizeExpandedHigh => this.Model.VolumeHeader.FirstFileSizeExpandedHigh;
#else
        public uint? FirstFileSizeExpandedHigh => this.Model.VolumeHeader?.FirstFileSizeExpandedHigh;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileSizeCompressed"/>
#if NET48
        public uint FirstFileSizeCompressed => this.Model.VolumeHeader.FirstFileSizeCompressed;
#else
        public uint? FirstFileSizeCompressed => this.Model.VolumeHeader?.FirstFileSizeCompressed;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.FirstFileSizeCompressedHigh"/>
#if NET48
        public uint FirstFileSizeCompressedHigh => this.Model.VolumeHeader.FirstFileSizeCompressedHigh;
#else
        public uint? FirstFileSizeCompressedHigh => this.Model.VolumeHeader?.FirstFileSizeCompressedHigh;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileOffset"/>
#if NET48
        public uint LastFileOffset => this.Model.VolumeHeader.LastFileOffset;
#else
        public uint? LastFileOffset => this.Model.VolumeHeader?.LastFileOffset;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileOffsetHigh"/>
#if NET48
        public uint LastFileOffsetHigh => this.Model.VolumeHeader.LastFileOffsetHigh;
#else
        public uint? LastFileOffsetHigh => this.Model.VolumeHeader?.LastFileOffsetHigh;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileSizeExpanded"/>
#if NET48
        public uint LastFileSizeExpanded => this.Model.VolumeHeader.LastFileSizeExpanded;
#else
        public uint? LastFileSizeExpanded => this.Model.VolumeHeader?.LastFileSizeExpanded;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileSizeExpandedHigh"/>
#if NET48
        public uint LastFileSizeExpandedHigh => this.Model.VolumeHeader.LastFileSizeExpandedHigh;
#else
        public uint? LastFileSizeExpandedHigh => this.Model.VolumeHeader?.LastFileSizeExpandedHigh;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileSizeCompressed"/>
#if NET48
        public uint LastFileSizeCompressed => this.Model.VolumeHeader.LastFileSizeCompressed;
#else
        public uint? LastFileSizeCompressed => this.Model.VolumeHeader?.LastFileSizeCompressed;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.VolumeHeader.LastFileSizeCompressedHigh"/>
#if NET48
        public uint LastFileSizeCompressedHigh => this.Model.VolumeHeader.LastFileSizeCompressedHigh;
#else
        public uint? LastFileSizeCompressedHigh => this.Model.VolumeHeader?.LastFileSizeCompressedHigh;
#endif

        #endregion

        #region Descriptor

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.StringsOffset"/>
#if NET48
        public uint StringsOffset => this.Model.Descriptor.StringsOffset;
#else
        public uint? StringsOffset => this.Model.Descriptor?.StringsOffset;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved0"/>
#if NET48
        public byte[] Reserved0 => this.Model.Descriptor.Reserved0;
#else
        public byte[]? Reserved0 => this.Model.Descriptor?.Reserved0;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.ComponentListOffset"/>
#if NET48
        public uint ComponentListOffset => this.Model.Descriptor.ComponentListOffset;
#else
        public uint? ComponentListOffset => this.Model.Descriptor?.ComponentListOffset;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.FileTableOffset"/>
#if NET48
        public uint FileTableOffset => this.Model.Descriptor.FileTableOffset;
#else
        public uint? FileTableOffset => this.Model.Descriptor?.FileTableOffset;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved1"/>
#if NET48
        public byte[] Reserved1 => this.Model.Descriptor.Reserved1;
#else
        public byte[]? Reserved1 => this.Model.Descriptor?.Reserved1;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.FileTableSize"/>
#if NET48
        public uint FileTableSize => this.Model.Descriptor.FileTableSize;
#else
        public uint? FileTableSize => this.Model.Descriptor?.FileTableSize;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.FileTableSize2"/>
#if NET48
        public uint FileTableSize2 => this.Model.Descriptor.FileTableSize2;
#else
        public uint? FileTableSize2 => this.Model.Descriptor?.FileTableSize2;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.DirectoryCount"/>
#if NET48
        public ushort DirectoryCount => this.Model.Descriptor.DirectoryCount;
#else
        public ushort? DirectoryCount => this.Model.Descriptor?.DirectoryCount;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved2"/>
#if NET48
        public byte[] Reserved2 => this.Model.Descriptor.Reserved2;
#else
        public byte[]? Reserved2 => this.Model.Descriptor?.Reserved2;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved3"/>
#if NET48
        public byte[] Reserved3 => this.Model.Descriptor.Reserved3;
#else
        public byte[]? Reserved3 => this.Model.Descriptor?.Reserved3;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved4"/>
#if NET48
        public byte[] Reserved4 => this.Model.Descriptor.Reserved4;
#else
        public byte[]? Reserved4 => this.Model.Descriptor?.Reserved4;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.FileCount"/>
#if NET48
        public uint FileCount => this.Model.Descriptor.FileCount;
#else
        public uint? FileCount => this.Model.Descriptor?.FileCount;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.FileTableOffset2"/>
#if NET48
        public uint FileTableOffset2 => this.Model.Descriptor.FileTableOffset2;
#else
        public uint? FileTableOffset2 => this.Model.Descriptor?.FileTableOffset2;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.ComponentTableInfoCount"/>
#if NET48
        public ushort ComponentTableInfoCount => this.Model.Descriptor.ComponentTableInfoCount;
#else
        public ushort? ComponentTableInfoCount => this.Model.Descriptor?.ComponentTableInfoCount;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.ComponentTableOffset"/>
#if NET48
        public uint ComponentTableOffset => this.Model.Descriptor.ComponentTableOffset;
#else
        public uint? ComponentTableOffset => this.Model.Descriptor?.ComponentTableOffset;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved5"/>
#if NET48
        public byte[] Reserved5 => this.Model.Descriptor.Reserved5;
#else
        public byte[]? Reserved5 => this.Model.Descriptor?.Reserved5;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved6"/>
#if NET48
        public byte[] Reserved6 => this.Model.Descriptor.Reserved6;
#else
        public byte[]? Reserved6 => this.Model.Descriptor?.Reserved6;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.FileGroupOffsets"/>
#if NET48
        public uint[] D_FileGroupOffsets => this.Model.Descriptor.FileGroupOffsets;
#else
        public uint[]? D_FileGroupOffsets => this.Model.Descriptor?.FileGroupOffsets;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.ComponentOffsets"/>
#if NET48
        public uint[] D_ComponentOffsets => this.Model.Descriptor.ComponentOffsets;
#else
        public uint[]? D_ComponentOffsets => this.Model.Descriptor?.ComponentOffsets;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.SetupTypesOffset"/>
#if NET48
        public uint SetupTypesOffset => this.Model.Descriptor.SetupTypesOffset;
#else
        public uint? SetupTypesOffset => this.Model.Descriptor?.SetupTypesOffset;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.SetupTableOffset"/>
#if NET48
        public uint SetupTableOffset => this.Model.Descriptor.SetupTableOffset;
#else
        public uint? SetupTableOffset => this.Model.Descriptor?.SetupTableOffset;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved7"/>
#if NET48
        public byte[] Reserved7 => this.Model.Descriptor.Reserved7;
#else
        public byte[]? Reserved7 => this.Model.Descriptor?.Reserved7;
#endif

        /// <inheritdoc cref="Models.InstallShieldCabinet.Descriptor.Reserved8"/>
#if NET48
        public byte[] Reserved8 => this.Model.Descriptor.Reserved8;
#else
        public byte[]? Reserved8 => this.Model.Descriptor?.Reserved8;
#endif

        #endregion

        #region File Descriptor Offsets

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.FileDescriptorOffsets"/>
#if NET48
        public uint[] FileDescriptorOffsets => this.Model.FileDescriptorOffsets;
#else
        public uint[]? FileDescriptorOffsets => this.Model.FileDescriptorOffsets;
#endif

        #endregion

        #region Directory Descriptors

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.DirectoryNames"/>
#if NET48
        public string[] DirectoryNames => this.Model.DirectoryNames;
#else
        public string[]? DirectoryNames => this.Model.DirectoryNames;
#endif

        #endregion

        #region File Descriptors

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.FileDescriptors"/>
#if NET48
        public SabreTools.Models.InstallShieldCabinet.FileDescriptor[] FileDescriptors => this.Model.FileDescriptors;
#else
        public SabreTools.Models.InstallShieldCabinet.FileDescriptor?[]? FileDescriptors => this.Model.FileDescriptors;
#endif

        #endregion

        #region File Group Offsets

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.FileGroupOffsets"/>
#if NET48
        public Dictionary<long, SabreTools.Models.InstallShieldCabinet.OffsetList> FileGroupOffsets => this.Model.FileGroupOffsets;
#else
        public Dictionary<long, SabreTools.Models.InstallShieldCabinet.OffsetList?>? FileGroupOffsets => this.Model.FileGroupOffsets;
#endif

        #endregion

        #region File Groups

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.FileGroups"/>
#if NET48
        public SabreTools.Models.InstallShieldCabinet.FileGroup[] FileGroups => this.Model.FileGroups;
#else
        public SabreTools.Models.InstallShieldCabinet.FileGroup?[]? FileGroups => this.Model.FileGroups;
#endif

        #endregion

        #region Component Offsets

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.ComponentOffsets"/>
#if NET48
        public Dictionary<long, SabreTools.Models.InstallShieldCabinet.OffsetList> ComponentOffsets => this.Model.ComponentOffsets;
#else
        public Dictionary<long, SabreTools.Models.InstallShieldCabinet.OffsetList?>? ComponentOffsets => this.Model.ComponentOffsets;
#endif

        #endregion

        #region Components

        /// <inheritdoc cref="Models.InstallShieldCabinet.Cabinet.Components"/>
#if NET48
        public SabreTools.Models.InstallShieldCabinet.Component[] Components => this.Model.Components;
#else
        public SabreTools.Models.InstallShieldCabinet.Component?[]? Components => this.Model.Components;
#endif

        #endregion

        #endregion

        #region Extension Properties

        /// <summary>
        /// The major version of the cabinet
        /// </summary>
        public int MajorVersion
        {
            get
            {
#if NET48
                uint majorVersion = Version;
#else
                uint majorVersion = Version ?? 0;
#endif
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

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public InstallShieldCabinet(SabreTools.Models.InstallShieldCabinet.Cabinet model, byte[] data, int offset)
#else
        public InstallShieldCabinet(SabreTools.Models.InstallShieldCabinet.Cabinet? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public InstallShieldCabinet(SabreTools.Models.InstallShieldCabinet.Cabinet model, Stream data)
#else
        public InstallShieldCabinet(SabreTools.Models.InstallShieldCabinet.Cabinet? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an InstallShield Cabinet from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the cabinet</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
#if NET48
        public static InstallShieldCabinet Create(byte[] data, int offset)
#else
        public static InstallShieldCabinet? Create(byte[]? data, int offset)
#endif
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a InstallShield Cabinet from a Stream
        /// </summary>
        /// <param name="data">Stream representing the cabinet</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
#if NET48
        public static InstallShieldCabinet Create(Stream data)
#else
        public static InstallShieldCabinet? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var cabinet = new SabreTools.Serialization.Streams.InstallShieldCabinet().Deserialize(data);
            if (cabinet == null)
                return null;

            try
            {
                return new InstallShieldCabinet(cabinet, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();
            Printing.InstallShieldCabinet.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}
