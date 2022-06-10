/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-output-iochannel.c
 *
 * Copyright (C) 2002-2006 Dom Lachowicz (cinamod@hotmail.com)
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
using System.IO;

namespace LibGSF.Output
{
    public class GsfOutputIOChannel : GsfOutput
    {
        // TODO: Enable once GIOChannel is converted

        //#region Properties

        //public GIOChannel Channel { get; set; } = null;

        //#endregion

        //#region Constructor and Destructor

        ///// <returns>A new file or null.</returns>
        //public static GsfOutputIOChannel Create(GIOChannel channel)
        //{
        //    if (channel == null)
        //        return null;

        //    return new GsfOutputIOChannel
        //    {
        //        Channel = channel,
        //    };
        //}

        //#endregion

        //#region Functions

        ///// <inheritdoc/>
        //protected override bool CloseImpl()
        //{
        //    g_io_channel_shutdown(Channel, true, null);
        //    return true;
        //}

        ///// <inheritdoc/>
        //protected override bool SeekImpl(long offset, SeekOrigin whence)
        //{
        //    if (!Channel.IsSeekable)
        //        return false;

        //    GIOStatus status = g_io_channel_seek_position(Channel, offset, whence, null);
        //    if (status == G_IO_STATUS_NORMAL)
        //        return true;

        //    Error = new Exception($"{status}?");
        //    return false;
        //}

        ///// <inheritdoc/>
        //protected override bool WriteImpl(int num_bytes, byte[] data)
        //{
        //    GIOStatus status = G_IO_STATUS_NORMAL;
        //    int bytes_written = 0, total_written = 0;

        //    while ((status == G_IO_STATUS_NORMAL) && (total_written < num_bytes))
        //    {
        //        status = g_io_channel_write_chars(Channel, data + total_written, num_bytes - total_written, ref bytes_written, null);
        //        total_written += bytes_written;
        //    }

        //    return (status == G_IO_STATUS_NORMAL && total_written == num_bytes);
        //}

        //#endregion
    }
}
