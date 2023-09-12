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
        public string Magic => _model.Header.Magic;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.LastPageBytes"/>
        public ushort LastPageBytes => _model.Header.LastPageBytes;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Pages"/>
        public ushort Pages => _model.Header.Pages;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationItems"/>
        public ushort RelocationItems => _model.Header.RelocationItems;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.HeaderParagraphSize"/>
        public ushort HeaderParagraphSize => _model.Header.HeaderParagraphSize;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MinimumExtraParagraphs"/>
        public ushort MinimumExtraParagraphs => _model.Header.MinimumExtraParagraphs;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MaximumExtraParagraphs"/>
        public ushort MaximumExtraParagraphs => _model.Header.MaximumExtraParagraphs;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSSValue"/>
        public ushort InitialSSValue => _model.Header.InitialSSValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSPValue"/>
        public ushort InitialSPValue => _model.Header.InitialSPValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Checksum"/>
        public ushort Checksum => _model.Header.Checksum;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialIPValue"/>
        public ushort InitialIPValue => _model.Header.InitialIPValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialCSValue"/>
        public ushort InitialCSValue => _model.Header.InitialCSValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationTableAddr"/>
        public ushort RelocationTableAddr => _model.Header.RelocationTableAddr;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OverlayNumber"/>
        public ushort OverlayNumber => _model.Header.OverlayNumber;

        #endregion

        #region PE Extensions

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved1"/>
        public ushort[] Reserved1 => _model.Header.Reserved1;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMIdentifier"/>
        public ushort OEMIdentifier => _model.Header.OEMIdentifier;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMInformation"/>
        public ushort OEMInformation => _model.Header.OEMInformation;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved2"/>
        public ushort[] Reserved2 => _model.Header.Reserved2;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.NewExeHeaderAddr"/>
        public uint NewExeHeaderAddr => _model.Header.NewExeHeaderAddr;

        #endregion

        #region Relocation Table

        /// <inheritdoc cref="Models.MSDOS.Executable.RelocationTable"/>
        public SabreTools.Models.MSDOS.RelocationEntry[] RelocationTable => _model.RelocationTable;

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
        public static MSDOS Create(byte[] data, int offset)
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
        public static MSDOS Create(Stream data)
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
            if (RelocationItems == 0 || RelocationTable.Length == 0)
            {
                builder.AppendLine("  No relocation table items");
            }
            else
            {
                for (int i = 0; i < RelocationTable.Length; i++)
                {
                    var entry = RelocationTable[i];
                    builder.AppendLine($"  Relocation Table Entry {i}");
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