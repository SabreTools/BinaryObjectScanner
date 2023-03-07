using System;
using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class PlayJAudioFile : WrapperBase
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string Description => "PlayJ Audio File (PLJ)";

        #endregion

        #region Pass-Through Properties

        #region Audio Header

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Signature"/>
        public uint Signature => _audioFile.Header.Signature;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Version"/>
        public uint Version => _audioFile.Header.Version;

        #region V1 Only

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.TrackID"/>
        public uint? V1_TrackID => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV1)?.TrackID;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.UnknownOffset1"/>
        public uint? V1_UnknownOffset1 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV1)?.UnknownOffset1;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.UnknownOffset2"/>
        public uint? V1_UnknownOffset2 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV1)?.UnknownOffset2;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.UnknownOffset3"/>
        public uint? V1_UnknownOffset3 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV1)?.UnknownOffset3;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.Unknown1"/>
        public uint? V1_Unknown1 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV1)?.Unknown1;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.Unknown2"/>
        public uint? V1_Unknown2 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV1)?.Unknown2;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.Year"/>
        public uint? V1_Year => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV1)?.Year;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.TrackNumber"/>
        public uint? V1_TrackNumber => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV1)?.TrackNumber;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.Subgenre"/>
        public BinaryObjectScanner.Models.PlayJ.Subgenre? V1_Subgenre => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV1)?.Subgenre;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.Duration"/>
        public uint? V1_Duration => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV1)?.Duration;

        #endregion

        #region V2 Only

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown1"/>
        public uint? V2_Unknown1 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown1;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown2"/>
        public uint? V2_Unknown2 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown2;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown3"/>
        public uint? V2_Unknown3 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown3;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown4"/>
        public uint? V2_Unknown4 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown4;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown5"/>
        public uint? V2_Unknown5 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown5;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown6"/>
        public uint? V2_Unknown6 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown6;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.UnknownOffset1"/>
        public uint? V2_UnknownOffset1 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.UnknownOffset1;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown7"/>
        public uint? V2_Unknown7 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown7;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown8"/>
        public uint? V2_Unknown8 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown8;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown9"/>
        public uint? V2_Unknown9 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown9;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.UnknownOffset2"/>
        public uint? V2_UnknownOffset2 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.UnknownOffset2;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown10"/>
        public uint? V2_Unknown10 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown10;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown11"/>
        public uint? V2_Unknown11 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown11;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown12"/>
        public uint? V2_Unknown12 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown12;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown13"/>
        public uint? V2_Unknown13 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown13;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown14"/>
        public uint? V2_Unknown14 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown14;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown15"/>
        public uint? V2_Unknown15 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown15;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown16"/>
        public uint? V2_Unknown16 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown16;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown17"/>
        public uint? V2_Unknown17 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown17;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.TrackID"/>
        public uint? V2_TrackID => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.TrackID;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Year"/>
        public uint? V2_Year => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Year;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.TrackNumber"/>
        public uint? V2_TrackNumber => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.TrackNumber;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown18"/>
        public uint? V2_Unknown18 => (_audioFile.Header as BinaryObjectScanner.Models.PlayJ.AudioHeaderV2)?.Unknown18;

        #endregion

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.TrackLength"/>
        public ushort TrackLength => _audioFile.Header.TrackLength;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Track"/>
        public string Track => _audioFile.Header.Track;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.ArtistLength"/>
        public ushort ArtistLength => _audioFile.Header.ArtistLength;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Artist"/>
        public string Artist => _audioFile.Header.Artist;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.AlbumLength"/>
        public ushort AlbumLength => _audioFile.Header.AlbumLength;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Album"/>
        public string Album => _audioFile.Header.Album;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.WriterLength"/>
        public ushort WriterLength => _audioFile.Header.WriterLength;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Writer"/>
        public string Writer => _audioFile.Header.Writer;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.PublisherLength"/>
        public ushort PublisherLength => _audioFile.Header.PublisherLength;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Publisher"/>
        public string Publisher => _audioFile.Header.Publisher;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.LabelLength"/>
        public ushort LabelLength => _audioFile.Header.LabelLength;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Label"/>
        public string Label => _audioFile.Header.Label;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.CommentsLength"/>
        public ushort CommentsLength => _audioFile.Header.CommentsLength;

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Comments"/>
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
        public BinaryObjectScanner.Models.PlayJ.DataFile[] DataFiles => _audioFile.DataFiles;

        #endregion

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the archive
        /// </summary>
        private BinaryObjectScanner.Models.PlayJ.AudioFile _audioFile;

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

            PrintAudioHeader(builder);
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
        /// Print audio header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintAudioHeader(StringBuilder builder)
        {
            builder.AppendLine("  Audio Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Signature: {Signature} (0x{Signature:X})");
            builder.AppendLine($"  Version: {Version} (0x{Version:X})");
            if (Version == 0x00000000)
            {
                builder.AppendLine($"  Track ID: {V1_TrackID} (0x{V1_TrackID:X})");
                builder.AppendLine($"  Unknown offset 1: {V1_UnknownOffset1} (0x{V1_UnknownOffset1:X})");
                builder.AppendLine($"  Unknown offset 2: {V1_UnknownOffset2} (0x{V1_UnknownOffset2:X})");
                builder.AppendLine($"  Unknown offset 3: {V1_UnknownOffset3} (0x{V1_UnknownOffset3:X})");
                builder.AppendLine($"  Unknown 1: {V1_Unknown1} (0x{V1_Unknown1:X})");
                builder.AppendLine($"  Unknown 2: {V1_Unknown2} (0x{V1_Unknown2:X})");
                builder.AppendLine($"  Year: {V1_Year} (0x{V1_Year:X})");
                builder.AppendLine($"  Track number: {V1_TrackNumber} (0x{V1_TrackNumber:X})");
                builder.AppendLine($"  Subgenre: {V1_Subgenre} (0x{V1_Subgenre:X})");
                builder.AppendLine($"  Duration in seconds: {V1_Duration} (0x{V1_Duration:X})");
            }
            else if (Version == 0x0000000A)
            {
                builder.AppendLine($"  Unknown 1: {V2_Unknown1} (0x{V2_Unknown1:X})");
                builder.AppendLine($"  Unknown 2: {V2_Unknown2} (0x{V2_Unknown2:X})");
                builder.AppendLine($"  Unknown 3: {V2_Unknown3} (0x{V2_Unknown3:X})");
                builder.AppendLine($"  Unknown 4: {V2_Unknown4} (0x{V2_Unknown4:X})");
                builder.AppendLine($"  Unknown 5: {V2_Unknown5} (0x{V2_Unknown5:X})");
                builder.AppendLine($"  Unknown 6: {V2_Unknown6} (0x{V2_Unknown6:X})");
                builder.AppendLine($"  Unknown Offset 1: {V2_UnknownOffset1} (0x{V2_UnknownOffset1:X})");
                builder.AppendLine($"  Unknown 7: {V2_Unknown7} (0x{V2_Unknown7:X})");
                builder.AppendLine($"  Unknown 8: {V2_Unknown8} (0x{V2_Unknown8:X})");
                builder.AppendLine($"  Unknown 9: {V2_Unknown9} (0x{V2_Unknown9:X})");
                builder.AppendLine($"  Unknown Offset 2: {V2_UnknownOffset2} (0x{V2_UnknownOffset2:X})");
                builder.AppendLine($"  Unknown 10: {V2_Unknown10} (0x{V2_Unknown10:X})");
                builder.AppendLine($"  Unknown 11: {V2_Unknown11} (0x{V2_Unknown11:X})");
                builder.AppendLine($"  Unknown 12: {V2_Unknown12} (0x{V2_Unknown12:X})");
                builder.AppendLine($"  Unknown 13: {V2_Unknown13} (0x{V2_Unknown13:X})");
                builder.AppendLine($"  Unknown 14: {V2_Unknown14} (0x{V2_Unknown14:X})");
                builder.AppendLine($"  Unknown 15: {V2_Unknown15} (0x{V2_Unknown15:X})");
                builder.AppendLine($"  Unknown 16: {V2_Unknown16} (0x{V2_Unknown16:X})");
                builder.AppendLine($"  Unknown 17: {V2_Unknown17} (0x{V2_Unknown17:X})");
                builder.AppendLine($"  Track ID: {V2_TrackID} (0x{V2_TrackID:X})");
                builder.AppendLine($"  Year: {V2_Year} (0x{V2_Year:X})");
                builder.AppendLine($"  Track number: {V2_TrackNumber} (0x{V2_TrackNumber:X})");
                builder.AppendLine($"  Unknown 18: {V2_Unknown18} (0x{V2_Unknown18:X})");
            }
            else
            {
                builder.AppendLine($"  Unrecognized version, not parsed...");
            }
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