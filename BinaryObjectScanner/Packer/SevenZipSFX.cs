using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // TODO: Add extraction
    public class SevenZipSFX : IExtractable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
#if NET48
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#else
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
#endif
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
#if NET48
        public string Extract(string file, bool includeDebug)
#else
        public string? Extract(string file, bool includeDebug)
#endif
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(Stream stream, string file, bool includeDebug)
#else
        public string? Extract(Stream? stream, string file, bool includeDebug)
#endif
        {
            return null;
        }
    }
}
