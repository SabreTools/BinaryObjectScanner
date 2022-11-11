using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

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
            while (stream.Read(buffer, 0, charWidth) != 0 && !buffer.SequenceEqual(nullTerminator))
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

        // TODO: Implement other resource types from https://learn.microsoft.com/en-us/windows/win32/menurc/resource-file-formats
        #region Portable Executable

        /// <summary>
        /// Convert a relative virtual address to a physical one
        /// </summary>
        /// <param name="rva">Relative virtual address to convert</param>
        /// <param name="sections">Array of sections to check against</param>
        /// <returns>Physical address, 0 on error</returns>
        public static uint ConvertVirtualAddress(this uint rva, Models.PortableExecutable.SectionHeader[] sections)
        {
            // If we have an invalid section table, we can't do anything
            if (sections == null || sections.Length == 0)
                return 0;

            // If the RVA is 0, we just return 0 because it's invalid
            if (rva == 0)
                return 0;

            // If the RVA matches a section start exactly, use that
            var matchingSection = sections.FirstOrDefault(s => s.VirtualAddress == rva);
            if (matchingSection != null)
                return rva - matchingSection.VirtualAddress + matchingSection.PointerToRawData;

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
                if (rva >= section.VirtualAddress && section.VirtualSize != 0 && rva <= section.VirtualAddress + section.VirtualSize)
                    return rva - section.VirtualAddress + section.PointerToRawData;
                else if (rva >= section.VirtualAddress && section.SizeOfRawData != 0 && rva <= section.VirtualAddress + section.SizeOfRawData)
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
        /// <param name="entry">Resource data entry to parse into an accelerator table resource</param>
        /// <returns>A filled accelerator table resource on success, null on error</returns>
        public static Models.PortableExecutable.AcceleratorTableEntry[] AsAcceleratorTableResource(this Models.PortableExecutable.ResourceDataEntry entry)
        {
            // If we have data that's invalid for this resource type, we can't do anything
            if (entry?.Data == null || entry.Data.Length % 8 != 0)
                return null;

            // Get the number of entries
            int count = entry.Data.Length / 8;

            // Initialize the iterator
            int offset = 0;

            // Create the output object
            var table = new Models.PortableExecutable.AcceleratorTableEntry[count];

            // Read in the table
            for (int i = 0; i < count; i++)
            {
                var acceleratorTableEntry = new Models.PortableExecutable.AcceleratorTableEntry();

                acceleratorTableEntry.Flags = (Models.PortableExecutable.AcceleratorTableFlags)entry.Data.ReadUInt16(ref offset);
                acceleratorTableEntry.Ansi = entry.Data.ReadUInt16(ref offset);
                acceleratorTableEntry.Id = entry.Data.ReadUInt16(ref offset);
                acceleratorTableEntry.Padding = entry.Data.ReadUInt16(ref offset);

                table[i] = acceleratorTableEntry;
            }

            return table;
        }

        /// <summary>
        /// Read resource data as a side-by-side assembly manifest
        /// </summary>
        /// <param name="entry">Resource data entry to parse into a side-by-side assembly manifest</param>
        /// <returns>A filled side-by-side assembly manifest on success, null on error</returns>
        public static Models.PortableExecutable.AssemblyManifest AsAssemblyManifest(this Models.PortableExecutable.ResourceDataEntry entry)
        {
            // If we have an invalid entry, just skip
            if (entry?.Data == null)
                return null;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Models.PortableExecutable.AssemblyManifest));
                return serializer.Deserialize(new MemoryStream(entry.Data)) as Models.PortableExecutable.AssemblyManifest;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Read resource data as a dialog box
        /// </summary>
        /// <param name="entry">Resource data entry to parse into a font group</param>
        /// <returns>A filled dialog box on success, null on error</returns>
        public static Models.PortableExecutable.DialogBoxResource AsDialogBox(this Models.PortableExecutable.ResourceDataEntry entry)
        {
            // If we have an invalid entry, just skip
            if (entry?.Data == null)
                return null;

            // Initialize the iterator
            int offset = 0;

            // Create the output object
            var dialogBoxResource = new Models.PortableExecutable.DialogBoxResource();

            // Try to read the signature for an extended dialog box template
            int signatureOffset = sizeof(ushort);
            int possibleSignature = entry.Data.ReadUInt16(ref signatureOffset);
            if (possibleSignature == 0xFFFF)
            {
                #region Extended dialog template

                var dialogTemplateExtended = new Models.PortableExecutable.DialogTemplateExtended();

                dialogTemplateExtended.Version = entry.Data.ReadUInt16(ref offset);
                dialogTemplateExtended.Signature = entry.Data.ReadUInt16(ref offset);
                dialogTemplateExtended.HelpID = entry.Data.ReadUInt32(ref offset);
                dialogTemplateExtended.ExtendedStyle = (Models.PortableExecutable.ExtendedWindowStyles)entry.Data.ReadUInt32(ref offset);
                dialogTemplateExtended.Style = (Models.PortableExecutable.WindowStyles)entry.Data.ReadUInt32(ref offset);
                dialogTemplateExtended.DialogItems = entry.Data.ReadUInt16(ref offset);
                dialogTemplateExtended.PositionX = entry.Data.ReadInt16(ref offset);
                dialogTemplateExtended.PositionY = entry.Data.ReadInt16(ref offset);
                dialogTemplateExtended.WidthX = entry.Data.ReadInt16(ref offset);
                dialogTemplateExtended.HeightY = entry.Data.ReadInt16(ref offset);

                #region Menu resource

                int currentOffset = offset;
                ushort menuResourceIdentifier = entry.Data.ReadUInt16(ref offset);
                offset = currentOffset;

                // 0x0000 means no elements
                if (menuResourceIdentifier == 0x0000)
                {
                    // Increment the pointer if it was empty
                    offset += sizeof(ushort);
                }
                else
                {
                    // Flag if there's an ordinal at the end
                    bool menuResourceHasOrdinal = menuResourceIdentifier == 0xFFFF;
                    if (menuResourceHasOrdinal)
                        offset += sizeof(ushort);

                    // Read the menu resource as a string
                    dialogTemplateExtended.MenuResource = entry.Data.ReadString(ref offset, Encoding.Unicode);

                    // Align to the WORD boundary
                    while ((offset % 2) != 0)
                        _ = entry.Data.ReadByte(ref offset);

                    // Read the ordinal if we have the flag set
                    if (menuResourceHasOrdinal)
                        dialogTemplateExtended.MenuResourceOrdinal = entry.Data.ReadUInt16(ref offset);
                }

                #endregion

                #region Class resource

                currentOffset = offset;
                ushort classResourceIdentifier = entry.Data.ReadUInt16(ref offset);
                offset = currentOffset;

                // 0x0000 means no elements
                if (classResourceIdentifier == 0x0000)
                {
                    // Increment the pointer if it was empty
                    offset += sizeof(ushort);
                }
                else
                {
                    // Flag if there's an ordinal at the end
                    bool classResourcehasOrdinal = classResourceIdentifier == 0xFFFF;
                    if (classResourcehasOrdinal)
                        offset += sizeof(ushort);

                    // Read the class resource as a string
                    dialogTemplateExtended.ClassResource = entry.Data.ReadString(ref offset, Encoding.Unicode);

                    // Align to the WORD boundary
                    while ((offset % 2) != 0)
                        _ = entry.Data.ReadByte(ref offset);

                    // Read the ordinal if we have the flag set
                    if (classResourcehasOrdinal)
                        dialogTemplateExtended.ClassResourceOrdinal = entry.Data.ReadUInt16(ref offset);
                }

                #endregion

                #region Title resource

                currentOffset = offset;
                ushort titleResourceIdentifier = entry.Data.ReadUInt16(ref offset);
                offset = currentOffset;

                // 0x0000 means no elements
                if (titleResourceIdentifier == 0x0000)
                {
                    // Increment the pointer if it was empty
                    offset += sizeof(ushort);
                }
                else
                {
                    // Read the title resource as a string
                    dialogTemplateExtended.TitleResource = entry.Data.ReadString(ref offset, Encoding.Unicode);

                    // Align to the WORD boundary
                    while ((offset % 2) != 0)
                        _ = entry.Data.ReadByte(ref offset);
                }

                #endregion

                #region Point size and typeface

                // Only if DS_SETFONT is set are the values here used
                if (dialogTemplateExtended.Style.HasFlag(Models.PortableExecutable.WindowStyles.DS_SETFONT))
                {
                    dialogTemplateExtended.PointSize = entry.Data.ReadUInt16(ref offset);
                    dialogTemplateExtended.Weight = entry.Data.ReadUInt16(ref offset);
                    dialogTemplateExtended.Italic = entry.Data.ReadByte(ref offset);
                    dialogTemplateExtended.CharSet = entry.Data.ReadByte(ref offset);
                    dialogTemplateExtended.Typeface = entry.Data.ReadString(ref offset, Encoding.Unicode);
                }

                // Align to the DWORD boundary
                while ((offset % 4) != 0)
                    _ = entry.Data.ReadByte(ref offset);

                #endregion

                dialogBoxResource.DialogTemplateExtended = dialogTemplateExtended;

                #endregion

                #region Extended dialog item templates

                // TODO: Implement extended dialog item templates

                #endregion
            }
            else
            {
                #region Dialog template

                var dialogTemplate = new Models.PortableExecutable.DialogTemplate();

                dialogTemplate.Style = (Models.PortableExecutable.WindowStyles)entry.Data.ReadUInt32(ref offset);
                dialogTemplate.ExtendedStyle = (Models.PortableExecutable.ExtendedWindowStyles)entry.Data.ReadUInt32(ref offset);
                dialogTemplate.ItemCount = entry.Data.ReadUInt16(ref offset);
                dialogTemplate.PositionX = entry.Data.ReadInt16(ref offset);
                dialogTemplate.PositionY = entry.Data.ReadInt16(ref offset);
                dialogTemplate.WidthX = entry.Data.ReadInt16(ref offset);
                dialogTemplate.HeightY = entry.Data.ReadInt16(ref offset);

                #region Menu resource

                int currentOffset = offset;
                ushort menuResourceIdentifier = entry.Data.ReadUInt16(ref offset);
                offset = currentOffset;

                // 0x0000 means no elements
                if (menuResourceIdentifier == 0x0000)
                {
                    // Increment the pointer if it was empty
                    offset += sizeof(ushort);
                }
                else
                {
                    // Flag if there's an ordinal at the end
                    bool menuResourceHasOrdinal = menuResourceIdentifier == 0xFFFF;
                    if (menuResourceHasOrdinal)
                        offset += sizeof(ushort);

                    // Read the menu resource as a string
                    dialogTemplate.MenuResource = entry.Data.ReadString(ref offset, Encoding.Unicode);

                    // Align to the WORD boundary
                    while ((offset % 2) != 0)
                        _ = entry.Data.ReadByte(ref offset);

                    // Read the ordinal if we have the flag set
                    if (menuResourceHasOrdinal)
                        dialogTemplate.MenuResourceOrdinal = entry.Data.ReadUInt16(ref offset);
                }

                #endregion

                #region Class resource

                currentOffset = offset;
                ushort classResourceIdentifier = entry.Data.ReadUInt16(ref offset);
                offset = currentOffset;

                // 0x0000 means no elements
                if (classResourceIdentifier == 0x0000)
                {
                    // Increment the pointer if it was empty
                    offset += sizeof(ushort);
                }
                else
                {
                    // Flag if there's an ordinal at the end
                    bool classResourcehasOrdinal = classResourceIdentifier == 0xFFFF;
                    if (classResourcehasOrdinal)
                        offset += sizeof(ushort);

                    // Read the class resource as a string
                    dialogTemplate.ClassResource = entry.Data.ReadString(ref offset, Encoding.Unicode);

                    // Align to the WORD boundary
                    while ((offset % 2) != 0)
                        _ = entry.Data.ReadByte(ref offset);

                    // Read the ordinal if we have the flag set
                    if (classResourcehasOrdinal)
                        dialogTemplate.ClassResourceOrdinal = entry.Data.ReadUInt16(ref offset);
                }

                #endregion

                #region Title resource

                currentOffset = offset;
                ushort titleResourceIdentifier = entry.Data.ReadUInt16(ref offset);
                offset = currentOffset;

                // 0x0000 means no elements
                if (titleResourceIdentifier == 0x0000)
                {
                    // Increment the pointer if it was empty
                    offset += sizeof(ushort);
                }
                else
                {
                    // Read the title resource as a string
                    dialogTemplate.TitleResource = entry.Data.ReadString(ref offset, Encoding.Unicode);

                    // Align to the WORD boundary
                    while ((offset % 2) != 0)
                        _ = entry.Data.ReadByte(ref offset);
                }

                #endregion

                #region Point size and typeface

                // Only if DS_SETFONT is set are the values here used
                if (dialogTemplate.Style.HasFlag(Models.PortableExecutable.WindowStyles.DS_SETFONT))
                {
                    dialogTemplate.PointSizeValue = entry.Data.ReadUInt16(ref offset);

                    // Read the font name as a string
                    dialogTemplate.Typeface = entry.Data.ReadString(ref offset, Encoding.Unicode);
                }

                // Align to the DWORD boundary
                while ((offset % 4) != 0)
                    _ = entry.Data.ReadByte(ref offset);

                #endregion

                dialogBoxResource.DialogTemplate = dialogTemplate;

                #endregion

                #region Dialog Item Templates

                var dialogItemTemplates = new List<Models.PortableExecutable.DialogItemTemplate>();

                for (int i = 0; i < dialogTemplate.ItemCount; i++)
                {
                    var dialogItemTemplate = new Models.PortableExecutable.DialogItemTemplate();

                    dialogItemTemplate.Style = (Models.PortableExecutable.WindowStyles)entry.Data.ReadUInt32(ref offset);
                    dialogItemTemplate.ExtendedStyle = (Models.PortableExecutable.ExtendedWindowStyles)entry.Data.ReadUInt32(ref offset);
                    dialogItemTemplate.PositionX = entry.Data.ReadInt16(ref offset);
                    dialogItemTemplate.PositionY = entry.Data.ReadInt16(ref offset);
                    dialogItemTemplate.WidthX = entry.Data.ReadInt16(ref offset);
                    dialogItemTemplate.HeightY = entry.Data.ReadInt16(ref offset);
                    dialogItemTemplate.ID = entry.Data.ReadUInt16(ref offset);

                    #region Class resource

                    currentOffset = offset;
                    ushort itemClassResourceIdentifier = entry.Data.ReadUInt16(ref offset);
                    offset = currentOffset;

                    // 0xFFFF means ordinal only
                    if (itemClassResourceIdentifier == 0xFFFF)
                    {
                        // Increment the pointer
                        _ = entry.Data.ReadUInt16(ref offset);

                        // Read the ordinal
                        dialogItemTemplate.ClassResourceOrdinal = (Models.PortableExecutable.DialogItemTemplateOrdinal)entry.Data.ReadUInt16(ref offset);
                    }
                    else
                    {
                        // Flag if there's an ordinal at the end
                        bool classResourcehasOrdinal = itemClassResourceIdentifier == 0xFFFF;
                        if (classResourcehasOrdinal)
                            offset += sizeof(ushort);

                        // Read the class resource as a string
                        dialogItemTemplate.ClassResource = entry.Data.ReadString(ref offset, Encoding.Unicode);

                        // Align to the WORD boundary
                        while ((offset % 2) != 0)
                            _ = entry.Data.ReadByte(ref offset);
                    }

                    #endregion

                    #region Title resource

                    currentOffset = offset;
                    ushort itemTitleResourceIdentifier = entry.Data.ReadUInt16(ref offset);
                    offset = currentOffset;

                    // 0xFFFF means ordinal only
                    if (itemTitleResourceIdentifier == 0xFFFF)
                    {
                        // Increment the pointer
                        _ = entry.Data.ReadUInt16(ref offset);

                        // Read the ordinal
                        dialogItemTemplate.TitleResourceOrdinal = entry.Data.ReadUInt16(ref offset);
                    }
                    else
                    {
                        // Read the title resource as a string
                        dialogItemTemplate.TitleResource = entry.Data.ReadString(ref offset, Encoding.Unicode);

                        // Align to the WORD boundary
                        while ((offset % 2) != 0)
                            _ = entry.Data.ReadByte(ref offset);
                    }

                    #endregion

                    #region Creation data

                    dialogItemTemplate.CreationDataSize = entry.Data.ReadUInt16(ref offset);
                    if (dialogItemTemplate.CreationDataSize != 0)
                        dialogItemTemplate.CreationData = entry.Data.ReadBytes(ref offset, dialogItemTemplate.CreationDataSize);

                    #endregion

                    // Align to the DWORD boundary if we're not at the end
                    if (offset != entry.Data.Length)
                    {
                        while ((offset % 4) != 0)
                            _ = entry.Data.ReadByte(ref offset);
                    }

                    dialogItemTemplates.Add(dialogItemTemplate);
                }

                dialogBoxResource.DialogItemTemplates = dialogItemTemplates.ToArray();

                #endregion
            }

            return dialogBoxResource;
        }

        /// <summary>
        /// Read resource data as a font group
        /// </summary>
        /// <param name="entry">Resource data entry to parse into a font group</param>
        /// <returns>A filled font group on success, null on error</returns>
        public static Models.PortableExecutable.FontGroupHeader AsFontGroup(this Models.PortableExecutable.ResourceDataEntry entry)
        {
            // If we have an invalid entry, just skip
            if (entry?.Data == null)
                return null;

            // Initialize the iterator
            int offset = 0;

            // Create the output object
            var fontGroupHeader = new Models.PortableExecutable.FontGroupHeader();

            fontGroupHeader.NumberOfFonts = entry.Data.ReadUInt16(ref offset);
            if (fontGroupHeader.NumberOfFonts > 0)
            {
                fontGroupHeader.DE = new Models.PortableExecutable.DirEntry[fontGroupHeader.NumberOfFonts];
                for (int i = 0; i < fontGroupHeader.NumberOfFonts; i++)
                {
                    var dirEntry = new Models.PortableExecutable.DirEntry();

                    dirEntry.FontOrdinal = entry.Data.ReadUInt16(ref offset);

                    dirEntry.Entry = new Models.PortableExecutable.FontDirEntry();
                    dirEntry.Entry.Version = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.Size = entry.Data.ReadUInt32(ref offset);
                    dirEntry.Entry.Copyright = entry.Data.ReadBytes(ref offset, 60);
                    dirEntry.Entry.Type = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.Points = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.VertRes = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.HorizRes = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.Ascent = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.InternalLeading = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.ExternalLeading = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.Italic = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.Underline = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.StrikeOut = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.Weight = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.CharSet = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.PixWidth = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.PixHeight = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.PitchAndFamily = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.AvgWidth = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.MaxWidth = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.FirstChar = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.LastChar = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.DefaultChar = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.BreakChar = entry.Data.ReadByte(ref offset);
                    dirEntry.Entry.WidthBytes = entry.Data.ReadUInt16(ref offset);
                    dirEntry.Entry.Device = entry.Data.ReadUInt32(ref offset);
                    dirEntry.Entry.Face = entry.Data.ReadUInt32(ref offset);
                    dirEntry.Entry.Reserved = entry.Data.ReadUInt32(ref offset);
                
                    // TODO: Determine how to read these two? Immediately after?
                    dirEntry.Entry.DeviceName = entry.Data.ReadString(ref offset);
                    dirEntry.Entry.FaceName = entry.Data.ReadString(ref offset);

                    fontGroupHeader.DE[i] = dirEntry;
                }
            }

            // TODO: Implement entry parsing
            return fontGroupHeader;
        }

        /// <summary>
        /// Read resource data as a string table resource
        /// </summary>
        /// <param name="entry">Resource data entry to parse into a string table resource</param>
        /// <returns>A filled string table resource on success, null on error</returns>
        public static Dictionary<int, string> AsStringTable(this Models.PortableExecutable.ResourceDataEntry entry)
        {
            // If we have an invalid entry, just skip
            if (entry?.Data == null)
                return null;

            // Initialize the iterators
            int offset = 0, stringIndex = 0;

            // Create the output table
            var stringTable = new Dictionary<int, string>();

            // Create the string encoding
            Encoding stringEncoding = (entry.Codepage != 0 ? Encoding.GetEncoding((int)entry.Codepage) : Encoding.Unicode);
            
            // Loop through and add 
            while (offset < entry.Data.Length)
            {
                ushort stringLength = entry.Data.ReadUInt16(ref offset);
                if (stringLength == 0)
                {
                    stringTable[stringIndex++] = "[EMPTY]";
                }
                else
                {
                    string fullEncodedString = stringEncoding.GetString(entry.Data, offset, entry.Data.Length - offset);
                    if (stringLength > fullEncodedString.Length)
                    {
                        // TODO: Do something better than print to console
                        Console.WriteLine($"Requested {stringLength} but only have {fullEncodedString.Length} remaining, truncating...");
                        stringLength = (ushort)fullEncodedString.Length;
                    }

                    string stringValue = fullEncodedString.Substring(0, stringLength);
                    offset += stringEncoding.GetByteCount(stringValue);
                    stringValue = stringValue.Replace("\n", "\\n").Replace("\r", "\\r");
                    stringTable[stringIndex++] = stringValue;
                }
            }

            return stringTable;
        }

        /// <summary>
        /// Read resource data as a version info resource
        /// </summary>
        /// <param name="entry">Resource data entry to parse into a version info resource</param>
        /// <returns>A filled version info resource on success, null on error</returns>
        public static Models.PortableExecutable.VersionInfo AsVersionInfo(this Models.PortableExecutable.ResourceDataEntry entry)
        {
            // If we have an invalid entry, just skip
            if (entry?.Data == null)
                return null;

            // Initialize the iterator
            int offset = 0;

            // Create the output object
            var versionInfo = new Models.PortableExecutable.VersionInfo();

            versionInfo.Length = entry.Data.ReadUInt16(ref offset);
            versionInfo.ValueLength = entry.Data.ReadUInt16(ref offset);
            versionInfo.ResourceType = (Models.PortableExecutable.VersionResourceType)entry.Data.ReadUInt16(ref offset);
            versionInfo.Key = entry.Data.ReadString(ref offset, Encoding.Unicode);
            if (versionInfo.Key != "VS_VERSION_INFO")
                return null;

            while ((offset % 4) != 0)
                versionInfo.Padding1 = entry.Data.ReadUInt16(ref offset);

            // Read fixed file info
            if (versionInfo.ValueLength != 0)
            {
                var fixedFileInfo = new Models.PortableExecutable.FixedFileInfo();
                fixedFileInfo.Signature = entry.Data.ReadUInt32(ref offset);
                if (fixedFileInfo.Signature != 0xFEEF04BD)
                    return null;

                fixedFileInfo.StrucVersion = entry.Data.ReadUInt32(ref offset);
                fixedFileInfo.FileVersionMS = entry.Data.ReadUInt32(ref offset);
                fixedFileInfo.FileVersionLS = entry.Data.ReadUInt32(ref offset);
                fixedFileInfo.ProductVersionMS = entry.Data.ReadUInt32(ref offset);
                fixedFileInfo.ProductVersionLS = entry.Data.ReadUInt32(ref offset);
                fixedFileInfo.FileFlagsMask = entry.Data.ReadUInt32(ref offset);
                fixedFileInfo.FileFlags = (Models.PortableExecutable.FixedFileInfoFlags)(entry.Data.ReadUInt32(ref offset) & fixedFileInfo.FileFlagsMask);
                fixedFileInfo.FileOS = (Models.PortableExecutable.FixedFileInfoOS)entry.Data.ReadUInt32(ref offset);
                fixedFileInfo.FileType = (Models.PortableExecutable.FixedFileInfoFileType)entry.Data.ReadUInt32(ref offset);
                fixedFileInfo.FileSubtype = (Models.PortableExecutable.FixedFileInfoFileSubtype)entry.Data.ReadUInt32(ref offset);
                fixedFileInfo.FileDateMS = entry.Data.ReadUInt32(ref offset);
                fixedFileInfo.FileDateLS = entry.Data.ReadUInt32(ref offset);
                versionInfo.Value = fixedFileInfo;
            }

            while ((offset % 4) != 0)
                versionInfo.Padding2 = entry.Data.ReadUInt16(ref offset);

            // TODO: Make the following block a private helper method

            // Determine if we have a StringFileInfo or VarFileInfo next
            if (offset < versionInfo.Length)
            {
                // Cache the current offset for reading
                int currentOffset = offset;

                offset += 6;
                string nextKey = entry.Data.ReadString(ref offset, Encoding.Unicode);
                offset = currentOffset;

                if (nextKey == "StringFileInfo")
                {
                    var stringFileInfo = new Models.PortableExecutable.StringFileInfo();

                    stringFileInfo.Length = entry.Data.ReadUInt16(ref offset);
                    stringFileInfo.ValueLength = entry.Data.ReadUInt16(ref offset);
                    stringFileInfo.ResourceType = (Models.PortableExecutable.VersionResourceType)entry.Data.ReadUInt16(ref offset);
                    stringFileInfo.Key = entry.Data.ReadString(ref offset, Encoding.Unicode);
                    if (stringFileInfo.Key != "StringFileInfo")
                        return null;

                    while ((offset % 4) != 0)
                        stringFileInfo.Padding = entry.Data.ReadUInt16(ref offset);

                    var stringFileInfoChildren = new List<Models.PortableExecutable.StringTable>();
                    while (offset < stringFileInfo.Length)
                    {
                        var stringTable = new Models.PortableExecutable.StringTable();

                        stringTable.Length = entry.Data.ReadUInt16(ref offset);
                        stringTable.ValueLength = entry.Data.ReadUInt16(ref offset);
                        stringTable.ResourceType = (Models.PortableExecutable.VersionResourceType)entry.Data.ReadUInt16(ref offset);
                        stringTable.Key = entry.Data.ReadString(ref offset, Encoding.Unicode);

                        while ((offset % 4) != 0)
                            stringTable.Padding = entry.Data.ReadUInt16(ref offset);

                        var stringTableChildren = new List<Models.PortableExecutable.StringData>();
                        while (offset < stringTable.Length)
                        {
                            var stringData = new Models.PortableExecutable.StringData();

                            stringData.Length = entry.Data.ReadUInt16(ref offset);
                            stringData.ValueLength = entry.Data.ReadUInt16(ref offset);
                            stringData.ResourceType = (Models.PortableExecutable.VersionResourceType)entry.Data.ReadUInt16(ref offset);
                            stringData.Key = entry.Data.ReadString(ref offset, Encoding.Unicode);

                            while ((offset % 4) != 0)
                                stringData.Padding = entry.Data.ReadUInt16(ref offset);

                            stringData.Value = entry.Data.ReadString(ref offset, Encoding.Unicode);

                            while ((offset % 4) != 0)
                                _ = entry.Data.ReadUInt16(ref offset);

                            stringTableChildren.Add(stringData);
                        }

                        stringTable.Children = stringTableChildren.ToArray();

                        stringFileInfoChildren.Add(stringTable);
                    }

                    stringFileInfo.Children = stringFileInfoChildren.ToArray();

                    versionInfo.StringFileInfo = stringFileInfo;
                }
                else if (nextKey == "VarFileInfo")
                {
                    var varFileInfo = new Models.PortableExecutable.VarFileInfo();

                    varFileInfo.Length = entry.Data.ReadUInt16(ref offset);
                    varFileInfo.ValueLength = entry.Data.ReadUInt16(ref offset);
                    varFileInfo.ResourceType = (Models.PortableExecutable.VersionResourceType)entry.Data.ReadUInt16(ref offset);
                    varFileInfo.Key = entry.Data.ReadString(ref offset, Encoding.Unicode);
                    if (varFileInfo.Key != "VarFileInfo")
                        return null;

                    while ((offset % 4) != 0)
                        varFileInfo.Padding = entry.Data.ReadUInt16(ref offset);

                    var varFileInfoChildren = new List<Models.PortableExecutable.VarData>();
                    while (offset < varFileInfo.Length)
                    {
                        var varData = new Models.PortableExecutable.VarData();

                        varData.Length = entry.Data.ReadUInt16(ref offset);
                        varData.ValueLength = entry.Data.ReadUInt16(ref offset);
                        varData.ResourceType = (Models.PortableExecutable.VersionResourceType)entry.Data.ReadUInt16(ref offset);
                        varData.Key = entry.Data.ReadString(ref offset, Encoding.Unicode);
                        if (varData.Key != "Translation")
                            return null;

                        while ((offset % 4) != 0)
                            varData.Padding = entry.Data.ReadUInt16(ref offset);

                        var varDataValue = new List<uint>();
                        while (offset < (varData.ValueLength * sizeof(ushort)))
                        {
                            uint languageAndCodeIdentifierPair = entry.Data.ReadUInt32(ref offset);
                            varDataValue.Add(languageAndCodeIdentifierPair);
                        }

                        varData.Value = varDataValue.ToArray();

                        varFileInfoChildren.Add(varData);
                    }

                    varFileInfo.Children = varFileInfoChildren.ToArray();

                    versionInfo.VarFileInfo = varFileInfo;
                }
            }

            // And again
            if (offset < versionInfo.Length)
            {
                // Cache the current offset for reading
                int currentOffset = offset;

                offset += 6;
                string nextKey = entry.Data.ReadString(ref offset, Encoding.Unicode);
                offset = currentOffset;

                if (nextKey == "StringFileInfo")
                {
                    var stringFileInfo = new Models.PortableExecutable.StringFileInfo();

                    stringFileInfo.Length = entry.Data.ReadUInt16(ref offset);
                    stringFileInfo.ValueLength = entry.Data.ReadUInt16(ref offset);
                    stringFileInfo.ResourceType = (Models.PortableExecutable.VersionResourceType)entry.Data.ReadUInt16(ref offset);
                    stringFileInfo.Key = entry.Data.ReadString(ref offset, Encoding.Unicode);
                    if (stringFileInfo.Key != "StringFileInfo")
                        return null;

                    while ((offset % 4) != 0)
                        stringFileInfo.Padding = entry.Data.ReadUInt16(ref offset);

                    var stringFileInfoChildren = new List<Models.PortableExecutable.StringTable>();
                    while (offset < stringFileInfo.Length)
                    {
                        var stringTable = new Models.PortableExecutable.StringTable();

                        stringTable.Length = entry.Data.ReadUInt16(ref offset);
                        stringTable.ValueLength = entry.Data.ReadUInt16(ref offset);
                        stringTable.ResourceType = (Models.PortableExecutable.VersionResourceType)entry.Data.ReadUInt16(ref offset);
                        stringTable.Key = entry.Data.ReadString(ref offset, Encoding.Unicode);

                        while ((offset % 4) != 0)
                            stringTable.Padding = entry.Data.ReadUInt16(ref offset);

                        var stringTableChildren = new List<Models.PortableExecutable.StringData>();
                        while (offset < stringTable.Length)
                        {
                            var stringData = new Models.PortableExecutable.StringData();

                            stringData.Length = entry.Data.ReadUInt16(ref offset);
                            stringData.ValueLength = entry.Data.ReadUInt16(ref offset);
                            stringData.ResourceType = (Models.PortableExecutable.VersionResourceType)entry.Data.ReadUInt16(ref offset);
                            stringData.Key = entry.Data.ReadString(ref offset, Encoding.Unicode);

                            while ((offset % 4) != 0)
                                stringData.Padding = entry.Data.ReadUInt16(ref offset);

                            stringData.Value = entry.Data.ReadString(ref offset, Encoding.Unicode);

                            while ((offset % 4) != 0)
                                _ = entry.Data.ReadUInt16(ref offset);

                            stringTableChildren.Add(stringData);
                        }

                        stringTable.Children = stringTableChildren.ToArray();

                        stringFileInfoChildren.Add(stringTable);
                    }

                    stringFileInfo.Children = stringFileInfoChildren.ToArray();

                    versionInfo.StringFileInfo = stringFileInfo;
                }
                else if (nextKey == "VarFileInfo")
                {
                    var varFileInfo = new Models.PortableExecutable.VarFileInfo();

                    varFileInfo.Length = entry.Data.ReadUInt16(ref offset);
                    varFileInfo.ValueLength = entry.Data.ReadUInt16(ref offset);
                    varFileInfo.ResourceType = (Models.PortableExecutable.VersionResourceType)entry.Data.ReadUInt16(ref offset);
                    varFileInfo.Key = entry.Data.ReadString(ref offset, Encoding.Unicode);
                    if (varFileInfo.Key != "VarFileInfo")
                        return null;

                    while ((offset % 4) != 0)
                        varFileInfo.Padding = entry.Data.ReadUInt16(ref offset);

                    var varFileInfoChildren = new List<Models.PortableExecutable.VarData>();
                    while (offset < varFileInfo.Length)
                    {
                        var varData = new Models.PortableExecutable.VarData();

                        varData.Length = entry.Data.ReadUInt16(ref offset);
                        varData.ValueLength = entry.Data.ReadUInt16(ref offset);
                        varData.ResourceType = (Models.PortableExecutable.VersionResourceType)entry.Data.ReadUInt16(ref offset);
                        varData.Key = entry.Data.ReadString(ref offset, Encoding.Unicode);
                        if (varData.Key != "Translation")
                            return null;

                        while ((offset % 4) != 0)
                            varData.Padding = entry.Data.ReadUInt16(ref offset);

                        var varDataValue = new List<uint>();
                        while (offset < (varData.ValueLength * sizeof(ushort)))
                        {
                            uint languageAndCodeIdentifierPair = entry.Data.ReadUInt32(ref offset);
                            varDataValue.Add(languageAndCodeIdentifierPair);
                        }

                        varData.Value = varDataValue.ToArray();

                        varFileInfoChildren.Add(varData);
                    }

                    varFileInfo.Children = varFileInfoChildren.ToArray();

                    versionInfo.VarFileInfo = varFileInfo;
                }
            }

            return versionInfo;
        }

        #endregion
    }
}