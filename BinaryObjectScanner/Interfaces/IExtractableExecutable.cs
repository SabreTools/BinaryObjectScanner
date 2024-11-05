using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Mark an executable type as being able to be extracted
    /// </summary>
    public interface IExtractableExecutable<T> : IExecutableCheck<T> where T : WrapperBase
    {
        /// <summary>
        /// Extract an Executable to a path, if possible
        /// </summary>
        /// <param name="file">Path to the input file</param>
        /// <param name="exe">Executable representing the read-in file</param>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Path to extracted files, null on error</returns>
        bool Extract(string file, T exe, string outDir, bool includeDebug);
    }
}
