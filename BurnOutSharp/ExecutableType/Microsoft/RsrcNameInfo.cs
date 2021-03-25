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
    /// Resource name information block
    /// </summary>
    internal class RsrcNameInfo
    {
        /*
         * The following two fields must be shifted left by the value of
         * the rs_align field to compute their actual value. This allows
         * resources to be larger than 64k, but they do not need to be
         * aligned on 512 byte boundaries, the way segments are.
        */

        /// <summary>
        /// File offset to resource data
        /// </summary>
        public ushort Offset { get; private set; }
        
        /// <summary>
        /// Length of resource data
        /// </summary>
        public ushort Length { get; private set; }
        
        /// <summary>
        /// Resource flags
        /// </summary>
        public ushort Flags { get; private set; }
        
        /// <summary>
        /// Resource name id
        /// </summary>
        public ushort NameID { get; private set; }
        
        /// <summary>
        /// If loaded, then global handle
        /// </summary>
        public ushort Handle { get; private set; }
        
        /// <summary>
        /// Initially zero. Number of times the handle for this resource has been given out
        /// </summary>
        public ushort UsageCount { get; private set; }

        public static RsrcNameInfo Deserialize(Stream stream)
        {
            RsrcNameInfo rni = new RsrcNameInfo();

            rni.Offset = stream.ReadUInt16();
            rni.Length = stream.ReadUInt16();
            rni.Flags = stream.ReadUInt16();
            rni.NameID = stream.ReadUInt16();
            rni.Handle = stream.ReadUInt16();
            rni.UsageCount = stream.ReadUInt16();

            return rni;
        }
    }
}