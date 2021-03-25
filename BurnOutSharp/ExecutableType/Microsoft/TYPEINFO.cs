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
    internal class TYPEINFO
    {
        public ushort TypeID { get; private set; }
        public ushort ResourceCount { get; private set; }
        public uint Reserved { get; private set; }
        public NAMEINFO NameInfo { get; private set; }

        public static TYPEINFO Deserialize(Stream stream)
        {
            TYPEINFO ti = new TYPEINFO();

            ti.TypeID = stream.ReadUInt16();
            ti.ResourceCount = stream.ReadUInt16();
            ti.Reserved = stream.ReadUInt32();
            ti.NameInfo = NAMEINFO.Deserialize(stream);

            return ti;
        }
    }
}