using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// Though not technically a packer, this detection is for any executables that include
    /// archives in their resources in some uncompressed manner to be used at runtime.
    /// </summary>
    public class EmbeddedArchive : IExtractableExecutable<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the resources that have a PKZIP signature
            if (pex.ResourceData != null)
            {
                foreach (var value in pex.ResourceData.Values)
                {
                    if (value == null || value is not byte[] ba)
                        continue;
                    if (!ba.StartsWith(SabreTools.Models.PKZIP.Constants.LocalFileHeaderSignatureBytes))
                        continue;

                    return "Embedded Archive";
                }
            }

            // Check the overlay, if it exists
            if (pex.OverlayData != null && pex.OverlayData.Length > 0)
            {
                if (pex.OverlayData.StartsWith(SabreTools.Models.PKZIP.Constants.LocalFileHeaderSignatureBytes))
                    return "Embedded Archive";
            }

            return null;
        }

        /// <inheritdoc/>
        public bool Extract(string file, PortableExecutable pex, string outDir, bool includeDebug)
        {
            bool overlay = ExtractFromOverlay(pex, outDir, includeDebug);
            bool resources = ExtractFromResources(pex, outDir, includeDebug);
            return overlay || resources;
        }

        /// <summary>
        /// Extract archive data from the overlay
        /// </summary>
        private static bool ExtractFromOverlay(PortableExecutable pex, string outDir, bool includeDebug)
        {
            try
            {
                // Get the overlay data for easier reading
                var overlayData = pex.OverlayData;
                if (overlayData == null)
                    return false;

                // Only process the overlay if it has an archive signature
                if (!overlayData.StartsWith(SabreTools.Models.PKZIP.Constants.LocalFileHeaderSignatureBytes))
                    return false;

                // Create the temp filename
                string tempFile = $"embedded_overlay.zip";
                tempFile = Path.Combine(outDir, tempFile);
                var directoryName = Path.GetDirectoryName(tempFile);
                if (directoryName != null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                // Write the resource data to a temp file
                using var tempStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                tempStream?.Write(overlayData, 0, overlayData.Length);

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Extract archive data from the resources
        /// </summary>
        private static bool ExtractFromResources(PortableExecutable pex, string outDir, bool includeDebug)
        {
            try
            {
                // If there are no resources
                if (pex.ResourceData == null)
                    return false;

                // Get the resources that have an archive signature
                int i = 0;
                foreach (var value in pex.ResourceData.Values)
                {
                    if (value == null || value is not byte[] ba)
                        continue;
                    if (!ba.StartsWith(SabreTools.Models.PKZIP.Constants.LocalFileHeaderSignatureBytes))
                        continue;

                    try
                    {
                        // Create the temp filename
                        string tempFile = $"embedded_resource_{i++}.zip";
                        tempFile = Path.Combine(outDir, tempFile);
                        var directoryName = Path.GetDirectoryName(tempFile);
                        if (directoryName != null && !Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);

                        // Write the resource data to a temp file
                        using var tempStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                        tempStream?.Write(ba, 0, ba.Length);
                    }
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.WriteLine(ex);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return false;
            }
        }
    }
}
