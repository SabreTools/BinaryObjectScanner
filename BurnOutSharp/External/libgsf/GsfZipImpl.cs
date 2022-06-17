/* THIS IS NOT INSTALLED */

/*
 * gsf-zip-impl.h:
 *
 * Copyright (C) 2002-2006 Tambet Ingo (tambet@ximian.com)
 * Copyright (C) 2014 Morten Welinder (terra@gnome.org)
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
using System.Collections.Generic;

namespace LibGSF
{
    #region Enums

    /// <summary>
    /// A few well-defined extra-field tags.
    /// </summary>
    public enum ExtraFieldTags : ushort
    {
        ZIP_DIRENT_EXTRA_FIELD_ZIP64 = 0x0001,

        /// <summary>
        /// "II" -- gsf defined
        /// </summary>
        ZIP_DIRENT_EXTRA_FIELD_IGNORE = 0x4949,

        /// <summary>
        /// "UT"
        /// </summary>
        ZIP_DIRENT_EXTRA_FIELD_UNIXTIME = 0x5455,

        /// <summary>
        /// "ux"
        /// </summary>
        ZIP_DIRENT_EXTRA_FIELD_UIDGID = 0x7875
    };

    /// <summary>
    /// OS codes.  There are plenty, but this is all we need.
    /// </summary>
    public enum OSCodes
    {
        ZIP_OS_MSDOS = 0,
        ZIP_OS_UNIX = 3
    };

    /// <remarks>From gsf-outfile-zip.h</remarks>
    public enum GsfZipCompressionMethod
    {
        /// <summary>
        /// Supported for export
        /// </summary>
        GSF_ZIP_STORED = 0,
        GSF_ZIP_SHRUNK = 1,
        GSF_ZIP_REDUCEDx1 = 2,
        GSF_ZIP_REDUCEDx2 = 3,
        GSF_ZIP_REDUCEDx3 = 4,
        GSF_ZIP_REDUCEDx4 = 5,
        GSF_ZIP_IMPLODED = 6,
        GSF_ZIP_TOKENIZED = 7,

        /// <summary>
        /// Supported for export
        /// </summary>
        GSF_ZIP_DEFLATED = 8,
        GSF_ZIP_DEFLATED_BETTER = 9,
        GSF_ZIP_IMPLODED_BETTER = 10
    }

    #endregion

    #region Classes

    public class GsfZipDirectoryEntry
    {
        #region Properties

        public string Name { get; set; }

        public int Flags { get; set; }

        public GsfZipCompressionMethod CompressionMethod { get; set; }

        public uint CRC32 { get; set; }

        public long CompressedSize { get; set; }

        public long UncompressedSize { get; set; }

        public long Offset { get; set; }

        public long DataOffset { get; set; }

        public uint DosTime { get; set; }

        public DateTime? ModifiedTime { get; set; }

        /// <summary>
        ///  null = auto, FALSE, TRUE.
        /// </summary>
        public bool? Zip64 { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfZipDirectoryEntry() { }

        /// <summary>
        /// Doesn't do much, but include for symmetry
        /// </summary>
        public static GsfZipDirectoryEntry Create() => new GsfZipDirectoryEntry();

        #endregion

        #region Functions

        public void Free()
        {
            Name = null;
        }

        public GsfZipDirectoryEntry Copy()
        {
            return new GsfZipDirectoryEntry
            {
                Name = Name,
                Flags = Flags,
                CompressionMethod = CompressionMethod,
                CRC32 = CRC32,
                CompressedSize = CompressedSize,
                UncompressedSize = UncompressedSize,
                Offset = Offset,
                DataOffset = DataOffset,
                DosTime = DosTime,
                ModifiedTime = ModifiedTime,
                Zip64 = Zip64,
            };
        }

        #endregion
    }

    public class GsfZipVDir
    {
        #region Properties

        public string Name { get; set; }

        public bool IsDirectory { get; set; }

        public GsfZipDirectoryEntry DirectoryEntry { get; set; }

        public List<GsfZipVDir> Children { get; set; }

        //public GSList* last_child { get; set; } /* Unused */

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfZipVDir() { }

        /// <returns>The newly created GsfZipVDir.</returns>
        /// <remarks>Since: 1.14.24</remarks>
        public static GsfZipVDir Create(string name, bool is_directory, GsfZipDirectoryEntry dirent)
        {
            return new GsfZipVDir
            {
                Name = name,
                IsDirectory = is_directory,
                DirectoryEntry = dirent,
                Children = new List<GsfZipVDir>(),
            };
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfZipVDir() => Free(true);

        #endregion

        #region Functions

        public void Free(bool free_dirent)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                GsfZipVDir c = Children[i];
                c.Free(free_dirent);
            }

            Children.Clear();
            Name = null;
            if (free_dirent && DirectoryEntry != null)
                DirectoryEntry.Free();
        }

        public GsfZipVDir Copy()
        {
            GsfZipVDir res = new GsfZipVDir();

            // It is not possible to add a ref_count without breaking the API,
            // so we need to really copy everything
            if (Name != null)
                res.Name = Name;

            res.IsDirectory = IsDirectory;
            if (DirectoryEntry != null)
                res.DirectoryEntry = DirectoryEntry.Copy();

            for (int i = 0; i < Children.Count; i++)
            {
                GsfZipVDir c = Children[i];
                res.AddChild(c.Copy());
            }
            return res;
        }

        public void AddChild(GsfZipVDir child)
        {
            Children.Add(child);
        }

        public GsfZipVDir ChildByName(string name)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                GsfZipVDir child = Children[i];
                if (child.Name == name)
                    return child;
            }

            return null;
        }

        public GsfZipVDir ChildByIndex(int target) => target < Children.Count ? Children[target] : null;

        public void Insert(string name, GsfZipDirectoryEntry dirent)
        {
            int p = name.IndexOf(GsfZipImpl.ZIP_NAME_SEPARATOR);
            if (p != -1)
            {
                // A directory
                string dirname = name.Substring(0, p);
                GsfZipVDir child = ChildByName(dirname);
                if (child == null)
                {
                    child = Create(dirname, true, null);
                    AddChild(child);
                }

                if (p + 1 < name.Length && name[p + 1] != '\0')
                {
                    name = name.Substring(p + 1);
                    child.Insert(name, dirent);
                }
            }
            else
            {
                // A simple file name
                GsfZipVDir child = Create(name, false, dirent);
                AddChild(child);
            }
        }

        #endregion
    }

    public static class GsfZipImpl
    {
        // Every member file is preceded by a header with this format.
        public const uint ZIP_HEADER_SIGNATURE = 0x04034b50;
        public const int ZIP_HEADER_SIZE = 30;
        public const int ZIP_HEADER_EXTRACT = 4;
        public const int ZIP_HEADER_FLAGS = 6;
        public const int ZIP_HEADER_COMP_METHOD = 8;
        public const int ZIP_HEADER_DOSTIME = 10;
        public const int ZIP_HEADER_CRC32 = 14;
        public const int ZIP_HEADER_CSIZE = 18;
        public const int ZIP_HEADER_USIZE = 22;
        public const int ZIP_HEADER_NAME_SIZE = 26;
        public const int ZIP_HEADER_EXTRAS_SIZE = 28;

        // Members may have this record after the compressed data.  It is meant
        // to be used only when it is not possible to seek back and patch the
        // right values into the header.
        public const uint ZIP_DDESC_SIGNATURE = 0x08074b50;
        public const int ZIP_DDESC_SIZE = 16;
        public const int ZIP_DDESC_CRC32 = 4;
        public const int ZIP_DDESC_CSIZE = 8;
        public const int ZIP_DDESC_USIZE = 12;

        // 64-bit version of above.  Used when the ZIP64 extra field is present
        // in the header.
        public const uint ZIP_DDESC64_SIGNATURE = ZIP_DDESC_SIGNATURE;
        public const int ZIP_DDESC64_SIZE = 24;
        public const int ZIP_DDESC64_CRC32 = 4;
        public const int ZIP_DDESC64_CSIZE = 8;
        public const int ZIP_DDESC64_USIZE = 16;

        // The whole archive ends with a trailer.
        public const uint ZIP_TRAILER_SIGNATURE = 0x06054b50;
        public const int ZIP_TRAILER_SIZE = 22;
        public const int ZIP_TRAILER_DISK = 4;
        public const int ZIP_TRAILER_DIR_DISK = 6;
        public const int ZIP_TRAILER_ENTRIES = 8;
        public const int ZIP_TRAILER_TOTAL_ENTRIES = 10;
        public const int ZIP_TRAILER_DIR_SIZE = 12;
        public const int ZIP_TRAILER_DIR_POS = 16;
        public const int ZIP_TRAILER_COMMENT_SIZE = 20;

        // A zip64 locator comes immediately before the trailer, if it is present.
        public const uint ZIP_ZIP64_LOCATOR_SIGNATURE = 0x07064b50;
        public const int ZIP_ZIP64_LOCATOR_SIZE = 20;
        public const int ZIP_ZIP64_LOCATOR_DISK = 4;
        public const int ZIP_ZIP64_LOCATOR_OFFSET = 8;
        public const int ZIP_ZIP64_LOCATOR_DISKS = 16;

        // A zip64 archive has this record somewhere to extend the field sizes.
        public const uint ZIP_TRAILER64_SIGNATURE = 0x06064b50;
        public const int ZIP_TRAILER64_SIZE = 56; // Or more
        public const int ZIP_TRAILER64_RECSIZE = 4;
        public const int ZIP_TRAILER64_ENCODER = 12;
        public const int ZIP_TRAILER64_EXTRACT = 14;
        public const int ZIP_TRAILER64_DISK = 16;
        public const int ZIP_TRAILER64_DIR_DISK = 20;
        public const int ZIP_TRAILER64_ENTRIES = 24;
        public const int ZIP_TRAILER64_TOTAL_ENTRIES = 32;
        public const int ZIP_TRAILER64_DIR_SIZE = 40;
        public const int ZIP_TRAILER64_DIR_POS = 48;

        // This defines the entries in the central directory.
        public const uint ZIP_DIRENT_SIGNATURE = 0x02014b50;
        public const int ZIP_DIRENT_SIZE = 46;
        public const int ZIP_DIRENT_ENCODER = 4;
        public const int ZIP_DIRENT_EXTRACT = 6;
        public const int ZIP_DIRENT_FLAGS = 8;
        public const int ZIP_DIRENT_COMPR_METHOD = 10;
        public const int ZIP_DIRENT_DOSTIME = 12;
        public const int ZIP_DIRENT_CRC32 = 16;
        public const int ZIP_DIRENT_CSIZE = 20;
        public const int ZIP_DIRENT_USIZE = 24;
        public const int ZIP_DIRENT_NAME_SIZE = 28;
        public const int ZIP_DIRENT_EXTRAS_SIZE = 30;
        public const int ZIP_DIRENT_COMMENT_SIZE = 32;
        public const int ZIP_DIRENT_DISKSTART = 34;
        public const int ZIP_DIRENT_FILE_TYPE = 36;
        public const int ZIP_DIRENT_FILE_MODE = 38;
        public const int ZIP_DIRENT_OFFSET = 42;

        public const int ZIP_DIRENT_FLAGS_HAS_DDESC = 8;

        public const char ZIP_NAME_SEPARATOR = '/';

        public const int ZIP_BLOCK_SIZE = 32768;
        public const int ZIP_BUF_SIZE = 512;

        /* z_flags */
        //#define ZZIP_IS_ENCRYPTED(p)    ((*(unsigned char*)p)&1)
        //#define ZZIP_IS_COMPRLEVEL(p)  (((*(unsigned char*)p)>>1)&3)
        //#define ZZIP_IS_STREAMED(p)    (((*(unsigned char*)p)>>3)&1)
    }

    #endregion
}
