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
    /// Relocation info
    /// </summary>
    internal class NewRlcInfo
    {
        /// <summary>
        /// Number of relocation items that follow
        /// </summary>
        public ushort RelocationItemCount { get; private set; }
    
        public static NewRlcInfo Deserialize(Stream stream)
        {
            NewRlcInfo nri = new NewRlcInfo();

            nri.RelocationItemCount = stream.ReadUInt16();

            return nri;
        }
    }
}