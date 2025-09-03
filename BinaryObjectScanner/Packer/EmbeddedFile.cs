using System;
using BinaryObjectScanner.Interfaces;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    /// <summary>
    /// Though not technically a packer, this detection is for any executables that include
    /// archives or executables in their resources in some uncompressed manner to be used at runtime.
    /// </summary>
    public class EmbeddedFile : IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // TODO: Have this return all detected things, not just the first

            // Only process the overlay if it is recognized
            if (pex.ResourceData != null)
            {
                // Cache the resource data for easier reading
                var resourceData = pex.ResourceData;

                // Get the resources that have an archive signature
                foreach (var value in resourceData.Values)
                {
                    if (value == null || value is not byte[] ba || ba.Length == 0)
                        continue;

                    // Set the output variables
                    int resourceOffset = 0;

                    // Only process the resource if it a recognized signature
                    for (; resourceOffset < 0x100 && resourceOffset < ba.Length - 0x10; resourceOffset++)
                    {
                        int temp = resourceOffset;
                        byte[] resourceSample = ba.ReadBytes(ref temp, 0x10);

                        if (resourceSample.StartsWith([0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C]))
                            return "Embedded 7-Zip Archive";
                        else if (resourceSample.StartsWith(SabreTools.Models.MicrosoftCabinet.Constants.SignatureBytes))
                            return "Embedded MS-CAB Archive";
                        else if (resourceSample.StartsWith(SabreTools.Models.PKZIP.Constants.LocalFileHeaderSignatureBytes))
                            return "Embedded PKZIP Archive";
                        else if (resourceSample.StartsWith(SabreTools.Models.PKZIP.Constants.EndOfCentralDirectoryRecordSignatureBytes))
                            return "Embedded PKZIP Archive";
                        else if (resourceSample.StartsWith(SabreTools.Models.PKZIP.Constants.EndOfCentralDirectoryRecord64SignatureBytes))
                            return "Embedded PKZIP Archive";
                        else if (resourceSample.StartsWith(SabreTools.Models.PKZIP.Constants.DataDescriptorSignatureBytes))
                            return "Embedded PKZIP Archive";
                        else if (resourceSample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00]))
                            return "Embedded RAR Archive";
                        else if (resourceSample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00]))
                            return "Embedded RAR Archive";
                        else if (resourceSample.StartsWith(SabreTools.Models.MSDOS.Constants.SignatureBytes))
                            return "Embedded Executable";
                    }
                }
            }

            // Check the overlay, if it exists
            if (pex.OverlayData != null && pex.OverlayData.Length > 0)
            {
                // Set the output variables
                int overlayOffset = 0;

                // Only process the overlay if it is recognized
                for (; overlayOffset < 0x100 && overlayOffset < pex.OverlayData.Length - 0x10; overlayOffset++)
                {
                    int temp = overlayOffset;
                    byte[] overlaySample = pex.OverlayData.ReadBytes(ref temp, 0x10);

                    if (overlaySample.StartsWith([0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C]))
                    {
                        return "Embedded 7-Zip Archive";
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.MicrosoftCabinet.Constants.SignatureBytes))
                    {
                        return "Embedded MS-CAB Archive";
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.PKZIP.Constants.LocalFileHeaderSignatureBytes))
                    {
                        return "Embedded PKZIP Archive";
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.PKZIP.Constants.EndOfCentralDirectoryRecordSignatureBytes))
                    {
                        return "Embedded PKZIP Archive";
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.PKZIP.Constants.EndOfCentralDirectoryRecord64SignatureBytes))
                    {
                        return "Embedded PKZIP Archive";
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.PKZIP.Constants.DataDescriptorSignatureBytes))
                    {
                        return "Embedded PKZIP Archive";
                    }
                    else if (overlaySample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00]))
                    {
                        return "Embedded RAR Archive";
                    }
                    else if (overlaySample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00]))
                    {
                        return "Embedded RAR Archive";
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.MSDOS.Constants.SignatureBytes))
                    {
                        return "Embedded Executable";
                    }
                    else if (overlaySample.StartsWith([0x3B, 0x21, 0x40, 0x49, 0x6E, 0x73, 0x74, 0x61, 0x6C, 0x6C]))
                    {
                        // 7-zip SFX script -- ";!@Install" to ";!@InstallEnd@!"
                        overlayOffset = pex.OverlayData.FirstPosition([0x3B, 0x21, 0x40, 0x49, 0x6E, 0x73, 0x74, 0x61, 0x6C, 0x6C, 0x45, 0x6E, 0x64, 0x40, 0x21]);
                        if (overlayOffset > -1)
                            return "Embedded 7-Zip Archive";
                    }
                }
            }

            return null;
        }
    }
}
