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
    /// Resource table
    /// </summary>
    internal class NewRsrc
    {
        /// <summary>
        /// Alignment shift count for resources
        /// </summary>
        public ushort AlignmentShiftCount { get; private set; }
        public RsrcTypeInfo TypeInfo { get; private set; }

        public static NewRsrc Deserialize(Stream stream)
        {
            NewRsrc nr = new NewRsrc();

            nr.AlignmentShiftCount = stream.ReadUInt16();
            nr.TypeInfo = RsrcTypeInfo.Deserialize(stream);

            return nr;
        }
    }
}