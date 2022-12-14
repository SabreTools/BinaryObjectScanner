using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.Tools;

namespace BurnOutSharp.FileType
{
    /// <see href="http://download.microsoft.com/download/5/0/1/501ED102-E53F-4CE0-AA6B-B0F93629DDC6/Exchange/%5BMS-CAB%5D.pdf"/>

    // TODO: Add multi-cabinet reading
    public class MSCABCabinet
    {
        #region Constants

        /// <summary>
        /// A maximum uncompressed size of an input file to store in CAB
        /// </summary>
        public const uint MaximumUncompressedFileSize = 0x7FFF8000;

        /// <summary>
        /// A maximum file COUNT
        /// </summary>
        public const ushort MaximumFileCount = 0xFFFF;

        /// <summary>
        /// A maximum size of a created CAB (compressed)
        /// </summary>
        public const uint MaximumCabSize = 0x7FFFFFFF;

        /// <summary>
        /// A maximum CAB-folder COUNT
        /// </summary>
        public const ushort MaximumFolderCount = 0xFFFF;

        /// <summary>
        /// A maximum uncompressed data size in a CAB-folder
        /// </summary>
        public const uint MaximumUncompressedFolderSize = 0x7FFF8000;

        #endregion

        #region Properties

        /// <summary>
        /// Cabinet header
        /// </summary>
        public CFHEADER Header { get; private set; }

        /// <summary>
        /// One or more CFFOLDER entries
        /// </summary>
        public CFFOLDER[] Folders { get; private set; }

        /// <summary>
        /// A series of one or more cabinet file (CFFILE) entries
        /// </summary>
        public CFFILE[] Files { get; private set; }

        #endregion

        #region Serialization

        /// <summary>
        /// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a MSCABCabinet object
        /// </summary>
        public static MSCABCabinet Deserialize(byte[] data, ref int dataPtr)
        {
            if (data == null || dataPtr < 0)
                return null;

            int basePtr = dataPtr;
            MSCABCabinet cabinet = new MSCABCabinet();

            // Start with the header
            cabinet.Header = CFHEADER.Deserialize(data, ref dataPtr);
            if (cabinet.Header == null)
                return null;

            // Then retrieve all folder headers
            cabinet.Folders = new CFFOLDER[cabinet.Header.FolderCount];
            for (int i = 0; i < cabinet.Header.FolderCount; i++)
            {
                cabinet.Folders[i] = CFFOLDER.Deserialize(data, ref dataPtr, basePtr, cabinet.Header);
                if (cabinet.Folders[i] == null)
                    return null;
            }

            // We need to move to where the file headers are stored
            dataPtr = basePtr + (int)cabinet.Header.FilesOffset;

            // Then retrieve all file headers
            cabinet.Files = new CFFILE[cabinet.Header.FileCount];
            for (int i = 0; i < cabinet.Header.FileCount; i++)
            {
                cabinet.Files[i] = CFFILE.Deserialize(data, ref dataPtr);
                if (cabinet.Files[i] == null)
                    return null;
            }

            return cabinet;
        }

        /// <summary>
        /// Deserialize <paramref name="data"/> into a MSCABCabinet object
        /// </summary>
        public static MSCABCabinet Deserialize(Stream data)
        {
            if (data == null || data.Position < 0)
                return null;

            MSCABCabinet cabinet = new MSCABCabinet();

            // Start with the header
            cabinet.Header = CFHEADER.Deserialize(data);
            if (cabinet.Header == null)
                return null;

            // Then retrieve all folder headers
            cabinet.Folders = new CFFOLDER[cabinet.Header.FolderCount];
            for (int i = 0; i < cabinet.Header.FolderCount; i++)
            {
                cabinet.Folders[i] = CFFOLDER.Deserialize(data, cabinet.Header);
                if (cabinet.Folders[i] == null)
                    return null;
            }

            // We need to move to where the file headers are stored
            data.Seek((int)cabinet.Header.FilesOffset, SeekOrigin.Begin);

            // Then retrieve all file headers
            cabinet.Files = new CFFILE[cabinet.Header.FileCount];
            for (int i = 0; i < cabinet.Header.FileCount; i++)
            {
                cabinet.Files[i] = CFFILE.Deserialize(data);
                if (cabinet.Files[i] == null)
                    return null;
            }

            return cabinet;
        }

        #endregion

        #region Public Functionality

        /// <summary>
        /// Find the start of an MS-CAB cabinet in a set of data, if possible
        /// </summary>
        public int FindCabinet(byte[] data)
        {
            if (data == null || data.Length < CFHEADER.SignatureBytes.Length)
                return -1;

            bool found = data.FirstPosition(CFHEADER.SignatureBytes, out int index);
            return found ? index : -1;
        }

        /// <summary>
        /// Extract all files from the archive to <paramref name="outputDirectory"/>
        /// </summary>
        public bool ExtractAllFiles(string outputDirectory)
        {
            // Perform sanity checks
            if (Header == null || Files == null || Files.Length == 0)
                return false;

            // Loop through and extract all files
            foreach (CFFILE file in Files)
            {
                // Create the output path
                string outputPath = Path.Combine(outputDirectory, file.Name);

                // Get the associated folder, if possible
                CFFOLDER folder = null;
                if (file.FolderIndex != FolderIndex.CONTINUED_FROM_PREV && file.FolderIndex != FolderIndex.CONTINUED_TO_NEXT && file.FolderIndex != FolderIndex.CONTINUED_PREV_AND_NEXT)
                    folder = Folders[(int)file.FolderIndex];

                // If we don't have a folder, we can't continue
                if (folder == null)
                    return false;

                // TODO: We don't keep the stream open or accessible here to seek
                // TODO: We don't check for other cabinets here yet
                // TODO: Read and decompress data blocks
            }

            return true;
        }

        /// <summary>
        /// Extract a single file from the archive to <paramref name="outputDirectory"/>
        /// </summary>
        public bool ExtractFile(string filePath, string outputDirectory, bool exact = false)
        {
            // Perform sanity checks
            if (Header == null || Files == null || Files.Length == 0)
                return false;

            // Check the file exists
            int fileIndex = -1;
            for (int i = 0; i < Files.Length; i++)
            {
                CFFILE tempFile = Files[i];
                if (tempFile == null)
                    continue;

                // Check for a match
                if (exact ? tempFile.Name == filePath : tempFile.Name.EndsWith(filePath, StringComparison.OrdinalIgnoreCase))
                {
                    fileIndex = i;
                    break;
                }
            }

            // -1 is an invalid file index
            if (fileIndex == -1)
                return false;

            // Get the file to extract
            CFFILE file = Files[fileIndex];

            // Create the output path
            string outputPath = Path.Combine(outputDirectory, file.Name);

            // Get the associated folder, if possible
            CFFOLDER folder = null;
            if (file.FolderIndex != FolderIndex.CONTINUED_FROM_PREV && file.FolderIndex != FolderIndex.CONTINUED_TO_NEXT && file.FolderIndex != FolderIndex.CONTINUED_PREV_AND_NEXT)
                folder = Folders[(int)file.FolderIndex];

            // If we don't have a folder, we can't continue
            if (folder == null)
                return false;

            // TODO: We don't keep the stream open or accessible here to seek
            // TODO: We don't check for other cabinets here yet
            // TODO: Read and decompress data blocks

            return true;
        }

        /// <summary>
        /// Print all info about the cabinet file
        /// </summary>
        public void PrintInfo()
        {
            #region CFHEADER

            if (Header == null)
            {
                Console.WriteLine("There is no header associated with this cabinet.");
                return;
            }

            Header.PrintInfo();

            #endregion

            #region CFFOLDER

            if (Folders == null || Folders.Length == 0)
            {
                Console.WriteLine("There are no folders associated with this cabinet.");
                return;
            }

            Console.WriteLine("CFFOLDER INFORMATION:");
            Console.WriteLine("--------------------------------------------");
            for (int i = 0; i < Folders.Length; i++)
            {
                CFFOLDER folder = Folders[i];
                Console.WriteLine($"    CFFOLDER {i:X4}:");

                if (folder == null)
                {
                    Console.WriteLine($"        Not found or null");
                    Console.WriteLine();
                    continue;
                }

                folder.PrintInfo();
            }

            Console.WriteLine();

            #endregion

            #region CFFILE

            if (Files == null || Files.Length == 0)
            {
                Console.WriteLine("There are no files associated with this cabinet.");
                return;
            }

            Console.WriteLine("CFFILE INFORMATION:");
            Console.WriteLine("--------------------------------------------");
            for (int i = 0; i < Files.Length; i++)
            {
                CFFILE file = Files[i];
                Console.WriteLine($"    CFFILE {i:X4}:");

                if (file == null)
                {
                    Console.WriteLine($"        Not found or null");
                    Console.WriteLine();
                    continue;
                }

                file.PrintInfo();
            }

            Console.WriteLine();

            #endregion
        }

        #endregion
    }

    /// <summary>
    /// The CFHEADER structure shown in the following packet diagram provides information about this
    /// cabinet (.cab) file.
    /// </summary>
    public class CFHEADER
    {
        #region Constants

        /// <summary>
        /// Human-readable signature
        /// </summary>
        public static readonly string SignatureString = "MSCF";

        /// <summary>
        /// Signature as an unsigned Int32 value
        /// </summary>
        public const uint SignatureValue = 0x4643534D;

        /// <summary>
        /// Signature as a byte array
        /// </summary>
        public static readonly byte[] SignatureBytes = new byte[] { 0x4D, 0x53, 0x43, 0x46 };

        #endregion

        #region Properties

        /// <summary>
        /// Contains the characters "M", "S", "C", and "F" (bytes 0x4D, 0x53, 0x43,
        /// 0x46). This field is used to ensure that the file is a cabinet(.cab) file.
        /// </summary>
        public uint Signature { get; private set; }

        /// <summary>
        /// Reserved field; MUST be set to 0 (zero).
        /// </summary>
        public uint Reserved1 { get; private set; }

        /// <summary>
        /// Specifies the total size of the cabinet file, in bytes.
        /// </summary>
        public uint CabinetSize { get; private set; }

        /// <summary>
        /// Reserved field; MUST be set to 0 (zero).
        /// </summary>
        public uint Reserved2 { get; private set; }

        /// <summary>
        /// Specifies the absolute file offset, in bytes, of the first CFFILE field entry.
        /// </summary>
        public uint FilesOffset { get; private set; }

        /// <summary>
        /// Reserved field; MUST be set to 0 (zero).
        /// </summary>
        public uint Reserved3 { get; private set; }

        /// <summary>
        /// Specifies the minor cabinet file format version. This value MUST be set to 3 (three).
        /// </summary>
        public byte VersionMinor { get; private set; }

        /// <summary>
        /// Specifies the major cabinet file format version. This value MUST be set to 1 (one).
        /// </summary>
        public byte VersionMajor { get; private set; }

        /// <summary>
        /// Specifies the number of CFFOLDER field entries in this cabinet file.
        /// </summary>
        public ushort FolderCount { get; private set; }

        /// <summary>
        /// Specifies the number of CFFILE field entries in this cabinet file.
        /// </summary>
        public ushort FileCount { get; private set; }

        /// <summary>
        /// Specifies bit-mapped values that indicate the presence of optional data.
        /// </summary>
        public HeaderFlags Flags { get; private set; }

        /// <summary>
        /// Specifies an arbitrarily derived (random) value that binds a collection of linked cabinet files
        /// together.All cabinet files in a set will contain the same setID field value.This field is used by
        /// cabinet file extractors to ensure that cabinet files are not inadvertently mixed.This value has no
        /// meaning in a cabinet file that is not in a set.
        /// </summary>
        public ushort SetID { get; private set; }

        /// <summary>
        /// Specifies the sequential number of this cabinet in a multicabinet set. The first cabinet has
        /// iCabinet=0. This field, along with the setID field, is used by cabinet file extractors to ensure that
        /// this cabinet is the correct continuation cabinet when spanning cabinet files.
        /// </summary>
        public ushort CabinetIndex { get; private set; }

        /// <summary>
        /// If the flags.cfhdrRESERVE_PRESENT field is not set, this field is not
        /// present, and the value of cbCFHeader field MUST be zero.Indicates the size, in bytes, of the
        /// abReserve field in this CFHEADER structure.Values for cbCFHeader field MUST be between 0-
        /// 60,000.
        /// </summary>
        public ushort HeaderReservedSize { get; private set; }

        /// <summary>
        /// If the flags.cfhdrRESERVE_PRESENT field is not set, this field is not
        /// present, and the value of cbCFFolder field MUST be zero.Indicates the size, in bytes, of the
        /// abReserve field in each CFFOLDER field entry.Values for fhe cbCFFolder field MUST be between
        /// 0-255.
        /// </summary>
        public byte FolderReservedSize { get; private set; }

        /// <summary>
        /// If the flags.cfhdrRESERVE_PRESENT field is not set, this field is not
        /// present, and the value for the cbCFDATA field MUST be zero.The cbCFDATA field indicates the
        /// size, in bytes, of the abReserve field in each CFDATA field entry. Values for the cbCFDATA field
        /// MUST be between 0 - 255.
        /// </summary>
        public byte DataReservedSize { get; private set; }

        /// <summary>
        /// If the flags.cfhdrRESERVE_PRESENT field is set and the
        /// cbCFHeader field is non-zero, this field contains per-cabinet-file application information. This field
        /// is defined by the application, and is used for application-defined purposes.
        /// </summary>
        public byte[] ReservedData { get; private set; }

        /// <summary>
        /// If the flags.cfhdrPREV_CABINET field is not set, this
        /// field is not present.This is a null-terminated ASCII string that contains the file name of the
        /// logically previous cabinet file. The string can contain up to 255 bytes, plus the null byte. Note that
        /// this gives the name of the most recently preceding cabinet file that contains the initial instance of a
        /// file entry.This might not be the immediately previous cabinet file, when the most recent file spans
        /// multiple cabinet files.If searching in reverse for a specific file entry, or trying to extract a file that is
        /// reported to begin in the "previous cabinet," the szCabinetPrev field would indicate the name of the
        /// cabinet to examine.
        /// </summary>
        public string CabinetPrev { get; private set; }

        /// <summary>
        /// If the flags.cfhdrPREV_CABINET field is not set, then this
        /// field is not present.This is a null-terminated ASCII string that contains a descriptive name for the
        /// media that contains the file named in the szCabinetPrev field, such as the text on the disk label.
        /// This string can be used when prompting the user to insert a disk. The string can contain up to 255
        /// bytes, plus the null byte.
        /// </summary>
        public string DiskPrev { get; private set; }

        /// <summary>
        /// If the flags.cfhdrNEXT_CABINET field is not set, this
        /// field is not present.This is a null-terminated ASCII string that contains the file name of the next
        /// cabinet file in a set. The string can contain up to 255 bytes, plus the null byte. Files that extend
        /// beyond the end of the current cabinet file are continued in the named cabinet file.
        /// </summary>
        public string CabinetNext { get; private set; }

        /// <summary>
        /// If the flags.cfhdrNEXT_CABINET field is not set, this field is
        /// not present.This is a null-terminated ASCII string that contains a descriptive name for the media
        /// that contains the file named in the szCabinetNext field, such as the text on the disk label. The
        /// string can contain up to 255 bytes, plus the null byte. This string can be used when prompting the
        /// user to insert a disk.
        /// </summary>
        public string DiskNext { get; private set; }

        #endregion

        #region Serialization

        /// <summary>
        /// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a CFHEADER object
        /// </summary>
        public static CFHEADER Deserialize(byte[] data, ref int dataPtr)
        {
            if (data == null || dataPtr < 0)
                return null;

            CFHEADER header = new CFHEADER();

            header.Signature = data.ReadUInt32(ref dataPtr);
            if (header.Signature != SignatureValue)
                return null;

            header.Reserved1 = data.ReadUInt32(ref dataPtr);
            if (header.Reserved1 != 0x00000000)
                return null;

            header.CabinetSize = data.ReadUInt32(ref dataPtr);
            if (header.CabinetSize > MSCABCabinet.MaximumCabSize)
                return null;

            header.Reserved2 = data.ReadUInt32(ref dataPtr);
            if (header.Reserved2 != 0x00000000)
                return null;

            header.FilesOffset = data.ReadUInt32(ref dataPtr);

            header.Reserved3 = data.ReadUInt32(ref dataPtr);
            if (header.Reserved3 != 0x00000000)
                return null;

            header.VersionMinor = data.ReadByte(ref dataPtr);
            header.VersionMajor = data.ReadByte(ref dataPtr);
            if (header.VersionMajor != 0x00000001 || header.VersionMinor != 0x00000003)
                return null;

            header.FolderCount = data.ReadUInt16(ref dataPtr);
            if (header.FolderCount > MSCABCabinet.MaximumFolderCount)
                return null;

            header.FileCount = data.ReadUInt16(ref dataPtr);
            if (header.FileCount > MSCABCabinet.MaximumFileCount)
                return null;

            header.Flags = (HeaderFlags)data.ReadUInt16(ref dataPtr);
            header.SetID = data.ReadUInt16(ref dataPtr);
            header.CabinetIndex = data.ReadUInt16(ref dataPtr);

            if (header.Flags.HasFlag(HeaderFlags.RESERVE_PRESENT))
            {
                header.HeaderReservedSize = data.ReadUInt16(ref dataPtr);
                if (header.HeaderReservedSize > 60_000)
                    return null;

                header.FolderReservedSize = data.ReadByte(ref dataPtr);
                header.DataReservedSize = data.ReadByte(ref dataPtr);

                if (header.HeaderReservedSize > 0)
                    header.ReservedData = data.ReadBytes(ref dataPtr, header.HeaderReservedSize);
            }

            if (header.Flags.HasFlag(HeaderFlags.PREV_CABINET))
            {
                header.CabinetPrev = data.ReadString(ref dataPtr, Encoding.ASCII);
                header.DiskPrev = data.ReadString(ref dataPtr, Encoding.ASCII);
            }

            if (header.Flags.HasFlag(HeaderFlags.NEXT_CABINET))
            {
                header.CabinetNext = data.ReadString(ref dataPtr, Encoding.ASCII);
                header.DiskNext = data.ReadString(ref dataPtr, Encoding.ASCII);
            }

            return header;
        }

        /// <summary>
        /// Deserialize <paramref name="data"/> into a CFHEADER object
        /// </summary>
        public static CFHEADER Deserialize(Stream data)
        {
            if (data == null || data.Position < 0)
                return null;

            CFHEADER header = new CFHEADER();

            header.Signature = data.ReadUInt32();
            if (header.Signature != SignatureValue)
                return null;

            header.Reserved1 = data.ReadUInt32();
            if (header.Reserved1 != 0x00000000)
                return null;

            header.CabinetSize = data.ReadUInt32();
            if (header.CabinetSize > MSCABCabinet.MaximumCabSize)
                return null;

            header.Reserved2 = data.ReadUInt32();
            if (header.Reserved2 != 0x00000000)
                return null;

            header.FilesOffset = data.ReadUInt32();

            header.Reserved3 = data.ReadUInt32();
            if (header.Reserved3 != 0x00000000)
                return null;

            header.VersionMinor = data.ReadByteValue();
            header.VersionMajor = data.ReadByteValue();
            if (header.VersionMajor != 0x00000001 || header.VersionMinor != 0x00000003)
                return null;

            header.FolderCount = data.ReadUInt16();
            if (header.FolderCount > MSCABCabinet.MaximumFolderCount)
                return null;

            header.FileCount = data.ReadUInt16();
            if (header.FileCount > MSCABCabinet.MaximumFileCount)
                return null;

            header.Flags = (HeaderFlags)data.ReadUInt16();
            header.SetID = data.ReadUInt16();
            header.CabinetIndex = data.ReadUInt16();

            if (header.Flags.HasFlag(HeaderFlags.RESERVE_PRESENT))
            {
                header.HeaderReservedSize = data.ReadUInt16();
                if (header.HeaderReservedSize > 60_000)
                    return null;

                header.FolderReservedSize = data.ReadByteValue();
                header.DataReservedSize = data.ReadByteValue();

                if (header.HeaderReservedSize > 0)
                    header.ReservedData = data.ReadBytes(header.HeaderReservedSize);
            }

            if (header.Flags.HasFlag(HeaderFlags.PREV_CABINET))
            {
                header.CabinetPrev = data.ReadString(Encoding.ASCII);
                header.DiskPrev = data.ReadString(Encoding.ASCII);
            }

            if (header.Flags.HasFlag(HeaderFlags.NEXT_CABINET))
            {
                header.CabinetNext = data.ReadString(Encoding.ASCII);
                header.DiskNext = data.ReadString(Encoding.ASCII);
            }

            return header;
        }

        #endregion

        #region Public Functionality

        /// <summary>
        /// Print all info about the cabinet file
        /// </summary>
        public void PrintInfo()
        {
            Console.WriteLine("CFHEADER INFORMATION:");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine($"    Signature:          {Encoding.ASCII.GetString(BitConverter.GetBytes(Signature))} (0x{Signature:X8})");
            Console.WriteLine($"    Reserved1:          {Reserved1} (0x{Reserved1:X8})");
            Console.WriteLine($"    CabinetSize:        {CabinetSize} (0x{CabinetSize:X8})");
            Console.WriteLine($"    Reserved2:          {Reserved2} (0x{Reserved2:X8})");
            Console.WriteLine($"    FilesOffset:        {FilesOffset} (0x{FilesOffset:X8})");
            Console.WriteLine($"    Reserved3:          {Reserved3} (0x{Reserved3:X8})");
            Console.WriteLine($"    Version:            {VersionMajor}.{VersionMinor}");
            Console.WriteLine($"    FolderCount:        {FolderCount} (0x{FolderCount:X4})");
            Console.WriteLine($"    FileCount:          {FileCount} (0x{FileCount:X4})");
            Console.WriteLine($"    Flags:              {Flags} (0x{(ushort)Flags:X4})");
            Console.WriteLine($"    SetID:              {SetID} (0x{SetID:X4})");
            Console.WriteLine($"    CabinetIndex:       {CabinetIndex} (0x{CabinetIndex:X4})");

            if (Flags.HasFlag(HeaderFlags.RESERVE_PRESENT))
            {
                Console.WriteLine($"    HeaderReservedSize: {HeaderReservedSize} (0x{HeaderReservedSize:X4})");
                Console.WriteLine($"    FolderReservedSize: {FolderReservedSize} (0x{FolderReservedSize:X2})");
                Console.WriteLine($"    DataReservedSize:   {DataReservedSize} (0x{DataReservedSize:X2})");
                // TODO: Output reserved data
            }

            if (Flags.HasFlag(HeaderFlags.PREV_CABINET))
            {
                Console.WriteLine($"    CabinetPrev:        {CabinetPrev}");
                Console.WriteLine($"    DiskPrev:           {DiskPrev}");
            }

            if (Flags.HasFlag(HeaderFlags.NEXT_CABINET))
            {
                Console.WriteLine($"    CabinetNext:        {CabinetNext}");
                Console.WriteLine($"    DiskNext:           {DiskNext}");
            }

            Console.WriteLine();
        }

        #endregion
    }

    [Flags]
    public enum HeaderFlags : ushort
    {
        /// <summary>
        /// The flag is set if this cabinet file is not the first in a set of cabinet files.
        /// When this bit is set, the szCabinetPrev and szDiskPrev fields are present in this CFHEADER
        /// structure. The value is 0x0001.
        /// </summary>
        PREV_CABINET = 0x0001,

        /// <summary>
        /// The flag is set if this cabinet file is not the last in a set of cabinet files.
        /// When this bit is set, the szCabinetNext and szDiskNext fields are present in this CFHEADER
        /// structure. The value is 0x0002.
        /// </summary>
        NEXT_CABINET = 0x0002,

        /// <summary>
        /// The flag is set if if this cabinet file contains any reserved fields. When
        /// this bit is set, the cbCFHeader, cbCFFolder, and cbCFData fields are present in this CFHEADER
        /// structure. The value is 0x0004.
        /// </summary>
        RESERVE_PRESENT = 0x0004,
    }

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
    public class CFFOLDER
    {
        #region Properties

        /// <summary>
        /// Specifies the absolute file offset of the first CFDATA field block for the folder.
        /// </summary>
        public uint CabStartOffset { get; private set; }

        /// <summary>
        /// Specifies the number of CFDATA structures for this folder that are actually in this cabinet.
        /// A folder can continue into another cabinet and have more CFDATA structure blocks in that cabinet
        /// file.A folder can start in a previous cabinet.This number represents only the CFDATA structures for
        /// this folder that are at least partially recorded in this cabinet.
        /// </summary>
        public ushort DataCount { get; private set; }

        /// <summary>
        /// Indicates the compression method used for all CFDATA structure entries in this
        /// folder.
        /// </summary>
        public CompressionType CompressionType { get; private set; }

        /// <summary>
        /// If the CFHEADER.flags.cfhdrRESERVE_PRESENT field is set
        /// and the cbCFFolder field is non-zero, then this field contains per-folder application information.
        /// This field is defined by the application, and is used for application-defined purposes.
        /// </summary>
        public byte[] ReservedData { get; private set; }

        /// <summary>
        /// Data blocks associated with this folder
        /// </summary>
        public Dictionary<int, CFDATA> DataBlocks { get; private set; } = new Dictionary<int, CFDATA>();

        #endregion

        #region Generated Properties

        /// <summary>
        /// Get the uncompressed data associated with this folder, if possible
        /// </summary>
        public byte[] UncompressedData
        {
            get
            {
                if (DataBlocks == null || DataBlocks.Count == 0)
                    return null;

                // Store the last decompressed block for MS-ZIP
                byte[] lastDecompressed = null;

                List<byte> data = new List<byte>();
                foreach (CFDATA dataBlock in DataBlocks.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value))
                {
                    byte[] decompressed = null;
                    switch (CompressionType)
                    {
                        case CompressionType.TYPE_NONE:
                            decompressed = dataBlock.CompressedData;
                            break;
                        case CompressionType.TYPE_MSZIP:
                            decompressed = MSZIPBlock.Deserialize(dataBlock.CompressedData).DecompressBlock(dataBlock.UncompressedSize, lastDecompressed);
                            break;
                        case CompressionType.TYPE_QUANTUM:
                            // TODO: UNIMPLEMENTED
                            break;
                        case CompressionType.TYPE_LZX:
                            // TODO: UNIMPLEMENTED
                            break;
                        default:
                            return null;
                    }

                    lastDecompressed = decompressed;
                    if (decompressed != null)
                        data.AddRange(decompressed);
                }

                return data.ToArray();
            }
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a CFFOLDER object
        /// </summary>
        public static CFFOLDER Deserialize(byte[] data, ref int dataPtr, int basePtr, CFHEADER header)
        {
            if (data == null || dataPtr < 0)
                return null;

            CFFOLDER folder = new CFFOLDER();

            folder.CabStartOffset = data.ReadUInt32(ref dataPtr);
            folder.DataCount = data.ReadUInt16(ref dataPtr);
            folder.CompressionType = (CompressionType)data.ReadUInt16(ref dataPtr);

            if (header.FolderReservedSize > 0)
                folder.ReservedData = data.ReadBytes(ref dataPtr, header.FolderReservedSize);

            if (folder.CabStartOffset > 0)
            {
                int blockPtr = basePtr + (int)folder.CabStartOffset;
                for (int i = 0; i < folder.DataCount; i++)
                {
                    int offset = blockPtr;
                    CFDATA dataBlock = CFDATA.Deserialize(data, ref blockPtr, header.DataReservedSize);
                    folder.DataBlocks[offset] = dataBlock;
                }
            }

            return folder;
        }

        /// <summary>
        /// Deserialize <paramref name="data"/> into a CFFOLDER object
        /// </summary>
        public static CFFOLDER Deserialize(Stream data, CFHEADER header)
        {
            if (data == null || data.Position < 0)
                return null;

            CFFOLDER folder = new CFFOLDER();

            folder.CabStartOffset = data.ReadUInt32();
            folder.DataCount = data.ReadUInt16();
            folder.CompressionType = (CompressionType)data.ReadUInt16();

            if (header.FolderReservedSize > 0)
                folder.ReservedData = data.ReadBytes(header.FolderReservedSize);

            if (folder.CabStartOffset > 0)
            {
                long currentPosition = data.Position;
                data.Seek(folder.CabStartOffset, SeekOrigin.Begin);

                for (int i = 0; i < folder.DataCount; i++)
                {
                    CFDATA dataBlock = CFDATA.Deserialize(data, header.DataReservedSize);
                    folder.DataBlocks[(int)folder.CabStartOffset] = dataBlock;
                }

                data.Seek(currentPosition, SeekOrigin.Begin);
            }

            return folder;
        }

        #endregion

        #region Public Functionality

        /// <summary>
        /// Print all info about the cabinet file
        /// </summary>
        public void PrintInfo()
        {
            Console.WriteLine($"        CabStartOffset:     {CabStartOffset} (0x{CabStartOffset:X8})");
            Console.WriteLine($"        DataCount:          {DataCount} (0x{DataCount:X4})");
            Console.WriteLine($"        CompressionType:    {CompressionType} (0x{(ushort)CompressionType:X4})");
            // TODO: Output reserved data

            Console.WriteLine();
        }

        #endregion
    }

    public enum CompressionType : ushort
    {
        /// <summary>
        /// Mask for compression type.
        /// </summary>
        MASK_TYPE = 0x000F,

        /// <summary>
        /// No compression.
        /// </summary>
        TYPE_NONE = 0x0000,

        /// <summary>
        /// MSZIP compression.
        /// </summary>
        TYPE_MSZIP = 0x0001,

        /// <summary>
        /// Quantum compression.
        /// </summary>
        TYPE_QUANTUM = 0x0002,

        /// <summary>
        /// LZX compression.
        /// </summary>
        TYPE_LZX = 0x0003,
    }

    /// <summary>
    /// Each CFFILE structure contains information about one of the files stored (or at least partially
    /// stored) in this cabinet, as shown in the following packet diagram.The first CFFILE structure entry in
    /// each cabinet is found at the absolute offset CFHEADER.coffFiles field. CFHEADER.cFiles field
    /// indicates how many of these entries are in the cabinet. The CFFILE structure entries in a cabinet
    /// are ordered by iFolder field value, and then by the uoffFolderStart field value.Entries for files
    /// continued from the previous cabinet will be first, and entries for files continued to the next cabinet
    /// will be last.
    /// </summary>
    public class CFFILE
    {
        #region Properties

        /// <summary>
        /// Specifies the uncompressed size of this file, in bytes.
        /// </summary>
        public uint FileSize { get; private set; }

        /// <summary>
        /// Specifies the uncompressed offset, in bytes, of the start of this file's data. For the
        /// first file in each folder, this value will usually be zero. Subsequent files in the folder will have offsets
        /// that are typically the running sum of the cbFile field values.
        /// </summary>
        public uint FolderStartOffset { get; private set; }

        /// <summary>
        /// Index of the folder that contains this file's data.
        /// </summary>
        public FolderIndex FolderIndex { get; private set; }

        /// <summary>
        /// Date of this file, in the format ((year–1980) << 9)+(month << 5)+(day), where
        /// month={1..12} and day = { 1..31 }. This "date" is typically considered the "last modified" date in local
        /// time, but the actual definition is application-defined.
        /// </summary>
        public ushort Date { get; private set; }

        /// <summary>
        /// Time of this file, in the format (hour << 11)+(minute << 5)+(seconds/2), where
        /// hour={0..23}. This "time" is typically considered the "last modified" time in local time, but the
        /// actual definition is application-defined.
        /// </summary>
        public ushort Time { get; private set; }

        /// <summary>
        /// Attributes of this file; can be used in any combination.
        /// </summary>
        public FileAttributes Attributes { get; private set; }

        /// <summary>
        /// The null-terminated name of this file. Note that this string can include path
        /// separator characters.The string can contain up to 256 bytes, plus the null byte. When the
        /// _A_NAME_IS_UTF attribute is set, this string can be converted directly to Unicode, avoiding
        /// locale-specific dependencies. When the _A_NAME_IS_UTF attribute is not set, this string is subject
        /// to interpretation depending on locale. When a string that contains Unicode characters larger than
        /// 0x007F is encoded in the szName field, the _A_NAME_IS_UTF attribute SHOULD be included in
        /// the file's attributes. When no characters larger than 0x007F are in the name, the
        /// _A_NAME_IS_UTF attribute SHOULD NOT be set. If byte values larger than 0x7F are found in
        /// CFFILE.szName field, but the _A_NAME_IS_UTF attribute is not set, the characters SHOULD be
        /// interpreted according to the current location.
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region Generated Properties

        /// <summary>
        /// Convert the internal values into a DateTime object, if possible
        /// </summary>
        public DateTime DateAndTimeAsDateTime
        {
            get
            {
                // If we have an invalid DateTime
                if (Date == 0 && Time == 0)
                    return DateTime.MinValue;

                try
                {
                    // Date property
                    int year = (Date >> 9) + 1980;
                    int month = (Date >> 5) & 0x0F;
                    int day = Date & 0x1F;

                    // Time property
                    int hour = Time >> 11;
                    int minute = (Time >> 5) & 0x3F;
                    int second = (Time << 1) & 0x3E;

                    return new DateTime(year, month, day, hour, minute, second);
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
            set
            {
                Date = (ushort)(((value.Year - 1980) << 9) + (value.Month << 5) + (value.Day));
                Time = (ushort)((value.Hour << 11) + (value.Minute << 5) + (value.Second / 2));
            }
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a CFFILE object
        /// </summary>
        public static CFFILE Deserialize(byte[] data, ref int dataPtr)
        {
            if (data == null || dataPtr < 0)
                return null;

            CFFILE file = new CFFILE();

            file.FileSize = data.ReadUInt32(ref dataPtr);
            file.FolderStartOffset = data.ReadUInt32(ref dataPtr);
            file.FolderIndex = (FolderIndex)data.ReadUInt16(ref dataPtr);
            file.Date = data.ReadUInt16(ref dataPtr);
            file.Time = data.ReadUInt16(ref dataPtr);
            file.Attributes = (FileAttributes)data.ReadUInt16(ref dataPtr);

            if (file.Attributes.HasFlag(FileAttributes.NAME_IS_UTF))
                file.Name = data.ReadString(ref dataPtr, Encoding.Unicode);
            else
                file.Name = data.ReadString(ref dataPtr, Encoding.ASCII);

            return file;
        }

        /// <summary>
        /// Deserialize <paramref name="data"/> into a CFFILE object
        /// </summary>
        public static CFFILE Deserialize(Stream data)
        {
            if (data == null || data.Position < 0)
                return null;

            CFFILE file = new CFFILE();

            file.FileSize = data.ReadUInt32();
            file.FolderStartOffset = data.ReadUInt32();
            file.FolderIndex = (FolderIndex)data.ReadUInt16();
            file.Date = data.ReadUInt16();
            file.Time = data.ReadUInt16();
            file.Attributes = (FileAttributes)data.ReadUInt16();

            if (file.Attributes.HasFlag(FileAttributes.NAME_IS_UTF))
                file.Name = data.ReadString(Encoding.Unicode);
            else
                file.Name = data.ReadString(Encoding.ASCII);

            return file;
        }

        #endregion

        #region Public Functionality

        /// <summary>
        /// Print all info about the cabinet file
        /// </summary>
        public void PrintInfo()
        {
            Console.WriteLine($"        FileSize:           {FileSize} (0x{FileSize:X8})");
            Console.WriteLine($"        FolderStartOffset:  {FolderStartOffset} (0x{FolderStartOffset:X4})");
            Console.WriteLine($"        FolderIndex:        {FolderIndex} (0x{(ushort)FolderIndex:X4})");
            Console.WriteLine($"        DateTime:           {DateAndTimeAsDateTime} (0x{Date:X4} 0x{Time:X4})");
            Console.WriteLine($"        Attributes:         {Attributes} (0x{(ushort)Attributes:X4})");
            Console.WriteLine($"        Name:               {Name}");

            Console.WriteLine();
        }

        #endregion
    }

    public enum FolderIndex : ushort
    {
        /// <summary>
        /// A value of zero indicates that this is the
        /// first folder in this cabinet file.
        /// </summary>
        FIRST_FOLDER = 0x0000,

        /// <summary>
        /// Indicates that the folder index is actually zero, but that
        /// extraction of this file would have to begin with the cabinet named in the
        /// CFHEADER.szCabinetPrev field. 
        /// </summary>
        CONTINUED_FROM_PREV = 0xFFFD,

        /// <summary>
        /// Indicates that the folder index
        /// is actually one less than THE CFHEADER.cFolders field value, and that extraction of this file will
        /// require continuation to the cabinet named in the CFHEADER.szCabinetNext field.
        /// </summary>
        CONTINUED_TO_NEXT = 0xFFFE,

        /// <see cref="CONTINUED_FROM_PREV"/>
        /// <see cref="CONTINUED_TO_NEXT"/>
        CONTINUED_PREV_AND_NEXT = 0xFFFF,
    }

    [Flags]
    public enum FileAttributes : ushort
    {
        /// <summary>
        /// File is read-only.
        /// </summary>
        RDONLY = 0x0001,

        /// <summary>
        /// File is hidden.
        /// </summary>
        HIDDEN = 0x0002,

        /// <summary>
        /// File is a system file.
        /// </summary>
        SYSTEM = 0x0004,

        /// <summary>
        /// File has been modified since last backup.
        /// </summary>
        ARCH = 0x0040,

        /// <summary>
        /// File will be run after extraction.
        /// </summary>
        EXEC = 0x0080,

        /// <summary>
        /// The szName field contains UTF.
        /// </summary>
        NAME_IS_UTF = 0x0100,
    }

    /// <summary>
    /// Each CFDATA structure describes some amount of compressed data, as shown in the following
    /// packet diagram. The first CFDATA structure entry for each folder is located by using the
    /// <see cref="CFFOLDER.CabStartOffset"/> field. Subsequent CFDATA structure records for this folder are
    /// contiguous.
    /// </summary>
    public class CFDATA
    {
        #region Properties

        /// <summary>
        /// Checksum of this CFDATA structure, from the <see cref="CompressedSize"/> through the
        /// <see cref="CompressedData"/> fields. It can be set to 0 (zero) if the checksum is not supplied.
        /// </summary>
        public uint Checksum { get; private set; }

        /// <summary>
        /// Number of bytes of compressed data in this CFDATA structure record. When the
        /// <see cref="UncompressedSize"/> field is zero, this field indicates only the number of bytes that fit into this cabinet file.
        /// </summary>
        public ushort CompressedSize { get; private set; }

        /// <summary>
        /// The uncompressed size of the data in this CFDATA structure entry in bytes. When this
        /// CFDATA structure entry is continued in the next cabinet file, the <see cref="UncompressedSize"/> field will be zero, and
        /// the <see cref="UncompressedSize"/> field in the first CFDATA structure entry in the next cabinet file will report the total
        /// uncompressed size of the data from both CFDATA structure blocks.
        /// </summary>
        public ushort UncompressedSize { get; private set; }

        /// <summary>
        /// If the <see cref="HeaderFlags.RESERVE_PRESENT"/> flag is set
        /// and the <see cref="CFHEADER.DataReservedSize"/> field value is non-zero, this field contains per-datablock application information.
        /// This field is defined by the application, and it is used for application-defined purposes.
        /// </summary>
        public byte[] ReservedData { get; private set; }

        /// <summary>
        /// The compressed data bytes, compressed by using the <see cref="CFFOLDER.CompressionType"/>
        /// method. When the <see cref="UncompressedSize"/> field value is zero, these data bytes MUST be combined with the data
        /// bytes from the next cabinet's first CFDATA structure entry before decompression. When the
        ///<see cref="CFFOLDER.CompressionType"/> field indicates that the data is not compressed, this field contains the
        /// uncompressed data bytes. In this case, the <see cref="CompressedSize"/> and <see cref="UncompressedSize"/> field values will be equal unless
        /// this CFDATA structure entry crosses a cabinet file boundary.
        /// </summary>
        public byte[] CompressedData { get; private set; }

        #endregion

        #region Serialization

        /// <summary>
        /// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a CFDATA object
        /// </summary>
        public static CFDATA Deserialize(byte[] data, ref int dataPtr, byte dataReservedSize = 0)
        {
            if (data == null || dataPtr < 0)
                return null;

            CFDATA dataBlock = new CFDATA();

            dataBlock.Checksum = data.ReadUInt32(ref dataPtr);
            dataBlock.CompressedSize = data.ReadUInt16(ref dataPtr);
            dataBlock.UncompressedSize = data.ReadUInt16(ref dataPtr);

            if (dataBlock.UncompressedSize != 0 && dataBlock.CompressedSize > dataBlock.UncompressedSize)
                return null;

            if (dataReservedSize > 0)
                dataBlock.ReservedData = data.ReadBytes(ref dataPtr, dataReservedSize);

            if (dataBlock.CompressedSize > 0)
                dataBlock.CompressedData = data.ReadBytes(ref dataPtr, dataBlock.CompressedSize);

            return dataBlock;
        }

        /// <summary>
        /// Deserialize <paramref name="data"/> into a CFDATA object
        /// </summary>
        public static CFDATA Deserialize(Stream data, byte dataReservedSize = 0)
        {
            if (data == null || data.Position < 0)
                return null;

            CFDATA dataBlock = new CFDATA();

            dataBlock.Checksum = data.ReadUInt32();
            dataBlock.CompressedSize = data.ReadUInt16();
            dataBlock.UncompressedSize = data.ReadUInt16();

            if (dataBlock.UncompressedSize != 0 && dataBlock.CompressedSize > dataBlock.UncompressedSize)
                return null;

            if (dataReservedSize > 0)
                dataBlock.ReservedData = data.ReadBytes(dataReservedSize);

            if (dataBlock.CompressedSize > 0)
                dataBlock.CompressedData = data.ReadBytes(dataBlock.CompressedSize);

            return dataBlock;
        }

        #endregion
    }

    /// <summary>
    /// The computation and verification of checksums found in CFDATA structure entries cabinet files is
    /// done by using a function described by the following mathematical notation. When checksums are
    /// not supplied by the cabinet file creating application, the checksum field is set to 0 (zero). Cabinet
    /// extracting applications do not compute or verify the checksum if the field is set to 0 (zero).
    /// </summary>
    public static class Checksum
    {
        public static uint ChecksumData(byte[] data)
        {
            uint[] C = new uint[4]
            {
                S(data, 1, data.Length),
                S(data, 2, data.Length),
                S(data, 3, data.Length),
                S(data, 4, data.Length),
            };

            return C[0] ^ C[1] ^ C[2] ^ C[3];
        }

        private static uint S(byte[] a, int b, int x)
        {
            int n = a.Length;

            if (x < 4 && b > n % 4)
                return 0;
            else if (x < 4 && b <= n % 4)
                return a[n - b + 1];
            else // if (x >= 4)
                return a[n - x + b] ^ S(a, b, x - 4);
        }
    }
}
