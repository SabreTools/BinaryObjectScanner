using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class NCF : WrapperBase<SabreTools.Models.NCF.File>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Half-Life No Cache File (NCF)";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public NCF(SabreTools.Models.NCF.File model, byte[] data, int offset)
#else
        public NCF(SabreTools.Models.NCF.File? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public NCF(SabreTools.Models.NCF.File model, Stream data)
#else
        public NCF(SabreTools.Models.NCF.File? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an NCF from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the NCF</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An NCF wrapper on success, null on failure</returns>
#if NET48
        public static NCF Create(byte[] data, int offset)
#else
        public static NCF? Create(byte[]? data, int offset)
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
        /// Create a NCF from a Stream
        /// </summary>
        /// <param name="data">Stream representing the NCF</param>
        /// <returns>An NCF wrapper on success, null on failure</returns>
#if NET48
        public static NCF Create(Stream data)
#else
        public static NCF? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = new SabreTools.Serialization.Streams.NCF().Deserialize(data);
            if (file == null)
                return null;

            try
            {
                return new NCF(file, data);
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
            Printing.NCF.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}