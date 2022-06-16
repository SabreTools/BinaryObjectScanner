/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-utils.c:
 *
 * Copyright (C) 2002-2006 Jody Goldberg (jody@gnome.org)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of version 2.1 of the GNU Lesser General Public
 * License as published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System;

namespace LibGSF
{
    /// <summary>
    /// Do this the ugly way so that we don't have to worry about alignment
    /// </summary>
    internal static class GsfUtils
    {
        #region GET

        /// <summary>
        /// Interpret binary data as an unsigned 8-bit integer in little endian order.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <returns>Interpreted data</returns>
        public static byte GSF_LE_GET_GUINT8(byte[] data, int p)
            => data[p];

        /// <summary>
        /// Interpret binary data as an unsigned 16-bit integer in little endian order.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <returns>Interpreted data</returns>
        public static ushort GSF_LE_GET_GUINT16(byte[] data, int p)
            => (ushort)((data[p] << 0)
                | (data[p + 1] << 8));

        /// <summary>
        /// Interpret binary data as an unsigned 32-bit integer in little endian order.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <returns>Interpreted data</returns>
        public static uint GSF_LE_GET_GUINT32(byte[] data, int p)
            => (uint)((data[p] << 0)
                | (data[p + 1] << 8)
                | (data[p + 2] << 16)
                | (data[p + 3] << 24));

        /// <summary>
        /// Interpret binary data as an unsigned 64-bit integer in little endian order.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <returns>Interpreted data</returns>
        public static ulong GSF_LE_GET_GUINT64(byte[] data, int p)
            => (uint)((data[p] << 0)
                | (data[p + 1] << 8)
                | (data[p + 2] << 16)
                | (data[p + 3] << 24)
                | (data[p + 4] << 32)
                | (data[p + 5] << 40)
                | (data[p + 6] << 48)
                | (data[p + 7] << 56));

        /// <summary>
        /// Interpret binary data as a signed 8-bit integer in little endian order.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <returns>Interpreted data</returns>
        public static sbyte GSF_LE_GET_GINT8(byte[] data, int p) => (sbyte)GSF_LE_GET_GINT8(data, p);

        /// <summary>
        /// Interpret binary data as a signed 16-bit integer in little endian order.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <returns>Interpreted data</returns>
        public static short GSF_LE_GET_GINT16(byte[] data, int p) => (short)GSF_LE_GET_GUINT16(data, p);

        /// <summary>
        /// Interpret binary data as a signed 32-bit integer in little endian order.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <returns>Interpreted data</returns>
        public static int GSF_LE_GET_GINT32(byte[] data, int p) => (int)GSF_LE_GET_GUINT32(data, p);

        /// <summary>
        /// Interpret binary data as a signed 64-bit integer in little endian order.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <returns>Interpreted data</returns>
        public static long GSF_LE_GET_GINT64(byte[] data, int p) => (long)GSF_LE_GET_GUINT64(data, p);

        /// <summary>
        /// Interpret binary data as a float in little endian order.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <returns>Interpreted data</returns>
        public static float GSF_LE_GET_FLOAT(byte[] data, int p)
        {
            byte[] temp = new ReadOnlySpan<byte>(data, p, 4).ToArray();
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(temp);

            return BitConverter.ToSingle(temp, 0);
        }

        /// <summary>
        /// Interpret binary data as a double in little endian order.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <returns>Interpreted data</returns>
        public static double GSF_LE_GET_DOUBLE(byte[] data, int p)
        {
            byte[] temp = new ReadOnlySpan<byte>(data, p, 8).ToArray();
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(temp);

            return BitConverter.ToDouble(temp, 0);
        }

        #endregion

        #region SET

        /// <summary>
        /// Store <paramref name="dat"/> in little endian order in memory pointed to by <paramref name="p"/>.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <param name="dat">8-bit unsigned integer</param>
        public static void GSF_LE_SET_GUINT8(byte[] data, int p, byte dat)
        {
            data[p] = (byte)(dat & 0xff);
        }

        /// <summary>
        /// Store <paramref name="dat"/> in little endian order in memory pointed to by <paramref name="p"/>.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <param name="dat">16-bit unsigned integer</param>
        public static void GSF_LE_SET_GUINT16(byte[] data, int p, ushort dat)
        {
            data[p + 0] = (byte)(dat & 0xff);
            data[p + 1] = (byte)((dat >> 8) & 0xff);
        }

        /// <summary>
        /// Store <paramref name="dat"/> in little endian order in memory pointed to by <paramref name="p"/>.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <param name="dat">32-bit unsigned integer</param>
        public static void GSF_LE_SET_GUINT32(byte[] data, int p, uint dat)
        {
            data[p + 0] = (byte)(dat & 0xff);
            data[p + 1] = (byte)((dat >> 8) & 0xff);
            data[p + 2] = (byte)((dat >> 16) & 0xff);
            data[p + 3] = (byte)((dat >> 24) & 0xff);
        }

        /// <summary>
        /// Store <paramref name="dat"/> in little endian order in memory pointed to by <paramref name="p"/>.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <param name="dat">64-bit unsigned integer</param>
        public static void GSF_LE_SET_GUINT64(byte[] data, int p, ulong dat)
        {
            data[p + 0] = (byte)(dat & 0xff);
            data[p + 1] = (byte)((dat >> 8) & 0xff);
            data[p + 2] = (byte)((dat >> 16) & 0xff);
            data[p + 3] = (byte)((dat >> 24) & 0xff);
            data[p + 4] = (byte)((dat >> 32) & 0xff);
            data[p + 5] = (byte)((dat >> 40) & 0xff);
            data[p + 6] = (byte)((dat >> 48) & 0xff);
            data[p + 7] = (byte)((dat >> 56) & 0xff);
        }

        /// <summary>
        /// Store <paramref name="dat"/> in little endian order in memory pointed to by <paramref name="p"/>.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <param name="dat">8-bit signed integer</param>
        public static void GSF_LE_SET_GINT8(byte[] data, int p, sbyte dat) => GSF_LE_SET_GUINT8(data, p, (byte)dat);

        /// <summary>
        /// Store <paramref name="dat"/> in little endian order in memory pointed to by <paramref name="p"/>.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <param name="dat">16-bit signed integer</param>
        public static void GSF_LE_SET_GINT16(byte[] data, int p, short dat) => GSF_LE_SET_GUINT16(data, p, (ushort)dat);

        /// <summary>
        /// Store <paramref name="dat"/> in little endian order in memory pointed to by <paramref name="p"/>.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <param name="dat">32-bit signed integer</param>
        public static void GSF_LE_SET_GINT32(byte[] data, int p, int dat) => GSF_LE_SET_GUINT32(data, p, (uint)dat);

        /// <summary>
        /// Store <paramref name="dat"/> in little endian order in memory pointed to by <paramref name="p"/>.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <param name="dat">64-bit signed integer</param>
        public static void GSF_LE_SET_GINT64(byte[] data, int p, long dat) => GSF_LE_SET_GUINT64(data, p, (ulong)dat);

        /// <summary>
        /// Store <paramref name="dat"/> in little endian order in memory pointed to by <paramref name="p"/>.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <param name="dat">Float to be stored</param>
        public static void GSF_LE_SET_FLOAT(byte[] data, int p, float dat)
        {
            byte[] temp = BitConverter.GetBytes(dat);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(temp);

            Array.Copy(temp, 0, data, p, 4);
        }

        /// <summary>
        /// Store <paramref name="dat"/> in little endian order in memory pointed to by <paramref name="p"/>.
        /// </summary>
        /// <param name="data">Storage</param>
        /// <param name="p">Pointer to storage</param>
        /// <param name="dat">Double to be stored</param>
        public static void GSF_LE_SET_DOUBLE(byte[] data, int p, double dat)
        {
            byte[] temp = BitConverter.GetBytes(dat);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(temp);

            Array.Copy(temp, 0, data, p, 8);
        }

        #endregion

    }
}
