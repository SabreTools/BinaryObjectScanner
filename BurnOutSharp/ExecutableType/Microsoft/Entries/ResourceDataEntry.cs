using System;
using System.IO;
using System.Runtime.InteropServices;
using BurnOutSharp.Tools;
 
namespace BurnOutSharp.ExecutableType.Microsoft.Entries
{
    /// <summary>
    /// Each Resource Data entry describes an actual unit of raw data in the Resource Data area.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class ResourceDataEntry
    {
        /// <summary>
        /// The address of a unit of resource data in the Resource Data area.
        /// </summary>
        public uint OffsetToData;

        /// <summary>
        /// The size, in bytes, of the resource data that is pointed to by the Data RVA field.
        /// </summary>
        public uint Size;

        /// <summary>
        /// The code page that is used to decode code point values within the resource data.
        /// Typically, the code page would be the Unicode code page.
        /// </summary>
        public uint CodePage;
        
        /// <summary>
        /// Reserved, must be 0.
        /// </summary>
        public uint Reserved;

        public static ResourceDataEntry Deserialize(Stream stream)
        {
            var rde = new ResourceDataEntry();

            rde.OffsetToData = stream.ReadUInt32();
            rde.Size = stream.ReadUInt32();
            rde.CodePage = stream.ReadUInt32();
            rde.Reserved = stream.ReadUInt32();

            return rde;
        }

        public static ResourceDataEntry Deserialize(byte[] content, int offset)
        {
            var rde = new ResourceDataEntry();

            rde.OffsetToData = BitConverter.ToUInt32(content, offset); offset += 4;
            rde.Size = BitConverter.ToUInt32(content, offset); offset += 4;
            rde.CodePage = BitConverter.ToUInt32(content, offset); offset += 4;
            rde.Reserved = BitConverter.ToUInt32(content, offset); offset += 4;

            return rde;
        }
    }
}