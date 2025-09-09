using System.Collections.Generic;
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
    public class EmbeddedFile : IExecutableCheck<NewExecutable>, IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, NewExecutable exe, bool includeDebug)
        {
            // Create a list for all detected file types
            List<string> embeddedTypes = [];

            // Check the overlay, if it exists
            byte[]? overlayData = exe.OverlayData;
            if (overlayData != null && overlayData.Length > 0)
            {
                // Set the output variables
                int overlayOffset = 0;

                // Only process the overlay if it is recognized
                for (; overlayOffset < 0x400 && overlayOffset < overlayData.Length - 0x10; overlayOffset++)
                {
                    int temp = overlayOffset;
                    byte[] overlaySample = overlayData.ReadBytes(ref temp, 0x10);

                    if (overlaySample.StartsWith([0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C]))
                    {
                        embeddedTypes.Add("Embedded 7-Zip Archive");
                    }
                    else if (overlaySample.StartsWith([0x1F, 0x8B]))
                    {
                        embeddedTypes.Add("Embedded GZip Archive");
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.MicrosoftCabinet.Constants.SignatureBytes))
                    {
                        embeddedTypes.Add("Embedded MS-CAB Archive");
                    }
                    else if (overlaySample.StartsWith([0x42, 0x5A, 0x68]))
                    {
                        embeddedTypes.Add("Embedded BZip2 Archive");
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.PKZIP.Constants.LocalFileHeaderSignatureBytes))
                    {
                        embeddedTypes.Add("Embedded PKZIP Archive");
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.PKZIP.Constants.EndOfCentralDirectoryRecordSignatureBytes))
                    {
                        embeddedTypes.Add("Embedded PKZIP Archive");
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.PKZIP.Constants.EndOfCentralDirectoryRecord64SignatureBytes))
                    {
                        embeddedTypes.Add("Embedded PKZIP Archive");
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.PKZIP.Constants.DataDescriptorSignatureBytes))
                    {
                        embeddedTypes.Add("Embedded PKZIP Archive");
                    }
                    else if (overlaySample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00]))
                    {
                        embeddedTypes.Add("Embedded RAR Archive");
                    }
                    else if (overlaySample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00]))
                    {
                        embeddedTypes.Add("Embedded RAR Archive");
                    }
                    else if (overlaySample.StartsWith([0x55, 0x48, 0x41, 0x06]))
                    {
                        embeddedTypes.Add("Embedded UHARC Archive");
                    }
                    else if (overlaySample.StartsWith([0xFD, 0x37, 0x7A, 0x58, 0x5A, 0x00]))
                    {
                        embeddedTypes.Add("Embedded XZ Archive");
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.MSDOS.Constants.SignatureBytes))
                    {
                        embeddedTypes.Add("Embedded Executable");
                    }
                    else if (overlaySample.StartsWith([0x3B, 0x21, 0x40, 0x49, 0x6E, 0x73, 0x74, 0x61, 0x6C, 0x6C]))
                    {
                        // 7-zip SFX script -- ";!@Install" to ";!@InstallEnd@!"
                        overlayOffset = overlayData.FirstPosition([0x3B, 0x21, 0x40, 0x49, 0x6E, 0x73, 0x74, 0x61, 0x6C, 0x6C, 0x45, 0x6E, 0x64, 0x40, 0x21]);
                        if (overlayOffset > -1)
                            embeddedTypes.Add("Embedded 7-Zip Archive");
                    }
                }
            }

            // If there are no embedded files
            if (embeddedTypes.Count == 0)
                return null;

            return string.Join(";", [.. embeddedTypes]);
        }

        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable exe, bool includeDebug)
        {
            // Create a list for all detected file types
            List<string> embeddedTypes = [];

            // Only process the resources if they are recognized
            var resourceData = exe.ResourceData;
            if (resourceData != null)
            {
                // Get the resources that have an archive signature
                foreach (var value in resourceData.Values)
                {
                    if (value == null || value is not byte[] ba || ba.Length == 0)
                        continue;

                    // Set the output variables
                    int resourceOffset = 0;

                    // Only process the resource if it a recognized signature
                    for (; resourceOffset < 0x400 && resourceOffset < ba.Length - 0x10; resourceOffset++)
                    {
                        int temp = resourceOffset;
                        byte[] resourceSample = ba.ReadBytes(ref temp, 0x10);

                        if (resourceSample.StartsWith([0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C]))
                            embeddedTypes.Add("Embedded 7-Zip Archive");
                        else if (resourceSample.StartsWith([0x42, 0x5A, 0x68]))
                            embeddedTypes.Add("Embedded BZip2 Archive");
                        else if (resourceSample.StartsWith([0x1F, 0x8B]))
                            embeddedTypes.Add("Embedded GZip Archive");
                        else if (resourceSample.StartsWith(SabreTools.Models.MicrosoftCabinet.Constants.SignatureBytes))
                            embeddedTypes.Add("Embedded MS-CAB Archive");
                        else if (resourceSample.StartsWith(SabreTools.Models.PKZIP.Constants.LocalFileHeaderSignatureBytes))
                            embeddedTypes.Add("Embedded PKZIP Archive");
                        else if (resourceSample.StartsWith(SabreTools.Models.PKZIP.Constants.EndOfCentralDirectoryRecordSignatureBytes))
                            embeddedTypes.Add("Embedded PKZIP Archive");
                        else if (resourceSample.StartsWith(SabreTools.Models.PKZIP.Constants.EndOfCentralDirectoryRecord64SignatureBytes))
                            embeddedTypes.Add("Embedded PKZIP Archive");
                        else if (resourceSample.StartsWith(SabreTools.Models.PKZIP.Constants.DataDescriptorSignatureBytes))
                            embeddedTypes.Add("Embedded PKZIP Archive");
                        else if (resourceSample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00]))
                            embeddedTypes.Add("Embedded RAR Archive");
                        else if (resourceSample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00]))
                            embeddedTypes.Add("Embedded RAR Archive");
                        else if (resourceSample.StartsWith([0x55, 0x48, 0x41, 0x06]))
                            embeddedTypes.Add("Embedded UHARC Archive");
                        else if (resourceSample.StartsWith([0xFD, 0x37, 0x7A, 0x58, 0x5A, 0x00]))
                            embeddedTypes.Add("Embedded XZ Archive");
                        else if (resourceSample.StartsWith(SabreTools.Models.MSDOS.Constants.SignatureBytes))
                            embeddedTypes.Add("Embedded Executable");
                    }
                }
            }

            // Check the overlay, if it exists
            byte[]? overlayData = exe.OverlayData;
            if (overlayData != null && overlayData.Length > 0)
            {
                // Set the output variables
                int overlayOffset = 0;

                // Only process the overlay if it is recognized
                for (; overlayOffset < 0x400 && overlayOffset < overlayData.Length - 0x10; overlayOffset++)
                {
                    int temp = overlayOffset;
                    byte[] overlaySample = overlayData.ReadBytes(ref temp, 0x10);

                    if (overlaySample.StartsWith([0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C]))
                    {
                        embeddedTypes.Add("Embedded 7-Zip Archive");
                    }
                    else if (overlaySample.StartsWith([0x42, 0x5A, 0x68]))
                    {
                        embeddedTypes.Add("Embedded BZip2 Archive");
                    }
                    else if (overlaySample.StartsWith([0x1F, 0x8B]))
                    {
                        embeddedTypes.Add("Embedded GZip Archive");
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.MicrosoftCabinet.Constants.SignatureBytes))
                    {
                        embeddedTypes.Add("Embedded MS-CAB Archive");
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.PKZIP.Constants.LocalFileHeaderSignatureBytes))
                    {
                        embeddedTypes.Add("Embedded PKZIP Archive");
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.PKZIP.Constants.EndOfCentralDirectoryRecordSignatureBytes))
                    {
                        embeddedTypes.Add("Embedded PKZIP Archive");
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.PKZIP.Constants.EndOfCentralDirectoryRecord64SignatureBytes))
                    {
                        embeddedTypes.Add("Embedded PKZIP Archive");
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.PKZIP.Constants.DataDescriptorSignatureBytes))
                    {
                        embeddedTypes.Add("Embedded PKZIP Archive");
                    }
                    else if (overlaySample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00]))
                    {
                        embeddedTypes.Add("Embedded RAR Archive");
                    }
                    else if (overlaySample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00]))
                    {
                        embeddedTypes.Add("Embedded RAR Archive");
                    }
                    else if (overlaySample.StartsWith([0x55, 0x48, 0x41, 0x06]))
                    {
                        embeddedTypes.Add("Embedded UHARC Archive");
                    }
                    else if (overlaySample.StartsWith([0xFD, 0x37, 0x7A, 0x58, 0x5A, 0x00]))
                    {
                        embeddedTypes.Add("Embedded XZ Archive");
                    }
                    else if (overlaySample.StartsWith(SabreTools.Models.MSDOS.Constants.SignatureBytes))
                    {
                        embeddedTypes.Add("Embedded Executable");
                    }
                    else if (overlaySample.StartsWith([0x3B, 0x21, 0x40, 0x49, 0x6E, 0x73, 0x74, 0x61, 0x6C, 0x6C]))
                    {
                        // 7-zip SFX script -- ";!@Install" to ";!@InstallEnd@!"
                        overlayOffset = exe.OverlayData.FirstPosition([0x3B, 0x21, 0x40, 0x49, 0x6E, 0x73, 0x74, 0x61, 0x6C, 0x6C, 0x45, 0x6E, 0x64, 0x40, 0x21]);
                        if (overlayOffset > -1)
                            embeddedTypes.Add("Embedded 7-Zip Archive");
                    }
                }
            }

            // If there are no embedded files
            if (embeddedTypes.Count == 0)
                return null;

            return string.Join(";", [.. embeddedTypes]);
        }
    }
}
