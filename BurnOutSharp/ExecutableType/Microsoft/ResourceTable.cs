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
    internal class ResourceTable
    {
        public ushort rscAlignShift;
        public TYPEINFO TypeInfo;
        public ushort rscEndTypes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0)]
        public sbyte[][] rscResourceNames;
        public byte rscEndNames;

        public static ResourceTable Deserialize(Stream stream)
        {
            var rt = new ResourceTable();

            rt.rscAlignShift = stream.ReadUInt16();
            rt.TypeInfo = TYPEINFO.Deserialize(stream);
            rt.rscEndTypes = stream.ReadUInt16();
            rt.rscResourceNames = null; // TODO: Figure out size
            rt.rscEndNames = stream.ReadByteValue();

            return rt;
        }
    }
}