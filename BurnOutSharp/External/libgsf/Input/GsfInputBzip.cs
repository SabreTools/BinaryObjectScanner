/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-input-iochannel.c: wrapper for glib's GIOChannel
 *
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
        #region Constants

        private const int BZ_BUFSIZ = 1024;

        #region bzlib.h

        private const int BZ_RUN = 0;
        private const int BZ_FLUSH = 1;
        private const int BZ_FINISH = 2;

        private const int BZ_OK = 0;
        private const int BZ_RUN_OK = 1;
        private const int BZ_FLUSH_OK = 2;
        private const int BZ_FINISH_OK = 3;
        private const int BZ_STREAM_END = 4;
        private const int BZ_SEQUENCE_ERROR = (-1);
        private const int BZ_PARAM_ERROR = (-2);
        private const int BZ_MEM_ERROR = (-3);
        private const int BZ_DATA_ERROR = (-4);
        private const int BZ_DATA_ERROR_MAGIC = (-5);
        private const int BZ_IO_ERROR = (-6);
        private const int BZ_UNEXPECTED_EOF = (-7);
        private const int BZ_OUTBUFF_FULL = (-8);
        private const int BZ_CONFIG_ERROR = (-9);

        #endregion

        #endregion

        // TODO: Implement BZIP reading

        #region Constructor

        /// <param name="err">Place to store an Exception if anything goes wrong</param>
        /// <returns>A new GsfInputMemory or null.</returns>
        public static GsfInputMemory CreateFromBzip(GsfInput source, ref Exception err)
        {
#if BZIP2
            bz_stream bzstm;
            GsfInput* mem = NULL;
            GsfOutput* sink = NULL;
            guint8 out_buf[BZ_BUFSIZ];
            int bzerr = BZ_OK;

            g_return_val_if_fail(source != NULL, NULL);

            memset(&bzstm, 0, sizeof(bzstm));
            if (BZ_OK != BZ2_bzDecompressInit(&bzstm, 0, 0))
            {
                if (err)
                    *err = g_error_new(gsf_input_error_id(), 0,
                                _("BZ2 decompress init failed"));
                return NULL;
            }

            sink = gsf_output_memory_new();

            for (; ; )
            {
                bzstm.next_out = (char*)out_buf;
                bzstm.avail_out = (unsigned int)sizeof(out_buf);

            if (bzstm.avail_in == 0)
            {
                bzstm.avail_in = (unsigned int)MIN(gsf_input_remaining(source), BZ_BUFSIZ);
                bzstm.next_in = (char*)gsf_input_read(source, bzstm.avail_in, NULL);
            }

            bzerr = BZ2_bzDecompress(&bzstm);
            if (bzerr != BZ_OK && bzerr != BZ_STREAM_END)
            {
                if (err)
                    *err = g_error_new(gsf_input_error_id(), 0,
                                _("BZ2 decompress failed"));
                BZ2_bzDecompressEnd(&bzstm);
                gsf_output_close(sink);
                g_object_unref(sink);
                return NULL;
            }

            gsf_output_write(sink, BZ_BUFSIZ - bzstm.avail_out, out_buf);
            if (bzerr == BZ_STREAM_END)
                break;
        }

        gsf_output_close(sink);

	if (BZ_OK != BZ2_bzDecompressEnd(&bzstm)) {
		if (err)
            * err = g_error_new(gsf_input_error_id(), 0,
                        _("BZ2 decompress end failed"));
        g_object_unref(sink);
		return NULL;
	}

    mem = gsf_input_memory_new_clone(
        gsf_output_memory_get_bytes (GSF_OUTPUT_MEMORY (sink)),
		gsf_output_size(sink));

	if (mem != NULL)
		gsf_input_set_name(mem, gsf_input_name (source));

    g_object_unref(sink);
	return mem;
#else
            err = new Exception("BZ2 support not enabled");
            return null;
#endif
        }

        #endregion
    }
}
