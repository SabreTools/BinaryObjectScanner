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

namespace BurnOutSharp.ExecutableType.Microsoft
{
    /// <summary>
    /// Relocation item
    /// </summary>
    internal class NewRlc
    {
        public char SourceType { get; private set; }            // Source type
        public char Flags { get; private set; }                 // Flag byte
        public ushort SourceOffset { get; private set; }        // Source offset

        // nr_intref - Internal Reference
        public char TargetSegmentNumber { get; private set; }   // Target segment number
        public char Reserved1 { get; private set; }             // Reserved
        public ushort TargetEntryTableOffset { get; private set; }      // Target Entry Table offset

        // nr_import - Import
        public ushort ModuleReferenceTableIndex { get; private set; }   // Index into Module Reference Table
        public ushort ProcedureOffset { get; private set; }     // Procedure ordinal or name offset

        // nr_osfix - Operating system fixup
        public ushort OperatingSystemFixupType { get; private set; }    // OSFIXUP type
        public ushort Reserved2 { get; private set; }           // Reserved

        public static NewRlc Deserialize(Stream stream)
        {
            NewRlc nr = new NewRlc();

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