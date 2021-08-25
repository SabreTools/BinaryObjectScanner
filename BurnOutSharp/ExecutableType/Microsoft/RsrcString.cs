/*
 *	  NEWEXE.H (C) Copyright Microsoft Corp 1984-1987
 *
 *	  Data structure definitions for the OS/2 & Windows
 *	  executable file format.
 *
 *	  Modified by IVS on 24-Jan-1991 for Resource DeCompiler
 *	  (C) Copyright IVS 1991
 *
 *    http://csn.ul.ie/~caolan/pub/winresdump/winresdump/newexe.h
 */

using System.IO;
using System.Runtime.InteropServices;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    /// <summary>
    /// Resource type or name string
    /// </summary>
    /// TODO: Fix this because SizeConst = 0 is not valid
    [StructLayout(LayoutKind.Sequential)]
    internal class RsrcString
    {
        /// <summary>
        /// Number of bytes in string
        /// </summary>
        public byte Length;
        
        /// <summary>
        /// Next of string
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0)]
        public char[] Text;

        public static RsrcString Deserialize(Stream stream)
        {
            var rs = new RsrcString();

            rs.Length = stream.ReadByteValue();
            rs.Text = stream.ReadChars(rs.Length);

            return rs;
        }
    }
}