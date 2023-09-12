using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class CFB : WrapperBase<SabreTools.Models.CFB.Binary>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Compact File Binary";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.CFB.FileHeader.Signature"/>
        public ulong Signature => _model.Header.Signature;

        /// <inheritdoc cref="Models.CFB.FileHeader.CLSID"/>
        public Guid CLSID => _model.Header.CLSID;

        /// <inheritdoc cref="Models.CFB.FileHeader.MinorVersion"/>
        public ushort MinorVersion => _model.Header.MinorVersion;

        /// <inheritdoc cref="Models.CFB.FileHeader.MajorVersion"/>
        public ushort MajorVersion => _model.Header.MajorVersion;

        /// <inheritdoc cref="Models.CFB.FileHeader.ByteOrder"/>
        public ushort ByteOrder => _model.Header.ByteOrder;

        /// <inheritdoc cref="Models.CFB.FileHeader.SectorShift"/>
        public ushort SectorShift => _model.Header.SectorShift;

        /// <inheritdoc cref="Models.CFB.FileHeader.MiniSectorShift"/>
        public ushort MiniSectorShift => _model.Header.MiniSectorShift;

        /// <inheritdoc cref="Models.CFB.FileHeader.Reserved"/>
        public byte[] Reserved => _model.Header.Reserved;

        /// <inheritdoc cref="Models.CFB.FileHeader.NumberOfDirectorySectors"/>
        public uint NumberOfDirectorySectors => _model.Header.NumberOfDirectorySectors;

        /// <inheritdoc cref="Models.CFB.FileHeader.NumberOfFATSectors"/>
        public uint NumberOfFATSectors => _model.Header.NumberOfFATSectors;

        /// <inheritdoc cref="Models.CFB.FileHeader.FirstDirectorySectorLocation"/>
        public uint FirstDirectorySectorLocation => _model.Header.FirstDirectorySectorLocation;

        /// <inheritdoc cref="Models.CFB.FileHeader.TransactionSignatureNumber"/>
        public uint TransactionSignatureNumber => _model.Header.TransactionSignatureNumber;

        /// <inheritdoc cref="Models.CFB.FileHeader.MiniStreamCutoffSize"/>
        public uint MiniStreamCutoffSize => _model.Header.MiniStreamCutoffSize;

        /// <inheritdoc cref="Models.CFB.FileHeader.FirstMiniFATSectorLocation"/>
        public uint FirstMiniFATSectorLocation => _model.Header.FirstMiniFATSectorLocation;

        /// <inheritdoc cref="Models.CFB.FileHeader.NumberOfMiniFATSectors"/>
        public uint NumberOfMiniFATSectors => _model.Header.NumberOfMiniFATSectors;

        /// <inheritdoc cref="Models.CFB.FileHeader.FirstDIFATSectorLocation"/>
        public uint FirstDIFATSectorLocation => _model.Header.FirstDIFATSectorLocation;

        /// <inheritdoc cref="Models.CFB.FileHeader.NumberOfDIFATSectors"/>
        public uint NumberOfDIFATSectors => _model.Header.NumberOfDIFATSectors;

        /// <inheritdoc cref="Models.CFB.FileHeader.DIFAT"/>
#if NET48
        public SabreTools.Models.CFB.SectorNumber[] DIFAT => _model.Header.DIFAT;
#else
        public SabreTools.Models.CFB.SectorNumber?[] DIFAT => _model.Header.DIFAT;
#endif

        #endregion

        #region FAT Sector Numbers

        /// <inheritdoc cref="Models.CFB.Binary.FATSectorNumbers"/>
#if NET48
        public SabreTools.Models.CFB.SectorNumber[] FATSectorNumbers => _model.FATSectorNumbers;
#else
        public SabreTools.Models.CFB.SectorNumber?[] FATSectorNumbers => _model.FATSectorNumbers;
#endif

        #endregion

        #region Mini FAT Sector Numbers

        /// <inheritdoc cref="Models.CFB.Binary.MiniFATSectorNumbers"/>
#if NET48
        public SabreTools.Models.CFB.SectorNumber[] MiniFATSectorNumbers => _model.MiniFATSectorNumbers;
#else
        public SabreTools.Models.CFB.SectorNumber?[] MiniFATSectorNumbers => _model.MiniFATSectorNumbers;
#endif

        #endregion

        #region DIFAT Sector Numbers

        /// <inheritdoc cref="Models.CFB.Binary.DIFATSectorNumbers"/>
#if NET48
        public SabreTools.Models.CFB.SectorNumber[] DIFATSectorNumbers => _model.DIFATSectorNumbers;
#else
        public SabreTools.Models.CFB.SectorNumber?[] DIFATSectorNumbers => _model.DIFATSectorNumbers;
#endif

        #endregion

        #region Directory Entries

        /// <inheritdoc cref="Models.CFB.Binary.DirectoryEntries"/>
#if NET48
        public SabreTools.Models.CFB.DirectoryEntry[] DirectoryEntries => _model.DirectoryEntries;
#else
        public SabreTools.Models.CFB.DirectoryEntry?[] DirectoryEntries => _model.DirectoryEntries;
#endif

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

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public CFB(SabreTools.Models.CFB.Binary model, byte[] data, int offset)
#else
        public CFB(SabreTools.Models.CFB.Binary? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public CFB(SabreTools.Models.CFB.Binary model, Stream data)
#else
        public CFB(SabreTools.Models.CFB.Binary? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

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

            var binary = new SabreTools.Serialization.Streams.CFB().Deserialize(data);
            if (binary == null)
                return null;

            try
            {
                return new CFB(binary, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region FAT Sector Data

        /// <summary>
        /// Get the ordered FAT sector chain for a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
#if NET48
        public List<SabreTools.Models.CFB.SectorNumber> GetFATSectorChain(SabreTools.Models.CFB.SectorNumber startingSector)
#else
        public List<SabreTools.Models.CFB.SectorNumber?> GetFATSectorChain(SabreTools.Models.CFB.SectorNumber? startingSector)
#endif
        {
            // If we have an invalid sector
            if (startingSector < 0 || (long)startingSector >= FATSectorNumbers.Length)
                return null;

            // Setup the returned list
#if NET48
            var sectors = new List<SabreTools.Models.CFB.SectorNumber> { startingSector };
#else
            var sectors = new List<SabreTools.Models.CFB.SectorNumber?> { startingSector };
#endif

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
#if NET48
        public long FATSectorToFileOffset(SabreTools.Models.CFB.SectorNumber sector)
#else
        public long FATSectorToFileOffset(SabreTools.Models.CFB.SectorNumber? sector)
#endif
        {
            // If we have an invalid sector number
            if (sector == null || sector > SabreTools.Models.CFB.SectorNumber.MAXREGSECT)
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
#if NET48
        public List<SabreTools.Models.CFB.SectorNumber> GetMiniFATSectorChain(SabreTools.Models.CFB.SectorNumber startingSector)
#else
        public List<SabreTools.Models.CFB.SectorNumber?> GetMiniFATSectorChain(SabreTools.Models.CFB.SectorNumber? startingSector)
#endif
        {
            // If we have an invalid sector
            if (startingSector < 0 || (long)startingSector >= MiniFATSectorNumbers.Length)
                return null;

            // Setup the returned list
        #if NET48
            var sectors = new List<SabreTools.Models.CFB.SectorNumber> { startingSector };
        #else
            var sectors = new List<SabreTools.Models.CFB.SectorNumber?> { startingSector };
        #endif

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
#if NET48
        public long MiniFATSectorToFileOffset(SabreTools.Models.CFB.SectorNumber sector)
#else
        public long MiniFATSectorToFileOffset(SabreTools.Models.CFB.SectorNumber? sector)
#endif
        {
            // If we have an invalid sector number
            if (sector == null || sector > SabreTools.Models.CFB.SectorNumber.MAXREGSECT)
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
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}