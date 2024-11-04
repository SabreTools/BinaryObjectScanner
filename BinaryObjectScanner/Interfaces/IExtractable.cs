using System.IO;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Mark a file type as being able to be extracted
    /// </summary>
    public interface IExtractable
    {
        /// <summary>
        /// Extract a file to a temporary path, if possible
        /// </summary>
        /// <param name="file">Path to the input file</param>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Indicates if the extractable was successfully extracted</returns>
        /// <remarks>Ideally, this should just point to the other extract implementation.</remarks>
        bool Extract(string file, string outDir, bool includeDebug);

        /// <summary>
        /// Extract a stream to a temporary path, if possible
        /// </summary>
        /// <param name="stream">Stream representing the input file</param>
        /// <param name="file">Path to the input file</param>
        /// <param name="outDir">Path to the output directory</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Indicates if the extractable was successfully extracted</returns>
        bool Extract(Stream? stream, string file, string outDir, bool includeDebug);
    }
}
