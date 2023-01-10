using System;
using System.Collections.Generic;
using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class CFB : WrapperBase
    {
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
        public Models.CFB.SectorNumber[] DIFAT => _binary.Header.DIFAT;

        #endregion

        #region FAT Sector Numbers

        /// <inheritdoc cref="Models.CFB.Binary.FATSectorNumbers"/>
        public Models.CFB.SectorNumber[] FATSectorNumbers => _binary.FATSectorNumbers;

        #endregion

        #region Mini FAT Sector Numbers

        /// <inheritdoc cref="Models.CFB.Binary.MiniFATSectorNumbers"/>
        public Models.CFB.SectorNumber[] MiniFATSectorNumbers => _binary.MiniFATSectorNumbers;

        #endregion

        #region DIFAT Sector Numbers

        /// <inheritdoc cref="Models.CFB.Binary.DIFATSectorNumbers"/>
        public Models.CFB.SectorNumber[] DIFATSectorNumbers => _binary.DIFATSectorNumbers;

        #endregion

        #region Directory Entries

        /// <inheritdoc cref="Models.CFB.Binary.DirectoryEntries"/>
        public Models.CFB.DirectoryEntry[] DirectoryEntries => _binary.DirectoryEntries;

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the file
        /// </summary>
        private Models.CFB.Binary _binary;

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

        #region Data

        /// <summary>
        /// Get the ordered FAT sector chain for a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        public List<Models.CFB.SectorNumber> GetFATSectorChain(Models.CFB.SectorNumber startingSector)
        {
            // If we have an invalid sector
            if (startingSector < 0 || (long)startingSector >= FATSectorNumbers.Length)
                return null;

            // Setup the returned list
            var sectors = new List<Models.CFB.SectorNumber> { startingSector };

            var lastSector = startingSector;
            while (true)
            {
                // Get the next sector from the lookup table
                var nextSector = FATSectorNumbers[(uint)lastSector];

                // If we have an end of chain or free sector
                if (nextSector == Models.CFB.SectorNumber.ENDOFCHAIN || nextSector == Models.CFB.SectorNumber.FREESECT)
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
        public byte[] GetFATSectorChainData(Models.CFB.SectorNumber startingSector)
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
                int sectorDataOffset = (int)ToFileOffset(sectorChain[i]);
                if (sectorDataOffset < 0 || sectorDataOffset >= GetEndOfFile())
                    return null;

                // Try to read the sector data
                int sectorDataLength = (int)Math.Pow(2, SectorShift);
                var sectorData = ReadFromDataSource(sectorDataOffset, sectorDataLength);
                if (sectorData == null)
                    return null;

                // Add the sector data to the output
                data.AddRange(sectorData);
            }

            return data.ToArray();
        }

        /// <summary>
        /// Get the ordered Mini FAT sector chain for a given starting sector
        /// </summary>
        /// <param name="startingSector">Initial Mini FAT sector</param>
        /// <returns>Ordered list of sector numbers, null on error</returns>
        public List<Models.CFB.SectorNumber> GetMiniFATSectorChain(Models.CFB.SectorNumber startingSector)
        {
            // If we have an invalid sector
            if (startingSector < 0 || (long)startingSector >= MiniFATSectorNumbers.Length)
                return null;

            // Setup the returned list
            var sectors = new List<Models.CFB.SectorNumber> { startingSector };

            var lastSector = startingSector;
            while (true)
            {
                // Get the next sector from the lookup table
                var nextSector = MiniFATSectorNumbers[(uint)lastSector];

                // If we have an end of chain or free sector
                if (nextSector == Models.CFB.SectorNumber.ENDOFCHAIN || nextSector == Models.CFB.SectorNumber.FREESECT)
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
        public byte[] GetMiniFATSectorChainData(Models.CFB.SectorNumber startingSector)
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
                int sectorDataOffset = (int)ToMiniFileOffset(sectorChain[i]);
                if (sectorDataOffset < 0 || sectorDataOffset >= GetEndOfFile())
                    return null;

                // Try to read the sector data
                int sectorDataLength = (int)Math.Pow(2, MiniSectorShift);
                var sectorData = ReadFromDataSource(sectorDataOffset, sectorDataLength);
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
        public long ToFileOffset(Models.CFB.SectorNumber sector)
        {
            // If we have an invalid sector number
            if (sector > Models.CFB.SectorNumber.MAXREGSECT)
                return -1;

            // Convert based on the sector shift value
            return (long)((long)(sector + 1) * Math.Pow(2, SectorShift));
        }

        /// <summary>
        /// Convert a Mini FAT sector value to a byte offset
        /// </summary>
        /// <param name="sector">Sector to convert</param>
        /// <returns>File offset in bytes, -1 on error</returns>
        public long ToMiniFileOffset(Models.CFB.SectorNumber sector)
        {
            // If we have an invalid sector number
            if (sector > Models.CFB.SectorNumber.MAXREGSECT)
                return -1;

            // Convert based on the sector shift value
            return (long)((long)(sector + 1) * Math.Pow(2, MiniSectorShift));
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override void Print()
        {
            Console.WriteLine("Compound File Binary Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            PrintFileHeader();
            PrintFATSectorNumbers();
            PrintMiniFATSectorNumbers();
            PrintDIFATSectorNumbers();
            PrintDirectoryEntries();
        }

        /// <summary>
        /// Print header information
        /// </summary>
        private void PrintFileHeader()
        {
            Console.WriteLine("  File Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Signature: {Signature}");
            Console.WriteLine($"  CLSID: {CLSID}");
            Console.WriteLine($"  Minor version: {MinorVersion}");
            Console.WriteLine($"  Major version: {MajorVersion}");
            Console.WriteLine($"  Byte order: {ByteOrder}");
            Console.WriteLine($"  Sector shift: {SectorShift} [{(long)Math.Pow(2, SectorShift)}]");
            Console.WriteLine($"  Mini sector shift: {MiniSectorShift} [{(long)Math.Pow(2, MiniSectorShift)}]");
            Console.WriteLine($"  Reserved: {BitConverter.ToString(Reserved).Replace('-', ' ')}");
            Console.WriteLine($"  Number of directory sectors: {NumberOfDirectorySectors}");
            Console.WriteLine($"  Number of FAT sectors: {NumberOfFATSectors}");
            Console.WriteLine($"  First directory sector location: {FirstDirectorySectorLocation}");
            Console.WriteLine($"  Transaction signature number: {TransactionSignatureNumber}");
            Console.WriteLine($"  Mini stream cutoff size: {MiniStreamCutoffSize}");
            Console.WriteLine($"  First mini FAT sector location: {FirstMiniFATSectorLocation}");
            Console.WriteLine($"  Number of mini FAT sectors: {NumberOfMiniFATSectors}");
            Console.WriteLine($"  First DIFAT sector location: {FirstDIFATSectorLocation}");
            Console.WriteLine($"  Number of DIFAT sectors: {NumberOfDIFATSectors}");
            Console.WriteLine($"  DIFAT:");
            for (int i = 0; i < DIFAT.Length; i++)
            {
                Console.WriteLine($"    DIFAT Entry {i}: {DIFAT[i]}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print FAT sector numbers
        /// </summary>
        private void PrintFATSectorNumbers()
        {
            Console.WriteLine("  FAT Sectors Information:");
            Console.WriteLine("  -------------------------");
            if (FATSectorNumbers == null || FATSectorNumbers.Length == 0)
            {
                Console.WriteLine("  No FAT sectors");
            }
            else
            {
                for (int i = 0; i < FATSectorNumbers.Length; i++)
                {
                    Console.WriteLine($"  FAT Sector Entry {i}: {FATSectorNumbers[i]}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print mini FAT sector numbers
        /// </summary>
        private void PrintMiniFATSectorNumbers()
        {
            Console.WriteLine("  Mini FAT Sectors Information:");
            Console.WriteLine("  -------------------------");
            if (MiniFATSectorNumbers == null || MiniFATSectorNumbers.Length == 0)
            {
                Console.WriteLine("  No mini FAT sectors");
            }
            else
            {
                for (int i = 0; i < MiniFATSectorNumbers.Length; i++)
                {
                    Console.WriteLine($"  Mini FAT Sector Entry {i}: {MiniFATSectorNumbers[i]}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print DIFAT sector numbers
        /// </summary>
        private void PrintDIFATSectorNumbers()
        {
            Console.WriteLine("  DIFAT Sectors Information:");
            Console.WriteLine("  -------------------------");
            if (DIFATSectorNumbers == null || DIFATSectorNumbers.Length == 0)
            {
                Console.WriteLine("  No DIFAT sectors");
            }
            else
            {
                for (int i = 0; i < DIFATSectorNumbers.Length; i++)
                {
                    Console.WriteLine($"  DIFAT Sector Entry {i}: {DIFATSectorNumbers[i]}");
                }
            }
            Console.WriteLine();
        }

        // <summary>
        /// Print directory entries
        /// </summary>
        private void PrintDirectoryEntries()
        {
            Console.WriteLine("  Directory Entries Information:");
            Console.WriteLine("  -------------------------");
            if (DirectoryEntries == null || DirectoryEntries.Length == 0)
            {
                Console.WriteLine("  No directory entries");
            }
            else
            {
                for (int i = 0; i < DirectoryEntries.Length; i++)
                {
                    var directoryEntry = DirectoryEntries[i];
                    Console.WriteLine($"  Directory Entry {i}");
                    Console.WriteLine($"    Name: {directoryEntry.Name}");
                    Console.WriteLine($"    Name length: {directoryEntry.NameLength}");
                    Console.WriteLine($"    Object type: {directoryEntry.ObjectType}");
                    Console.WriteLine($"    Color flag: {directoryEntry.ColorFlag}");
                    Console.WriteLine($"    Left sibling ID: {directoryEntry.LeftSiblingID}");
                    Console.WriteLine($"    Right sibling ID: {directoryEntry.RightSiblingID}");
                    Console.WriteLine($"    Child ID: {directoryEntry.ChildID}");
                    Console.WriteLine($"    CLSID: {directoryEntry.CLSID}");
                    Console.WriteLine($"    State bits: {directoryEntry.StateBits}");
                    Console.WriteLine($"    Creation time: {directoryEntry.CreationTime}");
                    Console.WriteLine($"    Modification time: {directoryEntry.ModifiedTime}");
                    Console.WriteLine($"    Staring sector location: {directoryEntry.StartingSectorLocation}");
                    Console.WriteLine($"    Stream size: {directoryEntry.StreamSize}");
                }
            }
            Console.WriteLine();
        }

        #endregion
    }
}