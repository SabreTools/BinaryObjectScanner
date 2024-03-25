using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;
using SabreTools.Compression.zlib;
using SabreTools.Matching;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // The official website for CExe also includes the source code (which does have to be retrieved by the Wayback Machine)
    // http://www.scottlu.com/Content/CExe.html
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class CExe : IExtractable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string? CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // If there are exactly 2 resources with type 99
            if (pex.FindResourceByNamedType("99, ").Count() == 2)
                return "CExe";

            if (pex.StubExecutableData != null)
            {
                var matchers = new List<ContentMatchSet>
            {
                new(new byte?[]
                {
                    0x25, 0x57, 0x6F, 0xC1, 0x61, 0x36, 0x01, 0x92,
                    0x61, 0x36, 0x01, 0x92, 0x61, 0x36, 0x01, 0x92,
                    0x61, 0x36, 0x00, 0x92, 0x7B, 0x36, 0x01, 0x92,
                    0x03, 0x29, 0x12, 0x92, 0x66, 0x36, 0x01, 0x92,
                    0x89, 0x29, 0x0A, 0x92, 0x60, 0x36, 0x01, 0x92,
                    0xD9, 0x30, 0x07, 0x92, 0x60, 0x36, 0x01, 0x92
                }, "CExe")
            };

                var match = MatchUtil.GetFirstMatch(file, pex.StubExecutableData, matchers, includeDebug);
                if (!string.IsNullOrEmpty(match))
                    return match;
            }

            return null;
        }

        /// <inheritdoc/>
        public string? Extract(string file, bool includeDebug)
        {
            if (!File.Exists(file))
                return null;

            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return Extract(fs, file, includeDebug);
        }

        /// <inheritdoc/>
        public string? Extract(Stream? stream, string file, bool includeDebug)
        {
            try
            {
                // Parse into an executable again for easier extraction
                var pex = PortableExecutable.Create(stream);
                if (pex == null)
                    return null;

                // Get the first resource of type 99 with index 2
                var payload = pex.FindResourceByNamedType("99, 2").FirstOrDefault();
                if (payload == null || payload.Length == 0)
                    return null;

                // Determine which compression was used
                bool zlib = pex.FindResourceByNamedType("99, 1").Any();

                // Create the output data buffer
                var data = new byte[0];

                // If we had the decompression DLL included, it's zlib
                if (zlib)
                {
                    try
                    {
                        // Inflate the data into the buffer
                        var zstream = new ZLib.z_stream_s();
                        data = new byte[payload.Length * 4];
                        unsafe
                        {
                            fixed (byte* payloadPtr = payload)
                            fixed (byte* dataPtr = data)
                            {
                                zstream.next_in = payloadPtr;
                                zstream.next_out = dataPtr;
                                int zret = ZLib.inflate(zstream, 1);
                            }
                        }

                        // Trim the buffer to the proper size
                        uint read = zstream.total_out;
#if NET462_OR_GREATER || NETCOREAPP
                        data = new ReadOnlySpan<byte>(data, 0, (int)read).ToArray();
#else
                        var temp = new byte[read];
                        Array.Copy(data, 0, temp, 0, read);
                        data = temp;
#endif
                    }
                    catch
                    {
                        // Reset the data
                        data = null;
                    }
                }

                // Otherwise, LZ is used via the Windows API
                else
                {
                    try
                    {
                        data = SabreTools.Compression.LZ.Decompressor.Decompress(payload);
                    }
                    catch
                    {
                        // Reset the data
                        data = null;
                    }
                }

                // If we have no data
                if (data == null)
                    return null;

                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Create the temp filename
                string tempFile = string.IsNullOrEmpty(file) ? "temp.sxe" : $"{Path.GetFileNameWithoutExtension(file)}.sxe";
                tempFile = Path.Combine(tempPath, tempFile);

                // Write the file data to a temp file
                using (Stream tempStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    tempStream.Write(data, 0, data.Length);
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
        }
    }
}
