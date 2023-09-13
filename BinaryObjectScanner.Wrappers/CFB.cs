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
#if NET48
        public ulong Signature => _model.Header.Signature;
#else
        public ulong? Signature => _model.Header?.Signature;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.CLSID"/>
#if NET48
        public Guid CLSID => _model.Header.CLSID;
#else
        public Guid? CLSID => _model.Header?.CLSID;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.MinorVersion"/>
#if NET48
        public ushort MinorVersion => _model.Header.MinorVersion;
#else
        public ushort? MinorVersion => _model.Header?.MinorVersion;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.MajorVersion"/>
#if NET48
        public ushort MajorVersion => _model.Header.MajorVersion;
#else
        public ushort? MajorVersion => _model.Header?.MajorVersion;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.ByteOrder"/>
#if NET48
        public ushort ByteOrder => _model.Header.ByteOrder;
#else
        public ushort? ByteOrder => _model.Header?.ByteOrder;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.SectorShift"/>
#if NET48
        public ushort SectorShift => _model.Header.SectorShift;
#else
        public ushort? SectorShift => _model.Header?.SectorShift;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.MiniSectorShift"/>
#if NET48
        public ushort MiniSectorShift => _model.Header.MiniSectorShift;
#else
        public ushort? MiniSectorShift => _model.Header?.MiniSectorShift;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.Reserved"/>
#if NET48
        public byte[] Reserved => _model.Header.Reserved;
#else
        public byte[]? Reserved => _model.Header?.Reserved;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.NumberOfDirectorySectors"/>
#if NET48
        public uint NumberOfDirectorySectors => _model.Header.NumberOfDirectorySectors;
#else
        public uint? NumberOfDirectorySectors => _model.Header?.NumberOfDirectorySectors;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.NumberOfFATSectors"/>
#if NET48
        public uint NumberOfFATSectors => _model.Header.NumberOfFATSectors;
#else
        public uint? NumberOfFATSectors => _model.Header?.NumberOfFATSectors;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.FirstDirectorySectorLocation"/>
#if NET48
        public uint FirstDirectorySectorLocation => _model.Header.FirstDirectorySectorLocation;
#else
        public uint? FirstDirectorySectorLocation => _model.Header?.FirstDirectorySectorLocation;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.TransactionSignatureNumber"/>
#if NET48
        public uint TransactionSignatureNumber => _model.Header.TransactionSignatureNumber;
#else
        public uint? TransactionSignatureNumber => _model.Header?.TransactionSignatureNumber;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.MiniStreamCutoffSize"/>
#if NET48
        public uint MiniStreamCutoffSize => _model.Header.MiniStreamCutoffSize;
#else
        public uint? MiniStreamCutoffSize => _model.Header?.MiniStreamCutoffSize;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.FirstMiniFATSectorLocation"/>
#if NET48
        public uint FirstMiniFATSectorLocation => _model.Header.FirstMiniFATSectorLocation;
#else
        public uint? FirstMiniFATSectorLocation => _model.Header?.FirstMiniFATSectorLocation;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.NumberOfMiniFATSectors"/>
#if NET48
        public uint NumberOfMiniFATSectors => _model.Header.NumberOfMiniFATSectors;
#else
        public uint? NumberOfMiniFATSectors => _model.Header?.NumberOfMiniFATSectors;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.FirstDIFATSectorLocation"/>
#if NET48
        public uint FirstDIFATSectorLocation => _model.Header.FirstDIFATSectorLocation;
#else
        public uint? FirstDIFATSectorLocation => _model.Header?.FirstDIFATSectorLocation;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.NumberOfDIFATSectors"/>
#if NET48
        public uint NumberOfDIFATSectors => _model.Header.NumberOfDIFATSectors;
#else
        public uint? NumberOfDIFATSectors => _model.Header?.NumberOfDIFATSectors;
#endif

        /// <inheritdoc cref="Models.CFB.FileHeader.DIFAT"/>
#if NET48
        public SabreTools.Models.CFB.SectorNumber[] DIFAT => _model.Header.DIFAT;
#else
        public SabreTools.Models.CFB.SectorNumber?[]? DIFAT => _model.Header?.DIFAT;
#endif

        #endregion

        #region FAT Sector Numbers

        /// <inheritdoc cref="Models.CFB.Binary.FATSectorNumbers"/>
#if NET48
        public SabreTools.Models.CFB.SectorNumber[] FATSectorNumbers => _model.FATSectorNumbers;
#else
        public SabreTools.Models.CFB.SectorNumber?[]? FATSectorNumbers => _model.FATSectorNumbers;
#endif

        #endregion

        #region Mini FAT Sector Numbers

        /// <inheritdoc cref="Models.CFB.Binary.MiniFATSectorNumbers"/>
#if NET48
        public SabreTools.Models.CFB.SectorNumber[] MiniFATSectorNumbers => _model.MiniFATSectorNumbers;
#else
        public SabreTools.Models.CFB.SectorNumber?[]? MiniFATSectorNumbers => _model.MiniFATSectorNumbers;
#endif

        #endregion

        #region DIFAT Sector Numbers

        /// <inheritdoc cref="Models.CFB.Binary.DIFATSectorNumbers"/>
#if NET48
        public SabreTools.Models.CFB.SectorNumber[] DIFATSectorNumbers => _model.DIFATSectorNumbers;
#else
        public SabreTools.Models.CFB.SectorNumber?[]? DIFATSectorNumbers => _model.DIFATSectorNumbers;
#endif

        #endregion

        #region Directory Entries

        /// <inheritdoc cref="Models.CFB.Binary.DirectoryEntries"/>
#if NET48
        public SabreTools.Models.CFB.DirectoryEntry[] DirectoryEntries => _model.DirectoryEntries;
#else
        public SabreTools.Models.CFB.DirectoryEntry?[]? DirectoryEntries => _model.DirectoryEntries;
#endif

        #endregion

        #endregion

        #region Extension Properties

        /// <summary>
        /// Normal sector size in bytes
        /// </summary>
#if NET48
        public long SectorSize => (long)Math.Pow(2, SectorShift);
#else
        public long SectorSize => (long)Math.Pow(2, SectorShift ?? 0);
#endif

        /// <summary>
        /// Mini sector size in bytes
        /// </summary>
#if NET48
        public long MiniSectorSize => (long)Math.Pow(2, MiniSectorShift);
#else
        public long MiniSectorSize => (long)Math.Pow(2, MiniSectorShift ?? 0);
#endif

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
#if NET48
        public static CFB Create(byte[] data, int offset)
#else
        public static CFB? Create(byte[]? data, int offset)
#endif
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
#if NET48
        public static CFB Create(Stream data)
#else
        public static CFB? Create(Stream? data)
#endif
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
        public List<SabreTools.Models.CFB.SectorNumber?>? GetFATSectorChain(SabreTools.Models.CFB.SectorNumber? startingSector)
#endif
        {
            // If we have an invalid sector
#if NET48
            if (startingSector < 0 || FATSectorNumbers == null || (long)startingSector >= FATSectorNumbers.Length)
#else
            if (startingSector == null || startingSector < 0 || FATSectorNumbers == null || (long)startingSector >= FATSectorNumbers.Length)
#endif
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
#if NET6_0_OR_GREATER
                if (lastSector == null)
                    break;
#endif

                // Get the next sector from the lookup table
#if NET48
                var nextSector = FATSectorNumbers[(uint)lastSector];
#else
                var nextSector = FATSectorNumbers[(uint)lastSector!.Value];
#endif

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
#if NET48
        public byte[] GetFATSectorChainData(SabreTools.Models.CFB.SectorNumber startingSector)
#else
        public byte[]? GetFATSectorChainData(SabreTools.Models.CFB.SectorNumber startingSector)
#endif
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
#if NET48
            if (sector > SabreTools.Models.CFB.SectorNumber.MAXREGSECT)
#else
            if (sector == null || sector > SabreTools.Models.CFB.SectorNumber.MAXREGSECT)
#endif
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
        public List<SabreTools.Models.CFB.SectorNumber?>? GetMiniFATSectorChain(SabreTools.Models.CFB.SectorNumber? startingSector)
#endif
        {
            // If we have an invalid sector
#if NET48
            if (startingSector < 0 || MiniFATSectorNumbers == null || (long)startingSector >= MiniFATSectorNumbers.Length)
#else
            if (startingSector == null || startingSector < 0 || MiniFATSectorNumbers == null || (long)startingSector >= MiniFATSectorNumbers.Length)
#endif
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
#if NET6_0_OR_GREATER
                if (lastSector == null)
                    break;
#endif

                // Get the next sector from the lookup table
#if NET48
                var nextSector = MiniFATSectorNumbers[(uint)lastSector];
#else
                var nextSector = MiniFATSectorNumbers[(uint)lastSector!.Value];
#endif

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
#if NET48
        public byte[] GetMiniFATSectorChainData(SabreTools.Models.CFB.SectorNumber startingSector)
#else
        public byte[]? GetMiniFATSectorChainData(SabreTools.Models.CFB.SectorNumber startingSector)
#endif
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
#if NET48
            if (sector > SabreTools.Models.CFB.SectorNumber.MAXREGSECT)
#else
            if (sector == null || sector > SabreTools.Models.CFB.SectorNumber.MAXREGSECT)
#endif
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
            Printing.CFB.Print(builder, _model);
            return builder;
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}