using System;
using System.IO;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// NovaLogic Game Archive Format
    /// </summary>
    public class PFF : IExtractable
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
                var pff = SabreTools.Serialization.Wrappers.PFF.Create(stream);
                if (pff == null)
                    return false;

                // Extract all files
                Directory.CreateDirectory(outDir);
                ExtractAll(pff, outDir);

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Extract all segments from the PFF to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all segments extracted, false otherwise</returns>
        public static bool ExtractAll(SabreTools.Serialization.Wrappers.PFF item, string outputDirectory)
        {
            // If we have no segments
            if (item.Model.Segments == null || item.Model.Segments.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < item.Model.Segments.Length; i++)
            {
                allExtracted &= ExtractSegment(item, i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a segment from the PFF to an output directory by index
        /// </summary>
        /// <param name="index">Segment index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the segment extracted, false otherwise</returns>
        public static bool ExtractSegment(SabreTools.Serialization.Wrappers.PFF item, int index, string outputDirectory)
        {
            // If we have no segments
            if (item.Model.Header?.NumberOfFiles == null || item.Model.Header.NumberOfFiles == 0 || item.Model.Segments == null || item.Model.Segments.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= item.Model.Segments.Length)
                return false;

            // Get the segment information
            var file = item.Model.Segments[index];
            if (file == null)
                return false;

            // Get the read index and length
            int offset = (int)file.FileLocation;
            int size = (int)file.FileSize;

            try
            {
                // Ensure the output directory exists
                Directory.CreateDirectory(outputDirectory);

                // Create the output path
                string filePath = Path.Combine(outputDirectory, file.FileName ?? $"file{index}");
                using (FileStream fs = File.OpenWrite(filePath))
                {
                    // Read the data block
                    var data = item.ReadFromDataSource(offset, size);
                    if (data == null)
                        return false;

                    // Write the data -- TODO: Compressed data?
                    fs.Write(data, 0, size);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
