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
    /// Resource table
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class NewRsrc
    {
        /// <summary>
        /// Alignment shift count for resources
        /// </summary>
        public ushort AlignmentShiftCount;
        public RsrcTypeInfo TypeInfo;

        public static NewRsrc Deserialize(Stream stream)
        {
            var nr = new NewRsrc();

            nr.AlignmentShiftCount = stream.ReadUInt16();
            nr.TypeInfo = RsrcTypeInfo.Deserialize(stream);

            return nr;
        }
    }
}