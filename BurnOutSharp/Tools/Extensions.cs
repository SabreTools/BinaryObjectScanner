using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.Matching;

namespace BurnOutSharp.Tools
{
    internal static class Extensions
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

        /// <summary>
        /// Find all positions of one array in another, if possible, if possible
        /// </summary>
        public static List<int> FindAllPositions(this byte[] stack, byte?[] needle, int start = 0, int end = -1)
        {
            // Get the outgoing list
            List<int> positions = new List<int>();

            // Initialize the loop variables
            bool found = true;
            int lastPosition = start;
            var matcher = new ContentMatch(needle, end: end);

            // Loop over and get all positions
            while (found)
            {
                matcher.Start = lastPosition;
                (found, lastPosition) = matcher.Match(stack, false);
                if (found)
                    positions.Add(lastPosition);
            }

            return positions;
        }

        /// <summary>
        /// Find the first position of one array in another, if possible
        /// </summary>
        public static bool FirstPosition(this byte[] stack, byte[] needle, out int position, int start = 0, int end = -1)
        {
            byte?[] nullableNeedle = needle != null ? needle.Select(b => (byte?)b).ToArray() : null;
            return stack.FirstPosition(nullableNeedle, out position, start, end);
        }

        /// <summary>
        /// Find the first position of one array in another, if possible
        /// </summary>
        public static bool FirstPosition(this byte[] stack, byte?[] needle, out int position, int start = 0, int end = -1)
        {
            var matcher = new ContentMatch(needle, start, end);
            (bool found, int foundPosition) = matcher.Match(stack, false);
            position = foundPosition;
            return found;
        }

        /// <summary>
        /// Find the last position of one array in another, if possible
        /// </summary>
        public static bool LastPosition(this byte[] stack, byte?[] needle, out int position, int start = 0, int end = -1)
        {
            var matcher = new ContentMatch(needle, start, end);
            (bool found, int foundPosition) = matcher.Match(stack, true);
            position = foundPosition;
            return found;
        }

        /// <summary>
        /// See if a byte array starts with another
        /// </summary>
        public static bool StartsWith(this byte[] stack, byte?[] needle)
        {
            return stack.FirstPosition(needle, out int _, start: 0, end: 1);
        }

        /// <summary>
        /// See if a byte array ends with another
        /// </summary>
        public static bool EndsWith(this byte[] stack, byte?[] needle)
        {
            return stack.FirstPosition(needle, out int _, start: stack.Length - needle.Length);
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
    }
}