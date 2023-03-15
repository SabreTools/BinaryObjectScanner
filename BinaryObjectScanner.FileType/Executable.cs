using System;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Executable or library
    /// </summary>
    /// <remarks>
    /// Due to the complexity of executables, all actual handling is offloaded to
    /// another class that is used by the scanner
    /// </remarks>
    public class Executable : IDetectable, IExtractable
    {
        /// <inheritdoc/>
        public string Detect(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Detect(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        /// <remarks>This implementation should never be invoked</remarks>
        public string Detect(Stream stream, string file, bool includeDebug)
        {
            throw new InvalidOperationException();
        }

        /// <inheritdoc/>
        public string Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        /// <remarks>This implementation should never be invoked</remarks>
        public string Extract(Stream stream, string file, bool includeDebug)
        {
            throw new InvalidOperationException();
        }
    }
}
