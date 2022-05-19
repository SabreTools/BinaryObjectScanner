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
    /// A structure which represents a file to be placed in a CHM helpfile.
    /// 
    /// A contiguous array of these structures should be passed to
    /// Compressor.Generate(). The array list is terminated with an
    /// entry whose InternalFile.Section field is set to #MSCHMC_ENDLIST, the
    /// other fields in this entry are ignored.
    /// </summary>
    public class CompressFile
    {
        /// <summary>
        /// One of #MSCHMC_ENDLIST, #MSCHMC_UNCOMP or #MSCHMC_MSCOMP.
        /// </summary>
        public SectionType Section { get; set; }

        /// <summary>
        /// The filename of the source file that will be added to the CHM. This
        /// is passed directly to mspack_system::open()
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// The full path and filename of the file within the CHM helpfile, a
        /// UTF-1 encoded null-terminated string.
        /// </summary>
        public string CHMFilename { get; set; }

        /// <summary>
        /// The length of the file, in bytes. This will be adhered to strictly
        /// and a read error will be issued if this many bytes cannot be read
        /// from the real file at CHM generation time.
        /// </summary>
        public long Length { get; set; }
    }
}
