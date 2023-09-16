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