using System;
using System.IO;
using System.Text;

namespace BurnOutSharp
{
    internal static class Ebuffertensions
    {
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
        public static char[] ReadChars(this Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            stream.Read(buffer, 0, count);
            return Encoding.Default.GetString(buffer).ToCharArray();
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
    }
}