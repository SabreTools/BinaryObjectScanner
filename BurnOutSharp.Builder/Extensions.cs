using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BurnOutSharp.Builder
{
    public static class Extensions
    {
        #region Byte Arrays

        /// <summary>
        /// Read a byte and increment the pointer to an array
        /// </summary>
        public static byte ReadByte(this byte[] content, ref int offset)
        {
            return content[offset++];
        }

        /// <summary>
        /// Read a byte array and increment the pointer to an array
        /// </summary>
        public static byte[] ReadBytes(this byte[] content, ref int offset, int count)
        {
            byte[] buffer = new byte[count];
            Array.Copy(content, offset, buffer, 0, Math.Min(count, content.Length - offset));
            offset += count;
            return buffer;
        }

        /// <summary>
        /// Read a char and increment the pointer to an array
        /// </summary>
        public static char ReadChar(this byte[] content, ref int offset)
        {
            return (char)content[offset++];
        }

        /// <summary>
        /// Read a character array and increment the pointer to an array
        /// </summary>
        public static char[] ReadChars(this byte[] content, ref int offset, int count) => content.ReadChars(ref offset, count, Encoding.Default);

        /// <summary>
        /// Read a character array and increment the pointer to an array
        /// </summary>
        public static char[] ReadChars(this byte[] content, ref int offset, int count, Encoding encoding)
        {
            // TODO: Fix the code below to make it work with byte arrays and not streams
            return null;

            // byte[] buffer = new byte[count];
            // stream.Read(buffer, 0, count);
            // return encoding.GetString(buffer).ToCharArray();
        }

        /// <summary>
        /// Read a short and increment the pointer to an array
        /// </summary>
        public static short ReadInt16(this byte[] content, ref int offset)
        {
            short value = BitConverter.ToInt16(content, offset);
            offset += 2;
            return value;
        }

        /// <summary>
        /// Read a ushort and increment the pointer to an array
        /// </summary>
        public static ushort ReadUInt16(this byte[] content, ref int offset)
        {
            ushort value = BitConverter.ToUInt16(content, offset);
            offset += 2;
            return value;
        }

        /// <summary>
        /// Read a int and increment the pointer to an array
        /// </summary>
        public static int ReadInt32(this byte[] content, ref int offset)
        {
            int value = BitConverter.ToInt32(content, offset);
            offset += 4;
            return value;
        }

        /// <summary>
        /// Read a uint and increment the pointer to an array
        /// </summary>
        public static uint ReadUInt32(this byte[] content, ref int offset)
        {
            uint value = BitConverter.ToUInt32(content, offset);
            offset += 4;
            return value;
        }

        /// <summary>
        /// Read a long and increment the pointer to an array
        /// </summary>
        public static long ReadInt64(this byte[] content, ref int offset)
        {
            long value = BitConverter.ToInt64(content, offset);
            offset += 8;
            return value;
        }

        /// <summary>
        /// Read a ulong and increment the pointer to an array
        /// </summary>
        public static ulong ReadUInt64(this byte[] content, ref int offset)
        {
            ulong value = BitConverter.ToUInt64(content, offset);
            offset += 8;
            return value;
        }

        /// <summary>
        /// Read a null-terminated string from the stream
        /// </summary>
        public static string ReadString(this byte[] content, ref int offset) => content.ReadString(ref offset, Encoding.Default);

        /// <summary>
        /// Read a null-terminated string from the stream
        /// </summary>
        public static string ReadString(this byte[] content, ref int offset, Encoding encoding)
        {
            byte[] nullTerminator = encoding.GetBytes(new char[] { '\0' });
            int charWidth = nullTerminator.Length;

            List<char> keyChars = new List<char>();
            while (BitConverter.ToUInt16(content, offset) != 0x0000)
            {
                keyChars.Add(encoding.GetChars(content, offset, charWidth)[0]); offset += charWidth;
            }
            offset += 2;

            return new string(keyChars.ToArray());
        }

        #endregion

        #region Streams

        /// <summary>
        /// Read a byte from the stream
        /// </summary>
        public static byte ReadByteValue(this Stream stream)
        {
            byte[] buffer = new byte[1];
            stream.Read(buffer, 0, 1);
            return buffer[0];
        }

        /// <summary>
        /// Read a byte array from the stream
        /// </summary>
        public static byte[] ReadBytes(this Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);
            return buffer;
        }

        /// <summary>
        /// Read a character from the stream
        /// </summary>
        public static char ReadChar(this Stream stream)
        {
            byte[] buffer = new byte[1];
            stream.Read(buffer, 0, 1);
            return (char)buffer[0];
        }

        /// <summary>
        /// Read a character array from the stream
        /// </summary>
        public static char[] ReadChars(this Stream stream, int count) => stream.ReadChars(count, Encoding.Default);

        /// <summary>
        /// Read a character array from the stream
        /// </summary>
        public static char[] ReadChars(this Stream stream, int count, Encoding encoding)
        {
            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);
            return encoding.GetString(buffer).ToCharArray();
        }

        /// <summary>
        /// Read a short from the stream
        /// </summary>
        public static short ReadInt16(this Stream stream)
        {
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            return BitConverter.ToInt16(buffer, 0);
        }

        /// <summary>
        /// Read a ushort from the stream
        /// </summary>
        public static ushort ReadUInt16(this Stream stream)
        {
            byte[] buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }

        /// <summary>
        /// Read an int from the stream
        /// </summary>
        public static int ReadInt32(this Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        /// Read a uint from the stream
        /// </summary>
        public static uint ReadUInt32(this Stream stream)
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>
        /// Read a long from the stream
        /// </summary>
        public static long ReadInt64(this Stream stream)
        {
            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// Read a ulong from the stream
        /// </summary>
        public static ulong ReadUInt64(this Stream stream)
        {
            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, 8);
            return BitConverter.ToUInt64(buffer, 0);
        }

        /// <summary>
        /// Read a null-terminated string from the stream
        /// </summary>
        public static string ReadString(this Stream stream) => stream.ReadString(Encoding.Default);

        /// <summary>
        /// Read a null-terminated string from the stream
        /// </summary>
        public static string ReadString(this Stream stream, Encoding encoding)
        {
            byte[] nullTerminator = encoding.GetBytes(new char[] { '\0' });
            int charWidth = nullTerminator.Length;

            List<byte> tempBuffer = new List<byte>();

            byte[] buffer = new byte[charWidth];
            while (stream.Read(buffer, 0, charWidth) != 0 && buffer.SequenceEqual(nullTerminator))
            {
                tempBuffer.AddRange(buffer);
            }

            return encoding.GetString(tempBuffer.ToArray());
        }

        #endregion

        #region New Executable

        /// <summary>
        /// Determine if a resource type information entry is an integer or offset
        /// </summary>
        /// <param name="entry">Resource type information entry to check</param>
        /// <returns>True if the entry is an integer type, false if an offset, null on error</returns>
        public static bool? IsIntegerType(this Models.NewExecutable.ResourceTypeInformationEntry entry)
        {
            // We can't do anything with an invalid entry
            if (entry == null)
                return null;

            // If the highest order bit is set, it's an integer type
            return (entry.TypeID & 0x8000) != 0;
        }

        /// <summary>
        /// Determine if a resource type resource entry is an integer or offset
        /// </summary>
        /// <param name="entry">Resource type resource entry to check</param>
        /// <returns>True if the entry is an integer type, false if an offset, null on error</returns>
        public static bool? IsIntegerType(this Models.NewExecutable.ResourceTypeResourceEntry entry)
        {
            // We can't do anything with an invalid entry
            if (entry == null)
                return null;

            // If the highest order bit is set, it's an integer type
            return (entry.ResourceID & 0x8000) != 0;
        }

        /// <summary>
        /// Get the segment entry type for an entry table bundle
        /// </summary>
        /// <param name="entry">Entry table bundle to check</param>
        /// <returns>SegmentEntryType corresponding to the type</returns>
        public static Models.NewExecutable.SegmentEntryType GetEntryType(this Models.NewExecutable.EntryTableBundle entry)
        {
            // We can't do anything with an invalid entry
            if (entry == null)
                return Models.NewExecutable.SegmentEntryType.Unused;

            // Determine the entry type based on segment indicator
            if (entry.SegmentIndicator == 0x00)
                return Models.NewExecutable.SegmentEntryType.Unused;
            else if (entry.SegmentIndicator >= 0x01 && entry.SegmentIndicator <= 0xFE)
                return Models.NewExecutable.SegmentEntryType.FixedSegment;
            else if (entry.SegmentIndicator == 0xFF)
                return Models.NewExecutable.SegmentEntryType.MoveableSegment;

            // We should never get here
            return Models.NewExecutable.SegmentEntryType.Unused;
        }

        #endregion

        // TODO: Write extension to parse resource data
        #region Portable Executable

        /// <summary>
        /// Convert a relative virtual address to a physical one
        /// </summary>
        /// <param name="rva">Relative virtual address to convert</param>
        /// <param name="sections">Array of sections to check against</param>
        /// <returns>Physical address, 0 on error</returns>
        public static uint ConvertVirtualAddress(this uint rva, Models.PortableExecutable.SectionHeader[] sections)
        {
            // Loop through all of the sections
            for (int i = 0; i < sections.Length; i++)
            {
                // If the section is invalid, just skip it
                if (sections[i] == null)
                    continue;

                // If the section "starts" at 0, just skip it
                if (sections[i].PointerToRawData == 0)
                    continue;

                // Attempt to derive the physical address from the current section
                var section = sections[i];
                if (rva >= section.VirtualAddress && rva <= section.VirtualAddress + section.VirtualSize)
                    return rva - section.VirtualAddress + section.PointerToRawData;
            }

            return 0;
        }

        /// <summary>
        /// Read resource data as a resource header
        /// </summary>
        /// <param name="data">Data to parse into a resource header</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled resource header on success, null on error</returns>
        public static Models.PortableExecutable.ResourceHeader AsResourceHeader(this byte[] data, ref int offset)
        {
            // If we have data that's invalid, we can't do anything
            if (data == null)
                return null;

            // Read in the table
            var header = new Models.PortableExecutable.ResourceHeader();
            header.DataSize = data.ReadUInt32(ref offset);
            header.HeaderSize = data.ReadUInt32(ref offset);
            header.ResourceType = (Models.PortableExecutable.ResourceType)data.ReadUInt32(ref offset); // TODO: Could be a string too
            header.Name = data.ReadUInt32(ref offset); // TODO: Could be a string too
            header.DataVersion = data.ReadUInt32(ref offset);
            header.MemoryFlags = (Models.PortableExecutable.MemoryFlags)data.ReadUInt16(ref offset);
            header.LanguageId = data.ReadUInt16(ref offset);
            header.Version = data.ReadUInt32(ref offset);
            header.Characteristics = data.ReadUInt32(ref offset);

            return header;
        }

        /// <summary>
        /// Read resource data as an accelerator table resource
        /// </summary>
        /// <param name="data">Data to parse into an accelerator table resource</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>A filled accelerator table resource on success, null on error</returns>
        public static Models.PortableExecutable.AcceleratorTableEntry[] AsAcceleratorTableResource(this byte[] data, ref int offset)
        {
            // If we have data that's invalid for this resource type, we can't do anything
            if (data == null || data.Length % 8 != 0)
                return null;

            // Get the number of entries
            int count = data.Length / 8;

            // Read in the table
            var table = new Models.PortableExecutable.AcceleratorTableEntry[count];
            for (int i = 0; i < count; i++)
            {
                var entry = new Models.PortableExecutable.AcceleratorTableEntry();
                entry.Flags = (Models.PortableExecutable.AcceleratorTableFlags)data.ReadUInt16(ref offset);
                entry.Ansi = data.ReadUInt16(ref offset);
                entry.Id = data.ReadUInt16(ref offset);
                entry.Padding = data.ReadUInt16(ref offset);
                table[i] = entry;
            }

            return table;
        }

        #endregion
    }
}