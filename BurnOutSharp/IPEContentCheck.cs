using BurnOutSharp.ExecutableType.Microsoft.PE;

namespace BurnOutSharp
{
    // TODO: This should either include an override that takes a Stream instead of the byte[]
    internal interface IPEContentCheck
    {
        /// <summary>
        /// Check a path for protections based on file contents
        /// </summary>
        /// <param name="file">File to check for protection indicators</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <param name="pex">PortableExecutable representing the read-in file</param>
        /// <returns>String containing any protections found in the file</returns>
        string CheckPEContents(string file, bool includeDebug, PortableExecutable pex);
    }
}
