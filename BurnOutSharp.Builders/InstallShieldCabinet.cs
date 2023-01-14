using System.Collections.Generic;
using System.IO;
using System.Text;
using BurnOutSharp.Models.InstallShieldCabinet;
using BurnOutSharp.Utilities;
using static BurnOutSharp.Models.InstallShieldCabinet.Constants;

namespace BurnOutSharp.Builders
{
    // TODO: Add multi-cabinet reading
    public class InstallShieldCabinet
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a InstallShield Cabinet file
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled cabinet on success, null on error</returns>
        public static Cabinet ParseCabinet(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseCabinet(dataStream);
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a InstallShield Cabinet file
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled cabinet on success, null on error</returns>
        public static Cabinet ParseCabinet(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new cabinet to fill
            var cabinet = new Cabinet();

            #region Common Header

            // Try to parse the cabinet header
            var commonHeader = ParseCommonHeader(data);
            if (commonHeader == null)
                return null;

            // Set the cabinet header
            cabinet.CommonHeader = commonHeader;

            #endregion

            #region Volume Header

            // Try to parse the volume header
            var volumeHeader = ParseVolumeHeader(data, GetMajorVersion(commonHeader));
            if (volumeHeader == null)
                return null;

            // Set the volume header
            cabinet.VolumeHeader = volumeHeader;

            #endregion

            #region Cabinet Descriptor

            // Get the cabinet descriptor offset
            uint cabinetDescriptorOffset = commonHeader.CabDescriptorOffset;
            if (cabinetDescriptorOffset < 0 || cabinetDescriptorOffset >= data.Length)
                return null;

            // Seek to the cabinet descriptor
            data.Seek(cabinetDescriptorOffset, SeekOrigin.Begin);

            // Try to parse the cabinet descriptor
            var cabinetDescriptor = ParseCabinetDescriptor(data);
            if (cabinetDescriptor == null)
                return null;

            // Set the cabinet descriptor
            cabinet.CabinetDescriptor = cabinetDescriptor;

            #endregion

            #region File Descriptor Offsets

            // Get the file table offset
            uint fileTableOffset = commonHeader.CabDescriptorOffset + cabinetDescriptor.FileTableOffset;
            if (fileTableOffset < 0 || fileTableOffset >= data.Length)
                return null;

            // Seek to the file table
            data.Seek(fileTableOffset, SeekOrigin.Begin);

            // Get the number of file table items
            uint fileTableItems;
            if (GetMajorVersion(commonHeader) <= 5)
                fileTableItems = cabinetDescriptor.DirectoryCount + cabinetDescriptor.FileCount;
            else
                fileTableItems = cabinetDescriptor.DirectoryCount;

            // Create and fill the file table
            cabinet.FileDescriptorOffsets = new uint[fileTableItems];
            for (int i = 0; i < cabinet.FileDescriptorOffsets.Length; i++)
            {
                cabinet.FileDescriptorOffsets[i] = data.ReadUInt32();
            }

            #endregion

            #region Directory Descriptors

            // Create and fill the directory descriptors
            cabinet.DirectoryDescriptors = new FileDescriptor[cabinetDescriptor.DirectoryCount];
            for (int i = 0; i < cabinetDescriptor.DirectoryCount; i++)
            {
                // Get the directory descriptor offset
                uint offset = cabinetDescriptorOffset
                    + cabinetDescriptor.FileTableOffset
                    + cabinet.FileDescriptorOffsets[i];

                // If we have an invalid offset
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the file descriptor offset
                data.Seek(offset, SeekOrigin.Begin);

                // Create and add the file descriptor
                FileDescriptor directoryDescriptor = ParseDirectoryDescriptor(data, GetMajorVersion(commonHeader));
                cabinet.DirectoryDescriptors[i] = directoryDescriptor;
            }

            #endregion

            #region File Descriptors

            // Create and fill the file descriptors
            cabinet.FileDescriptors = new FileDescriptor[cabinetDescriptor.FileCount];
            for (int i = 0; i < cabinetDescriptor.FileCount; i++)
            {
                // Get the file descriptor offset
                uint offset;
                if (GetMajorVersion(commonHeader) <= 5)
                {
                    offset = cabinetDescriptorOffset
                        + cabinetDescriptor.FileTableOffset
                        + cabinet.FileDescriptorOffsets[cabinetDescriptor.DirectoryCount + i];
                }
                else
                {
                    offset = cabinetDescriptorOffset
                        + cabinetDescriptor.FileTableOffset
                        + cabinetDescriptor.FileTableOffset2
                        + (uint)(i * 0x57);
                }

                // If we have an invalid offset
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the file descriptor offset
                data.Seek(offset, SeekOrigin.Begin);

                // Create and add the file descriptor
                FileDescriptor fileDescriptor = ParseFileDescriptor(data, GetMajorVersion(commonHeader), cabinetDescriptorOffset + cabinetDescriptor.FileTableOffset);
                cabinet.FileDescriptors[i] = fileDescriptor;
            }

            #endregion

            #region File Group Offsets

            // Create and fill the file group offsets
            cabinet.FileGroupOffsets = new Dictionary<long, OffsetList>();
            for (int i = 0; i < cabinetDescriptor.FileGroupOffsets.Length; i++)
            {
                // Get the file group offset
                uint offset = cabinetDescriptor.FileGroupOffsets[i];
                if (offset == 0)
                    continue;

                // Adjust the file group offset
                offset += commonHeader.CabDescriptorOffset;
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the file group offset
                data.Seek(offset, SeekOrigin.Begin);

                // Create and add the offset
                OffsetList offsetList = ParseOffsetList(data, GetMajorVersion(commonHeader), cabinetDescriptorOffset);
                cabinet.FileGroupOffsets[cabinetDescriptor.FileGroupOffsets[i]] = offsetList;

                // If we have a nonzero next offset
                uint nextOffset = offsetList.NextOffset;
                while (nextOffset != 0)
                {
                    // Get the next offset to read
                    uint internalOffset = nextOffset + commonHeader.CabDescriptorOffset;

                    // Seek to the file group offset
                    data.Seek(internalOffset, SeekOrigin.Begin);

                    // Create and add the offset
                    offsetList = ParseOffsetList(data, GetMajorVersion(commonHeader), cabinetDescriptorOffset);
                    cabinet.FileGroupOffsets[nextOffset] = offsetList;

                    // Set the next offset
                    nextOffset = offsetList.NextOffset;
                }
            }

            #endregion

            #region File Groups

            // Create and fill the file groups
            List<FileGroup> fileGroups = new List<FileGroup>();
            foreach (var kvp in cabinet.FileGroupOffsets)
            {
                // Get the offset
                OffsetList list = kvp.Value;
                if (list == null)
                    continue;

                /// Seek to the file group
                data.Seek(list.DescriptorOffset + cabinetDescriptorOffset, SeekOrigin.Begin);

                // Try to parse the file group
                FileGroup fileGroup = ParseFileGroup(data, GetMajorVersion(commonHeader), cabinetDescriptorOffset);

                // Add the file group
                fileGroups.Add(fileGroup);
            }

            // Set the file groups
            cabinet.FileGroups = fileGroups.ToArray();

            #endregion

            #region Component Offsets

            // Create and fill the component offsets
            cabinet.ComponentOffsets = new Dictionary<long, OffsetList>();
            for (int i = 0; i < cabinetDescriptor.ComponentOffsets.Length; i++)
            {
                // Get the component offset
                uint offset = cabinetDescriptor.ComponentOffsets[i];
                if (offset == 0)
                    continue;

                // Adjust the component offset
                offset += commonHeader.CabDescriptorOffset;
                if (offset < 0 || offset >= data.Length)
                    continue;

                // Seek to the component offset
                data.Seek(offset, SeekOrigin.Begin);

                // Create and add the offset
                OffsetList offsetList = ParseOffsetList(data, GetMajorVersion(commonHeader), cabinetDescriptorOffset);
                cabinet.ComponentOffsets[cabinetDescriptor.ComponentOffsets[i]] = offsetList;

                // If we have a nonzero next offset
                uint nextOffset = offsetList.NextOffset;
                while (nextOffset != 0)
                {
                    // Get the next offset to read
                    uint internalOffset = nextOffset + commonHeader.CabDescriptorOffset;

                    // Seek to the file group offset
                    data.Seek(internalOffset, SeekOrigin.Begin);

                    // Create and add the offset
                    offsetList = ParseOffsetList(data, GetMajorVersion(commonHeader), cabinetDescriptorOffset);
                    cabinet.ComponentOffsets[nextOffset] = offsetList;

                    // Set the next offset
                    nextOffset = offsetList.NextOffset;
                }
            }

            #endregion

            #region Components

            // Create and fill the components
            List<Component> components = new List<Component>();
            foreach (KeyValuePair<long, OffsetList> kvp in cabinet.ComponentOffsets)
            {
                // Get the offset
                OffsetList list = kvp.Value;
                if (list == null)
                    continue;

                // Seek to the component
                data.Seek(list.DescriptorOffset + cabinetDescriptorOffset, SeekOrigin.Begin);

                // Try to parse the component
                Component component = ParseComponent(data, GetMajorVersion(commonHeader), cabinetDescriptorOffset);

                // Add the component
                components.Add(component);
            }

            // Set the components
            cabinet.Components = components.ToArray();

            #endregion

            return cabinet;
        }

        /// <summary>
        /// Parse a Stream into a common header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled common header on success, null on error</returns>
        private static CommonHeader ParseCommonHeader(Stream data)
        {
            CommonHeader commonHeader = new CommonHeader();

            byte[] signature = data.ReadBytes(4);
            commonHeader.Signature = Encoding.ASCII.GetString(signature);
            if (commonHeader.Signature != SignatureString)
                return null;

            commonHeader.Version = data.ReadUInt32();
            commonHeader.VolumeInfo = data.ReadUInt32();
            commonHeader.CabDescriptorOffset = data.ReadUInt32();
            commonHeader.CabDescriptorSize = data.ReadUInt32();

            return commonHeader;
        }

        /// <summary>
        /// Parse a Stream into a volume header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">Major version of the cabinet</param>
        /// <returns>Filled volume header on success, null on error</returns>
        private static VolumeHeader ParseVolumeHeader(Stream data, int majorVersion)
        {
            VolumeHeader volumeHeader = new VolumeHeader();

            // Read the descriptor based on version
            if (majorVersion <= 5)
            {
                volumeHeader.DataOffset = data.ReadUInt32();
                _ = data.ReadBytes(0x04); // Skip 0x04 bytes, unknown data?
                volumeHeader.FirstFileIndex = data.ReadUInt32();
                volumeHeader.LastFileIndex = data.ReadUInt32();
                volumeHeader.FirstFileOffset = data.ReadUInt32();
                volumeHeader.FirstFileSizeExpanded = data.ReadUInt32();
                volumeHeader.FirstFileSizeCompressed = data.ReadUInt32();
                volumeHeader.LastFileOffset = data.ReadUInt32();
                volumeHeader.LastFileSizeExpanded = data.ReadUInt32();
                volumeHeader.LastFileSizeCompressed = data.ReadUInt32();
            }
            else
            {
                // TODO: Should standard and high values be combined?
                volumeHeader.DataOffset = data.ReadUInt32();
                volumeHeader.DataOffsetHigh = data.ReadUInt32();
                volumeHeader.FirstFileIndex = data.ReadUInt32();
                volumeHeader.LastFileIndex = data.ReadUInt32();
                volumeHeader.FirstFileOffset = data.ReadUInt32();
                volumeHeader.FirstFileOffsetHigh = data.ReadUInt32();
                volumeHeader.FirstFileSizeExpanded = data.ReadUInt32();
                volumeHeader.FirstFileSizeExpandedHigh = data.ReadUInt32();
                volumeHeader.FirstFileSizeCompressed = data.ReadUInt32();
                volumeHeader.FirstFileSizeCompressedHigh = data.ReadUInt32();
                volumeHeader.LastFileOffset = data.ReadUInt32();
                volumeHeader.LastFileOffsetHigh = data.ReadUInt32();
                volumeHeader.LastFileSizeExpanded = data.ReadUInt32();
                volumeHeader.LastFileSizeExpandedHigh = data.ReadUInt32();
                volumeHeader.LastFileSizeCompressed = data.ReadUInt32();
                volumeHeader.LastFileSizeCompressedHigh = data.ReadUInt32();
            }

            return volumeHeader;
        }

        /// <summary>
        /// Parse a Stream into a cabinet descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled cabinet descriptor on success, null on error</returns>
        private static CabDescriptor ParseCabinetDescriptor(Stream data)
        {
            CabDescriptor cabDescriptor = new CabDescriptor();

            cabDescriptor.Reserved0 = data.ReadBytes(0x0C);
            cabDescriptor.FileTableOffset = data.ReadUInt32();
            cabDescriptor.Reserved1 = data.ReadBytes(0x04);
            cabDescriptor.FileTableSize = data.ReadUInt32();
            cabDescriptor.FileTableSize2 = data.ReadUInt32();
            cabDescriptor.DirectoryCount = data.ReadUInt32();
            cabDescriptor.Reserved2 = data.ReadBytes(0x08);
            cabDescriptor.FileCount = data.ReadUInt32();
            cabDescriptor.FileTableOffset2 = data.ReadUInt32();
            cabDescriptor.Reserved3 = data.ReadBytes(0x0E);

            cabDescriptor.FileGroupOffsets = new uint[MAX_FILE_GROUP_COUNT];
            for (int i = 0; i < cabDescriptor.FileGroupOffsets.Length; i++)
            {
                cabDescriptor.FileGroupOffsets[i] = data.ReadUInt32();
            }

            cabDescriptor.ComponentOffsets = new uint[MAX_COMPONENT_COUNT];
            for (int i = 0; i < cabDescriptor.ComponentOffsets.Length; i++)
            {
                cabDescriptor.ComponentOffsets[i] = data.ReadUInt32();
            }

            return cabDescriptor;
        }

        /// <summary>
        /// Parse a Stream into an offset list
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">Major version of the cabinet</param>
        /// <param name="descriptorOffset">Offset of the cabinet descriptor</param>
        /// <returns>Filled offset list on success, null on error</returns>
        private static OffsetList ParseOffsetList(Stream data, int majorVersion, uint descriptorOffset)
        {
            OffsetList offsetList = new OffsetList();

            offsetList.NameOffset = data.ReadUInt32();
            offsetList.DescriptorOffset = data.ReadUInt32();
            offsetList.NextOffset = data.ReadUInt32();

            // Cache the current offset
            long currentOffset = data.Position;

            // Seek to the name offset
            data.Seek(offsetList.NameOffset + descriptorOffset, SeekOrigin.Begin);

            // Read the string
            if (majorVersion >= 17)
                offsetList.Name = data.ReadString(Encoding.Unicode);
            else
                offsetList.Name = data.ReadString(Encoding.ASCII);

            // Seek back to the correct offset
            data.Seek(currentOffset, SeekOrigin.Begin);

            return offsetList;
        }

        /// <summary>
        /// Parse a Stream into a file group
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">Major version of the cabinet</param>
        /// <param name="descriptorOffset">Offset of the cabinet descriptor</param>
        /// <returns>Filled file group on success, null on error</returns>
        private static FileGroup ParseFileGroup(Stream data, int majorVersion, uint descriptorOffset)
        {
            FileGroup fileGroup = new FileGroup();

            fileGroup.NameOffset = data.ReadUInt32();

            // Skip bytes based on the version
            if (majorVersion <= 5)
                _ = data.ReadBytes(0x48);
            else
                _ = data.ReadBytes(0x12);

            fileGroup.FirstFile = data.ReadUInt16();
            fileGroup.LastFile = data.ReadUInt32();

            // Cache the current position
            long currentPosition = data.Position;

            // Read the name, if possible
            if (fileGroup.NameOffset != 0)
            {
                // Seek to the name
                data.Seek(fileGroup.NameOffset + descriptorOffset, SeekOrigin.Begin);

                // Read the string
                if (majorVersion >= 17)
                    fileGroup.Name = data.ReadString(Encoding.Unicode);
                else
                    fileGroup.Name = data.ReadString(Encoding.ASCII);
            }

            // Seek back to the correct offset
            data.Seek(currentPosition, SeekOrigin.Begin);

            return fileGroup;
        }

        /// <summary>
        /// Parse a Stream into a component
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">Major version of the cabinet</param>
        /// <param name="descriptorOffset">Offset of the cabinet descriptor</param>
        /// <returns>Filled component on success, null on error</returns>
        private static Component ParseComponent(Stream data, int majorVersion, uint descriptorOffset)
        {
            Component component = new Component();

            component.NameOffset = data.ReadUInt32();

            // Skip bytes based on the version
            if (majorVersion <= 5)
                _ = data.ReadBytes(0x6C);
            else
                _ = data.ReadBytes(0x6B);

            component.FileGroupCount = data.ReadUInt16();
            component.FileGroupTableOffset = data.ReadUInt32();

            // Cache the current position
            long currentPosition = data.Position;

            // Read the name, if possible
            if (component.NameOffset != 0)
            {
                // Seek to the name
                data.Seek(component.NameOffset + descriptorOffset, SeekOrigin.Begin);

                // Read the string
                if (majorVersion >= 17)
                    component.Name = data.ReadString(Encoding.Unicode);
                else
                    component.Name = data.ReadString(Encoding.ASCII);
            }

            // Read the file group table, if possible
            if (component.FileGroupCount != 0 && component.FileGroupTableOffset != 0)
            {
                // Seek to the file group table offset
                data.Seek(component.FileGroupTableOffset + descriptorOffset, SeekOrigin.Begin);

                // Read the file group table
                component.FileGroupNames = new string[component.FileGroupCount];
                for (int j = 0; j < component.FileGroupCount; j++)
                {
                    if (majorVersion >= 17)
                        component.FileGroupNames[j] = data.ReadString(Encoding.Unicode);
                    else
                        component.FileGroupNames[j] = data.ReadString(Encoding.ASCII);
                }
            }

            // Seek back to the correct offset
            data.Seek(currentPosition, SeekOrigin.Begin);

            return component;
        }

        /// <summary>
        /// Parse a Stream into a directory descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">Major version of the cabinet</param>
        /// <returns>Filled directory descriptor on success, null on error</returns>
        private static FileDescriptor ParseDirectoryDescriptor(Stream data, int majorVersion)
        {
            FileDescriptor fileDescriptor = new FileDescriptor();

            // Read the string
            if (majorVersion >= 17)
                fileDescriptor.Name = data.ReadString(Encoding.Unicode);
            else
                fileDescriptor.Name = data.ReadString(Encoding.ASCII);

            return fileDescriptor;
        }

        /// <summary>
        /// Parse a Stream into a file descriptor
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="majorVersion">Major version of the cabinet</param>
        /// <param name="descriptorOffset">Offset of the cabinet descriptor</param>
        /// <returns>Filled file descriptor on success, null on error</returns>
        private static FileDescriptor ParseFileDescriptor(Stream data, int majorVersion, uint descriptorOffset)
        {
            FileDescriptor fileDescriptor = new FileDescriptor();

            // Read the descriptor based on version
            if (majorVersion <= 5)
            {
                fileDescriptor.Volume = 0xFFFF; // Set by the header index
                fileDescriptor.NameOffset = data.ReadUInt32();
                fileDescriptor.DirectoryIndex = data.ReadUInt32();
                fileDescriptor.Flags = (FileFlags)data.ReadUInt16();
                fileDescriptor.ExpandedSize = data.ReadUInt32();
                fileDescriptor.CompressedSize = data.ReadUInt32();
                _ = data.ReadBytes(0x14); // Skip 0x14 bytes, unknown data?
                fileDescriptor.DataOffset = data.ReadUInt32();

                if (majorVersion == 5)
                    fileDescriptor.MD5 = data.ReadBytes(0x10);
            }
            else
            {
                fileDescriptor.Flags = (FileFlags)data.ReadUInt16();
                fileDescriptor.ExpandedSize = data.ReadUInt64();
                fileDescriptor.CompressedSize = data.ReadUInt64();
                fileDescriptor.DataOffset = data.ReadUInt64();
                fileDescriptor.MD5 = data.ReadBytes(0x10);
                _ = data.ReadBytes(0x10); // Skip 0x10 bytes, unknown data?
                fileDescriptor.NameOffset = data.ReadUInt32();
                fileDescriptor.DirectoryIndex = data.ReadUInt16();
                _ = data.ReadBytes(0x0C); // Skip 0x0C bytes, unknown data?
                fileDescriptor.LinkPrevious = data.ReadUInt32();
                fileDescriptor.LinkNext = data.ReadUInt32();
                fileDescriptor.LinkFlags = (LinkFlags)data.ReadByteValue();
                fileDescriptor.Volume = data.ReadUInt16();
            }

            // Cache the current position
            long currentPosition = data.Position;

            // Read the name, if possible
            if (fileDescriptor.NameOffset != 0)
            {
                // Seek to the name
                data.Seek(fileDescriptor.NameOffset + descriptorOffset, SeekOrigin.Begin);

                // Read the string
                if (majorVersion >= 17)
                    fileDescriptor.Name = data.ReadString(Encoding.Unicode);
                else
                    fileDescriptor.Name = data.ReadString(Encoding.ASCII);
            }

            // Seek back to the correct offset
            data.Seek(currentPosition, SeekOrigin.Begin);

            return fileDescriptor;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get the major version of the cabinet
        /// </summary>
        /// <remarks>This should live in the wrapper but is needed during parsing</remarks>
        private static int GetMajorVersion(CommonHeader commonHeader)
        {
            uint majorVersion = commonHeader.Version;
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

        #endregion
    }
}
