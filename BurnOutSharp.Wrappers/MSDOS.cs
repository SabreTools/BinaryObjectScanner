using System;
using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class MSDOS : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Magic"/>
        public string Magic => _executable.Header.Magic;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.LastPageBytes"/>
        public ushort LastPageBytes => _executable.Header.LastPageBytes;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Pages"/>
        public ushort Pages => _executable.Header.Pages;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationItems"/>
        public ushort RelocationItems => _executable.Header.RelocationItems;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.HeaderParagraphSize"/>
        public ushort HeaderParagraphSize => _executable.Header.HeaderParagraphSize;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MinimumExtraParagraphs"/>
        public ushort MinimumExtraParagraphs => _executable.Header.MinimumExtraParagraphs;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MaximumExtraParagraphs"/>
        public ushort MaximumExtraParagraphs => _executable.Header.MaximumExtraParagraphs;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSSValue"/>
        public ushort InitialSSValue => _executable.Header.InitialSSValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSPValue"/>
        public ushort InitialSPValue => _executable.Header.InitialSPValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Checksum"/>
        public ushort Checksum => _executable.Header.Checksum;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialIPValue"/>
        public ushort InitialIPValue => _executable.Header.InitialIPValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialCSValue"/>
        public ushort InitialCSValue => _executable.Header.InitialCSValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationTableAddr"/>
        public ushort RelocationTableAddr => _executable.Header.RelocationTableAddr;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OverlayNumber"/>
        public ushort OverlayNumber => _executable.Header.OverlayNumber;

        #endregion

        #region PE Extensions

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved1"/>
        public ushort[] Reserved1 => _executable.Header.Reserved1;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMIdentifier"/>
        public ushort OEMIdentifier => _executable.Header.OEMIdentifier;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMInformation"/>
        public ushort OEMInformation => _executable.Header.OEMInformation;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved2"/>
        public ushort[] Reserved2 => _executable.Header.Reserved2;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.NewExeHeaderAddr"/>
        public uint NewExeHeaderAddr => _executable.Header.NewExeHeaderAddr;

        #endregion

        #region Relocation Table

        /// <inheritdoc cref="Models.MSDOS.Executable.RelocationTable"/>
        public Models.MSDOS.RelocationEntry[] RelocationTable => _executable.RelocationTable;

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the executable
        /// </summary>
        private Models.MSDOS.Executable _executable;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private MSDOS() { }

        /// <summary>
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

            var executable = Builders.MSDOS.ParseExecutable(data);
            if (executable == null)
                return null;

            var wrapper = new MSDOS
            {
                _executable = executable,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }
    
        #endregion

        #region Printing
    
        /// <inheritdoc/>
        public override void Print()
        {
            Console.WriteLine("MS-DOS Executable Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            PrintHeader();
            PrintRelocationTable();
        }

        /// <summary>
        /// Print header information
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Magic number: {Magic}");
            Console.WriteLine($"  Last page bytes: {LastPageBytes} (0x{LastPageBytes:X})");
            Console.WriteLine($"  Pages: {Pages} (0x{Pages:X})");
            Console.WriteLine($"  Relocation items: {RelocationItems} (0x{RelocationItems:X})");
            Console.WriteLine($"  Header paragraph size: {HeaderParagraphSize} (0x{HeaderParagraphSize:X})");
            Console.WriteLine($"  Minimum extra paragraphs: {MinimumExtraParagraphs} (0x{MinimumExtraParagraphs:X})");
            Console.WriteLine($"  Maximum extra paragraphs: {MaximumExtraParagraphs} (0x{MaximumExtraParagraphs:X})");
            Console.WriteLine($"  Initial SS value: {InitialSSValue} (0x{InitialSSValue:X})");
            Console.WriteLine($"  Initial SP value: {InitialSPValue} (0x{InitialSPValue:X})");
            Console.WriteLine($"  Checksum: {Checksum} (0x{Checksum:X})");
            Console.WriteLine($"  Initial IP value: {InitialIPValue} (0x{InitialIPValue:X})");
            Console.WriteLine($"  Initial CS value: {InitialCSValue} (0x{InitialCSValue:X})");
            Console.WriteLine($"  Relocation table address: {RelocationTableAddr} (0x{RelocationTableAddr:X})");
            Console.WriteLine($"  Overlay number: {OverlayNumber} (0x{OverlayNumber:X})");
        }

        /// <summary>
        /// Print relocation table information
        /// </summary>
        private void PrintRelocationTable()
        {
            Console.WriteLine("  Relocation Table Information:");
            Console.WriteLine("  -------------------------");
            if (RelocationItems == 0 || RelocationTable.Length == 0)
            {
                Console.WriteLine("  No relocation table items");
            }
            else
            {
                for (int i = 0; i < RelocationTable.Length; i++)
                {
                    var entry = RelocationTable[i];
                    Console.WriteLine($"  Relocation Table Entry {i}");
                    Console.WriteLine($"    Offset: {entry.Offset} (0x{entry.Offset:X})");
                    Console.WriteLine($"    Segment: {entry.Segment} (0x{entry.Segment:X})");
                }
            }
            Console.WriteLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_executable, _jsonSerializerOptions);

#endif

        #endregion
    }
}