using System;
using System.IO;
using System.Text;

namespace BurnOutSharp.Wrappers
{
    public class PlayJAudioFile : WrapperBase
    {
        #region Pass-Through Properties

        #region Entry Header

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Signature"/>
        public uint Signature => _audioFile.Header.Signature;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Version"/>
        public uint Version => _audioFile.Header.Version;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.TrackID"/>
        public uint TrackID => _audioFile.Header.TrackID;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.UnknownOffset1"/>
        public uint UnknownOffset1 => _audioFile.Header.UnknownOffset1;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.UnknownOffset2"/>
        public uint UnknownOffset2 => _audioFile.Header.UnknownOffset2;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.UnknownOffset3"/>
        public uint UnknownOffset3 => _audioFile.Header.UnknownOffset3;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Unknown1"/>
        public uint Unknown1 => _audioFile.Header.Unknown1;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Unknown2"/>
        public uint Unknown2 => _audioFile.Header.Unknown2;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Year"/>
        public uint Year => _audioFile.Header.Year;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.TrackNumber"/>
        public uint TrackNumber => _audioFile.Header.TrackNumber;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Subgenre"/>
        public Models.PlayJ.Subgenre Subgenre => _audioFile.Header.Subgenre;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Duration"/>
        public uint Duration => _audioFile.Header.Duration;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.TrackLength"/>
        public ushort TrackLength => _audioFile.Header.TrackLength;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Track"/>
        public string Track => _audioFile.Header.Track;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.ArtistLength"/>
        public ushort ArtistLength => _audioFile.Header.ArtistLength;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Artist"/>
        public string Artist => _audioFile.Header.Artist;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.AlbumLength"/>
        public ushort AlbumLength => _audioFile.Header.AlbumLength;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Album"/>
        public string Album => _audioFile.Header.Album;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.WriterLength"/>
        public ushort WriterLength => _audioFile.Header.WriterLength;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Writer"/>
        public string Writer => _audioFile.Header.Writer;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.PublisherLength"/>
        public ushort PublisherLength => _audioFile.Header.PublisherLength;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Publisher"/>
        public string Publisher => _audioFile.Header.Publisher;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.LabelLength"/>
        public ushort LabelLength => _audioFile.Header.LabelLength;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Label"/>
        public string Label => _audioFile.Header.Label;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.CommentsLength"/>
        public ushort CommentsLength => _audioFile.Header.CommentsLength;

        /// <inheritdoc cref="Models.PlayJ.EntryHeader.Comments"/>
        public string Comments => _audioFile.Header.Comments;

        #endregion

        #region Unknown Block 1

        /// <inheritdoc cref="Models.PlayJ.UnknownBlock1.Length"/>
        public uint UB1_Length => _audioFile.UnknownBlock1.Length;

        /// <inheritdoc cref="Models.PlayJ.UnknownBlock1.Data"/>
        public byte[] UB1_Data => _audioFile.UnknownBlock1.Data;

        #endregion

        #region V1 Only

        #region Unknown Value 2

        /// <inheritdoc cref="Models.PlayJ.AudioFile.UnknownValue2"/>
        public uint UnknownValue2 => _audioFile.UnknownValue2;

        #endregion

        #region Unknown Block 3

        /// <inheritdoc cref="Models.PlayJ.UnknownBlock3.Data"/>
        public byte[] UB3_Data => _audioFile.UnknownBlock3.Data;

        #endregion

        #endregion

        #region V2 Only

        #region Data Files Count

        /// <inheritdoc cref="Models.PlayJ.AudioFile.DataFilesCount"/>
        public uint DataFilesCount => _audioFile.DataFilesCount;

        #endregion

        #region Unknown Block 3

        /// <inheritdoc cref="Models.PlayJ.AudioFile.DataFiles"/>
        public Models.PlayJ.DataFile[] DataFiles => _audioFile.DataFiles;

        #endregion

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the archive
        /// </summary>
        private Models.PlayJ.AudioFile _audioFile;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private PlayJAudioFile() { }

        /// <summary>
        /// Create a PlayJ audio file from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PlayJ audio file wrapper on success, null on failure</returns>
        public static PlayJAudioFile Create(byte[] data, int offset)
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
        /// Create a PlayJ audio file from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A PlayJ audio file wrapper on success, null on failure</returns>
        public static PlayJAudioFile Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var audioFile = Builders.PlayJ.ParseAudioFile(data);
            if (audioFile == null)
                return null;

            var wrapper = new PlayJAudioFile
            {
                _audioFile = audioFile,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("PlayJ Audio File Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintEntryHeader(builder);
            PrintUnknownBlock1(builder);

            if (Version == 0x00000000)
            {
                PrintUnknownValue2(builder);
                PrintUnknownBlock3(builder);
            }
            else if (Version == 0x0000000A)
            {
                PrintDataFiles(builder);
            }

            return builder;
        }

        /// <summary>
        /// Print entry header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintEntryHeader(StringBuilder builder)
        {
            builder.AppendLine("  Entry Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Signature: {Signature} (0x{Signature:X})");
            builder.AppendLine($"  Version: {Version} (0x{Version:X})");
            builder.AppendLine($"  Track ID: {TrackID} (0x{TrackID:X})");
            builder.AppendLine($"  Unknown offset 1: {UnknownOffset1} (0x{UnknownOffset1:X})");
            builder.AppendLine($"  Unknown offset 2: {UnknownOffset2} (0x{UnknownOffset2:X})");
            builder.AppendLine($"  Unknown offset 3: {UnknownOffset3} (0x{UnknownOffset3:X})");
            builder.AppendLine($"  Unknown 1: {Unknown1} (0x{Unknown1:X})");
            builder.AppendLine($"  Unknown 2: {Unknown2} (0x{Unknown2:X})");
            builder.AppendLine($"  Year: {Year} (0x{Year:X})");
            builder.AppendLine($"  Track number: {TrackNumber} (0x{TrackNumber:X})");
            builder.AppendLine($"  Subgenre: {Subgenre} (0x{Subgenre:X})");
            builder.AppendLine($"  Duration in seconds: {Duration} (0x{Duration:X})");
            builder.AppendLine($"  Track length: {TrackLength} (0x{TrackLength:X})");
            builder.AppendLine($"  Track: {Track ?? "[NULL]"}");
            builder.AppendLine($"  Artist length: {ArtistLength} (0x{ArtistLength:X})");
            builder.AppendLine($"  Artist: {Artist ?? "[NULL]"}");
            builder.AppendLine($"  Album length: {AlbumLength} (0x{AlbumLength:X})");
            builder.AppendLine($"  Album: {Album ?? "[NULL]"}");
            builder.AppendLine($"  Writer length: {WriterLength} (0x{WriterLength:X})");
            builder.AppendLine($"  Writer: {Writer ?? "[NULL]"}");
            builder.AppendLine($"  Publisher length: {PublisherLength} (0x{PublisherLength:X})");
            builder.AppendLine($"  Publisher: {Publisher ?? "[NULL]"}");
            builder.AppendLine($"  Label length: {LabelLength} (0x{LabelLength:X})");
            builder.AppendLine($"  Label: {Label ?? "[NULL]"}");
            builder.AppendLine($"  Comments length: {CommentsLength} (0x{CommentsLength:X})");
            builder.AppendLine($"  Comments: {Comments ?? "[NULL]"}");
            builder.AppendLine();
        }

        /// <summary>
        /// Print unknown block 1 information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintUnknownBlock1(StringBuilder builder)
        {
            builder.AppendLine("  Unknown Block 1 Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Length: {UB1_Length} (0x{UB1_Length:X})");
            builder.AppendLine($"  Data: {BitConverter.ToString(UB1_Data ?? new byte[0]).Replace('-', ' ')}");
            builder.AppendLine();
        }

        /// <summary>
        /// Print unknown value 2 information (V1 only)
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintUnknownValue2(StringBuilder builder)
        {
            builder.AppendLine("  Unknown Value 2 Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Value: {UnknownValue2} (0x{UnknownValue2:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print unknown block 3 information (V1 only)
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintUnknownBlock3(StringBuilder builder)
        {
            builder.AppendLine("  Unknown Block 3 Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Data: {BitConverter.ToString(UB3_Data ?? new byte[0]).Replace('-', ' ')}");
            builder.AppendLine();
        }

        /// <summary>
        /// Print data files information (V2 only)
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintDataFiles(StringBuilder builder)
        {
            builder.AppendLine("  Data Files Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Data files count: {DataFilesCount} (0x{DataFilesCount:X})");
            if (DataFilesCount != 0 && DataFiles != null && DataFiles.Length != 0)
            {
                for (int i = 0; i < DataFiles.Length; i++)
                {
                    var dataFile = DataFiles[i];
                    builder.AppendLine($"  Data File {i}:");
                    builder.AppendLine($"    File name length: {dataFile.FileNameLength} (0x{dataFile.FileNameLength:X})");
                    builder.AppendLine($"    File name: {dataFile.FileName ?? "[NULL]"}");
                    builder.AppendLine($"    Data length: {dataFile.DataLength} (0x{dataFile.DataLength:X})");
                    builder.AppendLine($"    Data: {BitConverter.ToString(dataFile.Data ?? new byte[0]).Replace('-', ' ')}");
                }
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_audioFile, _jsonSerializerOptions);

#endif

        #endregion
    }
}