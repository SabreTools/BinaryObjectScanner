using System.Collections.Generic;
using System.IO;

namespace BurnOutSharp
{
    internal interface IScannable
    {
        /// <summary>
        /// Determine if a file signature matches one of the expected values
        /// </summary>
        /// <param name="magic">Byte array representing the file header</param>
        /// <returns>True if the signature is valid, false otherwise</returns>
        bool ShouldScan(byte[] magic);

        /// <summary>
        /// Scan a file for all internal protections
        /// </summary>
        /// <param name="scanner">Scanner object for state tracking</param>
        /// <param name="file">Path to the input file</param>
        /// <returns>Dictionary mapping paths to protection lists</returns>
        /// <remarks>Ideally, this should just point to the other scan implementation</remarks>
        Dictionary<string, List<string>> Scan(Scanner scanner, string file);

        /// <summary>
        /// Scan a stream for all internal protections
        /// </summary>
        /// <param name="scanner">Scanner object for state tracking</param>
        /// <param name="stream">Stream representing the input file</param>
        /// <param name="file">Path to the input file</param>
        /// <returns>Dictionary mapping paths to protection lists</returns>
        Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream, string filename);
    }
}
