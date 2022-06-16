/*
 * gsf-doc-meta-data.c:
 *
 * Copyright (C) 2002-2006 Dom Lachowicz (cinamod@hotmail.com)
 * 			   Jody Goldberg (jody@gnome.org)
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
using System.IO;
using System.Linq;
using System.Text;
using LibGSF.Input;
using LibGSF.Output;
using static LibGSF.GsfMSOleUtils;
using static LibGSF.GsfUtils;

namespace LibGSF
{
    public class GsfDocProp
    {
        #region Properties

        public string Name { get; set; }

        public object Value { get; set; }

        /// <summary>
        /// Optionally NULL
        /// </summary>
        public string LinkedTo { get; set; }

        public uint RefCount { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfDocProp() { }

        /// <param name="name">The name of the property.</param>
        /// <returns>A new GsfDocProp.</returns>
        public static GsfDocProp Create(string name)
        {
            if (name == null)
                return null;

            return new GsfDocProp
            {
                Name = name,
                Value = null,
                LinkedTo = null,
            };
        }

        #endregion

        #region Functions

        /// <summary>
        /// Release the given property.
        /// </summary>
        public void Free()
        {
            RefCount--;
            if (RefCount == 0)
            {
                LinkedTo = null;
                if (Value != null)
                    Value = null;

                Name = null;
            }
        }

        public GsfDocProp Reference()
        {
            RefCount++;
            return this;
        }

        /// <returns>The current value of prop, and replaces it with <paramref name="val"/>.</returns>
        public object SwapValue(object val)
        {
            object old_val = Value;
            Value = val;
            return old_val;
        }

        #endregion
    }

    public class GsfDocMetaData
    {
        #region Properties

        internal Dictionary<string, GsfDocProp> Table = new Dictionary<string, GsfDocProp>();

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfDocMetaData() { }

        /// <returns>
        /// A new metadata property collection
        /// </returns>
        public static GsfDocMetaData Create() => new GsfDocMetaData();

        #endregion

        #region Functions

        /// <returns>
        /// The property with <paramref name="name"/> in meta.  The caller can
        /// modify the property value and link but not the name.
        /// </returns>
        public GsfDocProp Lookup(string name)
        {
            if (name == null)
                return null;

            if (!Table.ContainsKey(name))
                return null;

            return Table[name];
        }

        /// <summary>
        /// Take ownership of <paramref name="name"/> and <paramref name="value"/> and insert a property into meta.
        /// If a property exists with @name, it is replaced (The link is lost)
        /// </summary>
        /// <param name="name">The id.</param>
        public void Insert(string name, object value)
        {
            if (name == null)
                return;

            GsfDocProp docProp = GsfDocProp.Create(name);
            docProp.Value = value;
            docProp.LinkedTo = null;
            docProp.RefCount = 1;

            Table[name] = docProp;
        }

        /// <summary>
        /// Read a stream formated as a set of MS OLE properties from <paramref name="input"/> and store the
        /// results in <paramref name="accum"/>.
        /// </summary>
        /// <returns>an Exception if there was an error.</returns>
        /// <remarks>Since: 1.14.24</remarks>
        public Exception ReadFromMSOLE(GsfInput input)
        {
            GsfDocProp prop;

            // http://bugzilla.gnome.org/show_bug.cgi?id=352055
            // psiwin generates files with empty property sections
            if (input.Size <= 0)
                return null;

            byte[] data = input.Read(28, null);
            if (data == null)
                return new Exception("Unable to read MS property stream header");

            /*
             * Validate the Property Set Header.
             * Format (bytes):
             *   00 - 01	Byte order		0xfffe
             *   02 - 03	Format			0
             *   04 - 05	OS Version		high word is the OS
             *   06 - 07				low  word is the OS version
             *					  0 = win16
             *					  1 = mac
             *					  2 = win32
             *   08 - 23	Class Identifier	Usually Format ID
             *   24 - 27	Section count		Should be at least 1
             */
            ushort os = GSF_LE_GET_GUINT16(data, 6);
            ushort version = GSF_LE_GET_GUINT16(data, 2);
            uint num_sections = BitConverter.ToUInt32(data, 24);

            if (GSF_LE_GET_GUINT16(data, 0) != 0xfffe
                || (version != 0 && version != 1)
                || os > 2
                || num_sections > input.Size / 20
                || num_sections > 100) // Arbitrary sanity check
            {
                return new Exception("Invalid MS property stream header");
            }

            // Extract the section info
            /*
             * The Format ID/Offset list follows.
             * Format:
             *   00 - 16	Section Name		Format ID
             *   16 - 19	Section Offset		The offset is the number of
             *					bytes from the start of the
             *					whole stream to where the
             *					section begins.
             */
            GsfMSOleMetaDataSection[] sections = new GsfMSOleMetaDataSection[num_sections];
            for (uint i = 0; i < num_sections; i++)
            {
                data = input.Read(20, null);
                if (data == null)
                    return new Exception("Unable to read MS property stream header");

                byte[] guid = new ReadOnlySpan<byte>(data, 0, 16).ToArray();
                if (guid.SequenceEqual(ComponentGUID))
                {
                    sections[i].Type = GsfMSOLEMetaDataType.COMPONENT_PROP;
                }
                else if (guid.SequenceEqual(DocumentGUID))
                {
                    sections[i].Type = GsfMSOLEMetaDataType.DOC_PROP;
                }
                else if (guid.SequenceEqual(UserGUID))
                {
                    sections[i].Type = GsfMSOLEMetaDataType.USER_PROP;
                }
                else
                {
                    sections[i].Type = GsfMSOLEMetaDataType.USER_PROP;
                    Console.Error.WriteLine("Unknown property section type, treating it as USER");
                }

                sections[i].Offset = BitConverter.ToUInt32(data, 16);
            }

            /*
             * A section is the third part of the property set stream.
             * Format (bytes):
             *   00 - 03	Section size	A byte count for the section (which is inclusive
             *				of the byte count itself and should always be a
             *				multiple of 4);
             *   04 - 07	Property count	A count of the number of properties
             *   08 - xx   			An array of 32-bit Property ID/Offset pairs
             *   yy - zz			An array of Property Type indicators/Value pairs
             */
            for (uint i = 0; i < num_sections; i++)
            {
                if (input.Seek(sections[i].Offset, SeekOrigin.Begin) || (data = input.Read(8, null)) == null)
                    return new Exception("Invalid MS property section");

                sections[i].IConvHandle = null;
                sections[i].CharSize = 1;
                sections[i].Dict = null;
                sections[i].Size = BitConverter.ToUInt32(data, 0); // Includes header
                sections[i].NumProps = BitConverter.ToUInt32(data, 4);

                if (sections[i].NumProps <= 0)
                    continue;

                if (sections[i].NumProps > input.Remaining() / 8)
                    return new Exception("Invalid MS property stream header or file truncated");

                if (sections[i].Offset + sections[i].Size > input.Size)
                    return new Exception("Invalid MS property stream header or file truncated");

                /*
                 * Get and save all the Property ID/Offset pairs.
                 * Format (bytes):
                 *   00 - 03	id	Property ID
                 *   04 - 07	offset	The distance from the start of the section to the
                 *			start of the Property Type/Value pair.
                 */
                GsfMSOleMetaDataProp[] props = new GsfMSOleMetaDataProp[sections[i].NumProps];
                for (uint j = 0; j < sections[i].NumProps; j++)
                {
                    if ((data = input.Read(8, null)) == null)
                        return new Exception("Invalid MS property section");

                    props[j].Id = BitConverter.ToUInt32(data, 0);
                    props[j].Offset = BitConverter.ToUInt32(data, 4);
                }

                // FIXME: Should we check that ids are distinct?

                // Order prop info by offset to facilitate bounds checking
                List<GsfMSOleMetaDataProp> tempProps = new List<GsfMSOleMetaDataProp>(props);
                tempProps.Sort(PropertyCompare);
                props = tempProps.ToArray();

                // Sanity checks.
                for (uint j = 0; j < sections[i].NumProps; j++)
                {
                    uint end = (uint)((j == sections[i].NumProps - 1) ? sections[i].Size : props[j + 1].Offset);
                    if (props[j].Offset < 0 || props[j].Offset + 4 > end)
                        return new Exception("Invalid MS property section");
                }

                // Find and process the code page.
                // Property ID 1 is reserved as an indicator of the code page.
                sections[i].IConvHandle = null;
                sections[i].CharSize = 1;

                for (uint j = 0; j < sections[i].NumProps; j++) // First codepage
                {
                    if (props[j].Id == 1)
                    {
                        sections[i].PropertyRead(input, props, j, this);
                        if ((prop = Lookup(GsfMetaNames.GSF_META_NAME_CODEPAGE)) != null)
                        {
                            object val = prop.Value;
                            if (val != null && val is int)
                            {
                                int codepage = (int)val;
                                sections[i].IConvHandle = Encoding.GetEncoding(codepage);
                                sections[i].CharSize = (uint)CodePageCharSize(codepage);
                            }
                        }
                    }
                }

                if (sections[i].IConvHandle == null)
                    sections[i].IConvHandle = Encoding.GetEncoding(1252);

                // Find and process the Property Set Dictionary
                // Property ID 0 is reserved as an indicator of the dictionary.
                // For User Defined Sections, Property ID 0 is NOT a dictionary.

                for (uint j = 0; j < sections[i].NumProps; j++) // The dictionary
                {
                    if (props[j].Id == 0)
                        sections[i].PropertyRead(input, props, j, this);
                }

                // Process all the properties
                for (uint j = 0; j < sections[i].NumProps; j++) // The rest
                {
                    if (props[j].Id > 1)
                        sections[i].PropertyRead(input, props, j, this);
                }

                sections[i].IConvHandle = null;
                if (sections[i].Dict != null)
                    sections[i].Dict = null;
            }

            return null;
        }

        /// <summary>
        /// If <paramref name="name"/> does not exist in the collection, do nothing. If @name does exist,
        /// remove it and its value from the collection
        /// </summary>
        /// <param name="name">The non-null string name of the property</param>
        public void Remove(string name)
        {
            if (name == null)
                return;

            if (!Table.ContainsKey(name))
                return;

            Table.Remove(name);
        }

        /// <returns>The property with <paramref name="name"/> in meta.</returns>
        public GsfDocProp Steal(string name)
        {
            if (name == null)
                return null;

            if (!Table.ContainsKey(name))
                return null;

            GsfDocProp prop = Table[name];
            if (prop != null)
                Table.Remove(name);

            return prop;
        }

        public void Store(GsfDocProp prop)
        {
            if (prop == null)
                return;

            if (Table.ContainsKey(prop.Name) && Table[prop.Name] == prop)
                return;

            Table[prop.Name] = prop;
        }

        /// <returns>The number of items in this collection</returns>
        public int Size() => Table.Count;

        /// <summary></summary>
        /// <param name="doc_not_component">A kludge to differentiate DocumentSummary from Summary</param>
        /// <returns>True on success</returns>
        /// <remarks>Since: 1.14.24</remarks>
        public bool WriteToMSOLE(GsfOutput output, bool doc_not_component)
        {
            byte[] header =
            {
                0xfe, 0xff,	// Byte order
                   0,    0,	// Format
                0x04, 0x0a,	// OS : XP == 0xA04
                0x02, 0x00,	// Win32 == 2
                0,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0, // Clasid = 0
            };

            bool success = false;
            byte[] buf = new byte[4];

            const int default_codepage = 1252;

            WritePropState state = new WritePropState
            {
                CodePage = 0,
                IConvHandle = null,
                CharSize = 1,
                Output = output,
                Dict = null,
                BuiltIn = new WritePropStatePropList
                {
                    Count = 1, // Codepage
                    Props = null,
                },
                User = new WritePropStatePropList
                {
                    Count = 2, // Codepage and Dictionary
                    Props = null,
                },
                DocNotComponent = doc_not_component,
            };

            foreach (var prop in Table)
            {
                state.CountProperties(prop.Key, prop.Value);
            }

            state.IConvHandle = Encoding.GetEncoding(default_codepage);
            if (state.CodePage == 0)
            {
                state.GuessCodePage(false);
                if (state.Dict != null)
                    state.GuessCodePage(true);

                if (state.CodePage == 0)
                    state.CodePage = default_codepage;
            }

            state.IConvHandle = Encoding.GetEncoding(state.CodePage);
            state.CharSize = CodePageCharSize(state.CodePage);

            // Write stream header
            GSF_LE_SET_GUINT32(buf, 0, (uint)(state.Dict != null ? 2 : 1));
            if (!output.Write(header.Length, header) || !output.Write(4, buf))
            {
                state.IConvHandle = null;
                state.BuiltIn.Props = null;
                state.User.Props = null;
                state.Dict = null;
                return success;
            }

            // Write section header(s)
            GSF_LE_SET_GUINT32(buf, 0, (uint)(state.Dict != null ? 0x44 : 0x30));
            if (!output.Write(16, doc_not_component ? DocumentGUID : ComponentGUID) || !output.Write(4, buf))
            {
                state.IConvHandle = null;
                state.BuiltIn.Props = null;
                state.User.Props = null;
                state.Dict = null;
                return success;
            }

            if (state.Dict != null)
            {
                GSF_LE_SET_GUINT32(buf, 0, 0);
                if (!output.Write(UserGUID.Length, UserGUID) || !output.Write(4, buf)) // Bogus position, fix it later
                {
                    state.IConvHandle = null;
                    state.BuiltIn.Props = null;
                    state.User.Props = null;
                    state.Dict = null;
                    return success;
                }
            }

            // Write section(s)
            if (!state.WriteSection(false))
            {
                state.IConvHandle = null;
                state.BuiltIn.Props = null;
                state.User.Props = null;
                state.Dict = null;
                return success;
            }

            if (state.Dict != null)
            {
                long baseOffset = state.Output.CurrentOffset;
                GSF_LE_SET_GUINT32(buf, 0, (uint)baseOffset);
                if (!state.Output.Seek(0x40, SeekOrigin.Begin)
                    || !output.Write(4, buf)
                    || !state.Output.Seek(0, SeekOrigin.End)
                    || !state.WriteSection(true))
                {
                    state.IConvHandle = null;
                    state.BuiltIn.Props = null;
                    state.User.Props = null;
                    state.Dict = null;
                    return success;
                }
            }

            state.IConvHandle = null;
            state.BuiltIn.Props = null;
            state.User.Props = null;
            state.Dict = null;
            return true;
        }

        #endregion

        #region Utilities

        private int CodePageCharSize(int codepage) => (codepage == 1200 || codepage == 1201 ? 2 : 1);

        private static int PropertyCompare(GsfMSOleMetaDataProp prop_a, GsfMSOleMetaDataProp prop_b)
        {
            if (prop_a.Offset < prop_b.Offset)
                return -1;
            else if (prop_a.Offset > prop_b.Offset)
                return +1;
            else
                return 0;
        }

        #endregion
    }
}
