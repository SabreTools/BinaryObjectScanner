using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Mark a MSDOS type as being able to be extracted
    /// </summary>
    public interface IExtractableMSDOSExecutable
    {
        /// <summary>
        /// Extract a MSDOS to a temporary path, if possible
        /// </summary>
        /// <param name="file">Path to the input file</param>
        /// <param name="mz">MSDOS representing the read-in file</param>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Path to extracted files, null on error</returns>
        bool Extract(string file, MSDOS mz, string outDir, bool includeDebug);
    }
}
