using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// StarForce Filesystem file
    /// </summary>
    /// <see href="https://forum.xentax.com/viewtopic.php?f=21&t=2084"/>
    public class SFFS : IExtractable, IDetectable
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
            try
            {
                byte[] magic = new byte[16];
                stream.Read(magic, 0, 16);

                if (magic.StartsWith(new byte?[] { 0x53, 0x46, 0x46, 0x53 }))
                    return "StarForce Filesystem Container";
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
            }

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
        public string? Extract(Stream stream, string file, bool includeDebug)
#endif
        {
            return null;
        }
    }
}
