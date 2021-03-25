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
    /// Resource type information block
    /// </summary>
    internal class RsrcTypeInfo
    {
        public ushort ID { get; private set; }
        public ushort rt_nres { get; private set; }
        public uint rt_proc { get; private set; }

        public static RsrcTypeInfo Deserialize(Stream stream)
        {
            RsrcTypeInfo rti = new RsrcTypeInfo();

            rti.ID = stream.ReadUInt16();
            rti.rt_nres = stream.ReadUInt16();
            rti.rt_proc = stream.ReadUInt32();

            return rti;
        }
    }
}