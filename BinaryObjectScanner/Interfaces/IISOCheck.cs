using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Check an ISO for protection
    /// </summary>
    public interface IISOCheck<T> where T : WrapperBase
    {
        /// <summary>
        /// Check a path for protections based on file contents
        /// </summary>
        /// <param name="file">File to check for protection indicators</param>
        /// <param name="iso">ISO representing the read-in file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>String containing any protections found in the file</returns>
        string? CheckISO(string file, T iso, bool includeDebug);
    }
}