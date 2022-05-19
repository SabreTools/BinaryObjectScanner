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

namespace LibMSPackSharp.CAB
{
    /// <summary>
    /// A structure which represents a single folder in a cabinet or cabinet set.
    /// 
    /// All fields are READ ONLY.
    /// 
    /// A folder is a single compressed stream of data. When uncompressed, it
    /// holds the data of one or more files. A folder may be split across more
    /// than one cabinet.
    /// </summary>
    public class Folder
    {
        /// <summary>
        /// A pointer to the next folder in this cabinet or cabinet set, or NULL
        /// if this is the final folder.
        /// </summary>
        public Folder Next { get; set; }

        /// <summary>
        /// The compression format used by this folder.
        /// 
        /// The macro MSCABD_COMP_METHOD() should be used on this field to get
        /// the algorithm used. The macro MSCABD_COMP_LEVEL() should be used to get
        /// the "compression level".
        /// </summary>
        /// <see cref="MSCABD_COMP_METHOD()"/>
        /// <see cref="MSCABD_COMP_LEVEL()"/>
        public CompressionType CompressionType { get; set; }

        /// <summary>
        /// The total number of data blocks used by this folder. This includes
        /// data blocks present in other files, if this folder spans more than
        /// one cabinet.
        /// </summary>
        public ushort NumBlocks { get; set; }

        /// <summary>
        /// Returns the compression method used by a folder.
        /// </summary>
        /// <param name="compType">a <see cref="CompressionType"/> value</param>
        /// <returns>a <see cref="CompressionType"/> value</returns>
        public CompressionType MSCABD_COMP_LEVEL(CompressionType compType) => (CompressionType)((((ushort)compType) >> 8) & 0x1F);
    }
}
