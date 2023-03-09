using System.IO;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Mark a file type as being able to be extracted
    /// </summary>
    /// TODO: Add debug flag to both of the declarations
    public interface IExtractable
    {
        /// <summary>
        /// Scan a file for all internal protections
        /// </summary>
        /// <param name="file">Path to the input file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Path to extracted files, null on error</returns>
        /// <remarks>Ideally, this should just point to the other extract implementation.</remarks>
        string Extract(string file, bool includeDebug);

        /// <summary>
        /// Scan a stream for all internal protections
        /// </summary>
        /// <param name="stream">Stream representing the input file</param>
        /// <param name="file">Path to the input file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>Path to extracted files, null on error</returns>
        string Extract(Stream stream, string file, bool includeDebug);
    }
}
