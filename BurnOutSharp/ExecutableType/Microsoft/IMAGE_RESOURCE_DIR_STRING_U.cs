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
    internal class IMAGE_RESOURCE_DIR_STRING_U
    {
        public ushort Length { get; private set; }
        public char[] NameString { get; private set; }

        public static IMAGE_RESOURCE_DIR_STRING_U Deserialize(Stream stream)
        {
            IMAGE_RESOURCE_DIR_STRING_U irdsu = new IMAGE_RESOURCE_DIR_STRING_U();

            irdsu.Length = stream.ReadUInt16();
            irdsu.NameString = stream.ReadChars(irdsu.Length);

            return irdsu;
        }
    }
}
