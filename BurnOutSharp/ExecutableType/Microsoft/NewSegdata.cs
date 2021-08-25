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

using System;
using System.IO;
using System.Runtime.InteropServices;
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    /// <summary>
    /// Segment data
    /// </summary>
    /// TODO: Fix this because Marshal will not work since it's not a direct read
    [StructLayout(LayoutKind.Sequential)]
    internal class NewSegdata
    {
        #region ns_iter

        /// <summary>
        /// Number of iterations
        /// </summary>
        public ushort Iterations;
        
        /// <summary>
        /// Number of bytes
        /// </summary>
        public ushort TotalBytes;
        
        /// <summary>
        /// Iterated data bytes
        /// </summary>
        public char IteratedDataBytes;

        #endregion

        #region ns_noiter

        /// <summary>
        /// Data bytes
        /// </summary>
        public char DataBytes;

        #endregion

        public static NewSegdata Deserialize(Stream stream)
        {
            var nsd = new NewSegdata();

            nsd.Iterations = stream.ReadUInt16();
            nsd.TotalBytes = stream.ReadUInt16();
            nsd.IteratedDataBytes = stream.ReadChar();
            nsd.DataBytes = (char)BitConverter.GetBytes(nsd.Iterations)[0];

            return nsd;
        }
    }
}
