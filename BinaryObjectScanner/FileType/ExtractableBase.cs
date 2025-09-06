using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Base class for all standard extractable types
    /// </summary>
    public abstract class ExtractableBase : IExtractable
    {
        #region Constructors

        public ExtractableBase() { }

        #endregion

        #region IExtractable Implementations

        /// <inheritdoc/>
        public bool Extract(string file, string outDir, bool includeDebug)
        {
            if (!File.Exists(file))
                return false;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, outDir, includeDebug);
        }

        /// <inheritdoc/>
        public abstract bool Extract(Stream? stream, string file, string outDir, bool includeDebug);

        #endregion
    }
}