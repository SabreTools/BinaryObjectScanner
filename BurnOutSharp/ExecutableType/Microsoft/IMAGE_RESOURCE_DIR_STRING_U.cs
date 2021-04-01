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
    internal class IMAGE_RESOURCE_DIR_STRING_U
    {
        public ushort Length;
        public char[] NameString;

        public static IMAGE_RESOURCE_DIR_STRING_U Deserialize(Stream stream)
        {
            var irdsu = new IMAGE_RESOURCE_DIR_STRING_U();

            irdsu.Length = stream.ReadUInt16();
            irdsu.NameString = stream.ReadChars(irdsu.Length);

            return irdsu;
        }
    }
}
