using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Check a New Executable (NE) for protection
    /// </summary>
    public interface INewExecutableCheck
    {
        /// <summary>
        /// Check a path for protections based on file contents
        /// </summary>
        /// <param name="file">File to check for protection indicators</param>
        /// <param name="nex">NewExecutable representing the read-in file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>String containing any protections found in the file</returns>
        string? CheckNewExecutable(string file, NewExecutable nex, bool includeDebug);
    }
}
