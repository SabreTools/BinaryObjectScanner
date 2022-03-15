using System;
using System.IO;
using System.Text;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.PE.Headers
{
    /// <summary>
    /// Each row of the section table is, in effect, a section header.
    /// This table immediately follows the optional header, if any.
    /// This positioning is required because the file header does not contain a direct pointer to the section table.
    /// Instead, the location of the section table is determined by calculating the location of the first byte after the headers.
    /// Make sure to use the size of the optional header as specified in the file header.
    /// </summary>
    public class SectionHeader
    {
        /// <summary>
        /// An 8-byte, null-padded UTF-8 encoded string.
        /// If the string is exactly 8 characters long, there is no terminating null.
        /// For longer names, this field contains a slash (/) that is followed by an ASCII representation of a decimal number
        /// that is an offset into the string table.
        /// Executable images do not use a string table and do not support section names longer than 8 characters.
        /// Long names in object files are truncated if they are emitted to an executable file.
        /// </summary>
        public byte[] Name;

        /// <summary>
        /// Section name as a string, trimming any trailing null bytes
        /// </summary>
        public string NameString
        {
            get
            {
                if (this.Name == null || this.Name.Length == 0)
                    return null;

                // First try decoding as UTF-8
                try
                {
                    return Encoding.UTF8.GetString(this.Name).TrimEnd('\0');
                }
                catch { }

                // Then try decoding as ASCII
                try
                {
                    return Encoding.ASCII.GetString(this.Name).TrimEnd('\0');
                }
                catch { }

                // If it fails, return null
                return null;
            }
        }
        
        /// <summary>
        /// The total size of the section when loaded into memory.
        /// If this value is greater than SizeOfRawData, the section is zero-padded.
        /// This field is valid only for executable images and should be set to zero for object files.
        /// </summary>
        public uint VirtualSize;
        
        /// <summary>
        /// For executable images, the address of the first byte of the section relative to the image base when the section
        /// is loaded into memory.
        /// For object files, this field is the address of the first byte before relocation is applied; for simplicity,
        /// compilers should set this to zero.
        /// Otherwise, it is an arbitrary value that is subtracted from offsets during relocation.
        /// </summary>
        public uint VirtualAddress;
       
        /// <summary>
        /// The size of the section (for object files) or the size of the initialized data on disk (for image files).
        /// For executable images, this must be a multiple of FileAlignment from the optional header.
        /// If this is less than VirtualSize, the remainder of the section is zero-filled.
        /// Because the SizeOfRawData field is rounded but the VirtualSize field is not, it is possible for SizeOfRawData
        /// to be greater than VirtualSize as well.
        /// When a section contains only uninitialized data, this field should be zero.
        /// </summary>
        public uint SizeOfRawData;
        
        /// <summary>
        /// The file pointer to the first page of the section within the COFF file.
        /// For executable images, this must be a multiple of FileAlignment from the optional header.
        /// For object files, the value should be aligned on a 4-byte boundary for best performance.
        /// When a section contains only uninitialized data, this field should be zero.
        /// </summary>
        public uint PointerToRawData;
        
        /// <summary>
        /// The file pointer to the beginning of relocation entries for the section.
        /// This is set to zero for executable images or if there are no relocations.
        /// </summary>
        public uint PointerToRelocations;
        
        /// <summary>
        /// The file pointer to the beginning of line-number entries for the section.
        /// This is set to zero if there are no COFF line numbers.
        /// This value should be zero for an image because COFF debugging information is deprecated.
        /// </summary>
        [Obsolete]
        public uint PointerToLinenumbers;
        
        /// <summary>
        /// The number of relocation entries for the section.
        /// This is set to zero for executable images.
        /// </summary>
        public ushort NumberOfRelocations;
        
        /// <summary>
        /// The number of line-number entries for the section.
        /// This value should be zero for an image because COFF debugging information is deprecated.
        /// </summary>
        [Obsolete]
        public ushort NumberOfLinenumbers;
        
        /// <summary>
        /// The flags that describe the characteristics of the section.
        /// </summary>
        public SectionCharacteristics Characteristics;

        public static SectionHeader Deserialize(Stream stream)
        {
            var ish = new SectionHeader();

            ish.Name = stream.ReadBytes(Constants.IMAGE_SIZEOF_SHORT_NAME);
            ish.VirtualSize = stream.ReadUInt32();
            ish.VirtualAddress = stream.ReadUInt32();
            ish.SizeOfRawData = stream.ReadUInt32();
            ish.PointerToRawData = stream.ReadUInt32();
            ish.PointerToRelocations = stream.ReadUInt32();
            ish.PointerToLinenumbers = stream.ReadUInt32();
            ish.NumberOfRelocations = stream.ReadUInt16();
            ish.NumberOfLinenumbers = stream.ReadUInt16();
            ish.Characteristics = (SectionCharacteristics)stream.ReadUInt32();

            return ish;
        }

        public static SectionHeader Deserialize(byte[] content, ref int offset)
        {
            var ish = new SectionHeader();

            ish.Name = new byte[Constants.IMAGE_SIZEOF_SHORT_NAME];
            Array.Copy(content, offset, ish.Name, 0, Constants.IMAGE_SIZEOF_SHORT_NAME); offset += Constants.IMAGE_SIZEOF_SHORT_NAME;
            ish.VirtualSize = content.ReadUInt32(ref offset);
            ish.VirtualAddress = content.ReadUInt32(ref offset);
            ish.SizeOfRawData = content.ReadUInt32(ref offset);
            ish.PointerToRawData = content.ReadUInt32(ref offset);
            ish.PointerToRelocations = content.ReadUInt32(ref offset);
            ish.PointerToLinenumbers = content.ReadUInt32(ref offset);
            ish.NumberOfRelocations = content.ReadUInt16(ref offset);
            ish.NumberOfLinenumbers = content.ReadUInt16(ref offset);
            ish.Characteristics = (SectionCharacteristics)content.ReadUInt32(ref offset);

            return ish;
        }
    }
}