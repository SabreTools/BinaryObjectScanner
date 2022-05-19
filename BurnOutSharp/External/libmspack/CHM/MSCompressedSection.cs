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

namespace LibMSPackSharp.CHM
{
    /// <summary>
    /// A structure which represents the LZX compressed section of a CHM helpfile. 
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    public class MSCompressedSection : Section
    {
        /// <summary>
        /// A pointer to the meta-file which represents all LZX compressed data.
        /// </summary>
        public DecompressFile Content { get; set; }

        /// <summary>
        /// A pointer to the file which contains the LZX control data.
        /// </summary>
        public DecompressFile Control { get; set; }

        /// <summary>
        /// A pointer to the file which contains the LZX reset table.
        /// </summary>
        public DecompressFile ResetTable { get; set; }

        /// <summary>
        /// A pointer to the file which contains the LZX span information.
        /// Available only in CHM decoder version 2 and above.
        /// </summary>
        public DecompressFile SpanInfo { get; set; }
    }
}
