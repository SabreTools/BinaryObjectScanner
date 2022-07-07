using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Tools;
using WixToolset.Dtf.Compression.Cab;

namespace BurnOutSharp.FileType
{
    // Specification available at http://download.microsoft.com/download/5/0/1/501ED102-E53F-4CE0-AA6B-B0F93629DDC6/Exchange/%5BMS-CAB%5D.pdf
    public class MicrosoftCAB : IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte?[] { 0x4d, 0x53, 0x43, 0x46 }))
                return true;

            return false;
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the cab file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                CabInfo cabInfo = new CabInfo(file);
                cabInfo.Unpack(tempPath);

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch (Exception ex)
                {
                    if (scanner.IncludeDebug) Console.WriteLine(ex);
                }

                // Remove temporary path references
                Utilities.StripFromKeys(protections, tempPath);

                return protections;
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }

        #region LibMSPackSharp

        // TODO: Add stream opening support
        /// <inheritdoc/>
        //public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        //{
        //    // If the cab file itself fails
        //    try
        //    {
        //        string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        //        Directory.CreateDirectory(tempPath);

        //        // Create the decompressor
        //        var decompressor = Library.CreateCABDecompressor(null);
        //        decompressor.Debug = scanner.IncludeDebug;

        //        // Open the cab file
        //        var cabFile = decompressor.Open(file);
        //        if (cabFile == null)
        //        {
        //            if (scanner.IncludeDebug) Console.WriteLine($"Error occurred opening of '{file}': {decompressor.Error}");
        //            return null;
        //        }

        //        // If we have a previous CAB and it exists, don't try scanning
        //        string directory = Path.GetDirectoryName(file);
        //        if (!string.IsNullOrWhiteSpace(cabFile.PreviousCabinetName))
        //        {
        //            if (File.Exists(Path.Combine(directory, cabFile.PreviousCabinetName)))
        //                return null;
        //        }

        //        // If there are additional next CABs, add those
        //        string fileName = Path.GetFileName(file);
        //        CABExtract.LoadSpanningCabinets(cabFile, fileName);

        //        // Loop through the found internal files
        //        var sub = cabFile.Files;
        //        while (sub != null)
        //        {
        //            // If an individual entry fails
        //            try
        //            {
        //                // The trim here is for some very odd and stubborn files
        //                string tempFile = Path.Combine(tempPath, sub.Filename.TrimEnd('\0', ' ', '.'));
        //                Error error = decompressor.Extract(sub, tempFile);
        //                if (error != Error.MSPACK_ERR_OK)
        //                {
        //                    if (scanner.IncludeDebug) Console.WriteLine($"Error occurred during extraction of '{sub.Filename}': {error}");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                if (scanner.IncludeDebug) Console.WriteLine(ex);
        //            }

        //            sub = sub.Next;
        //        }

        //        // Destroy the decompressor
        //        Library.DestroyCABDecompressor(decompressor);

        //        // Collect and format all found protections
        //        var protections = scanner.GetProtections(tempPath);

        //        // If temp directory cleanup fails
        //        try
        //        {
        //            Directory.Delete(tempPath, true);
        //        }
        //        catch (Exception ex)
        //        {
        //            if (scanner.IncludeDebug) Console.WriteLine(ex);
        //        }

        //        // Remove temporary path references
        //        Utilities.StripFromKeys(protections, tempPath);

        //        return protections;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (scanner.IncludeDebug) Console.WriteLine(ex);
        //    }

        //    return null;
        //}

        #endregion

        #region TEMPORARY AREA FOR MS-CAB FORMAT

        // TODO: Add multi-cabinet reading
        internal class MSCABCabinet
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

            /// <summary>
            /// The actual compressed file data in CFDATA entries.
            /// </summary>
            public CFDATA[] DataBlocks { get; private set; }

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
                    cabinet.Folders[i] = CFFOLDER.Deserialize(data, ref dataPtr, cabinet.Header.FolderReservedSize);
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

                // TODO: Should we populate the data blocks here?
                // TODO: How do we determine the number of data blocks?
                // TODO: If the data blocks start right after the CFFILE data, should we just store the data block offset?

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
                    string outputPath = Path.Combine(outputDirectory, file.NameAsString);
                }

                // TODO: We don't check for other cabinets here yet
                // TODO: Read and decompress data blocks

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
                    if (exact ? tempFile.NameAsString == filePath : tempFile.NameAsString.EndsWith(filePath, StringComparison.OrdinalIgnoreCase))
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
                string outputPath = Path.Combine(outputDirectory, file.NameAsString);

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
                        Console.WriteLine($"        Not found or NULL");
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
                        Console.WriteLine($"        Not found or NULL");
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
        internal class CFHEADER
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
            /// field is not present.This is a NULL-terminated ASCII string that contains the file name of the
            /// logically previous cabinet file. The string can contain up to 255 bytes, plus the NULL byte. Note that
            /// this gives the name of the most recently preceding cabinet file that contains the initial instance of a
            /// file entry.This might not be the immediately previous cabinet file, when the most recent file spans
            /// multiple cabinet files.If searching in reverse for a specific file entry, or trying to extract a file that is
            /// reported to begin in the "previous cabinet," the szCabinetPrev field would indicate the name of the
            /// cabinet to examine.
            /// </summary>
            public byte[] CabinetPrev { get; private set; }

            /// <summary>
            /// If the flags.cfhdrPREV_CABINET field is not set, then this
            /// field is not present.This is a NULL-terminated ASCII string that contains a descriptive name for the
            /// media that contains the file named in the szCabinetPrev field, such as the text on the disk label.
            /// This string can be used when prompting the user to insert a disk. The string can contain up to 255
            /// bytes, plus the NULL byte.
            /// </summary>
            public byte[] DiskPrev { get; private set; }

            /// <summary>
            /// If the flags.cfhdrNEXT_CABINET field is not set, this
            /// field is not present.This is a NULL-terminated ASCII string that contains the file name of the next
            /// cabinet file in a set. The string can contain up to 255 bytes, plus the NULL byte. Files that extend
            /// beyond the end of the current cabinet file are continued in the named cabinet file.
            /// </summary>
            public byte[] CabinetNext { get; private set; }

            /// <summary>
            /// If the flags.cfhdrNEXT_CABINET field is not set, this field is
            /// not present.This is a NULL-terminated ASCII string that contains a descriptive name for the media
            /// that contains the file named in the szCabinetNext field, such as the text on the disk label. The
            /// string can contain up to 255 bytes, plus the NULL byte. This string can be used when prompting the
            /// user to insert a disk.
            /// </summary>
            public byte[] DiskNext { get; private set; }

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

                header.Signature = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
                if (header.Signature != SignatureValue)
                    return null;

                header.Reserved1 = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
                if (header.Reserved1 != 0x00000000)
                    return null;

                header.CabinetSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
                if (header.CabinetSize > MSCABCabinet.MaximumCabSize)
                    return null;

                header.Reserved2 = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
                if (header.Reserved2 != 0x00000000)
                    return null;

                header.FilesOffset = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;

                header.Reserved3 = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
                if (header.Reserved3 != 0x00000000)
                    return null;

                header.VersionMinor = data[dataPtr++];
                header.VersionMajor = data[dataPtr++];
                if (header.VersionMajor != 0x00000001 || header.VersionMinor != 0x00000003)
                    return null;

                header.FolderCount = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
                if (header.FolderCount > MSCABCabinet.MaximumFolderCount)
                    return null;

                header.FileCount = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
                if (header.FileCount > MSCABCabinet.MaximumFileCount)
                    return null;

                header.Flags = (HeaderFlags)BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
                header.SetID = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
                header.CabinetIndex = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;

                if (header.Flags.HasFlag(HeaderFlags.RESERVE_PRESENT))
                {
                    header.HeaderReservedSize = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
                    header.FolderReservedSize = data[dataPtr++];
                    header.DataReservedSize = data[dataPtr++];

                    if (header.HeaderReservedSize > 0)
                    {
                        header.ReservedData = new byte[header.HeaderReservedSize];
                        Array.Copy(data, dataPtr, header.ReservedData, 0, header.HeaderReservedSize);
                        dataPtr += header.HeaderReservedSize;
                    }
                }

                // TODO: Make string-finding block a helper method
                if (header.Flags.HasFlag(HeaderFlags.PREV_CABINET))
                {
                    int nullIndex = Array.IndexOf<byte>(data, 0x00, dataPtr, 0xFF);
                    int stringSize = nullIndex - dataPtr;
                    if (stringSize < 0 || stringSize > 255)
                        return null;

                    header.CabinetPrev = new byte[stringSize];
                    Array.Copy(data, dataPtr, header.CabinetPrev, 0, stringSize);
                    dataPtr += stringSize + 1;

                    nullIndex = Array.IndexOf<byte>(data, 0x00, dataPtr, 0xFF);
                    stringSize = nullIndex - dataPtr;
                    if (stringSize < 0 || stringSize > 255)
                        return null;

                    header.DiskPrev = new byte[stringSize];
                    Array.Copy(data, dataPtr, header.DiskPrev, 0, stringSize);
                    dataPtr += stringSize + 1;
                }

                if (header.Flags.HasFlag(HeaderFlags.NEXT_CABINET))
                {
                    int nullIndex = Array.IndexOf<byte>(data, 0x00, dataPtr, 0xFF);
                    int stringSize = nullIndex - dataPtr;
                    if (stringSize < 0 || stringSize > 255)
                        return null;

                    header.CabinetNext = new byte[stringSize];
                    Array.Copy(data, dataPtr, header.CabinetNext, 0, stringSize);
                    dataPtr += stringSize + 1;

                    nullIndex = Array.IndexOf<byte>(data, 0x00, dataPtr, 0xFF);
                    stringSize = nullIndex - dataPtr;
                    if (stringSize < 0 || stringSize > 255)
                        return null;

                    header.DiskNext = new byte[stringSize];
                    Array.Copy(data, dataPtr, header.DiskNext, 0, stringSize);
                    dataPtr += stringSize + 1;
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
                Console.WriteLine($"    Signature:          {Signature:X8}");
                Console.WriteLine($"    Reserved1:          {Reserved1:X8}");
                Console.WriteLine($"    CabinetSize:        {CabinetSize:X8}");
                Console.WriteLine($"    Reserved2:          {Reserved2:X8}");
                Console.WriteLine($"    FilesOffset:        {FilesOffset} (0x{FilesOffset:X8})");
                Console.WriteLine($"    Reserved3:          {Reserved3:X8}");
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
                    Console.WriteLine($"    CabinetPrev:        {Encoding.ASCII.GetString(CabinetPrev).TrimEnd('\0')}");
                    Console.WriteLine($"    DiskPrev:           {Encoding.ASCII.GetString(DiskPrev).TrimEnd('\0')}");
                }

                if (Flags.HasFlag(HeaderFlags.NEXT_CABINET))
                {
                    Console.WriteLine($"    CabinetNext:        {Encoding.ASCII.GetString(CabinetNext).TrimEnd('\0')}");
                    Console.WriteLine($"    DiskNext:           {Encoding.ASCII.GetString(DiskNext).TrimEnd('\0')}");
                }

                Console.WriteLine();
            }

            #endregion
            }

        [Flags]
        internal enum HeaderFlags : ushort
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
        internal class CFFOLDER
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

            #endregion

            #region Serialization

            /// <summary>
            /// Deserialize <paramref name="data"/> at <paramref name="dataPtr"/> into a CFFOLDER object
            /// </summary>
            public static CFFOLDER Deserialize(byte[] data, ref int dataPtr, byte folderReservedSize = 0)
            {
                if (data == null || dataPtr < 0)
                    return null;

                CFFOLDER folder = new CFFOLDER();

                folder.CabStartOffset = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
                folder.DataCount = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
                folder.CompressionType = (CompressionType)BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;

                if (folderReservedSize > 0)
                {
                    folder.ReservedData = new byte[folderReservedSize];
                    Array.Copy(data, dataPtr, folder.ReservedData, 0, folderReservedSize);
                    dataPtr += folderReservedSize;
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

        internal enum CompressionType : ushort
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
        internal class CFFILE
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
            /// The NULL-terminated name of this file. Note that this string can include path
            /// separator characters.The string can contain up to 256 bytes, plus the NULL byte. When the
            /// _A_NAME_IS_UTF attribute is set, this string can be converted directly to Unicode, avoiding
            /// locale-specific dependencies. When the _A_NAME_IS_UTF attribute is not set, this string is subject
            /// to interpretation depending on locale. When a string that contains Unicode characters larger than
            /// 0x007F is encoded in the szName field, the _A_NAME_IS_UTF attribute SHOULD be included in
            /// the file's attributes. When no characters larger than 0x007F are in the name, the
            /// _A_NAME_IS_UTF attribute SHOULD NOT be set. If byte values larger than 0x7F are found in
            /// CFFILE.szName field, but the _A_NAME_IS_UTF attribute is not set, the characters SHOULD be
            /// interpreted according to the current location.
            /// </summary>
            public byte[] Name { get; private set; }

            #endregion

            #region Generated Properties

            /// <summary>
            /// Name value as a string (not null-terminated)
            /// </summary>
            public string NameAsString
            {
                get
                {
                    // Perform sanity checks
                    if (Name == null || Name.Length == 0)
                        return null;

                    // Attempt to respect the attribute flag for UTF-8
                    if (Attributes.HasFlag(FileAttributes.NAME_IS_UTF))
                    {
                        try
                        {
                            return Encoding.UTF8.GetString(Name).TrimEnd('\0');
                        }
                        catch { }
                    }

                    // Default case uses local encoding
                    return Encoding.Default.GetString(Name).TrimEnd('\0');
                }
            }

            /// <summary>
            /// Convert the internal values into a DateTime object, if possible
            /// </summary>
            public DateTime DateAndTimeAsDateTime
            {
                get
                {
                    // Date property
                    int year    = (Date >> 9) + 1980;
                    int month   = (Date >> 5) & 0x0F;
                    int day     = Date & 0x1F;

                    // Time property
                    int hour    = Time >> 11;
                    int minute  = (Time >> 5) & 0x3F;
                    int second  = (Time << 1) & 0x3E;

                    return new DateTime(year, month, day, hour, minute, second);
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

                file.FileSize = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
                file.FolderStartOffset = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
                file.FolderIndex = (FolderIndex)BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
                file.Date = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
                file.Time = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
                file.Attributes = (FileAttributes)BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;

                int nullIndex = Array.IndexOf<byte>(data, 0x00, dataPtr, 0xFF);
                int stringSize = nullIndex - dataPtr;
                if (stringSize < 0 || stringSize > 255)
                    return null;

                file.Name = new byte[stringSize];
                Array.Copy(data, dataPtr, file.Name, 0, stringSize);
                dataPtr += stringSize + 1;

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
                Console.WriteLine($"        Name:               {NameAsString}");

                Console.WriteLine();
            }

            #endregion
        }

        internal enum FolderIndex : ushort
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
        internal enum FileAttributes : ushort
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
        /// CFFOLDER.coffCabStart field. Subsequent CFDATA structure records for this folder are
        /// contiguous.
        /// </summary>
        internal class CFDATA
        {
            #region Properties

            /// <summary>
            /// Checksum of this CFDATA structure, from the CFDATA.cbData through the
            /// CFDATA.ab[cbData - 1] fields.It can be set to 0 (zero) if the checksum is not supplied.
            /// </summary>
            public uint Checksum { get; private set; }

            /// <summary>
            /// Number of bytes of compressed data in this CFDATA structure record. When the
            /// cbUncomp field is zero, this field indicates only the number of bytes that fit into this cabinet file.
            /// </summary>
            public ushort CompressedSize { get; private set; }

            /// <summary>
            /// The uncompressed size of the data in this CFDATA structure entry in bytes. When this
            /// CFDATA structure entry is continued in the next cabinet file, the cbUncomp field will be zero, and
            /// the cbUncomp field in the first CFDATA structure entry in the next cabinet file will report the total
            /// uncompressed size of the data from both CFDATA structure blocks.
            /// </summary>
            public ushort UncompressedSize { get; private set; }

            /// <summary>
            /// If the CFHEADER.flags.cfhdrRESERVE_PRESENT flag is set
            /// and the cbCFData field value is non-zero, this field contains per-datablock application information.
            /// This field is defined by the application, and it is used for application-defined purposes.
            /// </summary>
            public byte[] ReservedData { get; private set; }

            /// <summary>
            /// The compressed data bytes, compressed by using the CFFOLDER.typeCompress
            /// method. When the cbUncomp field value is zero, these data bytes MUST be combined with the data
            /// bytes from the next cabinet's first CFDATA structure entry before decompression. When the
            /// CFFOLDER.typeCompress field indicates that the data is not compressed, this field contains the
            /// uncompressed data bytes. In this case, the cbData and cbUncomp field values will be equal unless
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

                dataBlock.Checksum = BitConverter.ToUInt32(data, dataPtr); dataPtr += 4;
                dataBlock.CompressedSize = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
                dataBlock.UncompressedSize = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;

                if (dataReservedSize > 0)
                {
                    dataBlock.ReservedData = new byte[dataReservedSize];
                    Array.Copy(data, dataPtr, dataBlock.ReservedData, 0, dataReservedSize);
                    dataPtr += dataReservedSize;
                }

                if (dataBlock.CompressedSize > 0)
                {
                    dataBlock.CompressedData = new byte[dataBlock.CompressedSize];
                    Array.Copy(data, dataPtr, dataBlock.CompressedData, 0, dataBlock.CompressedSize);
                    dataPtr += dataBlock.CompressedSize;
                }

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
        internal static class Checksum
        {
            // TODO: Implement from `[MS-CAB].pdf`
            //public static uint ChecksumData(byte[] data)
            //{

            //}
        }

        #endregion

        #region TEMPORARY AREA FOR MS-ZIP COMPRESSION FORMAT

        /// <summary>
        /// Each MSZIP block MUST consist of a 2-byte MSZIP signature and one or more RFC 1951 blocks. The
        /// 2-byte MSZIP signature MUST consist of the bytes 0x43 and 0x4B. The MSZIP signature MUST be
        /// the first 2 bytes in the MSZIP block.The MSZIP signature is shown in the following packet diagram.       
        /// </summary>
        internal class MSZIPBlock
        {
            #region Constants

            /// <summary>
            /// Human-readable signature
            /// </summary>
            public static readonly string SignatureString = "CK";

            /// <summary>
            /// Signature as an unsigned Int16 value
            /// </summary>
            public const ushort SignatureValue = 0x4B43;

            /// <summary>
            /// Signature as a byte array
            /// </summary>
            public static readonly byte[] SignatureBytes = new byte[] { 0x43, 0x4B };

            #endregion

            #region Properties

            /// <summary>
            /// 'CB'
            /// </summary>
            public ushort Signature { get; private set; }

            /// <summary>
            /// Each MSZIP block is the result of a single deflate compression operation, as defined in [RFC1951].
            /// The compressor that performs the compression operation MUST generate one or more RFC 1951
            /// blocks, as defined in [RFC1951]. The number, deflation mode, and type of RFC 1951 blocks in each
            /// MSZIP block is determined by the compressor, as defined in [RFC1951]. The last RFC 1951 block in
            /// each MSZIP block MUST be marked as the "end" of the stream(1), as defined by[RFC1951]
            /// section 3.2.3. Decoding trees MUST be discarded after each RFC 1951 block, but the history buffer
            /// MUST be maintained.Each MSZIP block MUST represent no more than 32 KB of uncompressed data.
            /// 
            /// The maximum compressed size of each MSZIP block is 32 KB + 12 bytes.This enables the MSZIP
            /// block to contain 32 KB of data split between two noncompressed RFC 1951 blocks, each of which
            /// has a value of BTYPE = 00.
            /// </summary>
            public byte[] Data { get; private set; }

            #endregion

            #region Serialization

            public static MSZIPBlock Deserialize(byte[] data, ref int dataPtr, int blockSize)
            {
                if (data == null || dataPtr < 0 || blockSize <= 0)
                    return null;

                MSZIPBlock block = new MSZIPBlock();

                block.Signature = BitConverter.ToUInt16(data, dataPtr); dataPtr += 2;
                if (block.Signature != SignatureValue)
                    return null;

                block.Data = new byte[blockSize];
                Array.Copy(data, dataPtr, block.Data, 0, blockSize);
                dataPtr += blockSize;

                return block;
            }

            #endregion

            #region Public Functionality

            /// <summary>
            /// Decompress a single block of MS-ZIP data
            /// </summary>
            public byte[] DecompressBlock()
            {
                if (Data == null || Data.Length == 0)
                    return null;

                try
                {
                    // Create the input objects
                    MemoryStream blockStream = new MemoryStream(Data);
                    DeflateStream deflateStream = new DeflateStream(blockStream, CompressionMode.Decompress);

                    // Create the output object
                    MemoryStream outputStream = new MemoryStream();

                    // Inflate the data
                    deflateStream.CopyTo(outputStream);

                    // Return the inflated data
                    return outputStream.ToArray();
                }
                catch
                {
                    return null;
                }
            }

            #endregion
        }

        #endregion

        #region TEMPORARY AREA FOR QUANTUM COMPRESSION FORMAT

        // See http://www.russotto.net/quantumcomp.html for details about implementation

        internal enum SelectorModel
        {
            /// <summary>
            /// Literal model, 64 entries, start at symbol 0
            /// </summary>
            SELECTOR_0 = 0,

            /// <summary>
            /// Literal model, 64 entries, start at symbol 64
            /// </summary>
            SELECTOR_1 = 1,

            /// <summary>
            /// Literal model, 64 entries, start at symbol 128
            /// </summary>
            SELECTOR_2 = 2,

            /// <summary>
            /// Literal model, 64 entries, start at symbol 192
            /// </summary>
            SELECTOR_3 = 3,

            /// <summary>
            /// LZ model, 3 character matches, max 24 entries, start at symbol 0
            /// </summary>
            SELECTOR_4 = 4,

            /// <summary>
            /// LZ model, 4 character matches, max 36 entries, start at symbol 0
            /// </summary>
            SELECTOR_5 = 5,

            /// <summary>
            /// LZ model, 5+ character matches, max 42 entries, start at symbol 0
            /// </summary>
            SELECTOR_6_POSITION = 6,

            /// <summary>
            /// LZ model, 5+ character matches, max 27 entries, start at symbol 0
            /// </summary>
            SELECTOR_6_LENGTH = 7,
        }

        #region LZ Compression Tables

        internal static readonly uint[] PositionBaseTable = new uint[]
        {
             0x00000,  0x00001, 0x00002, 0x00003, 0x00004, 0x00006, 0x00008, 0x0000c,
             0x00010,  0x00018, 0x00020, 0x00030, 0x00040, 0x00060, 0x00080, 0x000c0,
             0x00100,  0x00180, 0x00200, 0x00300, 0x00400, 0x00600, 0x00800, 0x00c00,
             0x01000,  0x01800, 0x02000, 0x03000, 0x04000, 0x06000, 0x08000, 0x0c000,
             0x10000,  0x18000, 0x20000, 0x30000, 0x40000, 0x60000, 0x80000, 0xc0000,
            0x100000, 0x180000,
        };

        internal static readonly int[] PositionExtraBitsTable = new int[]
        {
             0,  0,  0,  0,  1,  1,  2,  2,
             3,  3,  4,  4,  5,  5,  6,  6,
             7,  7,  8,  8,  9,  9, 10, 10,
            11, 11, 12, 12, 13, 13, 14, 14,
            15, 15, 16, 16, 17, 17, 18, 18,
            19, 19,
        };

        internal static readonly byte[] LengthBaseTable = new byte[]
        {
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x08,
            0x0a, 0x0c, 0x0e, 0x12, 0x16, 0x1a, 0x1e, 0x26,
            0x2e, 0x36, 0x3e, 0x4e, 0x5e, 0x6e, 0x7e, 0x9e,
            0xbe, 0xde, 0xfe
        };

        internal static readonly int[] LengthExtraBitsTable = new int[]
        {
            0, 0, 0, 0, 0, 0, 1, 1,
            1, 1, 2, 2, 2, 2, 3, 3,
            3, 3, 4, 4, 4, 4, 5, 5,
            5, 5, 0,
        };

        #endregion

        /// <summary>
        /// Number of position slots for (tsize - 10)
        /// </summary>
        internal static readonly int[] NumberOfPositionSlots = new int[]
        {
            20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42,
        };

        internal static class QuantumCompressor
        {
            // TODO: Determine how these values are set
            private static uint CS_C = 0;
            private static uint CS_H = 0;
            private static uint CS_L = 0;

            /// <summary>
            /// Get frequency from code
            /// </summary>
            public static ushort GetFrequency(ushort totfreq)
            {
                uint range = ((CS_H - CS_L) & 0xFFFF) + 1;
                uint freq = ((CS_C - CS_L + 1) * totfreq - 1) / range;
                return (ushort)(freq & 0xFFFF);
            }

            /// <summary>
            /// The decoder renormalization loop
            /// </summary>
            public static int GetCode(int cumfreqm1, int cumfreq, int totfreq)
            {
                uint range = (CS_H - CS_L) + 1;
                CS_H = CS_L + (uint)((cumfreqm1 * range) / totfreq) - 1;
                CS_L = CS_L + (uint)((cumfreq * range) / totfreq);

                while (true)
                {
                    if ((CS_L & 0x8000) != (CS_H & 0x8000))
                    {
                        if ((CS_L & 0x4000) != 0 && (CS_H & 0x4000) == 0)
                        {
                            // Underflow case
                            CS_C ^= 0x4000;
                            CS_L &= 0x3FFF;
                            CS_H |= 0x4000;
                        }
                        else
                        {
                            break;
                        }
                    }

                    CS_L <<= 1;
                    CS_H = (CS_H << 1) | 1;
                    CS_C = (CS_C << 1) | 0; // TODO: Figure out what `getbit()` is and replace the placeholder `0`
                }

                // TODO: Figure out what is supposed to return here
                return 0;
            }

            public static int GetSymbol(Model model)
            {
                int freq = GetFrequency(model.Symbols[0].CumulativeFrequency);

                int i = 1;
                for (; i < model.Entries; i++)
                {
                    if (model.Symbols[i].CumulativeFrequency <= freq)
                        break;
                }

                int sym = model.Symbols[i - 1].Symbol;

                GetCode(model.Symbols[i - 1].CumulativeFrequency, model.Symbols[i].CumulativeFrequency, model.Symbols[0].CumulativeFrequency);

                // TODO: Figure out what `update_model` does
                //update_model(model, i);

                return sym;
            }
        }

        internal class ModelSymbol
        {
            public ushort Symbol { get; private set; }

            public ushort CumulativeFrequency { get; private set; }
        }

        internal class Model
        {
            public int Entries { get; private set; }

            public ModelSymbol[] Symbols { get; private set; }
        }

        #endregion

        #region TEMPORARY AREA FOR LZX COMPRESSION FORMAT

        // See the following for details about implementation (there is no open spec):
        // https://github.com/kyz/libmspack/blob/master/libmspack/mspack/lzx.h
        // https://github.com/kyz/libmspack/blob/master/libmspack/mspack/lzxc.c
        // https://github.com/kyz/libmspack/blob/master/libmspack/mspack/lzxd.c

        #endregion
    }
}
