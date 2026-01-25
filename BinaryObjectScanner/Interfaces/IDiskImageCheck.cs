using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Check a disk image for protection
    /// </summary>
    public interface IDiskImageCheck<T> where T : WrapperBase
    {
        /// <summary>
        /// Check a path for protections based on file contents
        /// </summary>
        /// <param name="file">File to check for protection indicators</param>
        /// <param name="diskImage">Disk image representing the read-in file</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>String containing any protections found in the file</returns>
        public string? CheckDiskImage(string file, T diskImage, bool includeDebug);
    }
}
