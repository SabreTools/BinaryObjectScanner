#if NET40_OR_GREATER || NETCOREAPP
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Check a file or directory path for protection
    /// </summary>
    /// <remarks>
    /// These checks rely primarily on filenames and paths, not file contents
    /// </remarks>
    public interface IPathCheck
    {
        /// <summary>
        /// Check a file path for protections based on path name
        /// </summary>
        /// <param name="path">Path to check for protection indicators</param>
        /// <param name="files">Enumerable of strings representing files in a directory</param>
        /// <remarks>This can do some limited content checking as well, but it's suggested to use a content check instead, if possible</remarks>
#if NET20 || NET35
        List<string> CheckDirectoryPath(string path, IEnumerable<string>? files);
#else
        ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string>? files);
#endif

        /// <summary>
        /// Check a file path for protections based on path name
        /// </summary>
        /// <param name="path">Path to check for protection indicators</param>
        /// <remarks>This can do some limited content checking as well, but it's suggested to use a content check instead, if possible</remarks>
        string? CheckFilePath(string path);
    }
}
