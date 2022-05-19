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
    /// A structure which represents a single file in a cabinet or cabinet set.
    /// 
    /// All fields are READ ONLY.
    /// </summary>
    public class InternalFile
    {
        /// <summary>
        /// The next file in the cabinet or cabinet set, or NULL if this is the final file.
        /// </summary>
        public InternalFile Next { get; set; }

        /// <summary>
        /// The filename of the file.
        /// 
        /// A null terminated string of up to 255 bytes in length, it may be in
        /// either ISO-8859-1 or UTF8 format, depending on the file attributes.
        /// </summary>
        /// <see cref="Attributes"/>
        public string Filename { get; set; }

        /// <summary>
        /// The uncompressed length of the file, in bytes.
        /// </summary>
        public uint Length { get; set; }

        /// <summary>
        /// File attributes.
        /// </summary>
        public FileAttributes Attributes { get; set; }

        /// <summary>
        /// File's last modified time, hour field.
        /// </summary>
        public byte LastModifiedTimeHour { get; set; }

        /// <summary>
        /// File's last modified time, minute field.
        /// </summary>
        public byte LastModifiedTimeMinute { get; set; }

        /// <summary>
        /// File's last modified time, second field.
        /// </summary>
        public byte LastModifiedTimeSecond { get; set; }

        /// <summary>
        /// File's last modified date, day field.
        /// </summary>
        public byte LastModifiedDateDay { get; set; }

        /// <summary>
        /// File's last modified date, month field.
        /// </summary>
        public byte LastModifiedDateMonth { get; set; }

        /// <summary>
        /// File's last modified date, year field.
        /// </summary>
        public int LastModifiedDateYear { get; set; }

        /// <summary>
        /// A pointer to the folder that contains this file.
        /// </summary>
        public Folder Folder { get; set; }

        /// <summary>
        /// The uncompressed offset of this file in its folder.
        /// </summary>
        public uint Offset { get; set; }
    }
}
