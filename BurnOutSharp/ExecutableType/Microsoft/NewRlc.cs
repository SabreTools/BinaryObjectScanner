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

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    /// <summary>
    /// Relocation item
    /// </summary>
    /// TODO: Fix this because Marshal will not work since it's not a direct read
    [StructLayout(LayoutKind.Sequential)]
    internal class NewRlc
    {
        public char SourceType;            // Source type
        public char Flags;                 // Flag byte
        public ushort SourceOffset;        // Source offset

        // nr_intref - Internal Reference
        public char TargetSegmentNumber;   // Target segment number
        public char Reserved1;             // Reserved
        public ushort TargetEntryTableOffset;      // Target Entry Table offset

        // nr_import - Import
        public ushort ModuleReferenceTableIndex;   // Index into Module Reference Table
        public ushort ProcedureOffset;     // Procedure ordinal or name offset

        // nr_osfix - Operating system fixup
        public ushort OperatingSystemFixupType;    // OSFIXUP type
        public ushort Reserved2;           // Reserved

        public static NewRlc Deserialize(Stream stream)
        {
            var nr = new NewRlc();

            nr.SourceType = stream.ReadChar();
            nr.Flags = stream.ReadChar();
            nr.SourceOffset = stream.ReadUInt16();

            // nr_intref
            nr.TargetSegmentNumber = stream.ReadChar();
            nr.Reserved1 = stream.ReadChar();
            nr.TargetEntryTableOffset = stream.ReadUInt16();

            // nr_import
            nr.ModuleReferenceTableIndex = BitConverter.ToUInt16(new byte[] { (byte)nr.SourceType, (byte)nr.Flags }, 0);
            nr.ProcedureOffset = nr.TargetEntryTableOffset;

            // nr_osfix
            nr.OperatingSystemFixupType = nr.ModuleReferenceTableIndex;
            nr.Reserved2 = nr.ProcedureOffset;

            return nr;
        }
    }
}