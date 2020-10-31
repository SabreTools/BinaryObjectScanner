using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpCompress.Compressors;
using SharpCompress.Compressors.Deflate;

namespace BurnOutSharp.FileType
{
    internal class BFPK
    {
        public static bool ShouldScan(byte[] magic)
        {
            if (magic.StartsWith(new byte[] { 0x42, 0x46, 0x50, 0x4b }))
                return true;

            return false;
        }

        public static List<string> Scan(Scanner parentScanner, Stream stream, bool includePosition = false)
        {
            List<string> protections = new List<string>();

            // If the BFPK file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Create a new scanner for the new temp path
                Scanner subScanner = new Scanner(tempPath, parentScanner.FileProgress)
                {
                    IncludePosition = parentScanner.IncludePosition,
                    ScanAllFiles = parentScanner.ScanAllFiles,
                    ScanArchives = parentScanner.ScanArchives,
                };

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
                var fileProtections = ProtectionFind.Scan(tempPath, includePosition);
                protections = fileProtections.Select(kvp => kvp.Key.Substring(tempPath.Length) + ": " + kvp.Value.TrimEnd()).ToList();

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch { }
            }
            catch { }

            return protections;
        }
    }
}
