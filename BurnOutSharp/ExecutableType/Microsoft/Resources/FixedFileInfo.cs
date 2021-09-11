using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Resources
{
    public class FixedFileInfo
    {
        /// <summary>
        /// Contains the value 0xFEEF04BD.
        /// This is used with the szKey member of the VS_VERSIONINFO structure when searching a file for the VS_FIXEDFILEINFO structure.
        /// </summary>
        public uint Signature;

        /// <summary>
        /// The binary version number of this structure.
        /// The high-order word of this member contains the major version number, and the low-order word contains the minor version number.
        /// </summary>
        public uint StrucVersion;

        /// <summary>
        /// The most significant 32 bits of the file's binary version number.
        /// This member is used with dwFileVersionLS to form a 64-bit value used for numeric comparisons.
        /// </summary>
        public uint FileVersionMS;

        /// <summary>
        /// The least significant 32 bits of the file's binary version number.
        /// This member is used with dwFileVersionMS to form a 64-bit value used for numeric comparisons.
        /// </summary>
        public uint FileVersionLS;

        /// <summary>
        /// The most significant 32 bits of the binary version number of the product with which this file was distributed.
        /// This member is used with dwProductVersionLS to form a 64-bit value used for numeric comparisons.
        /// </summary>
        public uint ProductVersionMS;

        /// <summary>
        /// The least significant 32 bits of the binary version number of the product with which this file was distributed.
        /// This member is used with dwProductVersionMS to form a 64-bit value used for numeric comparisons.
        /// </summary>
        public uint ProductVersionLS;

        /// <summary>
        /// Contains a bitmask that specifies the valid bits in dwFileFlags.
        /// A bit is valid only if it was defined when the file was created.
        /// </summary>
        public uint FileFlagsMask;

        /// <summary>
        /// Contains a bitmask that specifies the Boolean attributes of the file. This member can include one or more of the following values.
        /// </summary>
        public FileInfoFileFlags FileFlags;

        /// <summary>
        /// The operating system for which this file was designed. This member can be one of the following values.
        /// 
        /// An application can combine these values to indicate that the file was designed for one operating system running on another.
        /// The following dwFileOS values are examples of this, but are not a complete list.
        /// </summary>
        public FileInfoOS FileOS;

        /// <summary>
        /// The general type of file. This member can be one of the following values. All other values are reserved.
        /// </summary>
        public FileInfoFileType FileType;

        /// <summary>
        /// The function of the file. The possible values depend on the value of dwFileType.
        /// For all values of dwFileType not described in the following list, dwFileSubtype is zero.
        /// 
        /// If dwFileType is VFT_DRV, dwFileSubtype can be one of the following values.
        /// 
        /// If dwFileType is VFT_FONT, dwFileSubtype can be one of the following values.
        /// 
        /// If dwFileType is VFT_VXD, dwFileSubtype contains the virtual device identifier included in the virtual device control block.
        /// All dwFileSubtype values not listed here are reserved.
        /// </summary>
        public FileInfoFileSubtype FileSubtype;

        /// <summary>
        /// The most significant 32 bits of the file's 64-bit binary creation date and time stamp.
        /// </summary>
        public uint FileDateMS;

        /// <summary>
        /// The least significant 32 bits of the file's 64-bit binary creation date and time stamp.
        /// </summary>
        public uint FileDateLS;

        public static FixedFileInfo Deserialize(Stream stream)
        {
            FixedFileInfo ffi = new FixedFileInfo();

            ushort temp;
            while ((temp = stream.ReadUInt16()) == 0x0000);
            stream.Seek(-2, SeekOrigin.Current);

            ffi.Signature = stream.ReadUInt32();
            ffi.StrucVersion = stream.ReadUInt32();
            ffi.FileVersionMS = stream.ReadUInt32();
            ffi.FileVersionLS = stream.ReadUInt32();
            ffi.ProductVersionMS = stream.ReadUInt32();
            ffi.ProductVersionLS = stream.ReadUInt32();
            ffi.FileFlagsMask = stream.ReadUInt32();
            ffi.FileFlags = (FileInfoFileFlags)stream.ReadUInt32();
            ffi.FileOS = (FileInfoOS)stream.ReadUInt32();
            ffi.FileType = (FileInfoFileType)stream.ReadUInt32();
            ffi.FileSubtype = (FileInfoFileSubtype)stream.ReadUInt32();
            ffi.FileDateMS = stream.ReadUInt32();
            ffi.FileDateLS = stream.ReadUInt32();

            return ffi;
        }

        public static FixedFileInfo Deserialize(byte[] content, ref int offset)
        {
            FixedFileInfo ffi = new FixedFileInfo();

            ushort temp;
            bool padded = false;
            while ((temp = BitConverter.ToUInt16(content, offset)) == 0x0000)
            {
                offset += 2;
                padded = true;
            }

            if (padded)
                offset -= 2;

            ffi.Signature = BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.StrucVersion = BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileVersionMS = BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileVersionLS = BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.ProductVersionMS = BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.ProductVersionLS = BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileFlagsMask = BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileFlags = (FileInfoFileFlags)BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileOS = (FileInfoOS)BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileType = (FileInfoFileType)BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileSubtype = (FileInfoFileSubtype)BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileDateMS = BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileDateLS = BitConverter.ToUInt32(content, offset); offset += 4;

            return ffi;
        }
    }
}