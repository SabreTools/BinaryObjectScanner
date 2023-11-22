using System;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Half-Life 2 Level
    /// </summary>
    public class VBSP : IExtractable
    {
        /// <inheritdoc/>
        public string? Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
        public string? Extract(Stream? stream, string file, bool includeDebug)
        {
            try
            {
                // Create the wrapper
                var vbsp = SabreTools.Serialization.Wrappers.VBSP.Create(stream);
                if (vbsp == null)
                    return null;

                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Loop through and extract all files
                ExtractAllLumps(vbsp, tempPath);

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Extract all lumps from the VBSP to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all lumps extracted, false otherwise</returns>
        public static bool ExtractAllLumps(SabreTools.Serialization.Wrappers.VBSP item, string outputDirectory)
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
        /// Extract a lump from the VBSP to an output directory by index
        /// </summary>
        /// <param name="index">Lump index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the lump extracted, false otherwise</returns>
        public static bool ExtractLump(SabreTools.Serialization.Wrappers.VBSP item, int index, string outputDirectory)
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
            switch (index)
            {
                case SabreTools.Models.VBSP.Constants.HL_VBSP_LUMP_ENTITIES:
                    filename = "entities.ent";
                    break;
                case SabreTools.Models.VBSP.Constants.HL_VBSP_LUMP_PAKFILE:
                    filename = "pakfile.zip";
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
    }
}
