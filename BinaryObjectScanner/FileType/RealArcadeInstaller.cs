using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// RealArcade Installer. Known to use the ".rgs" file extension.
    /// 
    /// TODO: Add further parsing, game ID and name should be possible to parse.
    /// </summary>
    public class RealArcadeInstaller : IDetectable
    {
        /// <inheritdoc/>
        public string? Detect(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Detect(fs, file, includeDebug);
        }

        /// <inheritdoc/>
        public string? Detect(Stream stream, string file, bool includeDebug)
        {
            try
            {
                byte[] magic = new byte[16];
                int read = stream.Read(magic, 0, 16);

                // RASGI2.0
                // Found in the ".rgs" files in IA item "Nova_RealArcadeCD_USA".
#if NET20
                if (Extensions.StartsWith(magic, new byte?[] { 0x52, 0x41, 0x53, 0x47, 0x49, 0x32, 0x2E, 0x30 }))
#else
                if (magic.StartsWith(new byte?[] { 0x52, 0x41, 0x53, 0x47, 0x49, 0x32, 0x2E, 0x30 }))
#endif
                    return "RealArcade Installer";
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
