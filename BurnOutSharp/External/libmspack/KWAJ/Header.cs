/* libmspack -- a library for working with Microsoft compression formats.
 * (C) 2003-2019 Stuart Caie <kyzer@cabextract.org.uk>
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
 */

using System.IO;

namespace LibMSPackSharp.KWAJ
{
    /// <summary>
    /// A structure which represents an KWAJ compressed file.
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    public class Header : BaseHeader
    {
        #region Internal

        /// <summary>
        /// KWAJ header information
        /// </summary>
        internal _KWAJHeader KWAJHeader { get; set; }

        #endregion

        /// <summary>
        /// Flags indicating which optional headers were included.
        /// </summary>
        public OptionalHeaderFlag Headers { get; set; }

        /// <summary>
        /// The amount of uncompressed data in the file, or 0 if not present.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// Output filename, or NULL if not present
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Extra uncompressed data (usually text) in the header.
        /// This data can contain nulls so use extra_length to get the size.
        /// </summary>
        public string Extra { get; set; }

        /// <summary>
        /// Length of extra uncompressed data in the header
        /// </summary>
        public ushort ExtraLength { get; set; }

        /// <summary>
        /// Internal file handle
        /// </summary>
        public FileStream FileHandle { get; set; }
    }
}
