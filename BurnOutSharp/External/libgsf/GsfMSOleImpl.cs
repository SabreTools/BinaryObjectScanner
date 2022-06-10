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

namespace LibGSF
{
    public static class GsfMSOleImpl
    {
        public const int OLE_HEADER_SIZE = 0x200;  /* independent of big block size size */
        public const int OLE_HEADER_SIGNATURE = 0x00;
        public const int OLE_HEADER_CLSID = 0x08;  /* See ReadClassStg */
        public const int OLE_HEADER_MINOR_VER = 0x18;  /* 0x33 and 0x3e have been seen */
        public const int OLE_HEADER_MAJOR_VER = 0x1a;  /* 0x3 been seen in wild */
        public const int OLE_HEADER_BYTE_ORDER = 0x1c; /* 0xfe 0xff == Intel Little Endian */
        public const int OLE_HEADER_BB_SHIFT = 0x1e;
        public const int OLE_HEADER_SB_SHIFT = 0x20;
        /* 0x22..0x27 reserved == 0 */
        public const int OLE_HEADER_CSECTDIR = 0x28;
        public const int OLE_HEADER_NUM_BAT = 0x2c;
        public const int OLE_HEADER_DIRENT_START = 0x30;
        /* 0x34..0x37 transacting signature must be 0 */
        public const int OLE_HEADER_THRESHOLD = 0x38;
        public const int OLE_HEADER_SBAT_START = 0x3c;
        public const int OLE_HEADER_NUM_SBAT = 0x40;
        public const int OLE_HEADER_METABAT_BLOCK = 0x44;
        public const int OLE_HEADER_NUM_METABAT = 0x48;
        public const int OLE_HEADER_START_BAT = 0x4c;
        public const int BAT_INDEX_SIZE = 4;
        public const int OLE_HEADER_METABAT_SIZE = ((OLE_HEADER_SIZE - OLE_HEADER_START_BAT) / BAT_INDEX_SIZE);

        public const int DIRENT_MAX_NAME_SIZE = 0x40;
        public const int DIRENT_DETAILS_SIZE = 0x40;
        public const int DIRENT_SIZE = (DIRENT_MAX_NAME_SIZE + DIRENT_DETAILS_SIZE);
        public const int DIRENT_NAME_LEN = 0x40;   /* length in bytes incl 0 terminator */
        public const int DIRENT_TYPE = 0x42;
        public const int DIRENT_COLOUR = 0x43;
        public const int DIRENT_PREV = 0x44;
        public const int DIRENT_NEXT = 0x48;
        public const int DIRENT_CHILD = 0x4c;
        public const int DIRENT_CLSID = 0x50;  /* only for dirs */
        public const int DIRENT_USERFLAGS = 0x60;  /* only for dirs */
        public const int DIRENT_CREATE_TIME = 0x64;    /* for files */
        public const int DIRENT_MODIFY_TIME = 0x6c;    /* for files */
        public const int DIRENT_FIRSTBLOCK = 0x74;
        public const int DIRENT_FILE_SIZE = 0x78;
        /* 0x7c..0x7f reserved == 0 */

        public const int DIRENT_TYPE_INVALID = 0;
        public const int DIRENT_TYPE_DIR = 1;
        public const int DIRENT_TYPE_FILE = 2;
        public const int DIRENT_TYPE_LOCKBYTES = 3;    /* ? */
        public const int DIRENT_TYPE_PROPERTY = 4; /* ? */
        public const int DIRENT_TYPE_ROOTDIR = 5;
        public const uint DIRENT_MAGIC_END = 0xffffffff;

        /* flags in the block allocation list to denote special blocks */
        public const uint BAT_MAGIC_UNUSED = 0xffffffff;   /*		   -1 */
        public const uint BAT_MAGIC_END_OF_CHAIN = 0xfffffffe; /*		   -2 */
        public const uint BAT_MAGIC_BAT = 0xfffffffd;  /* a bat block,    -3 */
        public const uint BAT_MAGIC_METABAT = 0xfffffffc;  /* a metabat block -4 */
    }
}
