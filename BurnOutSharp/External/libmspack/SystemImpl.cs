/* libmspack -- a library for working with Microsoft compression formats.
 * (C) 2003-2019 Stuart Caie <kyzer@cabextract.org.uk>
 *
 * libmspack is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License (LGPL) version 2.1
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
 */

using System;
using System.IO;

namespace LibMSPackSharp
{
    /// <summary>
    /// A structure which abstracts file I/O and memory management.
    /// 
    /// The library always uses the SystemImpl structure for interaction
    /// with the file system and to allocate, free and copy all memory.It also
    /// uses it to send literal messages to the library user.
    /// 
    /// When the library is compiled normally, passing null to a compressor or
    /// decompressor constructor will result in a default SystemImpl being
    /// used, where all methods are implemented with the standard C library.
    /// However, all constructors support being given a custom created
    /// SystemImpl structure, with the library user's own methods. This
    /// allows for more abstract interaction, such as reading and writing files
    /// directly to memory, or from a network socket or pipe.
    /// 
    /// Implementors of an SystemImpl structure should read all
    /// documentation entries for every structure member, and write methods
    /// which conform to those standards.
    /// </summary>
    public class SystemImpl
    {
        /// <summary>
        /// Opens a file for reading, writing, appending or updating.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the SystemImpl
        /// structure whose Open() method is being called. If
        /// this pointer is required by Close(), Read(), Write(),
        /// Seek() or Tell(), it should be stored in the result
        /// structure at this time.
        /// </param>
        /// <param name="filename">
        /// the file to be opened. It is passed directly from the
        /// library caller without being modified, so it is up to
        /// the caller what this parameter actually represents.
        /// </param>
        /// <param name="mode">One of the <see cref="OpenMode"/> values</param>
        /// <returns>
        /// a pointer to a mspack_file structure. This structure officially
        /// contains no members, its true contents are up to the
        /// SystemImpl implementor. It should contain whatever is needed
        /// for other SystemImpl methods to operate. Returning the null
        /// pointer indicates an error condition.
        /// </returns>
        public Func<SystemImpl, string, OpenMode, object> Open;

        /// <summary>
        /// Closes a previously opened file. If any memory was allocated for this
        /// particular file handle, it should be freed at this time.
        /// </summary>
        /// <param name="file">the file to close</param>
        /// <see cref="Open"/>
        public Action<object> Close;

        /// <summary>
        /// Reads a given number of bytes from an open file.
        /// </summary>
        /// <param name="file">the file to read from</param>
        /// <param name="buffer">the location where the read bytes should be stored</param>
        /// <param name="bytes">the number of bytes to read from the file.</param>
        /// <returns>
        /// the number of bytes successfully read (this can be less than
        /// the number requested), zero to mark the end of file, or less
        /// than zero to indicate an error. The library does not "retry"
        /// reads and assumes short reads are due to EOF, so you should
        /// avoid returning short reads because of transient errors.
        /// </returns>
        /// <see cref="Open"/>
        /// <see cref="Write"/>
        public Func<object, byte[], int, int, int> Read;

        /// <summary>
        /// Writes a given number of bytes to an open file.
        /// </summary>
        /// <param name="file">the file to write to</param>
        /// <param name="buffer">the location where the written bytes should be read from</param>
        /// <param name="bytes">the number of bytes to write to the file.</param>
        /// <returns>
        /// the number of bytes successfully written, this can be less
        /// than the number requested. Zero or less can indicate an error
        /// where no bytes at all could be written. All cases where less
        /// bytes were written than requested are considered by the library
        /// to be an error.
        /// </returns>
        /// <see cref="Open"/>
        /// <see cref="Read"/>
        public Func<object, byte[], int, int, int> Write;

        /// <summary>
        /// Seeks to a specific file offset within an open file.
        /// 
        /// Sometimes the library needs to know the length of a file. It does
        /// this by seeking to the end of the file with seek(file, 0,
        /// MSPACK_SYS_SEEK_END), then calling Tell(). Implementations may want
        /// to make a special case for this.
        /// 
        /// Due to the potentially varying 32/64 bit datatype int on some
        /// architectures, the #MSPACK_SYS_SELFTEST macro MUST be used before
        /// using the library. If not, the error caused by the library passing an
        /// inappropriate stackframe to Seek() is subtle and hard to trace.
        /// </summary>
        /// <param name="file">the file to be seeked</param>
        /// <param name="offset">an offset to seek, measured in bytes</param>
        /// <param name="mode">One of the <see cref="SeekMode"/> values</param>
        /// <returns>zero for success, non-zero for an error</returns>
        /// <see cref="Open"/>
        /// <see cref="Tell"/>
        public Func<object, long, SeekMode, bool> Seek;

        /// <summary>
        /// Returns the current file position (in bytes) of the given file.
        /// </summary>
        /// <param name="file">the file whose file position is wanted</param>
        /// <returns>the current file position of the file</returns>
        /// <see cref="Open"/>
        /// <see cref="Seek"/>
        public Func<object, long> Tell;

        /// <summary>
        /// Used to send messages from the library to the user.
        /// 
        /// Occasionally, the library generates warnings or other messages in
        /// plain english to inform the human user. These are informational only
        /// and can be ignored if not wanted.
        /// </summary>
        /// <param name="file">
        /// may be a file handle returned from Open() if this message
        /// pertains to a specific open file, or null if not related to
        /// a specific file.
        /// </param>
        /// <param name="format">a printf() style format string. It does NOT include a trailing newline.</param>
        /// <see cref="Open"/>
        public Action<object, string> Message;

        /// <summary>
        /// Allocates memory.
        /// </summary>
        /// <param name="self">
        /// a self-referential pointer to the SystemImpl
        /// structure whose Alloc() method is being called.
        /// </param>
        /// <param name="bytes">the number of bytes to allocate</param>
        /// <returns>
        /// a pointer to the requested number of bytes, or null if
        /// not enough memory is available
        /// </returns>
        /// <see cref="Free"/>
        public Func<SystemImpl, int, byte[]> Alloc;

        /// <summary>
        /// Frees memory.
        /// </summary>
        /// <param name="ptr">the memory to be freed. null is accepted and ignored.</param>
        /// <see cref="Alloc"/>
        public Action<object> Free;

        /// <summary>
        /// Copies from one region of memory to another.
        /// 
        /// The regions of memory are guaranteed not to overlap, are usually less
        /// than 256 bytes, and may not be aligned. Please note that the source
        /// parameter comes before the destination parameter, unlike the standard
        /// C function memcpy().
        /// </summary>
        /// <param name="src">the region of memory to copy from</param>
        /// <param name="dest">the region of memory to copy to</param>
        /// <param name="bytes">the size of the memory region, in bytes</param>
        public Action<byte[], int, byte[], int, int> Copy;

        /// <summary>
        /// A null pointer to mark the end of SystemImpl. It must equal null.
        /// 
        /// Should the SystemImpl structure extend in the future, this null
        /// will be seen, rather than have an invalid method pointer called.
        /// </summary>
        public readonly object NullPtr = null;

        #region Helpers

        /// <summary>
        /// Returns the length of a file opened for reading
        /// </summary>
        public static Error GetFileLength(SystemImpl system, object file, out long length)
        {
            length = 0;
            long current;

            if (system == null || file == null)
                return Error.MSPACK_ERR_OPEN;

            // Get current offset
            current = system.Tell(file);

            // Seek to end of file
            if (!system.Seek(file, 0, SeekMode.MSPACK_SYS_SEEK_END))
                return Error.MSPACK_ERR_SEEK;

            // Get offset of end of file
            length = system.Tell(file);

            // Seek back to original offset
            if (!system.Seek(file, current, SeekMode.MSPACK_SYS_SEEK_START))
                return Error.MSPACK_ERR_SEEK;

            return Error.MSPACK_ERR_OK;
        }

        /// <summary>
        /// Validates a system structure
        /// </summary>
        public static bool ValidSystem(SystemImpl sys)
        {
            return (sys != null) && (sys.Open != null) && (sys.Close != null) &&
                (sys.Read != null) && (sys.Write != null) && (sys.Seek != null) &&
                (sys.Tell != null) && (sys.Message != null) && (sys.Alloc != null) &&
                (sys.Free != null) && (sys.Copy != null) && (sys.NullPtr == null);
        }

        #endregion

        #region Default Implementation

        public static SystemImpl DefaultSystem => new SystemImpl()
        {
            Open = DefaultOpen,
            Close = DefaultClose,
            Read = DefaultRead,
            Write = DefaultWrite,
            Seek = DefaultSeek,
            Tell = DefaultTell,
            Message = DefaultMessage,
            Alloc = DefaultAlloc,
            Free = DefaultFree,
            Copy = DefaultCopy,
        };

        private static object DefaultOpen(SystemImpl self, string filename, OpenMode mode)
        {
            DefaultFileImpl fileHandle = new DefaultFileImpl();
            switch (mode)
            {
                case OpenMode.MSPACK_SYS_OPEN_READ:
                    fileHandle.FileHandle = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    break;

                case OpenMode.MSPACK_SYS_OPEN_WRITE:
                    fileHandle.FileHandle = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                    break;

                case OpenMode.MSPACK_SYS_OPEN_UPDATE:
                    fileHandle.FileHandle = File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    break;

                case OpenMode.MSPACK_SYS_OPEN_APPEND:
                    fileHandle.FileHandle = File.Open(filename, FileMode.Append, FileAccess.ReadWrite, FileShare.ReadWrite);
                    break;

                default:
                    return null;
            }

            return fileHandle;
        }

        private static void DefaultClose(object file)
        {
            DefaultFileImpl self = file as DefaultFileImpl;
            if (self != null)
                self.FileHandle.Close();
        }

        private static int DefaultRead(object file, byte[] buffer, int pointer, int bytes)
        {
            DefaultFileImpl self = file as DefaultFileImpl;
            if (self != null && buffer != null && bytes >= 0)
            {
                try { return self.FileHandle.Read(buffer, pointer, bytes); }
                catch { }
            }

            return -1;
        }

        private static int DefaultWrite(object file, byte[] buffer, int pointer, int bytes)
        {
            DefaultFileImpl self = file as DefaultFileImpl;
            if (self != null && buffer != null && bytes >= 0)
            {
                try { self.FileHandle.Write(buffer, pointer, bytes); }
                catch { return -1; }
                return bytes;
            }
            return -1;
        }

        private static bool DefaultSeek(object file, long offset, SeekMode mode)
        {
            DefaultFileImpl self = file as DefaultFileImpl;
            if (self != null)
            {
                switch (mode)
                {
                    case SeekMode.MSPACK_SYS_SEEK_START:
                        try { self.FileHandle.Seek(offset, SeekOrigin.Begin); return true; }
                        catch { return false; }

                    case SeekMode.MSPACK_SYS_SEEK_CUR:
                        try { self.FileHandle.Seek(offset, SeekOrigin.Current); return true; }
                        catch { return false; }

                    case SeekMode.MSPACK_SYS_SEEK_END:
                        try { self.FileHandle.Seek(offset, SeekOrigin.End); return true; }
                        catch { return false; }

                    default:
                        return false;
                }
            }

            return false;
        }

        private static long DefaultTell(object file)
        {
            DefaultFileImpl self = file as DefaultFileImpl;
            return (self != null ? (int)self.FileHandle.Position : 0);
        }

        private static void DefaultMessage(object file, string format)
        {
            if (file != null)
                Console.Error.Write($"{(file as DefaultFileImpl)?.Name}: ");

            Console.Error.Write($"{format}\n");
        }

        private static byte[] DefaultAlloc(SystemImpl self, int bytes)
        {
            return new byte[bytes];
        }

        private static void DefaultFree(object buffer)
        {
            buffer = null;
        }

        private static void DefaultCopy(byte[] src, int srcPtr, byte[] dest, int destPtr, int bytes)
        {
            Array.Copy(src, srcPtr, dest, destPtr, bytes);
        }

        #endregion
    }
}