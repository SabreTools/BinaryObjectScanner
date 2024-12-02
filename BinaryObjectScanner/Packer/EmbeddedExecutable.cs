using System;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// Though not technically a packer, this detection is for any executables that include
    /// others in their resources in some uncompressed manner to be used at runtime.
    /// </summary>
    public class EmbeddedExecutable : IExtractableExecutable<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the resources that have an executable signature
            if (pex.ResourceData != null)
            {
                foreach (var value in pex.ResourceData.Values)
                {
                    if (value == null || value is not byte[] ba)
                        continue;
                    if (!ba.StartsWith(SabreTools.Models.MSDOS.Constants.SignatureBytes))
                        continue;

                    return "Embedded Executable";
                }
            }

            // Check the overlay, if it exists
            if (pex.OverlayData != null && pex.OverlayData.Length > 0)
            {
                if (pex.OverlayData.StartsWith(SabreTools.Models.MSDOS.Constants.SignatureBytes))
                    return "Embedded Executable";
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
        /// Extract executable data from the overlay
        /// </summary>
        private static bool ExtractFromOverlay(PortableExecutable pex, string outDir, bool includeDebug)
        {
            try
            {
                // Get the overlay data for easier reading
                var overlayData = pex.OverlayData;
                if (overlayData == null)
                    return false;

                // Only process the overlay if it has an executable signature
                if (!overlayData.StartsWith(SabreTools.Models.MSDOS.Constants.SignatureBytes))
                    return false;

                // Create the temp filename
                string tempFile = $"embedded_overlay.bin"; // exe/dll
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
        /// Extract executable data from the resources
        /// </summary>
        private static bool ExtractFromResources(PortableExecutable pex, string outDir, bool includeDebug)
        {
            try
            {
                // If there are no resources
                if (pex.ResourceData == null)
                    return false;

                // Get the resources that have an executable signature
                int i = 0;
                foreach (var value in pex.ResourceData.Values)
                {
                    if (value == null || value is not byte[] ba)
                        continue;
                    if (!ba.StartsWith(SabreTools.Models.MSDOS.Constants.SignatureBytes))
                        continue;

                    try
                    {
                        // Create the temp filename
                        string tempFile = $"embedded_resource_{i++}.bin"; // exe/dll
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
