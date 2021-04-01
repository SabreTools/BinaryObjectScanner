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
    /// <summary>
    /// Resource type information block
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class RsrcTypeInfo
    {
        public ushort ID;
        public ushort rt_nres;
        public uint rt_proc;

        public static RsrcTypeInfo Deserialize(Stream stream)
        {
            var rti = new RsrcTypeInfo();

            rti.ID = stream.ReadUInt16();
            rti.rt_nres = stream.ReadUInt16();
            rti.rt_proc = stream.ReadUInt32();

            return rti;
        }
    }
}