using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class PlayJAudioFile : WrapperBase<SabreTools.Models.PlayJ.AudioFile>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "PlayJ Audio File (PLJ)";

        #endregion

        #region Pass-Through Properties

        #region Audio Header

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Signature"/>
#if NET48
        public uint Signature => this.Model.Header.Signature;
#else
        public uint? Signature => this.Model.Header?.Signature;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Version"/>
#if NET48
        public uint Version => this.Model.Header.Version;
#else
        public uint? Version => this.Model.Header?.Version;
#endif

        #region V1 Only

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.TrackID"/>
        public uint? V1_TrackID => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV1)?.TrackID;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.UnknownOffset1"/>
        public uint? V1_UnknownOffset1 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV1)?.UnknownOffset1;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.UnknownOffset2"/>
        public uint? V1_UnknownOffset2 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV1)?.UnknownOffset2;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.UnknownOffset3"/>
        public uint? V1_UnknownOffset3 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV1)?.UnknownOffset3;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.Unknown1"/>
        public uint? V1_Unknown1 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV1)?.Unknown1;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.Unknown2"/>
        public uint? V1_Unknown2 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV1)?.Unknown2;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.Year"/>
        public uint? V1_Year => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV1)?.Year;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.TrackNumber"/>
        public uint? V1_TrackNumber => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV1)?.TrackNumber;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.Subgenre"/>
        public SabreTools.Models.PlayJ.Subgenre? V1_Subgenre => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV1)?.Subgenre;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV1.Duration"/>
        public uint? V1_Duration => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV1)?.Duration;

        #endregion

        #region V2 Only

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown1"/>
        public uint? V2_Unknown1 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown1;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown2"/>
        public uint? V2_Unknown2 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown2;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown3"/>
        public uint? V2_Unknown3 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown3;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown4"/>
        public uint? V2_Unknown4 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown4;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown5"/>
        public uint? V2_Unknown5 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown5;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown6"/>
        public uint? V2_Unknown6 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown6;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.UnknownOffset1"/>
        public uint? V2_UnknownOffset1 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.UnknownOffset1;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown7"/>
        public uint? V2_Unknown7 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown7;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown8"/>
        public uint? V2_Unknown8 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown8;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown9"/>
        public uint? V2_Unknown9 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown9;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.UnknownOffset2"/>
        public uint? V2_UnknownOffset2 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.UnknownOffset2;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown10"/>
        public uint? V2_Unknown10 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown10;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown11"/>
        public uint? V2_Unknown11 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown11;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown12"/>
        public uint? V2_Unknown12 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown12;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown13"/>
        public uint? V2_Unknown13 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown13;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown14"/>
        public uint? V2_Unknown14 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown14;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown15"/>
        public uint? V2_Unknown15 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown15;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown16"/>
        public uint? V2_Unknown16 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown16;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown17"/>
        public uint? V2_Unknown17 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown17;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.TrackID"/>
        public uint? V2_TrackID => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.TrackID;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Year"/>
        public uint? V2_Year => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Year;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.TrackNumber"/>
        public uint? V2_TrackNumber => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.TrackNumber;

        /// <inheritdoc cref="Models.PlayJ.AudioHeaderV2.Unknown18"/>
        public uint? V2_Unknown18 => (Model.Header as SabreTools.Models.PlayJ.AudioHeaderV2)?.Unknown18;

        #endregion

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.TrackLength"/>
#if NET48
        public ushort TrackLength => this.Model.Header.TrackLength;
#else
        public ushort? TrackLength => this.Model.Header?.TrackLength;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Track"/>
#if NET48
        public string Track => this.Model.Header.Track;
#else
        public string? Track => this.Model.Header?.Track;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.ArtistLength"/>
#if NET48
        public ushort ArtistLength => this.Model.Header.ArtistLength;
#else
        public ushort? ArtistLength => this.Model.Header?.ArtistLength;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Artist"/>
#if NET48
        public string Artist => this.Model.Header.Artist;
#else
        public string? Artist => this.Model.Header?.Artist;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.AlbumLength"/>
#if NET48
        public ushort AlbumLength => this.Model.Header.AlbumLength;
#else
        public ushort? AlbumLength => this.Model.Header?.AlbumLength;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Album"/>
#if NET48
        public string Album => this.Model.Header.Album;
#else
        public string? Album => this.Model.Header?.Album;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.WriterLength"/>
#if NET48
        public ushort WriterLength => this.Model.Header.WriterLength;
#else
        public ushort? WriterLength => this.Model.Header?.WriterLength;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Writer"/>
#if NET48
        public string Writer => this.Model.Header.Writer;
#else
        public string? Writer => this.Model.Header?.Writer;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.PublisherLength"/>
#if NET48
        public ushort PublisherLength => this.Model.Header.PublisherLength;
#else
        public ushort? PublisherLength => this.Model.Header?.PublisherLength;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Publisher"/>
#if NET48
        public string Publisher => this.Model.Header.Publisher;
#else
        public string? Publisher => this.Model.Header?.Publisher;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.LabelLength"/>
#if NET48
        public ushort LabelLength => this.Model.Header.LabelLength;
#else
        public ushort? LabelLength => this.Model.Header?.LabelLength;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Label"/>
#if NET48
        public string Label => this.Model.Header.Label;
#else
        public string? Label => this.Model.Header?.Label;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.CommentsLength"/>
#if NET48
        public ushort CommentsLength => this.Model.Header.CommentsLength;
#else
        public ushort? CommentsLength => this.Model.Header?.CommentsLength;
#endif

        /// <inheritdoc cref="Models.PlayJ.AudioHeader.Comments"/>
#if NET48
        public string Comments => this.Model.Header.Comments;
#else
        public string? Comments => this.Model.Header?.Comments;
#endif

        #endregion

        #region Unknown Block 1

        /// <inheritdoc cref="Models.PlayJ.UnknownBlock1.Length"/>
#if NET48
        public uint UB1_Length => this.Model.UnknownBlock1.Length;
#else
        public uint? UB1_Length => this.Model.UnknownBlock1?.Length;
#endif

        /// <inheritdoc cref="Models.PlayJ.UnknownBlock1.Data"/>
#if NET48
        public byte[] UB1_Data => this.Model.UnknownBlock1.Data;
#else
        public byte[]? UB1_Data => this.Model.UnknownBlock1?.Data;
#endif

        #endregion

        #region V1 Only

        #region Unknown Value 2

        /// <inheritdoc cref="Models.PlayJ.AudioFile.UnknownValue2"/>
        public uint UnknownValue2 => this.Model.UnknownValue2;

        #endregion

        #region Unknown Block 3

        /// <inheritdoc cref="Models.PlayJ.UnknownBlock3.Data"/>
#if NET48
        public byte[] UB3_Data => this.Model.UnknownBlock3.Data;
#else
        public byte[]? UB3_Data => this.Model.UnknownBlock3?.Data;
#endif

        #endregion

        #endregion

        #region V2 Only

        #region Data Files Count

        /// <inheritdoc cref="Models.PlayJ.AudioFile.DataFilesCount"/>
        public uint DataFilesCount => this.Model.DataFilesCount;

        #endregion

        #region Unknown Block 3

        /// <inheritdoc cref="Models.PlayJ.AudioFile.DataFiles"/>
#if NET48
        public SabreTools.Models.PlayJ.DataFile[] DataFiles => this.Model.DataFiles;
#else
        public SabreTools.Models.PlayJ.DataFile?[]? DataFiles => this.Model.DataFiles;
#endif

        #endregion

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public PlayJAudioFile(SabreTools.Models.PlayJ.AudioFile model, byte[] data, int offset)
#else
        public PlayJAudioFile(SabreTools.Models.PlayJ.AudioFile? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public PlayJAudioFile(SabreTools.Models.PlayJ.AudioFile model, Stream data)
#else
        public PlayJAudioFile(SabreTools.Models.PlayJ.AudioFile? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a PlayJ audio file from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PlayJ audio file wrapper on success, null on failure</returns>
#if NET48
        public static PlayJAudioFile Create(byte[] data, int offset)
#else
        public static PlayJAudioFile? Create(byte[]? data, int offset)
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
        /// Create a PlayJ audio file from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A PlayJ audio file wrapper on success, null on failure</returns>
#if NET48
        public static PlayJAudioFile Create(Stream data)
#else
        public static PlayJAudioFile? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var audioFile = new SabreTools.Serialization.Streams.PlayJAudio().Deserialize(data);
            if (audioFile == null)
                return null;

            try
            {
                return new PlayJAudioFile(audioFile, data);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();
            Printing.PlayJAudioFile.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}