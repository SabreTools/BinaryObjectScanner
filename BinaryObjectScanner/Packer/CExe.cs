using System;
using System.Collections.Generic;
using System.IO;
using BinaryObjectScanner.Interfaces;
using SabreTools.Compression.zlib;
using SabreTools.Matching;
using SabreTools.Matching.Content;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Packer
{
    // The official website for CExe also includes the source code (which does have to be retrieved by the Wayback Machine)
    // http://www.scottlu.com/Content/CExe.html
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class CExe : IExtractableExecutable<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // If there are exactly 2 resources with type 99
            if (pex.FindResourceByNamedType("99, ").Count == 2)
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
        public bool Extract(string file, PortableExecutable pex, string outDir, bool includeDebug)
        {
            try
            {
                // Get all resources of type 99 with index 2
                var resources = pex.FindResourceByNamedType("99, 2");
                if (resources == null || resources.Count == 0)
                    return false;

                // Get the first resource of type 99 with index 2
                var payload = resources[0];
                if (payload == null || payload.Length == 0)
                    return false;

                // Create the output data buffer
                byte[]? data = [];

                // If we had the decompression DLL included, it's zlib
                if (pex.FindResourceByNamedType("99, 1").Count > 0)
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
                                zstream.avail_in = (uint)payload.Length;
                                zstream.total_in = (uint)payload.Length;
                                zstream.next_out = dataPtr;
                                zstream.avail_out = (uint)data.Length;
                                zstream.total_out = 0;

                                ZLib.inflateInit_(zstream, ZLib.zlibVersion(), payload.Length);
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
                    }
                    catch
                    {
                        // Reset the data
                        data = null;
                    }
                }

                // Otherwise, LZ is used
                else
                {
                    try
                    {
                        var decompressor = SabreTools.Compression.SZDD.Decompressor.CreateSZDD(payload);
                        var dataStream = new MemoryStream();
                        decompressor.CopyTo(dataStream);
                        data = dataStream.ToArray();
                    }
                    catch
                    {
                        // Reset the data
                        data = null;
                    }
                }

                // If we have no data
                if (data == null)
                    return false;

                // Create the temp filename
                string tempFile = string.IsNullOrEmpty(file) ? "temp.sxe" : $"{Path.GetFileNameWithoutExtension(file)}.sxe";
                tempFile = Path.Combine(outDir, tempFile);
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
    }
}
