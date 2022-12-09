using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Tools;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// PlayJ audio file
    /// </summary>
    public class PLJ : IScannable
    {
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
            try
            {
                byte[] magic = new byte[16];
                stream.Read(magic, 0, 16);

                if (Utilities.GetFileType(magic) == SupportedFileType.PLJ)
                {
                    Utilities.AppendToDictionary(protections, file, "PlayJ Audio File");
                    return protections;
                }
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
