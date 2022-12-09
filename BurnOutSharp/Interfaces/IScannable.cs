using System.Collections.Concurrent;
using System.IO;

namespace BurnOutSharp.Interfaces
{
    /// <summary>
    /// Mark a file type as extractable
    /// </summary>
    internal interface IScannable
    {
        /// <summary>
        /// Scan a file for all internal protections
        /// </summary>
        /// <param name="scanner">Scanner object for state tracking</param>
        /// <param name="file">Path to the input file</param>
        /// <returns>Dictionary mapping paths to protection lists</returns>
        /// <remarks>Ideally, this should just point to the other scan implementation</remarks>
        ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file);

        /// <summary>
        /// Scan a stream for all internal protections
        /// </summary>
        /// <param name="scanner">Scanner object for state tracking</param>
        /// <param name="stream">Stream representing the input file</param>
        /// <param name="file">Path to the input file</param>
        /// <returns>Dictionary mapping paths to protection lists</returns>
        ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file);
    }
}
