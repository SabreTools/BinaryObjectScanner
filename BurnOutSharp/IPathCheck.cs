using System.Collections.Generic;

namespace BurnOutSharp
{
    public interface IPathCheck
    {
        /// <summary>
        /// Check a path for protections based on file and directory names
        /// </summary>
        /// <param name="path">Path to check for protection indicators</param>
        /// <param name="files">Enumerable of strings representing files in a directory if the path is a directory, assumed null otherwise</param>
        /// <param name="isDirectory">True if the path represents a directory, false otherwise</param>
        /// <returns>String containing any protections found in the path</returns>
        string CheckPath(string path, IEnumerable<string> files, bool isDirectory);
    }
}
