using System;
using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class WAD : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.WAD.Header.Signature"/>
        public string Signature => _file.Header.Signature;

        /// <inheritdoc cref="Models.WAD.Header.LumpCount"/>
        public uint LumpCount => _file.Header.LumpCount;

        /// <inheritdoc cref="Models.WAD.Header.LumpOffset"/>
        public uint LumpOffset => _file.Header.LumpOffset;

        #endregion

        #region Lumps

        /// <inheritdoc cref="Models.WAD.File.Lumps"/>
        public Models.WAD.Lump[] Lumps => _file.Lumps;

        #endregion

        #region Lump Infos

        /// <inheritdoc cref="Models.WAD.File.LumpInfos"/>
        public Models.WAD.LumpInfo[] LumpInfos => _file.LumpInfos;

        #endregion

        #endregion

        #region Extension Properties

        // TODO: Figure out what extension oroperties are needed

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the WAD
        /// </summary>
        private Models.WAD.File _file;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private WAD() { }

        /// <summary>
        /// Create a WAD from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the WAD</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A WAD wrapper on success, null on failure</returns>
        public static WAD Create(byte[] data, int offset)
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
        /// Create a WAD from a Stream
        /// </summary>
        /// <param name="data">Stream representing the WAD</param>
        /// <returns>An WAD wrapper on success, null on failure</returns>
        public static WAD Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var file = Builders.WAD.ParseFile(data);
            if (file == null)
                return null;

            var wrapper = new WAD
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
            Console.WriteLine("WAD Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            PrintHeader();
            PrintLumps();
            PrintLumpInfos();
        }

        /// <summary>
        /// Print header information
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine("  Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Signature: {Signature}");
            Console.WriteLine($"  Lump count: {LumpCount}");
            Console.WriteLine($"  Lump offset: {LumpOffset}");
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
                    Console.WriteLine($"  Lump {i}");
                    Console.WriteLine($"    Offset: {lump.Offset}");
                    Console.WriteLine($"    Disk length: {lump.DiskLength}");
                    Console.WriteLine($"    Length: {lump.Length}");
                    Console.WriteLine($"    Type: {lump.Type}");
                    Console.WriteLine($"    Compression: {lump.Compression}");
                    Console.WriteLine($"    Padding 0: {lump.Padding0}");
                    Console.WriteLine($"    Padding 1: {lump.Padding1}");
                    Console.WriteLine($"    Name: {lump.Name}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print lump infos information
        /// </summary>
        private void PrintLumpInfos()
        {
            Console.WriteLine("  Lump Infos Information:");
            Console.WriteLine("  -------------------------");
            if (LumpInfos == null || LumpInfos.Length == 0)
            {
                Console.WriteLine("  No lump infos");
            }
            else
            {
                for (int i = 0; i < LumpInfos.Length; i++)
                {
                    var lumpInfo = LumpInfos[i];
                    Console.WriteLine($"  Lump Info {i}");
                    if (lumpInfo == null)
                    {
                        Console.WriteLine("    Lump is compressed");
                    }
                    else
                    {
                        Console.WriteLine($"    Name: {lumpInfo.Name ?? "[NULL]"}");
                        Console.WriteLine($"    Width: {lumpInfo.Width}");
                        Console.WriteLine($"    Height: {lumpInfo.Height}");
                        Console.WriteLine($"    Pixel offset: {lumpInfo.PixelOffset}");
                        // TODO: Print unknown data?
                        // TODO: Print pixel data?
                        Console.WriteLine($"    Palette size: {lumpInfo.PaletteSize}");
                        // TODO: Print palette data?
                    }
                }
            }
            Console.WriteLine();
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Extract all lumps from the WAD to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all lumps extracted, false otherwise</returns>
        public bool ExtractAllLumps(string outputDirectory)
        {
            // If we have no lumps
            if (Lumps == null || Lumps.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < Lumps.Length; i++)
            {
                allExtracted &= ExtractLump(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a lump from the WAD to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the lump extracted, false otherwise</returns>
        public bool ExtractLump(int index, string outputDirectory)
        {
            // If we have no lumps
            if (Lumps == null || Lumps.Length == 0)
                return false;

            // If the lumps index is invalid
            if (index < 0 || index >= Lumps.Length)
                return false;

            // Get the lump
            var lump = Lumps[index];
            if (lump == null)
                return false;

            // Read the data -- TODO: Handle uncompressed lumps (see BSP.ExtractTexture)
            byte[] data = ReadFromDataSource((int)lump.Offset, (int)lump.Length);
            if (data == null)
                return false;

            // Create the filename
            string filename = $"{lump.Name}.lmp";

            // If we have an invalid output directory
            if (string.IsNullOrWhiteSpace(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
            Directory.CreateDirectory(Path.GetDirectoryName(filename));

            // Try to write the data
            try
            {
                // Open the output file for writing
                using (Stream fs = File.OpenWrite(filename))
                {
                    fs.Write(data, 0, data.Length);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}