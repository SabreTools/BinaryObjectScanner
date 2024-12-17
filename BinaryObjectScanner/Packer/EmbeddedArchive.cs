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
            // Get the resources that have a PKZIP signature
            if (pex.ResourceData != null)
            {
                foreach (var value in pex.ResourceData.Values)
                {
                    if (value == null || value is not byte[] ba)
                        continue;

                    if (ba.StartsWith([0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C]))
                        return "Embedded 7-Zip Archive";
                    if (ba.StartsWith([0x50, 0x4B, 0x03, 0x04]))
                        return "Embedded PKZIP Archive";
                    if (ba.StartsWith([0x50, 0x4B, 0x05, 0x06]))
                        return "Embedded PKZIP Archive";
                    if (ba.StartsWith([0x50, 0x4B, 0x07, 0x08]))
                        return "Embedded PKZIP Archive";
                    if (ba.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00]))
                        return "Embedded RAR Archive";
                    if (ba.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00]))
                        return "Embedded RAR Archive";
                }
            }

            // Check the overlay, if it exists
            if (pex.OverlayData != null && pex.OverlayData.Length > 0)
            {
                if (pex.OverlayData.StartsWith([0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C]))
                    return "Embedded 7-Zip Archive";
                if (pex.OverlayData.StartsWith([0x50, 0x4B, 0x03, 0x04]))
                    return "Embedded PKZIP Archive";
                if (pex.OverlayData.StartsWith([0x50, 0x4B, 0x05, 0x06]))
                    return "Embedded PKZIP Archive";
                if (pex.OverlayData.StartsWith([0x50, 0x4B, 0x07, 0x08]))
                    return "Embedded PKZIP Archive";
                if (pex.OverlayData.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00]))
                    return "Embedded RAR Archive";
                if (pex.OverlayData.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00]))
                    return "Embedded RAR Archive";
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
                string extension = string.Empty;
                if (overlayData.StartsWith([0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C]))
                    extension = "7z";
                else if (overlayData.StartsWith([0x50, 0x4B, 0x03, 0x04]))
                    extension = "zip";
                else if (overlayData.StartsWith([0x50, 0x4B, 0x05, 0x06]))
                    extension = "zip";
                else if (overlayData.StartsWith([0x50, 0x4B, 0x07, 0x08]))
                    extension = "zip";
                else if (overlayData.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00]))
                    extension = "rar";
                else if (overlayData.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00]))
                    extension = "rar";
                else
                    return false;

                // Create the temp filename
                string tempFile = $"embedded_overlay.{extension}";
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

                    // Only process the resource if it has an archive signature
                    string extension = string.Empty;
                    if (ba.StartsWith([0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C]))
                        extension = "7z";
                    else if (ba.StartsWith([0x50, 0x4B, 0x03, 0x04]))
                        extension = "zip";
                    else if (ba.StartsWith([0x50, 0x4B, 0x05, 0x06]))
                        extension = "zip";
                    else if (ba.StartsWith([0x50, 0x4B, 0x07, 0x08]))
                        extension = "zip";
                    else if (ba.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00]))
                        extension = "rar";
                    else if (ba.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00]))
                        extension = "rar";
                    else
                        continue;

                    try
                    {
                        // Create the temp filename
                        string tempFile = $"embedded_resource_{i++}.{extension}";
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
