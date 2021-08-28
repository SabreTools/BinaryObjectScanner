using System.Collections.Generic;
using BurnOutSharp.Matching;

namespace BurnOutSharp
{
    // TODO: This should either include an override that takes a Stream instead of the byte[]
    // OR have a completely separate check for when it's an executable specifically
    // TODO: Once more checks have been converted over to using PortableExecutable, NewExecutable,
    // and DOS Executable, then add an override for `CheckContents` that takes an executable type
    // as one of the arguments. This will reduce the amount of times the same file will be parsed
    // into an in-memory header
    internal interface IContentCheck
    {
        /// <summary>
        /// Get a list of content match sets that represent a protection
        /// </summary>
        /// <returns>List of content match sets, null if not applicable</returns>
        List<ContentMatchSet> GetContentMatchSets();

        /// <summary>
        /// Check a path for protections based on file contents
        /// </summary>
        /// <param name="file">File to check for protection indicators</param>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>String containing any protections found in the file</returns>
        string CheckContents(string file, byte[] fileContent, bool includeDebug);
    }
}
