using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class N3DS : WrapperBase<SabreTools.Models.N3DS.Cart>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Nintendo 3DS Cart Image";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public N3DS(SabreTools.Models.N3DS.Cart model, byte[] data, int offset)
#else
        public N3DS(SabreTools.Models.N3DS.Cart? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public N3DS(SabreTools.Models.N3DS.Cart model, Stream data)
#else
        public N3DS(SabreTools.Models.N3DS.Cart? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a 3DS cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A 3DS cart image wrapper on success, null on failure</returns>
#if NET48
        public static N3DS Create(byte[] data, int offset)
#else
        public static N3DS? Create(byte[]? data, int offset)
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
        /// Create a 3DS cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A 3DS cart image wrapper on success, null on failure</returns>
#if NET48
        public static N3DS Create(Stream data)
#else
        public static N3DS? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = new SabreTools.Serialization.Streams.N3DS().Deserialize(data);
            if (archive == null)
                return null;

            try
            {
                return new N3DS(archive, data);
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
            Printing.N3DS.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}