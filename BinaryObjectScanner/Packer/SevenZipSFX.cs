using System;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Readers;
#endif

namespace BinaryObjectScanner.Packer
{
    public class SevenZipSFX : IExtractablePortableExecutable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the assembly description, if possible
            if (pex.AssemblyDescription?.StartsWith("7-Zip Self-extracting Archive") == true)
                return $"7-Zip SFX {pex.AssemblyDescription.Substring("7-Zip Self-extracting Archive ".Length)}";

            // Get the file description, if it exists
            if (pex.FileDescription?.Equals("7z SFX") == true)
                return "7-Zip SFX";
            if (pex.FileDescription?.Equals("7z Self-Extract Setup") == true)
                return "7-Zip SFX";

            // Get the original filename, if it exists
            if (pex.OriginalFilename?.Equals("7z.sfx.exe") == true)
                return "7-Zip SFX";
            else if (pex.OriginalFilename?.Equals("7zS.sfx") == true)
                return "7-Zip SFX";

            // Get the internal name, if it exists
            if (pex.InternalName?.Equals("7z.sfx") == true)
                return "7-Zip SFX";
            else if (pex.InternalName?.Equals("7zS.sfx") == true)
                return "7-Zip SFX";

            // If any dialog boxes match
            if (pex.FindDialogByTitle("7-Zip self-extracting archive").Any())
                return "7-Zip SFX";

            return null;
        }

        /// <inheritdoc/>
        public bool Extract(string file, PortableExecutable pex, string outDir, bool includeDebug)
        {
            if (!File.Exists(file))
                return false;

#if NET462_OR_GREATER || NETCOREAPP
            try
            {
                using var sevenZip = SevenZipArchive.Open(file, new ReaderOptions() { LookForHeader = true });
                foreach (var entry in sevenZip.Entries)
                {
                    try
                    {
                        // If the entry is a directory
                        if (entry.IsDirectory)
                            continue;

                        // If the entry has an invalid key
                        if (entry.Key == null)
                            continue;

                        // If we have a partial entry due to an incomplete multi-part archive, skip it
                        if (!entry.IsComplete)
                            continue;

                        string tempFile = Path.Combine(outDir, entry.Key);
                        var directoryName = Path.GetDirectoryName(tempFile);
                        if (directoryName != null && !Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);

                        entry.WriteToFile(tempFile);
                    }
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.WriteLine(ex);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
#else
            return false;
#endif
        }
    }
}
