using System.IO;
using BurnOutSharp.Models.LinearExecutable;

namespace BurnOutSharp.Builder
{
    // TODO: Make Stream Data rely on Byte Data
    public static class LinearExecutable
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a Linear Executable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled executable on success, null on error</returns>
        public static Executable ParseExecutable(byte[] data, int offset)
        {
            // TODO: Implement LE/LX parsing
            return null;
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a Linear Executable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled executable on success, null on error</returns>
        public static Executable ParseExecutable(Stream data)
        {
            // TODO: Implement LE/LX parsing
            return null;
        }

        #endregion
    }
}