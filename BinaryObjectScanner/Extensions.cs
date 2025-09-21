using System.IO;

namespace BinaryObjectScanner
{
    internal static class Extensions
    {
        /// <summary>
        /// Helper to get the filesize from a path
        /// </summary>
        /// <returns>Size of the file path, -1 on error</returns>
        public static long FileSize(this string? filename)
        {
            // Invalid filenames are ignored
            if (string.IsNullOrEmpty(filename))
                return -1;

            // Non-file paths are ignored
            if (!File.Exists(filename))
                return -1;

            try
            {
                return new FileInfo(filename).Length;
            }
            catch
            {
                // Ignore errors
                return -1;
            }
        }
    }
}