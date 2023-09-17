using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// PlayJ audio file
    /// </summary>
    public class PLJ : IDetectable
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

                if (magic.StartsWith(new byte?[] { 0xFF, 0x9D, 0x53, 0x4B }))
                    return "PlayJ Audio File";
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
