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
    /// A structure which represents a section of a CHM helpfile.
    /// 
    /// All fields are READ ONLY.
    /// 
    /// Not used directly, but used as a generic base type for
    /// mschmd_sec_uncompressed and mschmd_sec_mscompressed.
    /// </summary>
    public class Section
    {
        /// <summary>
        /// A pointer to the CHM helpfile that contains this section.
        /// </summary>
        public CHM Header { get; set; }

        /// <summary>
        /// The section ID. Either 0 for the uncompressed section
        /// mschmd_sec_uncompressed, or 1 for the LZX compressed section
        /// mschmd_sec_mscompressed. No other section IDs are known.
        /// </summary>
        public uint ID { get; set; }
    }
}
