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
    /// New .EXE segment table entry
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class NewSeg
    {
        /// <summary>
        /// File sector of start of segment
        /// </summary>
        public ushort StartFileSector;
        
        /// <summary>
        /// Number of bytes in file
        /// </summary>
        public ushort BytesInFile;
        
        /// <summary>
        /// Attribute flags
        /// </summary>
        public ushort Flags;
        
        /// <summary>
        /// Minimum allocation in bytes
        /// </summary>
        public ushort MinimumAllocation;

        public static NewSeg Deserialize(Stream stream)
        {
            var ns = new NewSeg();

            ns.StartFileSector = stream.ReadUInt16();
            ns.BytesInFile = stream.ReadUInt16();
            ns.Flags = stream.ReadUInt16();
            ns.MinimumAllocation = stream.ReadUInt16();

            return ns;
        }
    }
}
