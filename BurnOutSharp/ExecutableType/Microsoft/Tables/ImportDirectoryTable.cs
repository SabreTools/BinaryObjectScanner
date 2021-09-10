using System.Collections.Generic;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.Entries;

namespace BurnOutSharp.ExecutableType.Microsoft.Tables
{
    /// <summary>
    /// The import information begins with the import directory table, which describes the remainder of the import information.
    /// The import directory table contains address information that is used to resolve fixup references to the entry points within a DLL image.
    /// The import directory table consists of an array of import directory entries, one entry for each DLL to which the image refers.
    /// The last directory entry is empty (filled with null values), which indicates the end of the directory table.
    /// </summary>
    /// <remarks>https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#import-directory-table</remarks>
    public class ImportDirectoryTable
    {
        /// <remarks>Number of entries is known after parsing</remarks>
        public ImportDirectoryTableEntry[] Entries;

        public static ImportDirectoryTable Deserialize(Stream stream)
        {
            var idt = new ImportDirectoryTable();

            List<ImportDirectoryTableEntry> tempEntries = new List<ImportDirectoryTableEntry>();
            while (true)
            {
                var entry = ImportDirectoryTableEntry.Deserialize(stream);
                tempEntries.Add(entry);
                if (entry.IsNull())
                    break;
            }

            idt.Entries = tempEntries.ToArray();
            return idt;
        }

        public static ImportDirectoryTable Deserialize(byte[] content, ref int offset)
        {
            var idt = new ImportDirectoryTable();

            List<ImportDirectoryTableEntry> tempEntries = new List<ImportDirectoryTableEntry>();
            while (true)
            {
                var entry = ImportDirectoryTableEntry.Deserialize(content, ref offset);
                tempEntries.Add(entry);
                if (entry.IsNull())
                    break;
            }

            idt.Entries = tempEntries.ToArray();
            return idt;
        }
    }
}