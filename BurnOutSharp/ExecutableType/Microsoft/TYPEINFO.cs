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
    internal class TYPEINFO
    {
        public ushort TypeID;
        public ushort ResourceCount;
        public uint Reserved;
        public NAMEINFO NameInfo;

        public static TYPEINFO Deserialize(Stream stream)
        {
            var ti = new TYPEINFO();

            ti.TypeID = stream.ReadUInt16();
            ti.ResourceCount = stream.ReadUInt16();
            ti.Reserved = stream.ReadUInt32();
            ti.NameInfo = NAMEINFO.Deserialize(stream);

            return ti;
        }
    }
}