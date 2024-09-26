using System;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;
#if NET462_OR_GREATER || NETCOREAPP
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Readers;
#endif

namespace BinaryObjectScanner.Packer
{
    public class WinRARSFX : IExtractablePortableExecutable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            var name = pex.AssemblyDescription;
            if (name?.Contains("WinRAR archiver") == true)
                return "WinRAR SFX";

            var resources = pex.FindDialogByTitle("WinRAR self-extracting archive");
            if (resources.Any())
                return "WinRAR SFX";

            return null;
        }

        /// <inheritdoc/>
        public string? Extract(string file, PortableExecutable pex, bool includeDebug)
        {
#if NET462_OR_GREATER || NETCOREAPP
            try
            {
                // Should be using stream instead of file, but stream fails to extract anything. My guess is that the executable portion of the archive is causing stream to fail, but not file.
                using (RarArchive rarFile = RarArchive.Open(file, new ReaderOptions() { LookForHeader = true }))
                {
                    if (!rarFile.IsComplete)
                        return null;

                    string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    Directory.CreateDirectory(tempPath);

                    foreach (var entry in rarFile.Entries)
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

                            string tempFile = Path.Combine(tempPath, entry.Key);
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

                    return tempPath;
                }
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
#else
            return null;
#endif
        }
    }
}
