using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class MSDOS
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
        private BurnOutSharp.Models.MSDOS.Executable _executable;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private MSDOS() { }

        /// <summary>
        /// Create an MS-DOS executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An MS-DOS executable wrapper on success, null on failure</returns>
        public static MSDOS Create(byte[] data, int offset)
        {
            var executable = BurnOutSharp.Builder.MSDOS.ParseExecutable(data, offset);
            if (executable == null)
                return null;

            var wrapper = new MSDOS { _executable = executable };
            return wrapper;
        }

        /// <summary>
        /// Create an MS-DOS executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An MS-DOS executable wrapper on success, null on failure</returns>
        public static MSDOS Create(Stream data)
        {
            var executable = BurnOutSharp.Builder.MSDOS.ParseExecutable(data);
            if (executable == null)
                return null;

            var wrapper = new MSDOS { _executable = executable };
            return wrapper;
        }
    }
}