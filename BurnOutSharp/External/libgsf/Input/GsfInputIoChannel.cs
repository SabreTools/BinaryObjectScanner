/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-input-iochannel.c: GIOChannel based input
 *
 * Copyright (C) 2003-2006 Rodrigo Moya (rodrigo@gnome-db.org)
 * Copyright (C) 2003-2006 Dom Lachowicz (cinamod@hotmail.com)
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

namespace LibGSF.Input
{
    public partial class GsfInputMemory
    {
        // TODO: Enable once GIOChannel is converted

        ///// <returns>A new GsfInputMemory or null.</returns>
        //public static GsfInputMemory CreateFromIoChannel(GIOChannel channel, ref Exception err)
        //{
        //    if (channel == null)
        //        return null;

        //    if (G_IO_STATUS_NORMAL != channel.ReadToEnd(out byte[] buf, out int len, err))
        //        return null;

        //    return Create(buf, len, true);
        //}
    }
}
