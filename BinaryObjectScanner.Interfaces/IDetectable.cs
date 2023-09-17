using System.IO;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Mark a file type as being able to be detected
    /// </summary>
    public interface IDetectable
    {
        /// <summary>
        /// Check if a file is detected as this file type
        /// </summary>
        /// <param name="file">Path to the input file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Detected file or protection type, null on error</returns>
        /// <remarks>Ideally, this should just point to the other detect implementation.</remarks>
#if NET48
        string Detect(string file, bool includeDebug);
#else
        string? Detect(string file, bool includeDebug);
#endif

        /// <summary>
        /// Check if a stream is detected as this file type
        /// </summary>
        /// <param name="stream">Stream representing the input file</param>
        /// <param name="file">Path to the input file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Detected file or protection type, null on error</returns>
#if NET48
        string Detect(Stream stream, string file, bool includeDebug);
#else
        string? Detect(Stream stream, string file, bool includeDebug);
#endif
    }
}
