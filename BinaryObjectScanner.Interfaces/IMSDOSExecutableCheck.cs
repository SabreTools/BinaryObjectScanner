using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Check a MS-DOS Executable (MZ) for protection
    /// </summary>
    public interface IMSDOSExecutableCheck
    {
        /// <summary>
        /// Check a path for protections based on file contents
        /// </summary>
        /// <param name="file">File to check for protection indicators</param>
        /// <param name="mz">MSDOS representing the read-in file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>String containing any protections found in the file</returns>
#if NET48
        string CheckMSDOSExecutable(string file, MSDOS mz, bool includeDebug);
#else
        string? CheckMSDOSExecutable(string file, MSDOS mz, bool includeDebug);
#endif
    }
}
