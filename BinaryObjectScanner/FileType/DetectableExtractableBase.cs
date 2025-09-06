using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Base class for all standard detectable/extractable types
    /// </summary>
    public abstract class DetectableExtractableBase : IDetectable, IExtractable
    {
        #region Constructors

        public DetectableExtractableBase() { }

        #endregion

        #region IDetectable Implementations

        /// <inheritdoc/>
        public string? Detect(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Detect(fs, file, includeDebug);
        }

        /// <inheritdoc/>
        public abstract string? Detect(Stream stream, string file, bool includeDebug);

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