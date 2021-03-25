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

namespace BurnOutSharp.ExecutableType.Microsoft
{
    /// <summary>
    /// Resource type or name string
    /// </summary>
    internal class RsrcString
    {
        /// <summary>
        /// Number of bytes in string
        /// </summary>
        public byte Length { get; private set; }
        
        /// <summary>
        /// Next of string
        /// </summary>
        public char[] Text { get; private set; }

        public static RsrcString Deserialize(Stream stream)
        {
            RsrcString rs = new RsrcString();

            rs.Length = stream.ReadByteValue();
            rs.Text = stream.ReadChars(rs.Length);

            return rs;
        }
    }
}