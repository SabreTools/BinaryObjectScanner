using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class PortableExecutable
    {
        #region Pass-Through Properties

        #region MS-DOS Stub

        #region Standard Fields

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Magic"/>
        public byte[] Stub_Magic => _executable.Stub.Header.Magic;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.LastPageBytes"/>
        public ushort Stub_LastPageBytes => _executable.Stub.Header.LastPageBytes;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Pages"/>
        public ushort Stub_Pages => _executable.Stub.Header.Pages;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationItems"/>
        public ushort Stub_RelocationItems => _executable.Stub.Header.RelocationItems;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.HeaderParagraphSize"/>
        public ushort Stub_HeaderParagraphSize => _executable.Stub.Header.HeaderParagraphSize;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MinimumExtraParagraphs"/>
        public ushort Stub_MinimumExtraParagraphs => _executable.Stub.Header.MinimumExtraParagraphs;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.MaximumExtraParagraphs"/>
        public ushort Stub_MaximumExtraParagraphs => _executable.Stub.Header.MaximumExtraParagraphs;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSSValue"/>
        public ushort Stub_InitialSSValue => _executable.Stub.Header.InitialSSValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialSPValue"/>
        public ushort Stub_Stub_InitialSPValue => _executable.Stub.Header.InitialSPValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Checksum"/>
        public ushort Stub_Checksum => _executable.Stub.Header.Checksum;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialIPValue"/>
        public ushort Stub_InitialIPValue => _executable.Stub.Header.InitialIPValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.InitialCSValue"/>
        public ushort Stub_InitialCSValue => _executable.Stub.Header.InitialCSValue;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.RelocationTableAddr"/>
        public ushort Stub_RelocationTableAddr => _executable.Stub.Header.RelocationTableAddr;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OverlayNumber"/>
        public ushort Stub_OverlayNumber => _executable.Stub.Header.OverlayNumber;

        #endregion

        #region PE Extensions

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved1"/>
        public ushort[] Stub_Reserved1 => _executable.Stub.Header.Reserved1;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMIdentifier"/>
        public ushort Stub_OEMIdentifier => _executable.Stub.Header.OEMIdentifier;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.OEMInformation"/>
        public ushort Stub_OEMInformation => _executable.Stub.Header.OEMInformation;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.Reserved2"/>
        public ushort[] Stub_Reserved2 => _executable.Stub.Header.Reserved2;

        /// <inheritdoc cref="Models.MSDOS.ExecutableHeader.NewExeHeaderAddr"/>
        public uint Stub_NewExeHeaderAddr => _executable.Stub.Header.NewExeHeaderAddr;

        #endregion

        #endregion

        // TODO: Determine what properties can be passed through

        #endregion

        #region Extension Properties

        // TODO: Determine what extension properties are needed

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the executable
        /// </summary>
        private Models.PortableExecutable.Executable _executable;

        #endregion

        /// <summary>
        /// Private constructor
        /// </summary>
        private PortableExecutable() { }

        /// <summary>
        /// Create a PE executable from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the executable</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A PE executable wrapper on success, null on failure</returns>
        public static PortableExecutable Create(byte[] data, int offset)
        {
            var executable = Builder.PortableExecutable.ParseExecutable(data, offset);
            if (executable == null)
                return null;

            var wrapper = new PortableExecutable { _executable = executable };
            return wrapper;
        }

        /// <summary>
        /// Create a PE executable from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>A PE executable wrapper on success, null on failure</returns>
        public static PortableExecutable Create(Stream data)
        {
            var executable = Builder.PortableExecutable.ParseExecutable(data);
            if (executable == null)
                return null;

            var wrapper = new PortableExecutable { _executable = executable };
            return wrapper;
        }
    }
}