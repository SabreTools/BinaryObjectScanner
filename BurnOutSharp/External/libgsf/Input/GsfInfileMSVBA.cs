/*
 * gsf-infile-msvba.c :
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

/* Info extracted from
 *	svx/source/msfilter/msvbasic.cxx
 *	Costin Raiu, Kaspersky Labs, 'Apple of Discord'
 *	Virus bulletin's bontchev.pdf, svajcer.pdf
 *
 * and lots and lots of reading.  There are lots of pieces missing still
 * but the structure seems to hold together.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace LibGSF.Input
{
    public class GsfInfileMSVBA : GsfInfile
    {
        #region Constants

        /// <summary>
        /// Magic (2 bytes)
        /// Version (4 bytes)
        /// 0x00, 0xFF (2 bytes)
        /// Unknown (22 bytes)
        /// </summary>
        private const int VBA56_DIRENT_RECORD_COUNT = 2 + 4 + 2 + 22;

        /// <summary>
        /// VBA56_DIRENT_RECORD_COUNT (30 bytes)
        /// Type1 Record Count (2 bytes)
        /// Unknown (2 bytes)
        /// </summary>
        private const int VBA56_DIRENT_HEADER_SIZE = VBA56_DIRENT_RECORD_COUNT + 2 + 2;

        #endregion

        #region Properties

        public GsfInfile Source { get; private set; } = null;

        public List<GsfInfile> Children { get; private set; } = null;

        public Dictionary<string, byte[]> Modules { get; private set; } = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfInfileMSVBA() { }

        public static GsfInfileMSVBA Create(GsfInfile source, ref Exception err)
        {
            if (source == null)
                return null;

            GsfInfileMSVBA vba = new GsfInfileMSVBA
            {
                Source = source,
            };

            // Find the name offset pairs
            if (vba.Read(ref err))
                return vba;

            if (err != null)
                err = new Exception("Unable to parse VBA header");

            return null;
        }

        #endregion

        #region Functions

        /// <summary>
        /// A collection of names and source code which the caller is responsible for destroying.
        /// </summary>
        /// <returns>A Dictionary of names and source code (unknown encoding).</returns>
        public Dictionary<string, byte[]> StealModules()
        {
            Dictionary<string, byte[]> res = Modules;
            Modules = null;
            return res;
        }

        /// <summary>
        /// A utility routine that attempts to find the VBA file withint a stream.
        /// </summary>
        /// <returns>A GsfInfile</returns>
        public static GsfInfileMSVBA FindVBA(GsfInput input, ref Exception err)
        {
            GsfInput vba = null;
            GsfInfile infile;

            if ((infile = GsfInfileMSOle.Create(input, ref err)) != null)
            {
                // 1) Try XLS
                vba = infile.ChildByVariableName("_VBA_PROJECT_CUR", "VBA");

                // 2) DOC
                if (null == vba)
                    vba = infile.ChildByVariableName("Macros", "VBA");

                // TODO : PPT is more complex
            }
            else if ((infile = GsfInfileZip.Create(input, ref err)) != null)
            {
                GsfInput main_part = infile.RelationByType("http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", ref err);
                if (main_part != null)
                {
                    GsfInput vba_stream = main_part.RelationByType("http://schemas.microsoft.com/office/2006/relationships/vbaProject", ref err);
                    if (vba_stream != null)
                    {
                        GsfInfile ole = GsfInfileMSOle.Create(vba_stream, ref err);
                        if (ole != null)
                            vba = ole.ChildByVariableName("VBA");
                    }
                }
            }

            if (vba != null)
                return Create(vba as GsfInfile, ref err);

            return null;
        }

        #endregion

        #region Utilities

        private void ExtractModuleSource(string name, uint src_offset)
        {
            if (name == null)
                return;

            Exception err = null;
            GsfInput module = Source.ChildByName(name, ref err);
            if (module == null)
                return;

            byte[] code = module.InflateMSVBA(src_offset, out _, false);

            if (code != null)
            {
                if (Modules == null)
                    Modules = new Dictionary<string, byte[]>();

                Modules[name] = code;
            }
            else
            {
                Console.Error.WriteLine($"Problems extracting the source for {name} @ {src_offset}");
            }
        }

        /// <summary>
        /// Read an VBA dirctory and its project file.
        /// along the way.
        /// </summary>
        /// <param name="err">Place to store an Exception if anything goes wrong</param>
        /// <returns>false on error setting <paramref name="err"/> if it is supplied.</returns>
        private bool Read(ref Exception err)
        {
            int element_count = -1;
            string name, elem_stream = null;

            // 0. Get the stream
            GsfInput dir = Source.ChildByName("dir", ref err);
            if (dir == null)
            {
                err = new Exception("Can't find the VBA directory stream");
                return false;
            }

            // 1. Decompress it
            byte[] inflated_data = dir.InflateMSVBA(0, out int inflated_size, true);
            if (inflated_data == null)
            {
                err = new Exception("Failed to inflate the VBA directory stream");
                return false;
            }

            int ptr = 0; // inflated_data[0]
            int end = inflated_size;

            // 2. GUESS : based on several xls with macros and XL8GARY this looks like a
            // series of sized records.  Be _extra_ careful
            ushort tag = 0;
            do
            {
                /* I have seen
                 * type		len	data
                 *  1		 4	 1 0 0 0
                 *  2		 4	 9 4 0 0
                 *  3		 2	 4 e4
                 *  4		<var>	 project name
                 *  5		 0
                 *  6		 0
                 *  7		 4
                 *  8		 4
                 *  0x3d	 0
                 *  0x40	 0
                 *  0x14	 4	 9 4 0 0
                 *
                 *  0x0f == number of elements
                 *  0x1c == (Size 0)
                 *  0x1e == (Size 4)
                 *  0x48 == (Size 0)
                 *  0x31 == stream offset of the compressed source !
                 *
                 *  0x16 == an ascii dependency name
                 *  0x3e == a unicode dependency name
                 *  0x33 == a classid for a dependency with no trialing data
                 *
                 *  0x2f == a dummy classid
                 *  0x30 == a classid
                 *  0x0d == the classid
                 *  0x2f, and 0x0d appear contain
                 * 	uint32 classid_size;
                 * 	<classid>
                 *	00 00 00 00 00 00
                 *	and sometimes some trailing junk
                 **/

                if ((ptr + 6) > end)
                {
                    err = new Exception("VBA project header problem");
                    return false;
                }

                tag = BitConverter.ToUInt16(inflated_data, ptr);
                uint len = BitConverter.ToUInt32(inflated_data, ptr + 2);

                ptr += 6;
                if ((ptr + len) > end)
                {
                    err = new Exception("VBA project header problem");
                    return false;
                }

                switch (tag)
                {
                    case 4:
                        name = Encoding.UTF8.GetString(inflated_data, ptr, (int)len);
                        break;

                    case 9:
                        // This seems to have an extra two bytes that are not
                        // part of the length ..??
                        len += 2;
                        break;

                    case 0xf:
                        if (len != 2)
                        {
                            Console.Error.WriteLine("Element count is not what we expected");
                            break;
                        }

                        if (element_count >= 0)
                        {
                            Console.Error.WriteLine("More than one element count ??");
                            break;
                        }

                        element_count = BitConverter.ToUInt16(inflated_data, ptr);
                        break;

                    // Dependencies
                    case 0x0d: break;
                    case 0x2f: break;
                    case 0x30: break;
                    case 0x33: break;
                    case 0x3e: break;
                    case 0x16:
                        break;

                    // Elements
                    case 0x47: break;
                    case 0x32: break;
                    case 0x1a:
                        break;

                    case 0x19:
                        elem_stream = Encoding.UTF8.GetString(inflated_data, ptr, (int)len);
                        break;

                    case 0x31:
                        if (len != 4)
                        {
                            Console.Error.WriteLine("Source offset property is not what we expected");
                            break;
                        }

                        ExtractModuleSource(elem_stream, BitConverter.ToUInt32(inflated_data, ptr));
                        elem_stream = null;
                        element_count--;
                        break;

                    default:
                        break;
                }

                ptr += (int)len;
            } while (tag != 0x10);

            if (element_count != 0)
                Console.Error.WriteLine("Number of elements differs from expectations");

            return true;
        }

        #endregion
    }
}
