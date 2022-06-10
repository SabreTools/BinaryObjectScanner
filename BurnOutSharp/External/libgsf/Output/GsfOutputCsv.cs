/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-output-csv.c: a GsfOutput to write .csv style files.
 *
 * Copyright (C) 2005-2006 Morten Welinder (terra@gnome.org)
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

using System.IO;
using System.Text;

namespace LibGSF.Output
{
    #region Enums

    /// <summary>
    /// Controls when to add quotes around fields.
    /// </summary>
    public enum GsfOutputCsvQuotingMode
    {
        /// <summary>
        /// Never add quotes around fields
        /// </summary>
        GSF_OUTPUT_CSV_QUOTING_MODE_NEVER,

        /// <summary>
        /// Add quotes around fields when needed
        /// </summary>
        GSF_OUTPUT_CSV_QUOTING_MODE_AUTO,

        /// <summary>
        /// Always add quotes around fields
        /// </summary>
        GSF_OUTPUT_CSV_QUOTING_MODE_ALWAYS
    }

    #endregion

    #region Classes

    public class GsfOutputCsv : GsfOutput
    {
        #region Properties

        public GsfOutput Sink { get; set; }

        public string Quote { get; set; }

        public GsfOutputCsvQuotingMode QuotingMode { get; set; }

        public string QuotingTriggers { get; set; } = string.Empty;

        public string EndOfLine { get; set; } = "\n";

        public string Separator { get; set; }

        public bool FieldsOnLine { get; set; }

        public string Buf { get; set; } = null;

        public bool? QuotingOnWhitespace { get; set; }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override bool WriteImpl(int num_bytes, byte[] data) => Sink.Write(num_bytes, data);

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence) => Sink.Seek(offset, whence);

        /// <inheritdoc/>
        protected override bool CloseImpl() => true;

        public bool WriteField(string field, int len)
        {
            if (field == null)
                return false;

            bool quote;
            bool ok;

            if (len == -1)
                len = field.Length;

            int end = len;
            if (FieldsOnLine && Separator.Length != 0)
                Buf += Separator;

            FieldsOnLine = true;

            switch (QuotingMode)
            {
                default:
                case GsfOutputCsvQuotingMode.GSF_OUTPUT_CSV_QUOTING_MODE_NEVER:
                    quote = false;
                    break;

                case GsfOutputCsvQuotingMode.GSF_OUTPUT_CSV_QUOTING_MODE_ALWAYS:
                    quote = true;
                    break;

                case GsfOutputCsvQuotingMode.GSF_OUTPUT_CSV_QUOTING_MODE_AUTO:
                    {
                        int p = 0; // field[0]
                        quote = false;
                        while (p < end)
                        {
                            if (QuotingTriggers.Contains(field[p].ToString()))
                            {
                                quote = true;
                                break;
                            }

                            p++;
                        }

                        if (!quote
                            && field[0] != '\0'
                            && (char.IsWhiteSpace(field[0]) || char.IsWhiteSpace(field[p - 1]))
                            && QuotingOnWhitespace != null)
                        {
                            quote = true;
                        }

                        break;
                    }
            }

            if (quote && Quote.Length > 0)
            {
                Buf += quote;
                int fieldPtr = 0; // field[0]
                while (fieldPtr < end)
                {
                    char c = field[fieldPtr];
                    if (this.Quote.Contains(c.ToString()))
                        Buf += this.Quote;

                    Buf += c;
                    fieldPtr++;
                }

                Buf += quote;
            }
            else
            {
                Buf += field;
            }

            ok = Sink.Write(Buf.Length, Encoding.UTF8.GetBytes(Buf));
            Buf = string.Empty;

            return ok;
        }

        public bool WriteEndOfLine()
        {
            FieldsOnLine = false;
            return Sink.Write(EndOfLine.Length, Encoding.UTF8.GetBytes(EndOfLine));
        }

        #endregion
    }

    #endregion
}
