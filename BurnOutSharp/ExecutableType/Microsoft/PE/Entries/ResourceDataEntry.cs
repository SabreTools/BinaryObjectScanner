using System;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft.PE.Headers;
using BurnOutSharp.Tools;
 
namespace BurnOutSharp.ExecutableType.Microsoft.PE.Entries
{
    /// <summary>
    /// Each Resource Data entry describes an actual unit of raw data in the Resource Data area.
    /// </summary>
    public class ResourceDataEntry
    {
        /// <summary>
        /// The address of a unit of resource data in the Resource Data area.
        /// </summary>
        public uint OffsetToData;

        /// <summary>
        /// A unit of resource data in the Resource Data area.
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// A unit of resource data in the Resource Data area.
        /// </summary>
        public string DataAsUTF8String
        {
            get
            {
                int codePage = (int)CodePage;
                if (Data == null || codePage < 0)
                    return string.Empty;

                // Try to convert to UTF-8 first
                try
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    var originalEncoding = Encoding.GetEncoding(codePage);
                    byte[] convertedData = Encoding.Convert(originalEncoding, Encoding.UTF8, Data);
                    return Encoding.UTF8.GetString(convertedData);
                }
                catch { }

                // Then try to read direct as ASCII
                try
                {
                    return Encoding.ASCII.GetString(Data);
                }
                catch { }

                // If both encodings fail, then just return an empty string
                return string.Empty;
            }
        }

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

        public static ResourceDataEntry Deserialize(Stream stream, SectionHeader[] sections)
        {
            var rde = new ResourceDataEntry();

            rde.OffsetToData = stream.ReadUInt32();
            rde.Size = stream.ReadUInt32();
            rde.CodePage = stream.ReadUInt32();
            rde.Reserved = stream.ReadUInt32();

            int realOffsetToData = (int)PortableExecutable.ConvertVirtualAddress(rde.OffsetToData, sections);
            if (realOffsetToData > -1 && realOffsetToData < stream.Length && (int)rde.Size > 0 && realOffsetToData + (int)rde.Size < stream.Length)
            {
                long lastPosition = stream.Position;
                stream.Seek(realOffsetToData, SeekOrigin.Begin);
                rde.Data = stream.ReadBytes((int)rde.Size);
                stream.Seek(lastPosition, SeekOrigin.Begin);
            }

            return rde;
        }

        public static ResourceDataEntry Deserialize(byte[] content, ref int offset, SectionHeader[] sections)
        {
            var rde = new ResourceDataEntry();

            rde.OffsetToData = content.ReadUInt32(ref offset);
            rde.Size = content.ReadUInt32(ref offset);
            rde.CodePage = content.ReadUInt32(ref offset);
            rde.Reserved = content.ReadUInt32(ref offset);

            int realOffsetToData = (int)PortableExecutable.ConvertVirtualAddress(rde.OffsetToData, sections);
            if (realOffsetToData > -1 && realOffsetToData < content.Length && (int)rde.Size > 0 && realOffsetToData + (int)rde.Size < content.Length)
                rde.Data = new ArraySegment<byte>(content, realOffsetToData, (int)rde.Size).ToArray();

            return rde;
        }
    }
}