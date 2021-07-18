using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BurnOutSharp
{
    internal interface IPathCheck
    {
        /// <summary>
        /// Check a file path for protections based on path name
        /// </summary>
        /// <param name="path">Path to check for protection indicators</param>
        /// <param name="files">Enumerable of strings representing files in a directory</param>
        /// <remarks>This can do some limited content checking as well, but it's suggested to use IContentCheck instead, if possible</remarks>
        ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files);

        /// <summary>
        /// Check a file path for protections based on path name
        /// </summary>
        /// <param name="path">Path to check for protection indicators</param>
        /// <remarks>This can do some limited content checking as well, but it's suggested to use IContentCheck instead, if possible</remarks>
        string CheckFilePath(string path);
    }
}
