using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BurnOutSharp.Utilities
{
    public static class Extensions
    {
        #region ConcurrentQueue

        /// <summary>
        /// Add a range of values from one queue to another
        /// </summary>
        /// <param name="original">Queue to add data to</param>
        /// <param name="values">Queue to get data from</param>
        public static void AddRange(this ConcurrentQueue<string> original, ConcurrentQueue<string> values)
        {
            while (!values.IsEmpty)
            {
                if (!values.TryDequeue(out string value))
                    return;

                original.Enqueue(value);
            }
        }

        #endregion

        #region Byte Array Reading

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
            // If there's an invalid byte count, don't do anything
            if (count <= 0)
                return null;

            byte[] buffer = new byte[count];
            Array.Copy(content, offset, buffer, 0, Math.Min(count, content.Length - offset));
            offset += count;
            return buffer;
        }

        /// <summary>
        /// Read an sbyte and increment the pointer to an array
        /// </summary>
        public static sbyte ReadSByte(this byte[] content, ref int offset)
        {
            return (sbyte)content[offset++];
        }

        /// <summary>
        /// Read a char and increment the pointer to an array
        /// </summary>
        public static char ReadChar(this byte[] content, ref int offset)
        {
            return (char)content[offset++];
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
            if (offset >= content.Length)
                return null;

            byte[] nullTerminator = encoding.GetBytes(new char[] { '\0' });
            int charWidth = nullTerminator.Length;

            List<char> keyChars = new List<char>();
            while (offset < content.Length)
            {
                char c = encoding.GetChars(content, offset, charWidth)[0];
                keyChars.Add(c);
                offset += charWidth;

                if (c == '\0')
                    break;
            }

            return new string(keyChars.ToArray()).TrimEnd('\0');
        }

        #endregion

        #region Stream Reading

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
            // If there's an invalid byte count, don't do anything
            if (count <= 0)
                return null;

            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);
            return buffer;
        }

        /// <summary>
        /// Read an sbyte from the stream
        /// </summary>
        public static sbyte ReadSByte(this Stream stream)
        {
            byte[] buffer = new byte[1];
            stream.Read(buffer, 0, 1);
            return (sbyte)buffer[0];
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
            if (stream.Position >= stream.Length)
                return null;

            byte[] nullTerminator = encoding.GetBytes(new char[] { '\0' });
            int charWidth = nullTerminator.Length;

            List<byte> tempBuffer = new List<byte>();

            byte[] buffer = new byte[charWidth];
            while (stream.Position < stream.Length && stream.Read(buffer, 0, charWidth) != 0 && !buffer.SequenceEqual(nullTerminator))
            {
                tempBuffer.AddRange(buffer);
            }

            return encoding.GetString(tempBuffer.ToArray());
        }

        #endregion
    }
}