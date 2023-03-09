namespace BinaryObjectScanner.Interfaces
{
    /// <summary>
    /// Check a generic file for protection
    /// </summary>
    public interface IContentCheck
    {
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
