using BurnOutSharp.ExecutableType.Microsoft.NE;

namespace BurnOutSharp
{
    // TODO: This should either include an override that takes a Stream instead of the byte[]
    internal interface INEContentCheck
    {
        /// <summary>
        /// Check a path for protections based on file contents
        /// </summary>
        /// <param name="file">File to check for protection indicators</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <param name="nex">NewExecutable representing the read-in file</param>
        /// <returns>String containing any protections found in the file</returns>
        string CheckNEContents(string file, bool includeDebug, NewExecutable nex);
    }
}
