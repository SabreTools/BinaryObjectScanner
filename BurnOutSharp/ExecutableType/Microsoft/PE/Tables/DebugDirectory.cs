using System.IO;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.PE.Tables
{
    /// <summary>
    /// Image files contain an optional debug directory that indicates what form of debug information is present and where it is.
    /// This directory consists of an array of debug directory entries whose location and size are indicated in the image optional header.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#debug-directory-image-only</remarks>
    public class DebugDirectory
    {
        /// <summary>
        /// Reserved, must be 0.
        /// </summary>
        public uint Characteristics;

        /// <summary>
        /// The time and date that the debug data was created.
        /// </summary>
        public uint TimeDateStamp;

        /// <summary>
        /// The major version number of the debug data format.
        /// </summary>
        public ushort MajorVersion;

        /// <summary>
        /// The minor version number of the debug data format.
        /// </summary>
        public ushort MinorVersion;

        /// <summary>
        /// The format of debugging information. This field enables support of multiple debuggers.
        /// </summary>
        public DebugType DebugType;

        /// <summary>
        /// The size of the debug data (not including the debug directory itself).
        /// </summary>
        public uint SizeOfData;

        /// <summary>
        /// The address of the debug data when loaded, relative to the image base.
        /// </summary>
        public uint AddressOfRawData;

        /// <summary>
        /// The file pointer to the debug data.
        /// </summary>
        public uint PointerToRawData;

        public static DebugDirectory Deserialize(Stream stream)
        {
            var dd = new DebugDirectory();

            dd.Characteristics = stream.ReadUInt32();
            dd.TimeDateStamp = stream.ReadUInt32();
            dd.MajorVersion = stream.ReadUInt16();
            dd.MinorVersion = stream.ReadUInt16();
            dd.DebugType = (DebugType)stream.ReadUInt32();
            dd.SizeOfData = stream.ReadUInt32();
            dd.AddressOfRawData = stream.ReadUInt32();
            dd.PointerToRawData = stream.ReadUInt32();

            return dd;
        }

        public static DebugDirectory Deserialize(byte[] content, ref int offset)
        {
            var dd = new DebugDirectory();

            dd.Characteristics = content.ReadUInt32(ref offset);
            dd.TimeDateStamp = content.ReadUInt32(ref offset);
            dd.MajorVersion = content.ReadUInt16(ref offset);
            dd.MinorVersion = content.ReadUInt16(ref offset);
            dd.DebugType = (DebugType)content.ReadUInt32(ref offset);
            dd.SizeOfData = content.ReadUInt32(ref offset);
            dd.AddressOfRawData = content.ReadUInt32(ref offset);
            dd.PointerToRawData = content.ReadUInt32(ref offset);

            return dd;
        }
    }
}