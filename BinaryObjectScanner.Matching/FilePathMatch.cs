using System.IO;

namespace BinaryObjectScanner.Matching
{
    /// <summary>
    /// File path matching criteria
    /// </summary>
    public class FilePathMatch : PathMatch
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="needle">String representing the search</param>
        /// <param name="matchExact">True to match exact casing, false otherwise</param>
        /// <param name="useEndsWith">True to match the end only, false for all contents</param>
        public FilePathMatch(string needle) : base($"{Path.DirectorySeparatorChar}{needle}", false, true) { }
    }
}