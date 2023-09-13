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
        public string Magic => _model.Header.Magic;
#else
        public string? Magic => _model.Header?.Magic;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.LastPageBytes"/>
#if NET48
        public ushort LastPageBytes => _model.Header.LastPageBytes;
#else
        public ushort? LastPageBytes => _model.Header?.LastPageBytes;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Pages"/>
#if NET48
        public ushort Pages => _model.Header.Pages;
#else
        public ushort? Pages => _model.Header?.Pages;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationItems"/>
#if NET48
        public ushort RelocationItems => _model.Header.RelocationItems;
#else
        public ushort? RelocationItems => _model.Header?.RelocationItems;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.HeaderParagraphSize"/>
#if NET48
        public ushort HeaderParagraphSize => _model.Header.HeaderParagraphSize;
#else
        public ushort? HeaderParagraphSize => _model.Header?.HeaderParagraphSize;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MinimumExtraParagraphs"/>
#if NET48
        public ushort MinimumExtraParagraphs => _model.Header.MinimumExtraParagraphs;
#else
        public ushort? MinimumExtraParagraphs => _model.Header?.MinimumExtraParagraphs;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MaximumExtraParagraphs"/>
#if NET48
        public ushort MaximumExtraParagraphs => _model.Header.MaximumExtraParagraphs;
#else
        public ushort? MaximumExtraParagraphs => _model.Header?.MaximumExtraParagraphs;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSSValue"/>
#if NET48
        public ushort InitialSSValue => _model.Header.InitialSSValue;
#else
        public ushort? InitialSSValue => _model.Header?.InitialSSValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSPValue"/>
#if NET48
        public ushort InitialSPValue => _model.Header.InitialSPValue;
#else
        public ushort? InitialSPValue => _model.Header?.InitialSPValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Checksum"/>
#if NET48
        public ushort Checksum => _model.Header.Checksum;
#else
        public ushort? Checksum => _model.Header?.Checksum;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialIPValue"/>
#if NET48
        public ushort InitialIPValue => _model.Header.InitialIPValue;
#else
        public ushort? InitialIPValue => _model.Header?.InitialIPValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialCSValue"/>
#if NET48
        public ushort InitialCSValue => _model.Header.InitialCSValue;
#else
        public ushort? InitialCSValue => _model.Header?.InitialCSValue;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationTableAddr"/>
#if NET48
        public ushort RelocationTableAddr => _model.Header.RelocationTableAddr;
#else
        public ushort? RelocationTableAddr => _model.Header?.RelocationTableAddr;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OverlayNumber"/>
#if NET48
        public ushort OverlayNumber => _model.Header.OverlayNumber;
#else
        public ushort? OverlayNumber => _model.Header?.OverlayNumber;
#endif

        #endregion

        #region PE Extensions

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved1"/>
#if NET48
        public ushort[] Reserved1 => _model.Header.Reserved1;
#else
        public ushort[]? Reserved1 => _model.Header?.Reserved1;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMIdentifier"/>
#if NET48
        public ushort OEMIdentifier => _model.Header.OEMIdentifier;
#else
        public ushort? OEMIdentifier => _model.Header?.OEMIdentifier;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMInformation"/>
#if NET48
        public ushort OEMInformation => _model.Header.OEMInformation;
#else
        public ushort? OEMInformation => _model.Header?.OEMInformation;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved2"/>
#if NET48
        public ushort[] Reserved2 => _model.Header.Reserved2;
#else
        public ushort[]? Reserved2 => _model.Header?.Reserved2;
#endif

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.NewExeHeaderAddr"/>
#if NET48
        public uint NewExeHeaderAddr => _model.Header.NewExeHeaderAddr;
#else
        public uint? NewExeHeaderAddr => _model.Header?.NewExeHeaderAddr;
#endif

        #endregion

        #region Relocation Table

        /// <inheritdoc cref="Models.MSDOS.Executable.RelocationTable"/>
#if NET48
        public SabreTools.Models.MSDOS.RelocationEntry[] RelocationTable => _model.RelocationTable;
#else
        public SabreTools.Models.MSDOS.RelocationEntry?[]? RelocationTable => _model.RelocationTable;
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

            builder.AppendLine("MS-DOS Executable Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintHeader(builder);
            PrintRelocationTable(builder);

            return builder;
        }

        /// <summary>
        /// Print header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintHeader(StringBuilder builder)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Magic number: {Magic}");
            builder.AppendLine($"  Last page bytes: {LastPageBytes} (0x{LastPageBytes:X})");
            builder.AppendLine($"  Pages: {Pages} (0x{Pages:X})");
            builder.AppendLine($"  Relocation items: {RelocationItems} (0x{RelocationItems:X})");
            builder.AppendLine($"  Header paragraph size: {HeaderParagraphSize} (0x{HeaderParagraphSize:X})");
            builder.AppendLine($"  Minimum extra paragraphs: {MinimumExtraParagraphs} (0x{MinimumExtraParagraphs:X})");
            builder.AppendLine($"  Maximum extra paragraphs: {MaximumExtraParagraphs} (0x{MaximumExtraParagraphs:X})");
            builder.AppendLine($"  Initial SS value: {InitialSSValue} (0x{InitialSSValue:X})");
            builder.AppendLine($"  Initial SP value: {InitialSPValue} (0x{InitialSPValue:X})");
            builder.AppendLine($"  Checksum: {Checksum} (0x{Checksum:X})");
            builder.AppendLine($"  Initial IP value: {InitialIPValue} (0x{InitialIPValue:X})");
            builder.AppendLine($"  Initial CS value: {InitialCSValue} (0x{InitialCSValue:X})");
            builder.AppendLine($"  Relocation table address: {RelocationTableAddr} (0x{RelocationTableAddr:X})");
            builder.AppendLine($"  Overlay number: {OverlayNumber} (0x{OverlayNumber:X})");
        }

        /// <summary>
        /// Print relocation table information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintRelocationTable(StringBuilder builder)
        {
            builder.AppendLine("  Relocation Table Information:");
            builder.AppendLine("  -------------------------");
            if (RelocationItems == 0 || RelocationTable == null || RelocationTable.Length == 0)
            {
                builder.AppendLine("  No relocation table items");
            }
            else
            {
                for (int i = 0; i < RelocationTable.Length; i++)
                {
                    var entry = RelocationTable[i];
                    builder.AppendLine($"  Relocation Table Entry {i}");
                    if (entry == null)
                    {
                        builder.AppendLine($"    [NULL]");
                        continue;
                    }

                    builder.AppendLine($"    Offset: {entry.Offset} (0x{entry.Offset:X})");
                    builder.AppendLine($"    Segment: {entry.Segment} (0x{entry.Segment:X})");
                }
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_model, _jsonSerializerOptions);

#endif

        #endregion
    }
}