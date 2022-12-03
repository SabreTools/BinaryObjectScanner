using System;
using System.IO;

// TODO: Create base class for all wrappers
namespace BurnOutSharp.Wrappers
{
    public class MSDOS
    {
        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Magic"/>
        public byte[] Magic => _executable.Header.Magic;

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

        /// <summary>
        /// Source of the original data
        /// </summary>
        private DataSource _dataSource = DataSource.UNKNOWN;

        /// <summary>
        /// Source byte array data
        /// </summary>
        /// <remarks>This is only populated if <see cref="_dataSource"/> is <see cref="DataSource.ByteArray"/></remarks>
        private byte[] _byteArrayData = null;

        /// <summary>
        /// Source byte array data offset
        /// </summary>
        /// <remarks>This is only populated if <see cref="_dataSource"/> is <see cref="DataSource.ByteArray"/></remarks>
        private int _byteArrayOffset = -1;

        /// <summary>
        /// Source Stream data
        /// </summary>
        /// <remarks>This is only populated if <see cref="_dataSource"/> is <see cref="DataSource.Stream"/></remarks>
        private Stream _streamData = null;

        #endregion

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
            var executable = Builder.MSDOS.ParseExecutable(data, offset);
            if (executable == null)
                return null;

            var wrapper = new MSDOS
            {
                _executable = executable,
                _dataSource = DataSource.ByteArray,
                _byteArrayData = data,
                _byteArrayOffset = offset,
            };
            return wrapper;
        }

        /// <summary>
        /// Create an MS-DOS executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An MS-DOS executable wrapper on success, null on failure</returns>
        public static MSDOS Create(Stream data)
        {
            var executable = Builder.MSDOS.ParseExecutable(data);
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
    
        /// <summary>
        /// Pretty print the MS-DOS executable information
        /// </summary>
        public void Print()
        {
            Console.WriteLine("MS-DOS Executable Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Magic number: {BitConverter.ToString(_executable.Header.Magic).Replace("-", string.Empty)}");
            Console.WriteLine($"  Last page bytes: {_executable.Header.LastPageBytes}");
            Console.WriteLine($"  Pages: {_executable.Header.Pages}");
            Console.WriteLine($"  Relocation items: {_executable.Header.RelocationItems}");
            Console.WriteLine($"  Header paragraph size: {_executable.Header.HeaderParagraphSize}");
            Console.WriteLine($"  Minimum extra paragraphs: {_executable.Header.MinimumExtraParagraphs}");
            Console.WriteLine($"  Maximum extra paragraphs: {_executable.Header.MaximumExtraParagraphs}");
            Console.WriteLine($"  Initial SS value: {_executable.Header.InitialSSValue}");
            Console.WriteLine($"  Initial SP value: {_executable.Header.InitialSPValue}");
            Console.WriteLine($"  Checksum: {_executable.Header.Checksum}");
            Console.WriteLine($"  Initial IP value: {_executable.Header.InitialIPValue}");
            Console.WriteLine($"  Initial CS value: {_executable.Header.InitialCSValue}");
            Console.WriteLine($"  Relocation table address: {_executable.Header.RelocationTableAddr}");
            Console.WriteLine($"  Overlay number: {_executable.Header.OverlayNumber}");

            Console.WriteLine("  Relocation Table Information:");
            Console.WriteLine("  -------------------------");
            if (_executable.Header.RelocationItems == 0 || _executable.RelocationTable.Length == 0)
            {
                Console.WriteLine("  No relocation table items");
            }
            else
            {
                for (int i = 0; i < _executable.RelocationTable.Length; i++)
                {
                    var entry = _executable.RelocationTable[i];
                    Console.WriteLine($"  Relocation Table Entry {i}");
                    Console.WriteLine($"    Offset = {entry.Offset}");
                    Console.WriteLine($"    Segment = {entry.Segment}");
                }
            }
            Console.WriteLine();
        }
    }
}