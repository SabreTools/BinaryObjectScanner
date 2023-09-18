using System;
using System.Collections;
using System.Collections.Concurrent;

namespace BinaryObjectScanner.Utilities
{
    public static class Extensions
    {
        #region ConcurrentQueue

        /// <summary>
        /// Add a range of values from one queue to another
        /// </summary>
        /// <param name="original">Queue to add data to</param>
        /// <param name="values">Array to get data from</param>
        public static void AddRange(this ConcurrentQueue<string> original, string[] values)
        {
            if (values == null || values.Length == 0)
                return;

            for (int i = 0; i < values.Length; i++)
            {
                original.Enqueue(values[i]);
            }
        }

        /// <summary>
        /// Add a range of values from one queue to another
        /// </summary>
        /// <param name="original">Queue to add data to</param>
        /// <param name="values">Queue to get data from</param>
        public static void AddRange(this ConcurrentQueue<string> original, ConcurrentQueue<string> values)
        {
            while (!values.IsEmpty)
            {
                if (!values.TryDequeue(out var value))
                    return;

                original.Enqueue(value);
            }
        }

        #endregion

        #region BitArray

        /// <summary>
        /// Convert a bit array into a byte
        /// </summary>
        public static byte AsByte(this BitArray array)
        {
            byte value = 0;

            int maxValue = Math.Min(8, array.Length);
            for (int i = maxValue - 1; i >= 0; i--)
            {
                value <<= 1;
                value |= (byte)(array[i] ? 1 : 0);
            }

            return value;
        }

        /// <summary>
        /// Convert a bit array into an sbyte
        /// </summary>
        public static sbyte AsSByte(this BitArray array)
        {
            sbyte value = 0;

            int maxValue = Math.Min(val1: 8, array.Length);
            for (int i = maxValue - 1; i >= 0; i--)
            {
                value <<= 1;
                value |= (sbyte)(array[i] ? 1 : 0);
            }

            return value;
        }

        /// <summary>
        /// Convert a bit array into a short
        /// </summary>
        public static short AsInt16(this BitArray array)
        {
            short value = 0;

            int maxValue = Math.Min(16, array.Length);
            for (int i = maxValue - 1; i >= 0; i--)
            {
                value <<= 1;
                value |= (short)(array[i] ? 1 : 0);
            }

            return value;
        }

        /// <summary>
        /// Convert a bit array into a ushort
        /// </summary>
        public static ushort AsUInt16(this BitArray array)
        {
            ushort value = 0;

            int maxValue = Math.Min(16, array.Length);
            for (int i = maxValue - 1; i >= 0; i--)
            {
                value <<= 1;
                value |= (ushort)(array[i] ? 1 : 0);
            }

            return value;
        }

        /// <summary>
        /// Convert a bit array into an int
        /// </summary>
        public static int AsInt32(this BitArray array)
        {
            int value = 0;

            int maxValue = Math.Min(32, array.Length);
            for (int i = maxValue - 1; i >= 0; i--)
            {
                value <<= 1;
                value |= (int)(array[i] ? 1 : 0);
            }

            return value;
        }

        /// <summary>
        /// Convert a bit array into a uint
        /// </summary>
        public static uint AsUInt32(this BitArray array)
        {
            uint value = 0;

            int maxValue = Math.Min(32, array.Length);
            for (int i = maxValue - 1; i >= 0; i--)
            {
                value <<= 1;
                value |= (uint)(array[i] ? 1 : 0);
            }

            return value;
        }

        /// <summary>
        /// Convert a bit array into a long
        /// </summary>
        public static long AsInt64(this BitArray array)
        {
            long value = 0;

            int maxValue = Math.Min(64, array.Length);
            for (int i = maxValue - 1; i >= 0; i--)
            {
                value <<= 1;
                value |= (long)(array[i] ? 1 : 0);
            }

            return value;
        }

        /// <summary>
        /// Convert a bit array into a ulong
        /// </summary>
        public static ulong AsUInt64(this BitArray array)
        {
            ulong value = 0;

            int maxValue = Math.Min(64, array.Length);
            for (int i = maxValue - 1; i >= 0; i--)
            {
                value <<= 1;
                value |= (ulong)(array[i] ? 1 : 0);
            }

            return value;
        }

        #endregion
    }
}