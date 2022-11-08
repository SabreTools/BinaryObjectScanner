using System.IO;
using BurnOutSharp.Models.PortableExecutable;

namespace BurnOutSharp.Builder
{
    // TODO: Make Stream Data rely on Byte Data
    public static class PortableExecutable
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a Portable Executable
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled executable on success, null on error</returns>
        public static Executable ParseExecutable(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = offset;

            // Create a new executable to fill
            var executable = new Executable();

            #region MS-DOS Stub

            // Parse the MS-DOS stub
            var stub = MSDOS.ParseExecutable(data, offset);
            if (stub?.Header == null || stub.Header.NewExeHeaderAddr == 0)
                return null;

            // Set the MS-DOS stub
            executable.Stub = stub;

            #endregion

            #region Signature

            offset = (int)(initialOffset + stub.Header.NewExeHeaderAddr);
            executable.Signature = new byte[4];
            for (int i = 0; i < executable.Signature.Length; i++)
            {
                executable.Signature[i] = data.ReadByte(ref offset);
            }
            if (executable.Signature[0] != 'P' || executable.Signature[1] != 'E' || executable.Signature[2] != '\0' || executable.Signature[3] != '\0')
                return null;

            #endregion

            #region COFF File Header

            // Try to parse the COFF file header
            var coffFileHeader = ParseCOFFFileHeader(data, ref offset);
            if (coffFileHeader == null)
                return null;

            // Set the COFF file header
            executable.COFFFileHeader = coffFileHeader;

            #endregion

            // TODO: Implement PE parsing
            return null;
        }

        /// <summary>
        /// Parse a byte array into a Portable Executable COFF file header
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled COFF file header on success, null on error</returns>
        private static COFFFileHeader ParseCOFFFileHeader(byte[] data, ref int offset)
        {
            // TODO: Use marshalling here instead of building
            var fileHeader = new COFFFileHeader();

            fileHeader.Machine = (MachineType)data.ReadUInt16(ref offset);
            fileHeader.NumberOfSections = data.ReadUInt16(ref offset);
            fileHeader.TimeDateStamp = data.ReadUInt32(ref offset);
            fileHeader.PointerToSymbolTable = data.ReadUInt32(ref offset);
            fileHeader.NumberOfSymbols = data.ReadUInt32(ref offset);
            fileHeader.SizeOfOptionalHeader = data.ReadUInt16(ref offset);
            fileHeader.Characteristics = (Characteristics)data.ReadUInt16(ref offset);

            return fileHeader;
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a Portable Executable
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled executable on success, null on error</returns>
        public static Executable ParseExecutable(Stream data)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new executable to fill
            var executable = new Executable();

            #region MS-DOS Stub

            // Parse the MS-DOS stub
            var stub = MSDOS.ParseExecutable(data);
            if (stub?.Header == null || stub.Header.NewExeHeaderAddr == 0)
                return null;

            // Set the MS-DOS stub
            executable.Stub = stub;

            #endregion

            #region Signature

            data.Seek(initialOffset + stub.Header.NewExeHeaderAddr, SeekOrigin.Begin);
            executable.Signature = new byte[4];
            for (int i = 0; i < executable.Signature.Length; i++)
            {
                executable.Signature[i] = data.ReadByteValue();
            }
            if (executable.Signature[0] != 'P' || executable.Signature[1] != 'E' || executable.Signature[2] != '\0' || executable.Signature[3] != '\0')
                return null;

            #endregion

            #region COFF File Header

            // Try to parse the COFF file header
            var coffFileHeader = ParseCOFFFileHeader(data);
            if (coffFileHeader == null)
                return null;

            // Set the COFF file header
            executable.COFFFileHeader = coffFileHeader;

            #endregion

            // TODO: Implement PE parsing
            return null;
        }

        /// <summary>
        /// Parse a Stream into a Portable Executable COFF file header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled executable header on success, null on error</returns>
        private static COFFFileHeader ParseCOFFFileHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            var fileHeader = new COFFFileHeader();

            fileHeader.Machine = (MachineType)data.ReadUInt16();
            fileHeader.NumberOfSections = data.ReadUInt16();
            fileHeader.TimeDateStamp = data.ReadUInt32();
            fileHeader.PointerToSymbolTable = data.ReadUInt32();
            fileHeader.NumberOfSymbols = data.ReadUInt32();
            fileHeader.SizeOfOptionalHeader = data.ReadUInt16();
            fileHeader.Characteristics = (Characteristics)data.ReadUInt16();

            return fileHeader;
        }

        #endregion
    }
}