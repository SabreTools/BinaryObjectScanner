using System.Collections.Concurrent;
using System.IO;
using BurnOutSharp;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Mark a file type as being able to be scanned
    /// </summary>
    /// <remarks>
    /// This is also used for packers, embedded archives, and other
    /// installer formats that may need to be "extracted" before they
    /// can be fully scanned.
    /// </remarks>
    public interface IScannable
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
