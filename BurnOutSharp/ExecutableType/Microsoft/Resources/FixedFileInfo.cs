using System;
using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.Resources
{
    internal class FixedFileInfo
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
        /// 
        /// VS_FF_DEBUG (0x00000001L)           The file contains debugging information or is compiled with debugging features enabled.
        /// VS_FF_INFOINFERRED (0x00000010L)    The file's version structure was created dynamically; therefore, some of the members in this structure may be empty or incorrect. This flag should never be set in a file's VS_VERSIONINFO data.
        /// VS_FF_PATCHED (0x00000004L)         The file has been modified and is not identical to the original shipping file of the same version number.
        /// VS_FF_PRERELEASE (0x00000002L)      The file is a development version, not a commercially released product.
        /// VS_FF_PRIVATEBUILD (0x00000008L)    The file was not built using standard release procedures. If this flag is set, the StringFileInfo structure should contain a PrivateBuild entry.
        /// VS_FF_SPECIALBUILD (0x00000020L)    The file was built by the original company using standard release procedures but is a variation of the normal file of the same version number. If this flag is set, the StringFileInfo structure should contain a SpecialBuild entry.
        /// </summary>
        /// <remarks>TODO: Make an enum out of this</remarks>
        public uint FileFlags;

        /// <summary>
        /// The operating system for which this file was designed. This member can be one of the following values.
        /// 
        /// VOS_DOS (0x00010000L)               The file was designed for MS-DOS.
        /// VOS_NT (0x00040000L)                The file was designed for Windows NT.
        /// VOS__WINDOWS16 (0x00000001L)        The file was designed for 16-bit Windows.
        /// VOS__WINDOWS32 (0x00000004L)        The file was designed for 32-bit Windows.
        /// VOS_OS216 (0x00020000L)             The file was designed for 16-bit OS/2.
        /// VOS_OS232 (0x00030000L)             The file was designed for 32-bit OS/2.
        /// VOS__PM16 (0x00000002L)             The file was designed for 16-bit Presentation Manager.
        /// VOS__PM32 (0x00000003L)             The file was designed for 32-bit Presentation Manager.
        /// VOS_UNKNOWN (0x00000000L)           The operating system for which the file was designed is unknown to the system.
        /// 
        /// An application can combine these values to indicate that the file was designed for one operating system running on another.
        /// The following dwFileOS values are examples of this, but are not a complete list.
        /// 
        /// VOS_DOS_WINDOWS16 (0x00010001L)     The file was designed for 16-bit Windows running on MS-DOS.
        /// VOS_DOS_WINDOWS32 (0x00010004L)     The file was designed for 32-bit Windows running on MS-DOS.
        /// VOS_NT_WINDOWS32 (0x00040004L)      The file was designed for Windows NT.
        /// VOS_OS216_PM16 (0x00020002L)        The file was designed for 16-bit Presentation Manager running on 16-bit OS/2.
        /// VOS_OS232_PM32 (0x00030003L)        The file was designed for 32-bit Presentation Manager running on 32-bit OS/2.
        /// </summary>
        /// <remarks>TODO: Make an enum out of this</remarks>
        public uint FileOS;

        /// <summary>
        /// The general type of file. This member can be one of the following values. All other values are reserved.
        /// 
        /// VFT_APP (0x00000001L)               The file contains an application.
        /// VFT_DLL (0x00000002L)               The file contains a DLL.
        /// VFT_DRV (0x00000003L)               The file contains a device driver. If dwFileType is VFT_DRV, dwFileSubtype contains a more specific description of the driver.
        /// VFT_FONT (0x00000004L)              The file contains a font. If dwFileType is VFT_FONT, dwFileSubtype contains a more specific description of the font file.
        /// VFT_STATIC_LIB (0x00000007L)        The file contains a static-link library.
        /// VFT_UNKNOWN (0x00000000L)           The file type is unknown to the system.
        /// VFT_VXD (0x00000005L)               The file contains a virtual device.
        /// </summary>
        /// <remarks>TODO: Make an enum out of this</remarks>
        public uint FileType;

        /// <summary>
        /// The function of the file. The possible values depend on the value of dwFileType.
        /// For all values of dwFileType not described in the following list, dwFileSubtype is zero.
        /// 
        /// If dwFileType is VFT_DRV, dwFileSubtype can be one of the following values.
        /// 
        /// VFT2_DRV_COMM (0x0000000AL)                 The file contains a communications driver.
        /// VFT2_DRV_DISPLAY (0x00000004L)              The file contains a display driver.
        /// VFT2_DRV_INSTALLABLE (0x00000008L)          The file contains an installable driver.
        /// VFT2_DRV_KEYBOARD (0x00000002L)             The file contains a keyboard driver.
        /// VFT2_DRV_LANGUAGE (0x00000003L)             The file contains a language driver.
        /// VFT2_DRV_MOUSE (0x00000005L)                The file contains a mouse driver.
        /// VFT2_DRV_NETWORK (0x00000006L)              The file contains a network driver.
        /// VFT2_DRV_PRINTER (0x00000001L)              The file contains a printer driver.
        /// VFT2_DRV_SOUND (0x00000009L)                The file contains a sound driver.
        /// VFT2_DRV_SYSTEM (0x00000007L)               The file contains a system driver.
        /// VFT2_DRV_VERSIONED_PRINTER (0x0000000CL)    The file contains a versioned printer driver.
        /// VFT2_UNKNOWN (0x00000000L)                  The driver type is unknown by the system.
        /// 
        /// If dwFileType is VFT_FONT, dwFileSubtype can be one of the following values.
        /// 
        /// VFT2_FONT_RASTER (0x00000001L)              The file contains a raster font.
        /// VFT2_FONT_TRUETYPE (0x00000003L)            The file contains a TrueType font.
        /// VFT2_FONT_VECTOR (0x00000002L)              The file contains a vector font.
        /// VFT2_UNKNOWN (0x00000000L)                  The font type is unknown by the system.
        /// 
        /// If dwFileType is VFT_VXD, dwFileSubtype contains the virtual device identifier included in the virtual device control block.
        /// All dwFileSubtype values not listed here are reserved.
        /// </summary>
        /// <remarks>TODO: Make an enum out of this</remarks>
        public uint FileSubtype;

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
            ffi.FileFlags = stream.ReadUInt32();
            ffi.FileOS = stream.ReadUInt32();
            ffi.FileType = stream.ReadUInt32();
            ffi.FileSubtype = stream.ReadUInt32();
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
            ffi.FileFlags = BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileOS = BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileType = BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileSubtype = BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileDateMS = BitConverter.ToUInt32(content, offset); offset += 4;
            ffi.FileDateLS = BitConverter.ToUInt32(content, offset); offset += 4;

            return ffi;
        }
    }
}