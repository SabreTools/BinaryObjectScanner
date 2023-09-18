using System;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// AACS media key block
    /// </summary>
    public class AACSMediaKeyBlock : IDetectable
    {
        /// <inheritdoc/>
#if NET48
        public string Detect(string file, bool includeDebug)
#else
        public string? Detect(string file, bool includeDebug)
#endif
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Detect(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
#if NET48
        public string Detect(Stream stream, string file, bool includeDebug)
#else
        public string? Detect(Stream stream, string file, bool includeDebug)
#endif
        {
            // If the MKB file itself fails
            try
            {
                // Create the wrapper
                var mkb = SabreTools.Serialization.Wrappers.AACSMediaKeyBlock.Create(stream);
                if (mkb == null)
                    return null;

                // Derive the version, if possible
                var typeAndVersion = mkb.Model.Records?.FirstOrDefault(r => r?.RecordType == SabreTools.Models.AACS.RecordType.TypeAndVersion);
                if (typeAndVersion == null)
                    return "AACS (Unknown Version)";
                else
                    return $"AACS {(typeAndVersion as SabreTools.Models.AACS.TypeAndVersionRecord)?.VersionNumber}";
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
