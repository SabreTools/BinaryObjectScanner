using System;
using System.IO;
using SabreTools.IO.Compression.zlib;
using SabreTools.IO.Extensions;
using SabreTools.Matching;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Wrappers
{
    public partial class PortableExecutable : IExtractable
    {
        /// <inheritdoc/>
        /// <remarks>
        /// This extracts the following data:
        /// - Archives and executables in the overlay
        /// - Archives and executables in resource data
        /// - CExe-compressed resource data
        /// - SFX archives (7z, MS-CAB, PKZIP, RAR)
        /// - Wise installers
        /// </remarks>
        public bool Extract(string outputDirectory, bool includeDebug)
        {
            bool cexe = ExtractCExe(outputDirectory, includeDebug);
            bool overlay = ExtractFromOverlay(outputDirectory, includeDebug);
            bool resources = ExtractFromResources(outputDirectory, includeDebug);
            bool wise = ExtractWise(outputDirectory, includeDebug);
            bool matroschka = ExtractMatroschka(outputDirectory, includeDebug);
            // TODO: extract matroschka here

            return cexe || overlay || resources | wise | matroschka;
        }

        /// <summary>
        /// Extract a CExe-compressed executable
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if extraction succeeded, false otherwise</returns>
        public bool ExtractCExe(string outputDirectory, bool includeDebug)
        {
            try
            {
                // Get all resources of type 99 with index 2
                var resources = FindResourceByNamedType("99, 2");
                if (resources == null || resources.Count == 0)
                    return false;

                // Get the first resource of type 99 with index 2
                var resource = resources[0];
                if (resource == null || resource.Length == 0)
                    return false;

                // Create the output data buffer
                byte[]? data = [];

                // If we had the decompression DLL included, it's zlib
                if (FindResourceByNamedType("99, 1").Count > 0)
                    data = DecompressCExeZlib(resource);
                else
                    data = DecompressCExeLZ(resource);

                // If we have no data
                if (data == null)
                    return false;

                // Create the temp filename
                string tempFile = string.IsNullOrEmpty(Filename) ? "temp.sxe" : $"{Path.GetFileNameWithoutExtension(Filename)}.sxe";
                tempFile = Path.Combine(outputDirectory, tempFile);
                var directoryName = Path.GetDirectoryName(tempFile);
                if (directoryName != null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                // Write the file data to a temp file
                var tempStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                tempStream.Write(data, 0, data.Length);

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Extract data from the overlay
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if extraction succeeded, false otherwise</returns>
        public bool ExtractFromOverlay(string outputDirectory, bool includeDebug)
        {
            try
            {
                // Cache the overlay data for easier reading
                var overlayData = OverlayData;
                if (overlayData.Length == 0)
                    return false;

                // Set the output variables
                int overlayOffset = 0;
                string extension = string.Empty;

                // Only process the overlay if it is recognized
                for (; overlayOffset < 0x400 && overlayOffset < overlayData.Length - 0x10; overlayOffset++)
                {
                    int temp = overlayOffset;
                    byte[] overlaySample = overlayData.ReadBytes(ref temp, 0x10);

                    if (overlaySample.StartsWith([0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C]))
                    {
                        extension = "7z";
                        break;
                    }
                    else if (overlaySample.StartsWith([0x3B, 0x21, 0x40, 0x49, 0x6E, 0x73, 0x74, 0x61, 0x6C, 0x6C]))
                    {
                        // 7-zip SFX script -- ";!@Install" to ";!@InstallEnd@!"
                        overlayOffset = overlayData.FirstPosition([0x3B, 0x21, 0x40, 0x49, 0x6E, 0x73, 0x74, 0x61, 0x6C, 0x6C, 0x45, 0x6E, 0x64, 0x40, 0x21]);
                        if (overlayOffset == -1)
                            return false;

                        overlayOffset += 15;
                        extension = "7z";
                        break;
                    }
                    else if (overlaySample.StartsWith([0x42, 0x5A, 0x68]))
                    {
                        extension = "bz2";
                        break;
                    }
                    else if (overlaySample.StartsWith([0x1F, 0x8B]))
                    {
                        extension = "gz";
                        break;
                    }
                    else if (overlaySample.StartsWith(Models.MicrosoftCabinet.Constants.SignatureBytes))
                    {
                        extension = "cab";
                        break;
                    }
                    else if (overlaySample.StartsWith(Models.PKZIP.Constants.LocalFileHeaderSignatureBytes))
                    {
                        extension = "zip";
                        break;
                    }
                    else if (overlaySample.StartsWith(Models.PKZIP.Constants.EndOfCentralDirectoryRecordSignatureBytes))
                    {
                        extension = "zip";
                        break;
                    }
                    else if (overlaySample.StartsWith(Models.PKZIP.Constants.EndOfCentralDirectoryRecord64SignatureBytes))
                    {
                        extension = "zip";
                        break;
                    }
                    else if (overlaySample.StartsWith(Models.PKZIP.Constants.DataDescriptorSignatureBytes))
                    {
                        extension = "zip";
                        break;
                    }
                    else if (overlaySample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00]))
                    {
                        extension = "rar";
                        break;
                    }
                    else if (overlaySample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00]))
                    {
                        extension = "rar";
                        break;
                    }
                    else if (overlaySample.StartsWith([0x55, 0x48, 0x41, 0x06]))
                    {
                        extension = "uha";
                        break;
                    }
                    else if (overlaySample.StartsWith([0xFD, 0x37, 0x7A, 0x58, 0x5A, 0x00]))
                    {
                        extension = "xz";
                        break;
                    }
                    else if (overlaySample.StartsWith(Models.MSDOS.Constants.SignatureBytes))
                    {
                        extension = "bin"; // exe/dll
                        break;
                    }
                }

                // If the extension is unset
                if (extension.Length == 0)
                    return false;

                // Create the temp filename
                string tempFile = $"embedded_overlay.{extension}";
                if (Filename != null)
                    tempFile = $"{Path.GetFileName(Filename)}-{tempFile}";

                tempFile = Path.Combine(outputDirectory, tempFile);
                var directoryName = Path.GetDirectoryName(tempFile);
                if (directoryName != null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                // Write the resource data to a temp file
                using var tempStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                tempStream?.Write(overlayData, overlayOffset, overlayData.Length - overlayOffset);

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Extract data from the resources
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if extraction succeeded, false otherwise</returns>
        public bool ExtractFromResources(string outputDirectory, bool includeDebug)
        {
            try
            {
                // Cache the resource data for easier reading
                var resourceData = ResourceData;
                if (resourceData.Count == 0)
                    return false;

                // Get the resources that have an archive signature
                int i = 0;
                foreach (var kvp in resourceData)
                {
                    // Get the key and value
                    string resourceKey = kvp.Key;
                    var value = kvp.Value;

                    if (value == null || value is not byte[] ba || ba.Length == 0)
                        continue;

                    // Set the output variables
                    int resourceOffset = 0;
                    string extension = string.Empty;

                    // Only process the resource if it a recognized signature
                    for (; resourceOffset < 0x400 && resourceOffset < ba.Length - 0x10; resourceOffset++)
                    {
                        int temp = resourceOffset;
                        byte[] resourceSample = ba.ReadBytes(ref temp, 0x10);

                        if (resourceSample.StartsWith([0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C]))
                        {
                            extension = "7z";
                            break;
                        }
                        else if (resourceSample.StartsWith([0x42, 0x4D]))
                        {
                            extension = "bmp";
                            break;
                        }
                        else if (resourceSample.StartsWith([0x42, 0x5A, 0x68]))
                        {
                            extension = "bz2";
                            break;
                        }
                        else if (resourceSample.StartsWith([0x47, 0x49, 0x46, 0x38]))
                        {
                            extension = "gif";
                            break;
                        }
                        else if (resourceSample.StartsWith([0x1F, 0x8B]))
                        {
                            extension = "gz";
                            break;
                        }
                        else if (resourceSample.StartsWith([0xFF, 0xD8, 0xFF, 0xE0]))
                        {
                            extension = "jpg";
                            break;
                        }
                        else if (resourceSample.StartsWith([0x3C, 0x68, 0x74, 0x6D, 0x6C]))
                        {
                            extension = "html";
                            break;
                        }
                        else if (resourceSample.StartsWith(Models.MicrosoftCabinet.Constants.SignatureBytes))
                        {
                            extension = "cab";
                            break;
                        }
                        else if (resourceSample.StartsWith(Models.PKZIP.Constants.LocalFileHeaderSignatureBytes))
                        {
                            extension = "zip";
                            break;
                        }
                        else if (resourceSample.StartsWith(Models.PKZIP.Constants.EndOfCentralDirectoryRecordSignatureBytes))
                        {
                            extension = "zip";
                            break;
                        }
                        else if (resourceSample.StartsWith(Models.PKZIP.Constants.EndOfCentralDirectoryRecord64SignatureBytes))
                        {
                            extension = "zip";
                            break;
                        }
                        else if (resourceSample.StartsWith(Models.PKZIP.Constants.DataDescriptorSignatureBytes))
                        {
                            extension = "zip";
                            break;
                        }
                        else if (resourceSample.StartsWith([0x89, 0x50, 0x4E, 0x47]))
                        {
                            extension = "png";
                            break;
                        }
                        else if (resourceSample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00]))
                        {
                            extension = "rar";
                            break;
                        }
                        else if (resourceSample.StartsWith([0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00]))
                        {
                            extension = "rar";
                            break;
                        }
                        else if (resourceSample.StartsWith([0x55, 0x48, 0x41, 0x06]))
                        {
                            extension = "uha";
                            break;
                        }
                        else if (resourceSample.StartsWith([0xFD, 0x37, 0x7A, 0x58, 0x5A, 0x00]))
                        {
                            extension = "xz";
                            break;
                        }
                        else if (resourceSample.StartsWith(Models.MSDOS.Constants.SignatureBytes))
                        {
                            extension = "bin"; // exe/dll
                            break;
                        }
                    }

                    // If the extension is unset
                    if (extension.Length == 0)
                        continue;

                    try
                    {
                        // Create the temp filename
                        string tempFile = $"embedded_resource_{i++} ({resourceKey}).{extension}";
                        if (Filename != null)
                            tempFile = $"{Path.GetFileName(Filename)}-{tempFile}";

                        tempFile = Path.Combine(outputDirectory, tempFile);
                        var directoryName = Path.GetDirectoryName(tempFile);
                        if (directoryName != null && !Directory.Exists(directoryName))
                            Directory.CreateDirectory(directoryName);

                        // Write the resource data to a temp file
                        using var tempStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                        tempStream?.Write(ba, resourceOffset, ba.Length - resourceOffset);
                    }
                    catch (Exception ex)
                    {
                        if (includeDebug) Console.Error.WriteLine(ex);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.Error.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Extract data from a Wise installer
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if extraction succeeded, false otherwise</returns>
        public bool ExtractWise(string outputDirectory, bool includeDebug)
        {
            // Get the source data for reading
            Stream source = _dataSource;
            if (Filename != null)
            {
                // Try to open a multipart file
                if (WiseOverlayHeader.OpenFile(Filename, includeDebug, out var temp) && temp != null)
                    source = temp;
            }

            // Try to find the overlay header
            long offset = FindWiseOverlayHeader();
            if (offset > 0 && offset < Length)
                return ExtractWiseOverlay(outputDirectory, includeDebug, source, offset);

            // Try to find the section header
            var section = FindWiseSection();
            if (section != null)
                return ExtractWiseSection(outputDirectory, includeDebug, source, section);

            // Everything else could not extract
            return false;
        }
        
        /// <summary>
        /// Extract data from a SecuROM Matroschka Package
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <returns>True if extraction succeeded, false otherwise</returns>
        public bool ExtractMatroschka(string outputDirectory, bool includeDebug)
        {
            // Check if executable contains Matroschka package or not
            if (MatroschkaPackage == null)
                return false;

            // Attempt to extract section
            return MatroschkaPackage.ExtractHeaderDefinedFiles(outputDirectory, includeDebug, MatroschkaPackageFileData);
        }

        /// <summary>
        /// Decompress CExe data compressed with LZ
        /// </summary>
        /// <param name="resource">Resource data to inflate</param>
        /// <returns>Inflated data on success, null otherwise</returns>
        private static byte[]? DecompressCExeLZ(byte[] resource)
        {
            try
            {
                var decompressor = IO.Compression.SZDD.Decompressor.CreateSZDD(resource);
                using var dataStream = new MemoryStream();
                decompressor.CopyTo(dataStream);
                return dataStream.ToArray();
            }
            catch
            {
                // Reset the data
                return null;
            }
        }

        /// <summary>
        /// Decompress CExe data compressed with zlib
        /// </summary>
        /// <param name="resource">Resource data to inflate</param>
        /// <returns>Inflated data on success, null otherwise</returns>
        private static byte[]? DecompressCExeZlib(byte[] resource)
        {
            try
            {
                // Inflate the data into the buffer
                var zstream = new ZLib.z_stream_s();
                byte[] data = new byte[resource.Length * 4];
                unsafe
                {
                    fixed (byte* payloadPtr = resource)
                    fixed (byte* dataPtr = data)
                    {
                        zstream.next_in = payloadPtr;
                        zstream.avail_in = (uint)resource.Length;
                        zstream.total_in = (uint)resource.Length;
                        zstream.next_out = dataPtr;
                        zstream.avail_out = (uint)data.Length;
                        zstream.total_out = 0;

                        ZLib.inflateInit_(zstream, ZLib.zlibVersion(), resource.Length);
                        int zret = ZLib.inflate(zstream, 1);
                        ZLib.inflateEnd(zstream);
                    }
                }

                // Trim the buffer to the proper size
                uint read = zstream.total_out;
#if NETFRAMEWORK
                var temp = new byte[read];
                Array.Copy(data, temp, read);
                data = temp;
#else
                data = new ReadOnlySpan<byte>(data, 0, (int)read).ToArray();
#endif
                return data;
            }
            catch
            {
                // Reset the data
                return null;
            }
        }

        /// <summary>
        /// Extract using Wise overlay
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <param name="source">Potentially multi-part stream to read</param>
        /// <param name="offset">Offset to the start of the overlay header</param>
        /// <returns>True if extraction succeeded, false otherwise</returns>
        private bool ExtractWiseOverlay(string outputDirectory, bool includeDebug, Stream source, long offset)
        {
            // Seek to the overlay and parse
            source.Seek(offset, SeekOrigin.Begin);
            var header = WiseOverlayHeader.Create(source);
            if (header == null)
            {
                if (includeDebug) Console.Error.WriteLine("Could not parse the overlay header");
                return false;
            }

            // Extract the header-defined files
            bool extracted = header.ExtractHeaderDefinedFiles(outputDirectory, includeDebug);
            if (!extracted)
            {
                if (includeDebug) Console.Error.WriteLine("Could not extract header-defined files");
                return false;
            }

            // Open the script file from the output directory
            var scriptStream = File.OpenRead(Path.Combine(outputDirectory, "WiseScript.bin"));
            var script = WiseScript.Create(scriptStream);
            if (script == null)
            {
                if (includeDebug) Console.Error.WriteLine("Could not parse WiseScript.bin");
                return false;
            }

            // Get the source directory
            string? sourceDirectory = null;
            if (Filename != null)
                sourceDirectory = Path.GetDirectoryName(Path.GetFullPath(Filename));

            // Process the state machine
            return script.ProcessStateMachine(header, sourceDirectory, outputDirectory, includeDebug);
        }

        /// <summary>
        /// Extract using Wise section
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <param name="includeDebug">True to include debug data, false otherwise</param>
        /// <param name="source">Potentially multi-part stream to read</param>
        /// <param name="section">Wise section information</param>
        /// <returns>True if extraction succeeded, false otherwise</returns>
        private bool ExtractWiseSection(string outputDirectory, bool includeDebug, Stream source, Models.PortableExecutable.SectionHeader section)
        {
            // Get the offset
            long offset = section.VirtualAddress.ConvertVirtualAddress(SectionTable);
            if (offset < 0 || offset >= source.Length)
                return false;

            // Read the section into a local array
            int sectionLength = (int)section.VirtualSize;
            byte[]? sectionData;
            lock (source)
            {
                sectionData = source.ReadFrom(offset, sectionLength, retainPosition: true);
            }

            // Parse the section header
            var header = WiseSectionHeader.Create(sectionData, 0);
            if (header == null)
            {
                if (includeDebug) Console.Error.WriteLine("Could not parse the section header");
                return false;
            }

            // Attempt to extract section
            return header.Extract(outputDirectory, includeDebug);
        }
    }
}
