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
    internal class NAMEINFO
    {
        public ushort Offset { get; private set; }
        public ushort Length { get; private set; }
        public ushort Flags { get; private set; }
        public ushort ID { get; private set; }
        public ushort Handle { get; private set; }
        public ushort Usage { get; private set; }

        public static NAMEINFO Deserialize(Stream stream)
        {
            NAMEINFO ni = new NAMEINFO();

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