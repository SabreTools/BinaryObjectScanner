namespace BurnOutSharp
{
    public interface IContentCheck
    {
        /// <summary>
        /// Check a path for protections based on file contents
        /// </summary>
        /// <param name="file">File to check for protection indicators</param>
        /// <param name="fileContent">Byte array representing the file contents</param>
        /// <param name="includePosition">True to include positional data, false otherwise</param>
        /// <returns>String containing any protections found in the file</returns>
        /// TODO: This should be replaced with a "GenerateMatchers" that produces a list of matchers to be run instead
        string CheckContents(string file, byte[] fileContent, bool includePosition);
    }
}
