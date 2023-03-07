using System;
using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp.Interfaces;
using static BinaryObjectScanner.Utilities.Dictionary;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Link Data Security encrypted file
    /// </summary>
    public class LDSCRYPT : IScannable
    {
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
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

                if (Tools.Utilities.GetFileType(magic) == SupportedFileType.LDSCRYPT)
                {
                    AppendToDictionary(protections, file, "Link Data Security encrypted file");
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
