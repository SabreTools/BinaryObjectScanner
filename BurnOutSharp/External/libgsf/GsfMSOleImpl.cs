/* THIS IS NOT INSTALLED */

/*
 * gsf-MSOLE-impl.h:
 *
 * Copyright (C) 2002-2006 Jody Goldberg (jody@gnome.org)
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
using System.Linq;
using System.Text;
using static LibGSF.GsfUtils;

namespace LibGSF
{
    #region Enums

    internal enum DIRENT_TYPE : byte
    {
        DIRENT_TYPE_INVALID = 0,
        DIRENT_TYPE_DIR = 1,
        DIRENT_TYPE_FILE = 2,
        DIRENT_TYPE_LOCKBYTES = 3,
        DIRENT_TYPE_PROPERTY = 4,
        DIRENT_TYPE_ROOTDIR = 5,
    }

    #endregion

    #region Classes

    /// <summary>
    /// MS-OLE Header (.msi)
    /// </summary>
    internal class MSOleHeader
    {
        #region Constants

        /// <summary>
        /// Independent of big block size size
        /// </summary>
        public const int OLE_HEADER_SIZE = 0x200;

        /// <summary>
        /// OLE Signature as a byte array
        /// </summary>
        public static readonly byte[] SIGNATURE_BYTES = { 0xd0, 0xcf, 0x11, 0xe0, 0xa1, 0xb1, 0x1a, 0xe1 };

        /// <summary>
        /// OLE Signature as an unsigned Int64
        /// </summary>
        public const ulong SIGNATURE_VALUE = 0x00E11AB1A1E011CFD0;

        #region Offsets

        private const int OLE_HEADER_SIGNATURE = 0x00;

        private const int OLE_HEADER_CLSID = 0x08;

        private const int OLE_HEADER_MINOR_VER = 0x18;

        private const int OLE_HEADER_MAJOR_VER = 0x1a;

        private const int OLE_HEADER_BYTE_ORDER = 0x1c;

        private const int OLE_HEADER_BB_SHIFT = 0x1e;

        private const int OLE_HEADER_SB_SHIFT = 0x20;

        private const int OLE_HEADER_RESERVED = 0x22;

        private const int OLE_HEADER_CSECTDIR = 0x28;

        private const int OLE_HEADER_NUM_BAT = 0x2c;

        private const int OLE_HEADER_DIRENT_START = 0x30;

        private const int OLE_HEADER_TRANSACTING_SIGNATURE = 0x34;

        private const int OLE_HEADER_THRESHOLD = 0x38;

        private const int OLE_HEADER_SBAT_START = 0x3c;

        private const int OLE_HEADER_NUM_SBAT = 0x40;

        private const int OLE_HEADER_METABAT_BLOCK = 0x44;

        private const int OLE_HEADER_NUM_METABAT = 0x48;

        private const int OLE_HEADER_START_BAT = 0x4c;

        #endregion

        #endregion

        #region Properties

        /// <remarks>0x00</remarks>
        public byte[] SIGNATURE { get; set; }

        /// <summary>
        /// See ReadClassStg
        /// </summary>
        /// <remarks>0x08</remarks>
        public byte[] CLSID { get; set; }

        /// <summary>
        /// 0x33 and 0x3E have been seen
        /// </summary>
        /// <remarks>0x18</remarks>
        public ushort MINOR_VER { get; set; }

        /// <summary>
        /// 0x03 been seen in wild
        /// </summary>
        /// <remarks>0x1A</remarks>
        public ushort MAJOR_VER { get; set; }

        /// <summary>
        /// 0xFE 0xFF == Intel Little Endian
        /// </summary>
        /// <remarks>0x1C</remarks>
        public ushort BYTE_ORDER { get; set; }

        /// <remarks>0x1E</remarks>
        public ushort BB_SHIFT { get; set; }

        /// <remarks>0x20</remarks>
        public ushort SB_SHIFT { get; set; }

        /// <summary>
        /// 0x22..0x27 reserved == 0
        /// </summary>
        /// <remarks>0x22</remarks>
        public byte[] RESERVED { get; set; }

        /// <remarks>0x28</remarks>
        public uint CSECTDIR { get; set; }

        /// <remarks>0x2C</remarks>
        public uint NUM_BAT { get; set; }

        /// <remarks>0x30</remarks>
        public uint DIRENT_START { get; set; }

        /// <summary>
        /// 0x34..0x37 transacting signature must be 0
        /// </summary>
        /// <remarks>0x34</remarks>
        public uint TRANSACTING_SIGNATURE { get; set; }

        /// <summary>
        /// Transition between small and big blocks
        /// </summary>
        /// <remarks>0x38</remarks>
        public uint THRESHOLD { get; set; }

        /// <remarks>0x3C</remarks>
        public uint SBAT_START { get; set; }

        /// <remarks>0x40</remarks>
        public uint NUM_SBAT { get; set; }

        /// <remarks>0x44</remarks>
        public uint METABAT_BLOCK { get; set; }

        /// <remarks>0x48</remarks>
        public uint NUM_METABAT { get; set; }

        /// <remarks>0x4C</remarks>
        public uint START_BAT { get; set; }

        #endregion

        #region Derived Properties

        /// <summary>
        /// Indicate if the contents are Intel Little-Endian
        /// </summary>
        public bool LITTLE_ENDIAN => BYTE_ORDER == 0xFFFE;

        public int BB_SIZE => 1 << BB_SHIFT;

        public int BB_FILTER => BB_SIZE << 1;

        public int SB_SIZE => 1 << SB_SHIFT;

        public int SB_FILTER => SB_SIZE << 1;

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private MSOleHeader() { }

        /// <summary>
        /// Create a new default MSOleHeader
        /// </summary>
        public static MSOleHeader CreateDefault()
        {
            MSOleHeader header = new MSOleHeader();

            header.SIGNATURE                = SIGNATURE_BYTES;
            header.CLSID                    = new byte[16];
            header.MINOR_VER                = 0x003E;
            header.MAJOR_VER                = 0x0003;
            header.BYTE_ORDER               = 0xFFFE;
            header.BB_SHIFT                 = 0x0009;
            header.SB_SHIFT                 = 0x0006;
            header.RESERVED                 = new byte[6];
            header.CSECTDIR                 = 0x00000000;
            header.NUM_BAT                  = 0x00000000;
            header.DIRENT_START             = 0xFFFFFFFF;
            header.TRANSACTING_SIGNATURE    = 0x00000000;
            header.THRESHOLD                = 0x00001000;

            // Remainder have default values for their types

            return header;
        }

        /// <summary>
        /// Create a new MSOleHeader from data
        /// </summary>
        public static MSOleHeader Create(byte[] data, int ptr, ref Exception err)
        {
            if (data == null || data.Length < OLE_HEADER_SIZE)
                return null;

            MSOleHeader header = new MSOleHeader();

            header.SIGNATURE = new byte[8];
            Array.Copy(data, ptr + OLE_HEADER_SIGNATURE, header.SIGNATURE, 0, 8);

            if (!header.SIGNATURE.SequenceEqual(SIGNATURE_BYTES))
            {
                err = new ArgumentException("Signature is malformed");
                return null;
            }

            header.CLSID = new byte[16];
            Array.Copy(data, ptr + OLE_HEADER_CLSID, header.CLSID, 0, 16);
            header.MINOR_VER = GSF_LE_GET_GUINT16(data, ptr + OLE_HEADER_MINOR_VER);
            header.MAJOR_VER = GSF_LE_GET_GUINT16(data, ptr + OLE_HEADER_MAJOR_VER);
            header.BYTE_ORDER = GSF_LE_GET_GUINT16(data, ptr + OLE_HEADER_BYTE_ORDER);
            header.BB_SHIFT = GSF_LE_GET_GUINT16(data, ptr + OLE_HEADER_BB_SHIFT);
            header.SB_SHIFT = GSF_LE_GET_GUINT16(data, ptr + OLE_HEADER_SB_SHIFT);

            // It makes no sense to have a block larger than 2^31 for now.
            //    Maybe relax this later, but not much.
            if (6 > header.BB_SHIFT || header.BB_SHIFT >= 31 || header.SB_SHIFT > header.BB_SHIFT)
            {
                err = new ArgumentException("Unreasonable block sizes");
                return null;
            }

            header.RESERVED = new byte[6];
            Array.Copy(data, ptr + OLE_HEADER_RESERVED, header.RESERVED, 0, 6);
            header.CSECTDIR = GSF_LE_GET_GUINT32(data, ptr + OLE_HEADER_CSECTDIR);
            header.NUM_BAT = GSF_LE_GET_GUINT32(data, ptr + OLE_HEADER_NUM_BAT);
            header.DIRENT_START = GSF_LE_GET_GUINT32(data, ptr + OLE_HEADER_DIRENT_START);
            header.TRANSACTING_SIGNATURE = GSF_LE_GET_GUINT32(data, ptr + OLE_HEADER_TRANSACTING_SIGNATURE);

            if (header.TRANSACTING_SIGNATURE != 0)
            {
                err = new ArgumentException("Transacting signature must be 0");
                return null;
            }

            header.THRESHOLD = GSF_LE_GET_GUINT32(data, ptr + OLE_HEADER_THRESHOLD);
            header.SBAT_START = GSF_LE_GET_GUINT32(data, ptr + OLE_HEADER_SBAT_START);
            header.NUM_SBAT = GSF_LE_GET_GUINT32(data, ptr + OLE_HEADER_NUM_SBAT);
            header.METABAT_BLOCK = GSF_LE_GET_GUINT32(data, ptr + OLE_HEADER_METABAT_BLOCK);
            header.NUM_METABAT = GSF_LE_GET_GUINT32(data, ptr + OLE_HEADER_NUM_METABAT);
            header.START_BAT = GSF_LE_GET_GUINT32(data, ptr + OLE_HEADER_START_BAT);

            if (header.NUM_SBAT == 0
                && header.SBAT_START != GsfMSOleImpl.BAT_MAGIC_END_OF_CHAIN
                && header.SBAT_START != GsfMSOleImpl.BAT_MAGIC_UNUSED)
            {
                Console.Error.WriteLine("There are not supposed to be any blocks in the small block allocation table, yet there is a link to some.  Ignoring it.");
            }

            return header;
        }

        /// <summary>
        /// Write to data from an existing MSOleHeader
        /// </summary>
        public bool Write(byte[] data, int ptr)
        {
            if (data == null || data.Length < OLE_HEADER_SIZE)
                return false;

            Array.Copy(SIGNATURE, 0, data, ptr + OLE_HEADER_SIGNATURE, SIGNATURE.Length);
            Array.Copy(CLSID, 0, data, ptr + OLE_HEADER_CLSID, CLSID.Length);
            GSF_LE_SET_GUINT16(data, ptr + OLE_HEADER_MINOR_VER, MINOR_VER);
            GSF_LE_SET_GUINT16(data, ptr + OLE_HEADER_MAJOR_VER, MAJOR_VER);
            GSF_LE_SET_GUINT16(data, ptr + OLE_HEADER_BYTE_ORDER, BYTE_ORDER);
            GSF_LE_SET_GUINT16(data, ptr + OLE_HEADER_BB_SHIFT, BB_SHIFT);
            GSF_LE_SET_GUINT16(data, ptr + OLE_HEADER_SB_SHIFT, SB_SHIFT);
            Array.Copy(RESERVED, 0, data, ptr + OLE_HEADER_RESERVED, RESERVED.Length);
            GSF_LE_SET_GUINT32(data, ptr + OLE_HEADER_CSECTDIR, CSECTDIR);
            GSF_LE_SET_GUINT32(data, ptr + OLE_HEADER_NUM_BAT, NUM_BAT);
            GSF_LE_SET_GUINT32(data, ptr + OLE_HEADER_DIRENT_START, DIRENT_START);
            GSF_LE_SET_GUINT32(data, ptr + OLE_HEADER_TRANSACTING_SIGNATURE, TRANSACTING_SIGNATURE);
            GSF_LE_SET_GUINT32(data, ptr + OLE_HEADER_THRESHOLD, THRESHOLD);
            GSF_LE_SET_GUINT32(data, ptr + OLE_HEADER_SBAT_START, SBAT_START);
            GSF_LE_SET_GUINT32(data, ptr + OLE_HEADER_NUM_SBAT, NUM_SBAT);
            GSF_LE_SET_GUINT32(data, ptr + OLE_HEADER_METABAT_BLOCK, METABAT_BLOCK);
            GSF_LE_SET_GUINT32(data, ptr + OLE_HEADER_START_BAT, START_BAT);

            return true;
        }

        #endregion
    }

    /// <summary>
    /// MS-OLE Directory Entry (.msi)
    /// </summary>
    internal class MSOleDirectoryEntry
    {
        #region Constants

        public const int DIRENT_MAX_NAME_SIZE = 0x40;

        public const int DIRENT_DETAILS_SIZE = 0x40;

        public const int DIRENT_SIZE = (DIRENT_MAX_NAME_SIZE + DIRENT_DETAILS_SIZE);

        public const uint DIRENT_MAGIC_END = 0xffffffff;

        #region Offsets

        private const int DIRENT_NAME = 0x00;

        private const int DIRENT_NAME_LEN = 0x40;

        private const int DIRENT_TYPE_FLAG = 0x42;

        private const int DIRENT_COLOR = 0x43;

        private const int DIRENT_PREV = 0x44;

        private const int DIRENT_NEXT = 0x48;

        private const int DIRENT_CHILD = 0x4c;

        private const int DIRENT_CLSID = 0x50;

        private const int DIRENT_USERFLAGS = 0x60;

        private const int DIRENT_CREATE_TIME = 0x64;

        private const int DIRENT_MODIFY_TIME = 0x6C;

        private const int DIRENT_FIRSTBLOCK = 0x74;

        private const int DIRENT_FILE_SIZE = 0x78;

        private const int DIRENT_RESERVED = 0x7C;

        #endregion

        #endregion

        #region Properties

        /// <remarks>0x00</remarks>
        public byte[] NAME { get; set; }

        /// <summary>
        /// Length in bytes incl 0 terminator
        /// </summary>
        /// <remarks>0x40</remarks>
        public ushort NAME_LEN { get; set; }

        /// <remarks>0x42</remarks>
        public DIRENT_TYPE TYPE_FLAG { get; set; }

        /// <remarks>0x43</remarks>
        public byte COLOR { get; set; }

        /// <remarks>0x44</remarks>
        public uint PREV { get; set; }

        /// <remarks>0x48</remarks>
        public uint NEXT { get; set; }

        /// <remarks>0x4C</remarks>
        public uint CHILD { get; set; }

        /// <summary>
        /// 16 byte GUID used by some apps; Only for dirs
        /// </summary>
        /// <remarks>0x50</remarks>
        public byte[] CLSID { get; set; }

        /// <summary>
        /// Only for dirs
        /// </summary>
        /// <remarks>0x60</remarks>
        public uint USERFLAGS { get; set; }

        /// <summary>
        /// For files
        /// </summary>
        /// <remarks>0x64</remarks>
        public ulong CREATE_TIME { get; set; }

        /// <summary>
        /// For files
        /// </summary>
        /// <remarks>0x6C</remarks>
        public ulong MODIFY_TIME { get; set; }

        /// <remarks>0x74</remarks>
        public uint FIRSTBLOCK { get; set; }

        /// <remarks>0x78</remarks>
        public uint FILE_SIZE { get; set; }

        /// <summary>
        /// 0x7c..0x7f reserved == 0
        /// </summary>
        /// <remarks>0x7C</remarks>
        public uint RESERVED { get; set; }

        #endregion

        #region Derived Properties

        /// <summary>
        /// Directory Entry name as a UTF-8 encoded string
        /// </summary>
        public string NAME_STRING
        {
            get
            {
                if (NAME_LEN <= 0 || NAME_LEN > DIRENT_MAX_NAME_SIZE)
                    return string.Empty;

                // !#%!@$#^
                // Sometimes, rarely, people store the stream name as ascii
                // rather than utf16.  Do a validation first just in case.
                int end;
                try { end = new UnicodeEncoding(false, true).GetCharCount(NAME); }
                catch { end = -1; }

                if (end == -1)
                {
                    byte[] direntNameBytes = Encoding.Convert(Encoding.ASCII, Encoding.UTF8, NAME);
                    return Encoding.UTF8.GetString(direntNameBytes).TrimEnd('\0');
                }
                else
                {
                    byte[] direntNameBytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, NAME);
                    return Encoding.UTF8.GetString(direntNameBytes).TrimEnd('\0');
                }
            }
        }

        /// <summary>
        /// Last modified time as a nullable DateTime
        /// </summary>
        public DateTime? MODIFY_DATETIME => MODIFY_TIME == 0 ? (DateTime?)null : DateTime.FromFileTime((long)MODIFY_TIME);

        /// <summary>
        /// Determine if this entry is a directory
        /// </summary>
        public bool IS_DIRECTORY => TYPE_FLAG != DIRENT_TYPE.DIRENT_TYPE_FILE;

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private MSOleDirectoryEntry() { }

        /// <summary>
        /// Create a new default MSOleDirectoryEntry
        /// </summary>
        public static MSOleDirectoryEntry CreateDefault()
        {
            // TODO: Figure out if there are any sane defaults
            return new MSOleDirectoryEntry();
        }

        /// <summary>
        /// Create a new MSOleDirectoryEntry from data
        /// </summary>
        public static MSOleDirectoryEntry Create(byte[] data, int ptr, ref Exception err)
        {
            if (data == null || data.Length < DIRENT_SIZE)
                return null;

            MSOleDirectoryEntry directoryEntry = new MSOleDirectoryEntry();

            directoryEntry.NAME = new byte[DIRENT_MAX_NAME_SIZE];
            Array.Copy(data, ptr + DIRENT_NAME, directoryEntry.NAME, 0, directoryEntry.NAME.Length);
            directoryEntry.NAME_LEN = GSF_LE_GET_GUINT16(data, ptr + DIRENT_NAME_LEN);
            directoryEntry.TYPE_FLAG = (DIRENT_TYPE)GSF_LE_GET_GUINT8(data, ptr + DIRENT_TYPE_FLAG);

            if (directoryEntry.TYPE_FLAG != DIRENT_TYPE.DIRENT_TYPE_DIR
                && directoryEntry.TYPE_FLAG != DIRENT_TYPE.DIRENT_TYPE_FILE
                && directoryEntry.TYPE_FLAG != DIRENT_TYPE.DIRENT_TYPE_ROOTDIR)
            {
                err = new Exception($"Unknown stream type 0x{directoryEntry.TYPE_FLAG:x}");
                return null;
            }

            directoryEntry.COLOR = GSF_LE_GET_GUINT8(data, ptr + DIRENT_COLOR);
            directoryEntry.PREV = GSF_LE_GET_GUINT32(data, ptr + DIRENT_PREV);
            directoryEntry.NEXT = GSF_LE_GET_GUINT32(data, ptr + DIRENT_NEXT);
            directoryEntry.CHILD = GSF_LE_GET_GUINT32(data, ptr + DIRENT_CHILD);
            directoryEntry.CLSID = new byte[0x10];
            Array.Copy(data, ptr + DIRENT_CLSID, directoryEntry.CLSID, 0, directoryEntry.CLSID.Length);
            directoryEntry.USERFLAGS = GSF_LE_GET_GUINT32(data, ptr + DIRENT_USERFLAGS);
            directoryEntry.CREATE_TIME = GSF_LE_GET_GUINT64(data, ptr + DIRENT_CREATE_TIME);
            directoryEntry.MODIFY_TIME = GSF_LE_GET_GUINT64(data, ptr + DIRENT_MODIFY_TIME);
            directoryEntry.FIRSTBLOCK = GSF_LE_GET_GUINT32(data, ptr + DIRENT_FIRSTBLOCK);
            directoryEntry.FILE_SIZE = GSF_LE_GET_GUINT32(data, ptr + DIRENT_FILE_SIZE);
            directoryEntry.RESERVED = GSF_LE_GET_GUINT32(data, ptr + DIRENT_RESERVED);

            return directoryEntry;
        }

        /// <summary>
        /// Write to data from an existing MSOleHeader
        /// </summary>
        public bool Write(byte[] data, int ptr)
        {
            if (data == null || data.Length < DIRENT_SIZE)
                return false;

            Array.Copy(NAME, 0, data, ptr + DIRENT_NAME, NAME.Length);
            GSF_LE_SET_GUINT16(data, ptr + DIRENT_NAME_LEN, NAME_LEN);
            GSF_LE_SET_GUINT8(data, ptr + DIRENT_TYPE_FLAG, (byte)TYPE_FLAG);
            GSF_LE_SET_GUINT8(data, ptr + DIRENT_COLOR, COLOR);
            GSF_LE_SET_GUINT32(data, ptr + DIRENT_PREV, PREV);
            GSF_LE_SET_GUINT32(data, ptr + DIRENT_NEXT, NEXT);
            GSF_LE_SET_GUINT32(data, ptr + DIRENT_CHILD, CHILD);
            Array.Copy(CLSID, 0, data, ptr + DIRENT_CLSID, CLSID.Length);
            GSF_LE_SET_GUINT32(data, ptr + DIRENT_USERFLAGS, USERFLAGS);
            GSF_LE_SET_GUINT64(data, ptr + DIRENT_CREATE_TIME, CREATE_TIME);
            GSF_LE_SET_GUINT64(data, ptr + DIRENT_MODIFY_TIME, MODIFY_TIME);
            GSF_LE_SET_GUINT32(data, ptr + DIRENT_FIRSTBLOCK, FIRSTBLOCK);
            GSF_LE_SET_GUINT32(data, ptr + DIRENT_FILE_SIZE, FILE_SIZE);
            GSF_LE_SET_GUINT32(data, ptr + DIRENT_RESERVED, RESERVED);

            return true;
        }

        #endregion
    }

    public static class GsfMSOleImpl
    {
        public const int OLE_HEADER_CSECTDIR = 0x28;
        public const int OLE_HEADER_NUM_BAT = 0x2C;
        public const int OLE_HEADER_SBAT_START = 0x3C;
        public const int OLE_HEADER_START_BAT = 0x4C;
        public const int BAT_INDEX_SIZE = 4;
        public const int OLE_HEADER_METABAT_SIZE = ((MSOleHeader.OLE_HEADER_SIZE - OLE_HEADER_START_BAT) / BAT_INDEX_SIZE);

        /* flags in the block allocation list to denote special blocks */
        public const uint BAT_MAGIC_UNUSED = 0xffffffff;   /*		   -1 */
        public const uint BAT_MAGIC_END_OF_CHAIN = 0xfffffffe; /*		   -2 */
        public const uint BAT_MAGIC_BAT = 0xfffffffd;  /* a bat block,    -3 */
        public const uint BAT_MAGIC_METABAT = 0xfffffffc;  /* a metabat block -4 */
    }

    #endregion
}
