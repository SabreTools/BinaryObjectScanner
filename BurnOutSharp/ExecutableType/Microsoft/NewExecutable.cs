using System;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.Headers;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    /// <summary>
    /// The WIN-NE executable format, designed for Windows 3.x, was the "NE", or "New Executable" format.
    /// Again, a 16bit format, it alleviated the maximum size restrictions that the MZ format had.
    /// </summary>
    public class NewExecutable
    {
        /// <summary>
        /// Value determining if the executable is initialized or not
        /// </summary>
        public bool Initialized { get; } = false;

        /// <summary>
        /// Source array that the executable was parsed from
        /// </summary>
        public byte[] SourceArray { get; } = null;

        /// <summary>
        /// Source stream that the executable was parsed from
        /// </summary>
        public Stream SourceStream { get; } = null;

        #region Headers

        /// <summary>
        /// he DOS stub is a valid MZ exe.
        /// This enables the develper to package both an MS-DOS and Win16 version of the program,
        /// but normally just prints "This Program requires Microsoft Windows".
        /// The e_lfanew field (offset 0x3C) points to the NE header.
        // </summary>
        public MSDOSExecutableHeader DOSStubHeader;

        /// <summary>
        /// The NE header is a relatively large structure with multiple characteristics.
        /// Because of the age of the format some items are unclear in meaning. 
        /// </summary>
        public NewExecutableHeader NewExecutableHeader;

        #endregion

        #region Constructors

        // TODO: Add more and more parts of a standard NE executable, not just the header
        // TODO: Tables? What about the tables?
        // TODO: Implement the rest of the structures found at http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm
        // (Left off at RESIDENT-NAME TABLE)

        /// <summary>
        /// Create a NewExecutable object from a stream
        /// </summary>
        /// <param name="stream">Stream representing a file</param>
        /// <remarks>
        /// This constructor assumes that the stream is already in the correct position to start parsing
        /// </remarks>
        public NewExecutable(Stream stream)
        {
            if (stream == null || !stream.CanRead || !stream.CanSeek)
                return;

            this.Initialized = Deserialize(stream);
            this.SourceStream = stream;
        }

        /// <summary>
        /// Create a NewExecutable object from a byte array
        /// </summary>
        /// <param name="fileContent">Byte array representing a file</param>
        /// <param name="offset">Positive offset representing the current position in the array</param>
        public NewExecutable(byte[] fileContent, int offset)
        {
            if (fileContent == null || fileContent.Length == 0 || offset < 0)
                return;

            this.Initialized = Deserialize(fileContent, offset);
            this.SourceArray = fileContent;
        }

        /// <summary>
        /// Deserialize a NewExecutable object from a stream
        /// </summary>
        /// <param name="stream">Stream representing a file</param>
        private bool Deserialize(Stream stream)
        {
            try
            {
                // Attempt to read the DOS header first
                this.DOSStubHeader = MSDOSExecutableHeader.Deserialize(stream);
                stream.Seek(this.DOSStubHeader.NewExeHeaderAddr, SeekOrigin.Begin);
                if (this.DOSStubHeader.Magic != Constants.IMAGE_DOS_SIGNATURE)
                    return false;
                
                // If the new header address is invalid for the file, it's not a NE
                if (this.DOSStubHeader.NewExeHeaderAddr >= stream.Length)
                    return false;

                // Then attempt to read the NE header
                this.NewExecutableHeader = NewExecutableHeader.Deserialize(stream);
                if (this.NewExecutableHeader.Magic != Constants.IMAGE_OS2_SIGNATURE)
                    return false;

            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Errored out on a file: {ex}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deserialize a NewExecutable object from a byte array
        /// </summary>
        /// <param name="fileContent">Byte array representing a file</param>
        /// <param name="offset">Positive offset representing the current position in the array</param>
        private bool Deserialize(byte[] content, int offset)
        {
            try
            {
                // Attempt to read the DOS header first
                this.DOSStubHeader = MSDOSExecutableHeader.Deserialize(content, ref offset);
                offset = this.DOSStubHeader.NewExeHeaderAddr;
                if (this.DOSStubHeader.Magic != Constants.IMAGE_DOS_SIGNATURE)
                    return false;

                // If the new header address is invalid for the file, it's not a PE
                if (this.DOSStubHeader.NewExeHeaderAddr >= content.Length)
                    return false;

                // Then attempt to read the NE header
                this.NewExecutableHeader = NewExecutableHeader.Deserialize(content, ref offset);
                if (this.NewExecutableHeader.Magic != Constants.IMAGE_OS2_SIGNATURE)
                    return false;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Errored out on a file: {ex}");
                return false;
            }

            return true;
        }

        #endregion
    }
}