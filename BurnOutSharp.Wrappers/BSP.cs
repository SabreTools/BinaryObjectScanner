using System;
using System.IO;
using System.Linq;
using BurnOutSharp.Utilities;

namespace BurnOutSharp.Wrappers
{
    public class BSP : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.BSP.Header.Version"/>
        public uint Version => _file.Header.Version;

        #endregion

        #region Lumps

        /// <inheritdoc cref="Models.BSP.File.Lumps"/>
        public Models.BSP.Lump[] Lumps => _file.Lumps;

        #endregion

        #region Texture Header

        /// <inheritdoc cref="Models.BSP.TextureHeader.TextureCount"/>
        public uint TextureCount => _file.TextureHeader.TextureCount;

        /// <inheritdoc cref="Models.BSP.TextureHeader.Offsets"/>
        public uint[] Offsets => _file.TextureHeader.Offsets;

        #endregion

        #endregion

        #region Extension Properties

        // TODO: Figure out what extension oroperties are needed

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the BSP
        /// </summary>
        private Models.BSP.File _file;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private BSP() { }

        /// <summary>
        /// Create an BSP from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the BSP</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>An BSP wrapper on success, null on failure</returns>
        public static BSP Create(byte[] data, int offset)
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
        /// Create anVPK from a Stream
        /// </summary>
        /// <param name="data">Stream representing the executable</param>
        /// <returns>An VPK wrapper on success, null on failure</returns>
        public static BSP Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = Builders.BSP.ParseFile(data);
            if (file == null)
                return null;

            var wrapper = new BSP
            {
                _file = file,
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
            Console.WriteLine("BSP Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            PrintHeader();
            PrintLumps();
            PrintTextureHeader();
        }

        /// <summary>
        /// Print header information
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Version: {Version}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print lumps information
        /// </summary>
        private void PrintLumps()
        {
            Console.WriteLine("  Lumps Information:");
            Console.WriteLine("  -------------------------");
            if (Lumps == null || Lumps.Length == 0)
            {
                Console.WriteLine("  No lumps");
            }
            else
            {
                for (int i = 0; i < Lumps.Length; i++)
                {
                    var lump = Lumps[i];
                    string specialLumpName = string.Empty;
                    switch (i)
                    {
                        case Builders.BSP.HL_BSP_LUMP_ENTITIES:
                            specialLumpName = " (entities)";
                            break;
                        case Builders.BSP.HL_BSP_LUMP_TEXTUREDATA:
                            specialLumpName = " (texture data)";
                            break;
                    }

                    Console.WriteLine($"  Lump {i}{specialLumpName}");
                    Console.WriteLine($"    Offset: {lump.Offset}");
                    Console.WriteLine($"    Length: {lump.Length}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print texture header information
        /// </summary>
        private void PrintTextureHeader()
        {
            Console.WriteLine("  Texture Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Texture count: {TextureCount}");
            Console.WriteLine($"  Offsets: {string.Join(", ", Offsets)}");
            Console.WriteLine();
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all files from the VPK to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            return false;
        }

        /// <summary>
        /// Extract a lump from the VPK to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the lump extracted, false otherwise</returns>
        public bool ExtractLump(int index, string outputDirectory)
        {
            return false;
        }

        /// <summary>
        /// Extract a texture from the VPK to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the texture extracted, false otherwise</returns>
        public bool ExtractTexture(int index, string outputDirectory)
        {
            return false;
        }

        #endregion
    }
}