using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public partial class InstallShieldCabinet : WrapperBase<SabreTools.Models.InstallShieldCabinet.Cabinet>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "InstallShield Cabinet";

        #endregion

        #region Extension Properties

        /// <summary>
        /// The major version of the cabinet
        /// </summary>
        public int MajorVersion
        {
            get
            {
                uint majorVersion = this.Model.CommonHeader?.Version ?? 0;
                if (majorVersion >> 24 == 1)
                {
                    majorVersion = (majorVersion >> 12) & 0x0F;
                }
                else if (majorVersion >> 24 == 2 || majorVersion >> 24 == 4)
                {
                    majorVersion = majorVersion & 0xFFFF;
                    if (majorVersion != 0)
                        majorVersion /= 100;
                }

                return (int)majorVersion;
            }
        }

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public InstallShieldCabinet(SabreTools.Models.InstallShieldCabinet.Cabinet model, byte[] data, int offset)
#else
        public InstallShieldCabinet(SabreTools.Models.InstallShieldCabinet.Cabinet? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public InstallShieldCabinet(SabreTools.Models.InstallShieldCabinet.Cabinet model, Stream data)
#else
        public InstallShieldCabinet(SabreTools.Models.InstallShieldCabinet.Cabinet? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }

        /// <summary>
        /// Create an InstallShield Cabinet from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the cabinet</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
#if NET48
        public static InstallShieldCabinet Create(byte[] data, int offset)
#else
        public static InstallShieldCabinet? Create(byte[]? data, int offset)
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
        /// Create a InstallShield Cabinet from a Stream
        /// </summary>
        /// <param name="data">Stream representing the cabinet</param>
        /// <returns>A cabinet wrapper on success, null on failure</returns>
#if NET48
        public static InstallShieldCabinet Create(Stream data)
#else
        public static InstallShieldCabinet? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var cabinet = new SabreTools.Serialization.Streams.InstallShieldCabinet().Deserialize(data);
            if (cabinet == null)
                return null;

            try
            {
                return new InstallShieldCabinet(cabinet, data);
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
            Printing.InstallShieldCabinet.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}
