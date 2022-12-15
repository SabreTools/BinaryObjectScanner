using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using BurnOutSharp.Interfaces;
using BurnOutSharp.Tools;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// BFPK custom archive format
    /// </summary>
    public class BFPK : IScannable
    {
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the BFPK file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                stream.ReadBytes(4); // Skip magic number

                int version = stream.ReadInt32();
                int files = stream.ReadInt32();
                long current = stream.Position;

                for (int i = 0; i < files; i++)
                {
                    stream.Seek(current, SeekOrigin.Begin);

                    int nameSize = stream.ReadInt32();
                    string name = new string(stream.ReadBytes(nameSize).Select(b => (char)b).ToArray());

                    uint uncompressedSize = stream.ReadUInt32();
                    int offset = stream.ReadInt32();

                    current = stream.Position;

                    stream.Seek(offset, SeekOrigin.Begin);
                    uint compressedSize = stream.ReadUInt32();

                    // Some files can lack the length prefix
                    if (compressedSize > stream.Length)
                    {
                        stream.Seek(-4, SeekOrigin.Current);
                        compressedSize = uncompressedSize;
                    }

                    // If an individual entry fails
                    try
                    {
                        string tempFile = Path.Combine(tempPath, name);
                        if (!Directory.Exists(Path.GetDirectoryName(tempFile)))
                            Directory.CreateDirectory(Path.GetDirectoryName(tempFile));

                        if (compressedSize == uncompressedSize)
                        {
                            using (FileStream fs = File.OpenWrite(tempFile))
                            {
                                fs.Write(stream.ReadBytes((int)uncompressedSize), 0, (int)uncompressedSize);
                            }
                        }
                        else
                        {
                            using (FileStream fs = File.OpenWrite(tempFile))
                            {
                                try
                                {
                                    ZlibStream zs = new ZlibStream(stream, CompressionMode.Decompress);
                                    zs.CopyTo(fs);
                                }
                                catch (ZlibException)
                                {
                                    stream.Seek(offset + 4, SeekOrigin.Begin);
                                    fs.Write(stream.ReadBytes((int)compressedSize), 0, (int)compressedSize);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (scanner.IncludeDebug) Console.WriteLine(ex);
                    }

                    stream.Seek(current, SeekOrigin.Begin);
                }

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch (Exception ex)
                {
                    if (scanner.IncludeDebug) Console.WriteLine(ex);
                }

                // Remove temporary path references
                Utilities.StripFromKeys(protections, tempPath);

                return protections;
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }
    }
}
