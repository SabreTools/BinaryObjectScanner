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
using BurnOutSharp.Tools;

namespace BurnOutSharp.ExecutableType.Microsoft
{
    /// <summary>
    /// DOS 1, 2, 3 .EXE header
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class IMAGE_DOS_HEADER
    {
        public ushort Magic;                   // 00 Magic number
        public ushort LastPageBytes;           // 02 Bytes on last page of file
        public ushort Pages;                   // 04 Pages in file
        public ushort Relocations;             // 06 Relocations
        public ushort HeaderParagraphSize;     // 08 Size of header in paragraphs
        public ushort MinimumExtraParagraphs;  // 0A Minimum extra paragraphs needed
        public ushort MaximumExtraParagraphs;  // 0C Maximum extra paragraphs needed
        public ushort InitialSSValue;          // 0E Initial (relative) SS value
        public ushort InitialSPValue;          // 10 Initial SP value
        public ushort Checksum;                // 12 Checksum
        public ushort InitialIPValue;          // 14 Initial IP value
        public ushort InitialCSValue;          // 16 Initial (relative) CS value
        public ushort RelocationTableAddr;     // 18 File address of relocation table
        public ushort OverlayNumber;           // 1A Overlay number
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ERES1WDS)]
        public ushort[] Reserved1;             // 1C Reserved words
        public ushort OEMIdentifier;           // 24 OEM identifier (for e_oeminfo)
        public ushort OEMInformation;          // 26 OEM information; e_oemid specific
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ERES2WDS)]
        public ushort[] Reserved2;             // 28 Reserved words
        public int NewExeHeaderAddr;           // 3C File address of new exe header

        public static IMAGE_DOS_HEADER Deserialize(Stream stream)
        {
            IMAGE_DOS_HEADER idh = new IMAGE_DOS_HEADER();

            idh.Magic = stream.ReadUInt16();
            idh.LastPageBytes = stream.ReadUInt16();
            idh.Pages = stream.ReadUInt16();
            idh.Relocations = stream.ReadUInt16();
            idh.HeaderParagraphSize = stream.ReadUInt16();
            idh.MinimumExtraParagraphs = stream.ReadUInt16();
            idh.MaximumExtraParagraphs = stream.ReadUInt16();
            idh.InitialSSValue = stream.ReadUInt16();
            idh.InitialSPValue = stream.ReadUInt16();
            idh.Checksum = stream.ReadUInt16();
            idh.InitialIPValue = stream.ReadUInt16();
            idh.InitialCSValue = stream.ReadUInt16();
            idh.RelocationTableAddr = stream.ReadUInt16();
            idh.OverlayNumber = stream.ReadUInt16();
            idh.Reserved1 = new ushort[Constants.ERES1WDS];
            for (int i = 0; i < Constants.ERES1WDS; i++)
            {
                idh.Reserved1[i] = stream.ReadUInt16();
            }
            idh.OEMIdentifier = stream.ReadUInt16();
            idh.OEMInformation = stream.ReadUInt16();
            idh.Reserved2 = new ushort[Constants.ERES2WDS];
            for (int i = 0; i < Constants.ERES2WDS; i++)
            {
                idh.Reserved2[i] = stream.ReadUInt16();
            }
            idh.NewExeHeaderAddr = stream.ReadInt32();

            return idh;
        }

        public static IMAGE_DOS_HEADER Deserialize(byte[] content, int offset)
        {
            IMAGE_DOS_HEADER idh = new IMAGE_DOS_HEADER();

            idh.Magic = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.LastPageBytes = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.Pages = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.Relocations = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.HeaderParagraphSize = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.MinimumExtraParagraphs = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.MaximumExtraParagraphs = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.InitialSSValue = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.InitialSPValue = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.Checksum = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.InitialIPValue = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.InitialCSValue = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.RelocationTableAddr = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.OverlayNumber = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.Reserved1 = new ushort[Constants.ERES1WDS];
            for (int i = 0; i < Constants.ERES1WDS; i++)
            {
                idh.Reserved1[i] = BitConverter.ToUInt16(content, offset); offset += 2;
            }
            idh.OEMIdentifier = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.OEMInformation = BitConverter.ToUInt16(content, offset); offset += 2;
            idh.Reserved2 = new ushort[Constants.ERES2WDS];
            for (int i = 0; i < Constants.ERES2WDS; i++)
            {
                idh.Reserved2[i] = BitConverter.ToUInt16(content, offset); offset += 2;
            }
            idh.NewExeHeaderAddr = BitConverter.ToInt32(content, offset); offset += 4;

            return idh;
        }
    }
}
