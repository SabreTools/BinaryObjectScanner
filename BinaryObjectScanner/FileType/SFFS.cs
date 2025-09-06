using System;
using System.IO;
using SabreTools.Matching;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// StarForce Filesystem file
    /// </summary>
    /// <see href="https://forum.xentax.com/viewtopic.php?f=21&t=2084"/>
    public class SFFS : DetectableExtractableBase
    {
        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
        {
            try
            {
                byte[] magic = new byte[16];
                int read = stream.Read(magic, 0, 16);

                if (magic.StartsWith(new byte?[] { 0x53, 0x46, 0x46, 0x53 }))
                    return "StarForce Filesystem Container";
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
            }

            return null;
        }

        /// <inheritdoc/>
        public override bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            return false;
        }
    }
}
