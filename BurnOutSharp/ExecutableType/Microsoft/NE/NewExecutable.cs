using System;
using System.IO;
using BurnOutSharp.ExecutableType.Microsoft.MZ.Headers;
using BurnOutSharp.ExecutableType.Microsoft.NE.Headers;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft.NE
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
        private readonly Stream _sourceStream = null;

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

        #region Tables

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
            this._sourceStream = stream;
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

        #region Helpers

        /// <summary>
        /// Read an arbitrary range from the source
        /// </summary>
        /// <param name="rangeStart">The start of where to read data from, -1 means start of source</param>
        /// <param name="length">How many bytes to read, -1 means read until end</param>
        /// <returns></returns>
        public byte[] ReadArbitraryRange(int rangeStart = -1, int length = -1)
        {
            // If we have a source stream, use that
            if (this._sourceStream != null)
                return ReadArbitraryRangeFromSourceStream(rangeStart, length);

            // If we have a source array, use that
            if (this.SourceArray != null)
                return ReadArbitraryRangeFromSourceArray(rangeStart, length);

            // Otherwise, return null
            return null;
        }

        /// <summary>
        /// Read an arbitrary range from the stream source, if possible
        /// </summary>
        /// <param name="rangeStart">The start of where to read data from, -1 means start of source</param>
        /// <param name="length">How many bytes to read, -1 means read until end</param>
        /// <returns></returns>
        private byte[] ReadArbitraryRangeFromSourceStream(int rangeStart, int length)
        {
            lock (this._sourceStream)
            {
                int startingIndex = (int)Math.Max(rangeStart, 0);
                int readLength = (int)Math.Min(length == -1 ? length = Int32.MaxValue : length, this._sourceStream.Length);

                long originalPosition = this._sourceStream.Position;
                this._sourceStream.Seek(startingIndex, SeekOrigin.Begin);
                byte[] sectionData = this._sourceStream.ReadBytes(readLength);
                this._sourceStream.Seek(originalPosition, SeekOrigin.Begin);
                return sectionData;
            }
        }

        /// <summary>
        /// Read an arbitrary range from the array source, if possible
        /// </summary>
        /// <param name="rangeStart">The start of where to read data from, -1 means start of source</param>
        /// <param name="length">How many bytes to read, -1 means read until end</param>
        /// <returns></returns>
        private byte[] ReadArbitraryRangeFromSourceArray(int rangeStart, int length)
        {
            int startingIndex = (int)Math.Max(rangeStart, 0);
            int readLength = (int)Math.Min(length == -1 ? length = Int32.MaxValue : length, this.SourceArray.Length);

            try
            {
                return this.SourceArray.ReadBytes(ref startingIndex, readLength);
            }
            catch
            {
                // Just absorb errors for now
                // TODO: Investigate why and when this would be hit
                return null;
            }
        }

        #endregion
    }
}