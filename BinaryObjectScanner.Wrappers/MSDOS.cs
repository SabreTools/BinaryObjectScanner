using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class MSDOS : WrapperBase<SabreTools.Models.MSDOS.Executable>
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string DescriptionString => "MS-DOS Executable";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Magic"/>
#if NET48
        public string Magic => this.Model.Header.Magic;
#else
        public string? Magic => this.Model.Header?.Magic;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.LastPageBytes"/>
#if NET48
        public ushort LastPageBytes => this.Model.Header.LastPageBytes;
#else
        public ushort? LastPageBytes => this.Model.Header?.LastPageBytes;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Pages"/>
#if NET48
        public ushort Pages => this.Model.Header.Pages;
#else
        public ushort? Pages => this.Model.Header?.Pages;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationItems"/>
#if NET48
        public ushort RelocationItems => this.Model.Header.RelocationItems;
#else
        public ushort? RelocationItems => this.Model.Header?.RelocationItems;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.HeaderParagraphSize"/>
#if NET48
        public ushort HeaderParagraphSize => this.Model.Header.HeaderParagraphSize;
#else
        public ushort? HeaderParagraphSize => this.Model.Header?.HeaderParagraphSize;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MinimumExtraParagraphs"/>
#if NET48
        public ushort MinimumExtraParagraphs => this.Model.Header.MinimumExtraParagraphs;
#else
        public ushort? MinimumExtraParagraphs => this.Model.Header?.MinimumExtraParagraphs;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MaximumExtraParagraphs"/>
#if NET48
        public ushort MaximumExtraParagraphs => this.Model.Header.MaximumExtraParagraphs;
#else
        public ushort? MaximumExtraParagraphs => this.Model.Header?.MaximumExtraParagraphs;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSSValue"/>
#if NET48
        public ushort InitialSSValue => this.Model.Header.InitialSSValue;
#else
        public ushort? InitialSSValue => this.Model.Header?.InitialSSValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSPValue"/>
#if NET48
        public ushort InitialSPValue => this.Model.Header.InitialSPValue;
#else
        public ushort? InitialSPValue => this.Model.Header?.InitialSPValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Checksum"/>
#if NET48
        public ushort Checksum => this.Model.Header.Checksum;
#else
        public ushort? Checksum => this.Model.Header?.Checksum;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialIPValue"/>
#if NET48
        public ushort InitialIPValue => this.Model.Header.InitialIPValue;
#else
        public ushort? InitialIPValue => this.Model.Header?.InitialIPValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialCSValue"/>
#if NET48
        public ushort InitialCSValue => this.Model.Header.InitialCSValue;
#else
        public ushort? InitialCSValue => this.Model.Header?.InitialCSValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationTableAddr"/>
#if NET48
        public ushort RelocationTableAddr => this.Model.Header.RelocationTableAddr;
#else
        public ushort? RelocationTableAddr => this.Model.Header?.RelocationTableAddr;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OverlayNumber"/>
#if NET48
        public ushort OverlayNumber => this.Model.Header.OverlayNumber;
#else
        public ushort? OverlayNumber => this.Model.Header?.OverlayNumber;
#endif

        #endregion

        #region PE Extensions

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved1"/>
#if NET48
        public ushort[] Reserved1 => this.Model.Header.Reserved1;
#else
        public ushort[]? Reserved1 => this.Model.Header?.Reserved1;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMIdentifier"/>
#if NET48
        public ushort OEMIdentifier => this.Model.Header.OEMIdentifier;
#else
        public ushort? OEMIdentifier => this.Model.Header?.OEMIdentifier;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMInformation"/>
#if NET48
        public ushort OEMInformation => this.Model.Header.OEMInformation;
#else
        public ushort? OEMInformation => this.Model.Header?.OEMInformation;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved2"/>
#if NET48
        public ushort[] Reserved2 => this.Model.Header.Reserved2;
#else
        public ushort[]? Reserved2 => this.Model.Header?.Reserved2;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.NewExeHeaderAddr"/>
#if NET48
        public uint NewExeHeaderAddr => this.Model.Header.NewExeHeaderAddr;
#else
        public uint? NewExeHeaderAddr => this.Model.Header?.NewExeHeaderAddr;
#endif

        #endregion

        #region Relocation Table

        /// <inheritdoc cref="Models.MSDOS.Executable.RelocationTable"/>
#if NET48
        public SabreTools.Models.MSDOS.RelocationEntry[] RelocationTable => this.Model.RelocationTable;
#else
        public SabreTools.Models.MSDOS.RelocationEntry?[]? RelocationTable => this.Model.RelocationTable;
#endif

        #endregion

        #endregion

        #region Constructors

        /// <inheritdoc/>
#if NET48
        public MSDOS(SabreTools.Models.MSDOS.Executable model, byte[] data, int offset)
#else
        public MSDOS(SabreTools.Models.MSDOS.Executable? model, byte[]? data, int offset)
#endif
            : base(model, data, offset)
        {
            // All logic is handled by the base class
        }

        /// <inheritdoc/>
#if NET48
        public MSDOS(SabreTools.Models.MSDOS.Executable model, Stream data)
#else
        public MSDOS(SabreTools.Models.MSDOS.Executable? model, Stream? data)
#endif
            : base(model, data)
        {
            // All logic is handled by the base class
        }/// <summary>
         /// Create an MS-DOS executable from a byte array and offset
         /// </summary>
         /// <param name="data">Byte array representing the executable</param>
         /// <param name="offset">Offset within the array to parse</param>
         /// <returns>An MS-DOS executable wrapper on success, null on failure</returns>
#if NET48
        public static MSDOS Create(byte[] data, int offset)
#else
        public static MSDOS? Create(byte[]? data, int offset)
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
        /// Create an MS-DOS executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An MS-DOS executable wrapper on success, null on failure</returns>
#if NET48
        public static MSDOS Create(Stream data)
#else
        public static MSDOS? Create(Stream? data)
#endif
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var executable = new SabreTools.Serialization.Streams.MSDOS().Deserialize(data);
            if (executable == null)
                return null;

            try
            {
                return new MSDOS(executable, data);
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
            Printing.MSDOS.Print(builder, this.Model);
            return builder;
        }

        #endregion
    }
}