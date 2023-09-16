using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class Nitro : WrapperBase<SabreTools.Models.Nitro.Cart>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "Nintendo DS/DSi Cart Image";

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public Nitro(SabreTools.Models.Nitro.Cart model, byte[] data, int offset)
#else
        public Nitro(SabreTools.Models.Nitro.Cart? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public Nitro(SabreTools.Models.Nitro.Cart model, Stream data)
#else
        public Nitro(SabreTools.Models.Nitro.Cart? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create a NDS cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A NDS cart image wrapper on success, null on failure</returns>
#if NET48
        public static Nitro Create(byte[] data, int offset)
#else
        public static Nitro? Create(byte[]? data, int offset)
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
        /// Create a NDS cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A NDS cart image wrapper on success, null on failure</returns>
#if NET48
        public static Nitro Create(Stream data)
#else
        public static Nitro? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = new SabreTools.Serialization.Streams.Nitro().Deserialize(data);
            if (archive == null)
                return null;

            try
            {
                return new Nitro(archive, data);
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
            Printing.Nitro.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}