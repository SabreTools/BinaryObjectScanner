using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using BurnOutSharp.Tools;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

namespace BurnOutSharp.FileType
{
    public class BFPK : IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte?[] { 0x42, 0x46, 0x50, 0x4b }))
                return true;

            return false;
        }

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

                using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
                {
                    br.ReadBytes(4); // Skip magic number

                    int version = br.ReadInt32();
                    int files = br.ReadInt32();
                    long current = br.BaseStream.Position;

                    for (int i = 0; i < files; i++)
                    {
                        br.BaseStream.Seek(current, SeekOrigin.Begin);

                        int nameSize = br.ReadInt32();
                        string name = new string(br.ReadChars(nameSize));

                        uint uncompressedSize = br.ReadUInt32();
                        int offset = br.ReadInt32();

                        current = br.BaseStream.Position;

                        br.BaseStream.Seek(offset, SeekOrigin.Begin);
                        uint compressedSize = br.ReadUInt32();
                        
                        // Some files can lack the length prefix
                        if (compressedSize > br.BaseStream.Length)
                        {
                            br.BaseStream.Seek(-4, SeekOrigin.Current);
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
                                    fs.Write(br.ReadBytes((int)uncompressedSize), 0, (int)uncompressedSize);
                                }
                            }
                            else
                            {
                                using (FileStream fs = File.OpenWrite(tempFile))
                                {
                                    try
                                    {
                                        ZlibStream zs = new ZlibStream(br.BaseStream, CompressionMode.Decompress);
                                        zs.CopyTo(fs);
                                    }
                                    catch (ZlibException)
                                    {
                                        br.BaseStream.Seek(offset + 4, SeekOrigin.Begin);
                                        fs.Write(br.ReadBytes((int)compressedSize), 0, (int)compressedSize);
                                    }
                                }
                            }
                        }
                        catch { }
                        
                        br.BaseStream.Seek(current, SeekOrigin.Begin);
                    }
                }

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch { }

                // Remove temporary path references
                Utilities.StripFromKeys(protections, tempPath);

                return protections;
            }
            catch { }

            return null;
        }
    }
}
