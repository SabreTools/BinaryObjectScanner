using System;
using System.IO;
using System.Security.Cryptography;

namespace BinaryObjectScanner.Utilities
{
    /// <summary>
    /// Data hashing methods
    /// </summary>
    public static class Hashing
    {
        /// <summary>
        /// Get the SHA1 hash of a file, if possible
        /// </summary>
        /// <param name="path">Path to the file to be hashed</param>
        /// <returns>SHA1 hash as a string on success, null on error</returns>
        public static string? GetFileSHA1(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            try
            {
                var sha1 = SHA1.Create();
                using (Stream fileStream = File.OpenRead(path))
                {
                    byte[] buffer = new byte[32768];
                    while (true)
                    {
                        int bytesRead = fileStream.Read(buffer, 0, 32768);
                        if (bytesRead == 32768)
                        {
                            sha1.TransformBlock(buffer, 0, bytesRead, null, 0);
                        }
                        else
                        {
                            sha1.TransformFinalBlock(buffer, 0, bytesRead);
                            break;
                        }
                    }
                }

                string hash = BitConverter.ToString(sha1.Hash!);
                hash = hash.Replace("-", string.Empty);
                return hash;
            }
            catch
            {
                return null;
            }
        }
    }
}
