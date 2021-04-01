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

namespace BurnOutSharp.ExecutableType.Microsoft
{
    [StructLayout(LayoutKind.Sequential)]
    internal class NAMEINFO
    {
        public ushort Offset;
        public ushort Length;
        public ushort Flags;
        public ushort ID;
        public ushort Handle;
        public ushort Usage;

        public static NAMEINFO Deserialize(Stream stream)
        {
            var ni = new NAMEINFO();

            ni.Offset = stream.ReadUInt16();
            ni.Length = stream.ReadUInt16();
            ni.Flags = stream.ReadUInt16();
            ni.ID = stream.ReadUInt16();
            ni.Handle = stream.ReadUInt16();
            ni.Usage = stream.ReadUInt16();

            return ni;
        }
    }
}