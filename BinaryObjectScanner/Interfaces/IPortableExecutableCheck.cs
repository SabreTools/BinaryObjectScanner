using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Check a Portable Executable (PE) for protection
    /// </summary>
    public interface IPortableExecutableCheck
    {
        /// <summary>
        /// Check a path for protections based on file contents
        /// </summary>
        /// <param name="file">File to check for protection indicators</param>
        /// <param name="pex">PortableExecutable representing the read-in file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>String containing any protections found in the file</returns>
        string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug);
    }
}
