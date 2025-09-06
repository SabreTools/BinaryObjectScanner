using System;
using System.IO;
using SabreTools.Matching;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// PlayJ audio file
    /// </summary>
    public class PLJ : DetectableBase<SabreTools.Serialization.Wrappers.PlayJAudioFile>
    {
        /// <inheritdoc/>
        public override string? Detect(Stream stream, string file, bool includeDebug)
        {
            try
            {
                byte[] magic = new byte[16];
                int read = stream.Read(magic, 0, 16);

                if (magic.StartsWith(new byte?[] { 0xFF, 0x9D, 0x53, 0x4B }))
                    return "PlayJ Audio File";
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
            }

            return null;
        }
    }
}
