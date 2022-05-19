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

namespace LibMSPackSharp.SZDD
{
    /// <summary>
    /// A structure which represents an SZDD compressed file.
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    public class Header
    {
        /// <summary>
        /// The file format
        /// </summary>
        public Format Format { get; set; }

        /// <summary>
        /// The amount of data in the SZDD file once uncompressed.
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// The last character in the filename, traditionally replaced with an
        /// underscore to show the file is compressed. The null character is used
        /// to show that this character has not been stored (e.g. because the
        /// filename is not known). Generally, only characters that may appear in
        /// an MS-DOS filename (except ".") are valid.
        /// </summary>
        public char MissingChar { get; set; }
    }
}
