using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Base class for all standard detectable types
    /// </summary>
    public abstract class DetectableBase : IDetectable
    {
        #region Constructors

        public DetectableBase() { }

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
    }
}