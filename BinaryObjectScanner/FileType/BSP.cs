using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Models.BSP;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life Level
    /// </summary>
    public class BSP : IExtractable
    {
        /// <inheritdoc/>
        public bool Extract(string file, string outDir, bool includeDebug)
        {
            if (!File.Exists(file))
                return false;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, outDir, includeDebug);
        }

        /// <inheritdoc/>
        public bool Extract(Stream? stream, string file, string outDir, bool includeDebug)
        {
            try
            {
                // Create the wrapper
                var bsp = SabreTools.Serialization.Wrappers.BSP.Create(stream);
                if (bsp == null)
                    return false;

                // TODO: Introduce helper methods for all specialty lump types

                // Loop through and extract all files
                Directory.CreateDirectory(outDir);
                ExtractAllLumps(bsp, outDir);

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Extract all lumps from the BSP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all lumps extracted, false otherwise</returns>
        public static bool ExtractAllLumps(SabreTools.Serialization.Wrappers.BSP item, string outputDirectory)
        {
            // If we have no lumps
            if (item.Model.Header?.Lumps == null || item.Model.Header.Lumps.Length == 0)
                return false;

            // Loop through and extract all lumps to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.Header.Lumps.Length; i++)
            {
                allExtracted &= ExtractLump(item, i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a lump from the BSP to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the lump extracted, false otherwise</returns>
        public static bool ExtractLump(SabreTools.Serialization.Wrappers.BSP item, int index, string outputDirectory)
        {
            // If we have no lumps
            if (item.Model.Header?.Lumps == null || item.Model.Header.Lumps.Length == 0)
                return false;

            // If the lumps index is invalid
            if (index < 0 || index >= item.Model.Header.Lumps.Length)
                return false;

            // Get the lump
            var lump = item.Model.Header.Lumps[index];
            if (lump == null)
                return false;

            // Read the data
            var data = item.ReadFromDataSource((int)lump.Offset, (int)lump.Length);
            if (data == null)
                return false;

            // Create the filename
            string filename = $"lump_{index}.bin";
            switch ((LumpType)index)
            {
                case LumpType.LUMP_ENTITIES:
                    filename = "entities.ent";
                    break;
                case LumpType.LUMP_TEXTURES:
                    filename = "texture_data.bin";
                    break;
            }

            // If we have an invalid output directory
            if (string.IsNullOrEmpty(outputDirectory))
                return false;

            // Create the full output path
            filename = Path.Combine(outputDirectory, filename);

            // Ensure the output directory is created
            var directoryName = Path.GetDirectoryName(filename);
            if (directoryName != null)
                Directory.CreateDirectory(directoryName);

            // Try to write the data
            try
            {
                // Open the output file for writing
                using Stream fs = File.OpenWrite(filename);
                fs.Write(data, 0, data.Length);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
