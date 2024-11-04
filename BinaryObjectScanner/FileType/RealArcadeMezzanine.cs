using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// RealArcade Mezzanine files, which contain metadata. Known to use the ".mez" file extension.
    /// 
    /// TODO: Add further parsing, game ID should be possible to parse.
    /// </summary>
    public class RealArcadeMezzanine : IDetectable
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
                stream.Read(magic, 0, 16);

                // XZip2.0
                // Found in the ".mez" files in IA item "Nova_RealArcadeCD_USA".
                if (magic.StartsWith(new byte?[] { 0x58, 0x5A, 0x69, 0x70, 0x32, 0x2E, 0x30 }))
                    return "RealArcade Mezzanine";
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
