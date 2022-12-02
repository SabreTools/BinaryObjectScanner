using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class LinearExecutable
    {
        #region Pass-Through Properties

        // TODO: Determine what properties can be passed through

        #endregion

        #region Extension Properties

        // TODO: Determine what extension properties are needed

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the executable
        /// </summary>
        private BurnOutSharp.Models.LinearExecutable.Executable _executable;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private LinearExecutable() { }

        /// <summary>
        /// Create an LE/LX executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An LE/LX executable wrapper on success, null on failure</returns>
        public static LinearExecutable Create(byte[] data, int offset)
        {
            var executable = BurnOutSharp.Builder.LinearExecutable.ParseExecutable(data, offset);
            if (executable == null)
                return null;

            var wrapper = new LinearExecutable { _executable = executable };
            return wrapper;
        }

        /// <summary>
        /// Create an LE/LX executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An LE/LX executable wrapper on success, null on failure</returns>
        public static LinearExecutable Create(Stream data)
        {
            var executable = BurnOutSharp.Builder.LinearExecutable.ParseExecutable(data);
            if (executable == null)
                return null;

            var wrapper = new LinearExecutable { _executable = executable };
            return wrapper;
        }
    }
}