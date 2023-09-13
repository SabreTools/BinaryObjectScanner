using System.IO;
using System.Text;
using SabreTools.Models.AACS;

namespace BinaryObjectScanner.Wrappers
{
    public class AACSMediaKeyBlock : WrapperBase<MediaKeyBlock>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "AACS Media Key Block";

        #endregion

        #region Pass-Through Properties

        #region Records

        /// <inheritdoc cref="Models.AACS.MediaKeyBlock.Records"/>
#if NET48
        public Record[] Records => _model.Records;
#else
        public Record?[]? Records => _model.Records;
#endif

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public AACSMediaKeyBlock(MediaKeyBlock model, byte[] data, int offset)
#else
        public AACSMediaKeyBlock(MediaKeyBlock? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public AACSMediaKeyBlock(MediaKeyBlock model, Stream data)
#else
        public AACSMediaKeyBlock(MediaKeyBlock? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an AACS media key block from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An AACS media key block wrapper on success, null on failure</returns>
#if NET48
        public static AACSMediaKeyBlock Create(byte[] data, int offset)
#else
        public static AACSMediaKeyBlock? Create(byte[]? data, int offset)
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
        /// Create an AACS media key block from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>An AACS media key block wrapper on success, null on failure</returns>
#if NET48
        public static AACSMediaKeyBlock Create(Stream data)
#else
        public static AACSMediaKeyBlock? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var mediaKeyBlock = new SabreTools.Serialization.Streams.AACS().Deserialize(data);
            if (mediaKeyBlock == null)
                return null;

            try
            {
                return new AACSMediaKeyBlock(mediaKeyBlock, data);
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
            Printing.AACSMediaKeyBlock.Print(builder, _model);
            return builder;
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}