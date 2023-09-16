using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class CIA : WrapperBase<SabreTools.Models.N3DS.CIA>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "CTR Importable Archive (CIA)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public CIA(SabreTools.Models.N3DS.CIA model, byte[] data, int offset)
#else
        public CIA(SabreTools.Models.N3DS.CIA? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public CIA(SabreTools.Models.N3DS.CIA model, Stream data)
#else
        public CIA(SabreTools.Models.N3DS.CIA? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a CIA archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A CIA archive wrapper on success, null on failure</returns>
#if NET48
        public static CIA Create(byte[] data, int offset)
#else
        public static CIA? Create(byte[]? data, int offset)
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
        /// Create a CIA archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A CIA archive wrapper on success, null on failure</returns>
#if NET48
        public static CIA Create(Stream data)
#else
        public static CIA? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = new SabreTools.Serialization.Streams.CIA().Deserialize(data);
            if (archive == null)
                return null;

            try
            {
                return new CIA(archive, data);
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
            Printing.CIA.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}