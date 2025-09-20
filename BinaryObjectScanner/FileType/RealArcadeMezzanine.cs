using System;
using System.IO;
using SabreTools.IO.Extensions;
using SabreTools.Matching;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// RealArcade Mezzanine files, which contain metadata. Known to use the ".mez" file extension.
    /// 
    /// TODO: Add further parsing, game ID should be possible to parse.
    /// </summary>
    public class RealArcadeMezzanine : DetectableBase
    {
        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
        {
            try
            {
                byte[] magic = stream.ReadBytes(16);

                // XZip2.0
                // Found in the ".mez" files in IA item "Nova_RealArcadeCD_USA".
                if (magic.StartsWith(new byte?[] { 0x58, 0x5A, 0x69, 0x70, 0x32, 0x2E, 0x30 }))
                    return "RealArcade Mezzanine";
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
            }

            return null;
        }
    }
}
