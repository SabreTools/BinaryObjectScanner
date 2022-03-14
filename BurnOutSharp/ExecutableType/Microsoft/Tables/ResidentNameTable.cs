using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.Entries;

namespace BurnOutSharp.ExecutableType.Microsoft.Tables
{
    /// <summary>
    /// The resident-name table follows the resource table, and contains this
    /// module's name string and resident exported procedure name strings. The
    /// first string in this table is this module's name. These name strings
    /// are case-sensitive and are not null-terminated.
    /// </summary>
    public class ResidentNameTable
    {
        /// <summary>
        /// The first string in this table is this module's name.
        /// These name strings are case-sensitive and are not null-terminated.
        /// </summary>
        public ResidentNameTableEntry[] NameTableEntries;

        public static ResidentNameTable Deserialize(Stream stream)
        {
            var rnt = new ResidentNameTable();

            var nameTableEntries = new List<ResidentNameTableEntry>();
            while (true)
            {
                var rnte = ResidentNameTableEntry.Deserialize(stream);
                if (rnte == null || rnte.Length == 0)
                    break;

                nameTableEntries.Add(rnte);
            }

            rnt.NameTableEntries = nameTableEntries.ToArray();

            return rnt;
        }

        public static ResidentNameTable Deserialize(byte[] content, ref int offset)
        {
            var rnt = new ResidentNameTable();

            var nameTableEntries = new List<ResidentNameTableEntry>();
            while (true)
            {
                var rnte = ResidentNameTableEntry.Deserialize(content, ref offset);
                if (rnte == null || rnte.Length == 0)
                    break;

                nameTableEntries.Add(rnte);
            }

            rnt.NameTableEntries = nameTableEntries.ToArray();

            return rnt;
        }
    }
}