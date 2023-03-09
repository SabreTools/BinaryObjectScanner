using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Compression;
using BinaryObjectScanner.Interfaces;
using BinaryObjectScanner.Matching;
using BinaryObjectScanner.Wrappers;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace BurnOutSharp.PackerType
{
    // The official website for CExe also includes the source code (which does have to be retrieved by the Wayback Machine)
    // http://www.scottlu.com/Content/CExe.html
    // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
    public class CExe : IExtractable, IPortableExecutableCheck
    {
        /// <inheritdoc/>
        public string CheckPortableExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // If there are exactly 2 resources with type 99
            if (pex.FindResourceByNamedType("99, ").Count() == 2)
                return "CExe";

            var matchers = new List<ContentMatchSet>
            {
                new ContentMatchSet(new byte?[]
                {
                    0x25, 0x57, 0x6F, 0xC1, 0x61, 0x36, 0x01, 0x92,
                    0x61, 0x36, 0x01, 0x92, 0x61, 0x36, 0x01, 0x92,
                    0x61, 0x36, 0x00, 0x92, 0x7B, 0x36, 0x01, 0x92,
                    0x03, 0x29, 0x12, 0x92, 0x66, 0x36, 0x01, 0x92,
                    0x89, 0x29, 0x0A, 0x92, 0x60, 0x36, 0x01, 0x92,
                    0xD9, 0x30, 0x07, 0x92, 0x60, 0x36, 0x01, 0x92
                }, "CExe")
            };

            string match = MatchUtil.GetFirstMatch(file, pex.StubExecutableData, matchers, includeDebug);
            if (!string.IsNullOrWhiteSpace(match))
                return match;

            return null;
        }

        /// <inheritdoc/>
        public string Extract(string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file);
            }
        }

        /// <inheritdoc/>
        public string Extract(Stream stream, string file)
        {
            // Parse into an executable again for easier extraction
            PortableExecutable pex = PortableExecutable.Create(stream);
            if (pex == null)
                return null;

            // Get the first resource of type 99 with index 2
            byte[] payload = pex.FindResourceByNamedType("99, 2").FirstOrDefault();
            if (payload == null || payload.Length == 0)
                return null;

            // Determine which compression was used
            bool zlib = pex.FindResourceByNamedType("99, 1").Any();

            // Create the output data buffer
            byte[] data;

            // If we had the decompression DLL included, it's zlib
            if (zlib)
            {
                try
                {
                    // Inflate the data into the buffer
                    Inflater inflater = new Inflater();
                    inflater.SetInput(payload);
                    data = new byte[payload.Length * 4];
                    int read = inflater.Inflate(data);

                    // Trim the buffer to the proper size
                    data = new ReadOnlySpan<byte>(data, 0, read).ToArray();
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
                    data = LZ.Decompress(payload);
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
    }
}
