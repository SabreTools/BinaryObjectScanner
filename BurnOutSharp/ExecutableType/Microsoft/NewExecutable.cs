using System;
using System.IO;
using System.Runtime.InteropServices;
using BurnOutSharp.ExecutableType.Microsoft.Headers;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    /// <summary>
    /// The WIN-NE executable format, designed for Windows 3.x, was the "NE", or "New Executable" format.
    /// Again, a 16bit format, it alleviated the maximum size restrictions that the MZ format had.
    /// </summary>
    internal class NewExecutable
    {
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

        // TODO: Add more and more parts of a standard NE executable, not just the header
        // TODO: Tables? What about the tables?
        // TODO: Implement the rest of the structures found at http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm
        // (Left off at RESIDENT-NAME TABLE)

        public static NewExecutable Deserialize(Stream stream)
        {
            NewExecutable nex = new NewExecutable();

            try
            {
                // Attempt to read the DOS header first
                nex.DOSStubHeader = MSDOSExecutableHeader.Deserialize(stream); stream.Seek(nex.DOSStubHeader.NewExeHeaderAddr, SeekOrigin.Begin);
                if (nex.DOSStubHeader.Magic != Constants.IMAGE_DOS_SIGNATURE)
                    return null;
                
                // If the new header address is invalid for the file, it's not a NE
                if (nex.DOSStubHeader.NewExeHeaderAddr >= stream.Length)
                    return null;

                // Then attempt to read the NE header
                nex.NewExecutableHeader = NewExecutableHeader.Deserialize(stream);
                if (nex.NewExecutableHeader.Magic != Constants.IMAGE_OS2_SIGNATURE)
                    return null;

            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Errored out on a file: {ex}");
                return null;
            }

            return nex;
        }

        public static NewExecutable Deserialize(byte[] content, int offset)
        {
            NewExecutable nex = new NewExecutable();

            try
            {
                unsafe
                {
                    // Attempt to read the DOS header first
                    nex.DOSStubHeader = MSDOSExecutableHeader.Deserialize(content, offset); offset = nex.DOSStubHeader.NewExeHeaderAddr;
                    if (nex.DOSStubHeader.Magic != Constants.IMAGE_DOS_SIGNATURE)
                        return null;

                    // If the new header address is invalid for the file, it's not a PE
                    if (nex.DOSStubHeader.NewExeHeaderAddr >= content.Length)
                        return null;

                    // Then attempt to read the NE header
                    nex.NewExecutableHeader = NewExecutableHeader.Deserialize(content, offset); offset += Marshal.SizeOf(nex.NewExecutableHeader);
                    if (nex.NewExecutableHeader.Magic != Constants.IMAGE_OS2_SIGNATURE)
                        return null;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Errored out on a file: {ex}");
                return null;
            }

            return nex;
        }
    }
}