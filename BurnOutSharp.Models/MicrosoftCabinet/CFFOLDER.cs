using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BurnOutSharp.Models.MicrosoftCabinet
{
    /// <summary>
    /// Each CFFOLDER structure contains information about one of the folders or partial folders stored in
    /// this cabinet file, as shown in the following packet diagram.The first CFFOLDER structure entry
    /// immediately follows the CFHEADER structure entry. The CFHEADER.cFolders field indicates how
    /// many CFFOLDER structure entries are present.
    /// 
    /// Folders can start in one cabinet, and continue on to one or more succeeding cabinets. When the
    /// cabinet file creator detects that a folder has been continued into another cabinet, it will complete
    /// that folder as soon as the current file has been completely compressed.Any additional files will be
    /// placed in the next folder.Generally, this means that a folder would span at most two cabinets, but it
    /// could span more than two cabinets if the file is large enough.
    /// 
    /// CFFOLDER structure entries actually refer to folder fragments, not necessarily complete folders. A
    /// CFFOLDER structure is the beginning of a folder if the iFolder field value in the first file that
    /// references the folder does not indicate that the folder is continued from the previous cabinet file.
    /// 
    /// The typeCompress field can vary from one folder to the next, unless the folder is continued from a
    /// previous cabinet file.
    /// </summary>
    /// <see href="http://download.microsoft.com/download/5/0/1/501ED102-E53F-4CE0-AA6B-B0F93629DDC6/Exchange/%5BMS-CAB%5D.pdf"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class CFFOLDER
    {
        /// <summary>
        /// Specifies the absolute file offset of the first CFDATA field block for the folder.
        /// </summary>
        public uint CabStartOffset;

        /// <summary>
        /// Specifies the number of CFDATA structures for this folder that are actually in this cabinet.
        /// A folder can continue into another cabinet and have more CFDATA structure blocks in that cabinet
        /// file.A folder can start in a previous cabinet.This number represents only the CFDATA structures for
        /// this folder that are at least partially recorded in this cabinet.
        /// </summary>
        public ushort DataCount;

        /// <summary>
        /// Indicates the compression method used for all CFDATA structure entries in this
        /// folder.
        /// </summary>
        public CompressionType CompressionType;

        /// <summary>
        /// If the CFHEADER.flags.cfhdrRESERVE_PRESENT field is set
        /// and the cbCFFolder field is non-zero, then this field contains per-folder application information.
        /// This field is defined by the application, and is used for application-defined purposes.
        /// </summary>
        public byte[] ReservedData;

        /// <summary>
        /// Data blocks associated with this folder
        /// </summary>
        public Dictionary<int, CFDATA> DataBlocks;
    }
}
