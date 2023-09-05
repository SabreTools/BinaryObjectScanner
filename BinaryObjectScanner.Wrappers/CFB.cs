using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class CFB : WrapperBase
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string Description => "Compact File Binary";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.CFB.FileHeader.Signature"/>
        public ulong Signature => _binary.Header.Signature;

        /// <inheritdoc cref="Models.CFB.FileHeader.CLSID"/>
        public Guid CLSID => _binary.Header.CLSID;

        /// <inheritdoc cref="Models.CFB.FileHeader.MinorVersion"/>
        public ushort MinorVersion => _binary.Header.MinorVersion;

        /// <inheritdoc cref="Models.CFB.FileHeader.MajorVersion"/>
        public ushort MajorVersion => _binary.Header.MajorVersion;

        /// <inheritdoc cref="Models.CFB.FileHeader.ByteOrder"/>
        public ushort ByteOrder => _binary.Header.ByteOrder;

        /// <inheritdoc cref="Models.CFB.FileHeader.SectorShift"/>
        public ushort SectorShift => _binary.Header.SectorShift;

        /// <inheritdoc cref="Models.CFB.FileHeader.MiniSectorShift"/>
        public ushort MiniSectorShift => _binary.Header.MiniSectorShift;

        /// <inheritdoc cref="Models.CFB.FileHeader.Reserved"/>
        public byte[] Reserved => _binary.Header.Reserved;

        /// <inheritdoc cref="Models.CFB.FileHeader.NumberOfDirectorySectors"/>
        public uint NumberOfDirectorySectors => _binary.Header.NumberOfDirectorySectors;

        /// <inheritdoc cref="Models.CFB.FileHeader.NumberOfFATSectors"/>
        public uint NumberOfFATSectors => _binary.Header.NumberOfFATSectors;

        /// <inheritdoc cref="Models.CFB.FileHeader.FirstDirectorySectorLocation"/>
        public uint FirstDirectorySectorLocation => _binary.Header.FirstDirectorySectorLocation;

        /// <inheritdoc cref="Models.CFB.FileHeader.TransactionSignatureNumber"/>
        public uint TransactionSignatureNumber => _binary.Header.TransactionSignatureNumber;

        /// <inheritdoc cref="Models.CFB.FileHeader.MiniStreamCutoffSize"/>
        public uint MiniStreamCutoffSize => _binary.Header.MiniStreamCutoffSize;

        /// <inheritdoc cref="Models.CFB.FileHeader.FirstMiniFATSectorLocation"/>
        public uint FirstMiniFATSectorLocation => _binary.Header.FirstMiniFATSectorLocation;

        /// <inheritdoc cref="Models.CFB.FileHeader.NumberOfMiniFATSectors"/>
        public uint NumberOfMiniFATSectors => _binary.Header.NumberOfMiniFATSectors;

        /// <inheritdoc cref="Models.CFB.FileHeader.FirstDIFATSectorLocation"/>
        public uint FirstDIFATSectorLocation => _binary.Header.FirstDIFATSectorLocation;

        /// <inheritdoc cref="Models.CFB.FileHeader.NumberOfDIFATSectors"/>
        public uint NumberOfDIFATSectors => _binary.Header.NumberOfDIFATSectors;

        /// <inheritdoc cref="Models.CFB.FileHeader.DIFAT"/>
        public SabreTools.Models.CFB.SectorNumber[] DIFAT => _binary.Header.DIFAT;

        #endregion

        #region FAT Sector Numbers

        /// <inheritdoc cref="Models.CFB.Binary.FATSectorNumbers"/>
        public SabreTools.Models.CFB.SectorNumber[] FATSectorNumbers => _binary.FATSectorNumbers;

        #endregion

        #region Mini FAT Sector Numbers

        /// <inheritdoc cref="Models.CFB.Binary.MiniFATSectorNumbers"/>
        public SabreTools.Models.CFB.SectorNumber[] MiniFATSectorNumbers => _binary.MiniFATSectorNumbers;

        #endregion

        #region DIFAT Sector Numbers

        /// <inheritdoc cref="Models.CFB.Binary.DIFATSectorNumbers"/>
        public SabreTools.Models.CFB.SectorNumber[] DIFATSectorNumbers => _binary.DIFATSectorNumbers;

        #endregion

        #region Directory Entries

        /// <inheritdoc cref="Models.CFB.Binary.DirectoryEntries"/>
        public SabreTools.Models.CFB.DirectoryEntry[] DirectoryEntries => _binary.DirectoryEntries;

        #endregion

        #endregion

        #region Extension Properties

        /// <summary>
        /// Normal sector size in bytes
        /// </summary>
        public long SectorSize => (long)Math.Pow(2, SectorShift);

        /// <summary>
        /// Mini sector size in bytes
        /// </summary>
        public long MiniSectorSize => (long)Math.Pow(2, MiniSectorShift);

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the file
        /// </summary>
        private SabreTools.Models.CFB.Binary _binary;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private CFB() { }

        /// <summary>
        /// Create a Compound File Binary from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A Compound File Binary wrapper on success, null on failure</returns>
        public static CFB Create(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a Compound File Binary from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A Compound File Binary wrapper on success, null on failure</returns>
        public static CFB Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var binary = Builders.CFB.ParseBinary(data);
            if (binary == null)
                return null;

            var wrapper = new CFB
            {
                _binary = binary,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region FAT Sector Data

        /// <summary>
        /// Get the ordered FAT sector chain for a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        public List<SabreTools.Models.CFB.SectorNumber> GetFATSectorChain(SabreTools.Models.CFB.SectorNumber startingSector)
        {
            // If we have an invalid sector
            if (startingSector < 0 || (long)startingSector >= FATSectorNumbers.Length)
                return null;

            // Setup the returned list
            var sectors = new List<SabreTools.Models.CFB.SectorNumber> { startingSector };

            var lastSector = startingSector;
            while (true)
            {
                // Get the next sector from the lookup table
                var nextSector = FATSectorNumbers[(uint)lastSector];

                // If we have an end of chain or free sector
                if (nextSector == SabreTools.Models.CFB.SectorNumber.ENDOFCHAIN || nextSector == SabreTools.Models.CFB.SectorNumber.FREESECT)
                    break;

                // Add the next sector to the list and replace the last sector
                sectors.Add(nextSector);
                lastSector = nextSector;
            }

            return sectors;
        }

        /// <summary>
        /// Get the data for the FAT sector chain starting at a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        public byte[] GetFATSectorChainData(SabreTools.Models.CFB.SectorNumber startingSector)
        {
            // Get the sector chain first
            var sectorChain = GetFATSectorChain(startingSector);
            if (sectorChain == null)
                return null;

            // Sequentially read the sectors
            var data = new List<byte>();
            for (int i = 0; i < sectorChain.Count; i++)
            {
                // Try to get the sector data offset
                int sectorDataOffset = (int)FATSectorToFileOffset(sectorChain[i]);
                if (sectorDataOffset < 0 || sectorDataOffset >= GetEndOfFile())
                    return null;

                // Try to read the sector data
                var sectorData = ReadFromDataSource(sectorDataOffset, (int)SectorSize);
                if (sectorData == null)
                    return null;

                // Add the sector data to the output
                data.AddRange(sectorData);
            }

            return data.ToArray();
        }

        /// <summary>
        /// Convert a FAT sector value to a byte offset
        /// </summary>
        /// <param name="sector">Sector to convert</param>
        /// <returns>File offset in bytes, -1 on error</returns>
        public long FATSectorToFileOffset(SabreTools.Models.CFB.SectorNumber sector)
        {
            // If we have an invalid sector number
            if (sector > SabreTools.Models.CFB.SectorNumber.MAXREGSECT)
                return -1;

            // Convert based on the sector shift value
            return (long)(sector + 1) * SectorSize;
        }

        #endregion

        #region Mini FAT Sector Data

        /// <summary>
        /// Get the ordered Mini FAT sector chain for a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial Mini FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        public List<SabreTools.Models.CFB.SectorNumber> GetMiniFATSectorChain(SabreTools.Models.CFB.SectorNumber startingSector)
        {
            // If we have an invalid sector
            if (startingSector < 0 || (long)startingSector >= MiniFATSectorNumbers.Length)
                return null;

            // Setup the returned list
            var sectors = new List<SabreTools.Models.CFB.SectorNumber> { startingSector };

            var lastSector = startingSector;
            while (true)
            {
                // Get the next sector from the lookup table
                var nextSector = MiniFATSectorNumbers[(uint)lastSector];

                // If we have an end of chain or free sector
                if (nextSector == SabreTools.Models.CFB.SectorNumber.ENDOFCHAIN || nextSector == SabreTools.Models.CFB.SectorNumber.FREESECT)
                    break;

                // Add the next sector to the list and replace the last sector
                sectors.Add(nextSector);
                lastSector = nextSector;
            }

            return sectors;
        }

        /// <summary>
        /// Get the data for the Mini FAT sector chain starting at a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial Mini FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        public byte[] GetMiniFATSectorChainData(SabreTools.Models.CFB.SectorNumber startingSector)
        {
            // Get the sector chain first
            var sectorChain = GetMiniFATSectorChain(startingSector);
            if (sectorChain == null)
                return null;

            // Sequentially read the sectors
            var data = new List<byte>();
            for (int i = 0; i < sectorChain.Count; i++)
            {
                // Try to get the sector data offset
                int sectorDataOffset = (int)MiniFATSectorToFileOffset(sectorChain[i]);
                if (sectorDataOffset < 0 || sectorDataOffset >= GetEndOfFile())
                    return null;

                // Try to read the sector data
                var sectorData = ReadFromDataSource(sectorDataOffset, (int)MiniSectorSize);
                if (sectorData == null)
                    return null;

                // Add the sector data to the output
                data.AddRange(sectorData);
            }

            return data.ToArray();
        }

        /// <summary>
        /// Convert a Mini FAT sector value to a byte offset
        /// </summary>
        /// <param name="sector">Sector to convert</param>
        /// <returns>File offset in bytes, -1 on error</returns>
        public long MiniFATSectorToFileOffset(SabreTools.Models.CFB.SectorNumber sector)
        {
            // If we have an invalid sector number
            if (sector > SabreTools.Models.CFB.SectorNumber.MAXREGSECT)
                return -1;

            // Convert based on the sector shift value
            return (long)(sector + 1) * MiniSectorSize;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();
            
            builder.AppendLine("Compound File Binary Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintFileHeader(builder);
            PrintFATSectorNumbers(builder);
            PrintMiniFATSectorNumbers(builder);
            PrintDIFATSectorNumbers(builder);
            PrintDirectoryEntries(builder);

            return builder;
        }

        /// <summary>
        /// Print header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFileHeader(StringBuilder builder)
        {
            builder.AppendLine("  File Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Signature: {Signature} (0x{Signature:X})");
            builder.AppendLine($"  CLSID: {CLSID}");
            builder.AppendLine($"  Minor version: {MinorVersion} (0x{MinorVersion:X})");
            builder.AppendLine($"  Major version: {MajorVersion} (0x{MajorVersion:X})");
            builder.AppendLine($"  Byte order: {ByteOrder} (0x{ByteOrder:X})");
            builder.AppendLine($"  Sector shift: {SectorShift} (0x{SectorShift:X}) => {SectorSize}");
            builder.AppendLine($"  Mini sector shift: {MiniSectorShift} (0x{MiniSectorShift:X}) => {MiniSectorSize}");
            builder.AppendLine($"  Reserved: {BitConverter.ToString(Reserved).Replace('-', ' ')}");
            builder.AppendLine($"  Number of directory sectors: {NumberOfDirectorySectors} (0x{NumberOfDirectorySectors:X})");
            builder.AppendLine($"  Number of FAT sectors: {NumberOfFATSectors} (0x{NumberOfFATSectors:X})");
            builder.AppendLine($"  First directory sector location: {FirstDirectorySectorLocation} (0x{FirstDirectorySectorLocation:X})");
            builder.AppendLine($"  Transaction signature number: {TransactionSignatureNumber} (0x{TransactionSignatureNumber:X})");
            builder.AppendLine($"  Mini stream cutoff size: {MiniStreamCutoffSize} (0x{MiniStreamCutoffSize:X})");
            builder.AppendLine($"  First mini FAT sector location: {FirstMiniFATSectorLocation} (0x{FirstMiniFATSectorLocation:X})");
            builder.AppendLine($"  Number of mini FAT sectors: {NumberOfMiniFATSectors} (0x{NumberOfMiniFATSectors:X})");
            builder.AppendLine($"  First DIFAT sector location: {FirstDIFATSectorLocation} (0x{FirstDIFATSectorLocation:X})");
            builder.AppendLine($"  Number of DIFAT sectors: {NumberOfDIFATSectors} (0x{NumberOfDIFATSectors:X})");
            builder.AppendLine($"  DIFAT:");
            for (int i = 0; i < DIFAT.Length; i++)
            {
                builder.AppendLine($"    DIFAT Entry {i}: {DIFAT[i]} (0x{DIFAT[i]:X})");
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print FAT sector numbers
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFATSectorNumbers(StringBuilder builder)
        {
            builder.AppendLine("  FAT Sectors Information:");
            builder.AppendLine("  -------------------------");
            if (FATSectorNumbers == null || FATSectorNumbers.Length == 0)
            {
                builder.AppendLine("  No FAT sectors");
            }
            else
            {
                for (int i = 0; i < FATSectorNumbers.Length; i++)
                {
                    builder.AppendLine($"  FAT Sector Entry {i}: {FATSectorNumbers[i]} (0x{FATSectorNumbers[i]:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print mini FAT sector numbers
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintMiniFATSectorNumbers(StringBuilder builder)
        {
            builder.AppendLine("  Mini FAT Sectors Information:");
            builder.AppendLine("  -------------------------");
            if (MiniFATSectorNumbers == null || MiniFATSectorNumbers.Length == 0)
            {
                builder.AppendLine("  No mini FAT sectors");
            }
            else
            {
                for (int i = 0; i < MiniFATSectorNumbers.Length; i++)
                {
                    builder.AppendLine($"  Mini FAT Sector Entry {i}: {MiniFATSectorNumbers[i]} (0x{MiniFATSectorNumbers[i]:X})");
                }
            }
            builder.AppendLine();
        }

        /// <summary>
        /// Print DIFAT sector numbers
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDIFATSectorNumbers(StringBuilder builder)
        {
            builder.AppendLine("  DIFAT Sectors Information:");
            builder.AppendLine("  -------------------------");
            if (DIFATSectorNumbers == null || DIFATSectorNumbers.Length == 0)
            {
                builder.AppendLine("  No DIFAT sectors");
            }
            else
            {
                for (int i = 0; i < DIFATSectorNumbers.Length; i++)
                {
                    builder.AppendLine($"  DIFAT Sector Entry {i}: {DIFATSectorNumbers[i]} (0x{DIFATSectorNumbers[i]:X})");
                }
            }
            builder.AppendLine();
        }

        // <summary>
        /// Print directory entries
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDirectoryEntries(StringBuilder builder)
        {
            builder.AppendLine("  Directory Entries Information:");
            builder.AppendLine("  -------------------------");
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
            {
                builder.AppendLine("  No directory entries");
            }
            else
            {
                for (int i = 0; i < DirectoryEntries.Length; i++)
                {
                    var directoryEntry = DirectoryEntries[i];
                    builder.AppendLine($"  Directory Entry {i}");
                    builder.AppendLine($"    Name: {directoryEntry.Name}");
                    builder.AppendLine($"    Name length: {directoryEntry.NameLength} (0x{directoryEntry.NameLength:X})");
                    builder.AppendLine($"    Object type: {directoryEntry.ObjectType} (0x{directoryEntry.ObjectType:X})");
                    builder.AppendLine($"    Color flag: {directoryEntry.ColorFlag} (0x{directoryEntry.ColorFlag:X})");
                    builder.AppendLine($"    Left sibling ID: {directoryEntry.LeftSiblingID} (0x{directoryEntry.LeftSiblingID:X})");
                    builder.AppendLine($"    Right sibling ID: {directoryEntry.RightSiblingID} (0x{directoryEntry.RightSiblingID:X})");
                    builder.AppendLine($"    Child ID: {directoryEntry.ChildID} (0x{directoryEntry.ChildID:X})");
                    builder.AppendLine($"    CLSID: {directoryEntry.CLSID}");
                    builder.AppendLine($"    State bits: {directoryEntry.StateBits} (0x{directoryEntry.StateBits:X})");
                    builder.AppendLine($"    Creation time: {directoryEntry.CreationTime} (0x{directoryEntry.CreationTime:X})");
                    builder.AppendLine($"    Modification time: {directoryEntry.ModifiedTime} (0x{directoryEntry.ModifiedTime:X})");
                    builder.AppendLine($"    Staring sector location: {directoryEntry.StartingSectorLocation} (0x{directoryEntry.StartingSectorLocation:X})");
                    builder.AppendLine($"    Stream size: {directoryEntry.StreamSize} (0x{directoryEntry.StreamSize:X})");
                }
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_binary, _jsonSerializerOptions);

#endif

        #endregion
    }
}