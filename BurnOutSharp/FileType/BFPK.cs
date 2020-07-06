using System;
using System.Collections.Generic;
using System.IO;
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

        public static List<string> Scan(Stream stream)
        {
            List<string> protections = new List<string>();

            using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
            {
                Console.WriteLine($"Name\tOffset\tZip Size\tActual Size");

                br.ReadBytes(4); // Skip magic number

                int version = br.ReadInt32();
                int files = br.ReadInt32();
                long TMP = br.BaseStream.Position;

                for (int i = 0; i < files; i++)
                {
                    br.BaseStream.Seek(TMP, SeekOrigin.Begin);

                    int NSIZE = br.ReadInt32();
                    string name = new string(br.ReadChars(NSIZE));

                    int size = br.ReadInt32();
                    int offset = br.ReadInt32();

                    TMP = br.BaseStream.Position;

                    br.BaseStream.Seek(offset, SeekOrigin.Begin);
                    int zsize = br.ReadInt32();

                    if (zsize == size)
                        Console.WriteLine($"{name}\t{offset}\t{zsize}");
                    else
                        Console.WriteLine($"{name}\t{offset}\t{zsize}\t{size}");

                    // TODO: Figure out compression scheme
                    string tempfile = Path.Combine(Path.GetTempPath(), name);
                    if (!Directory.Exists(Path.GetDirectoryName(tempfile)))
                        Directory.CreateDirectory(Path.GetDirectoryName(tempfile));

                    using (FileStream fs = File.OpenWrite(tempfile))
                    using (DeflateStream ds = new DeflateStream(br.BaseStream, CompressionMode.Decompress))
                    {
                        int totalRead = 0;
                        while (totalRead < size)
                        {
                            byte[] buffer = new byte[3 * 1024 * 1024];
                            int toread = Math.Min(buffer.Length, size - totalRead);
                            int read = ds.Read(buffer, 0, toread);
                            totalRead += read;
                            fs.Write(buffer, 0, read);
                        }
                    }

                    br.BaseStream.Seek(TMP, SeekOrigin.Begin);
                }
            }

            return protections;
        }
    }
}
