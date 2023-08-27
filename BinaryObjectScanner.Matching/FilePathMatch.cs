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
        public FilePathMatch(string needle) : base($"{Path.DirectorySeparatorChar}{needle}", false, true) { }
    }
}