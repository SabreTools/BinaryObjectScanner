using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Check a Linear Executable (LE) for protection
    /// </summary>
    public interface ILinearExecutableCheck
    {
        /// <summary>
        /// Check a path for protections based on file contents
        /// </summary>
        /// <param name="file">File to check for protection indicators</param>
        /// <param name="lex">LinearExecutable representing the read-in file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>String containing any protections found in the file</returns>
        string CheckLinearExecutable(string file, LinearExecutable lex, bool includeDebug);
    }
}
