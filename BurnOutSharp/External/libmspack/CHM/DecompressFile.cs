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
    /// A structure which represents a file stored in a CHM helpfile.
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    public class DecompressFile
    {
        /// <summary>
        /// A pointer to the next file in the list, or NULL if this is the final file.
        /// </summary>
        public DecompressFile Next { get; set; }

        /// <summary>
        /// A pointer to the section that this file is located in. Indirectly,
        /// it also points to the CHM helpfile the file is located in.
        /// </summary>
        public Section Section { get; set; }

        /// <summary>
        /// The offset within the section data that this file is located at.
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// The length of this file, in bytes
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        /// The filename of this file -- a null terminated string in UTF-8.
        /// </summary>
        public string Filename { get; set; }
    }
}
