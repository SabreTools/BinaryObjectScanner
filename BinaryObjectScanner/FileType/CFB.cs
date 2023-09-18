using System;
using System.IO;
using System.Text;
using BinaryObjectScanner.Interfaces;
using OpenMcdf;

namespace BinaryObjectScanner.FileType
{
    /// <summary>
    /// Compound File Binary
    /// </summary>
    public class CFB : IExtractable
    {
        /// <inheritdoc/>
#if NET48
        public string Extract(string file, bool includeDebug)
#else
        public string? Extract(string file, bool includeDebug)
#endif
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Extract(fs, file, includeDebug);
            }
        }

        /// <inheritdoc/>
#if NET48
        public string Extract(Stream stream, string file, bool includeDebug)
#else
        public string? Extract(Stream stream, string file, bool includeDebug)
#endif
        {
            try
            {
                // Create a temp output directory
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (CompoundFile msi = new CompoundFile(stream, CFSUpdateMode.ReadOnly, CFSConfiguration.Default))
                {
                    msi.RootStorage.VisitEntries((e) =>
                    {
                        try
                        {
                            if (!e.IsStream)
                                return;

                            var str = msi.RootStorage.GetStream(e.Name);
                            if (str == null)
                                return;

                            byte[] strData = str.GetData();
                            if (strData == null)
                                return;

                            var decoded = DecodeStreamName(e.Name)?.TrimEnd('\0');
                            if (decoded == null)
                                return;

                            byte[] nameBytes = Encoding.UTF8.GetBytes(e.Name);

                            // UTF-8 encoding of 0x4840.
                            if (nameBytes[0] == 0xe4 && nameBytes[1] == 0xa1 && nameBytes[2] == 0x80)
                                decoded = decoded.Substring(3);

                            foreach (char c in Path.GetInvalidFileNameChars())
                            {
                                decoded = decoded.Replace(c, '_');
                            }

                            string filename = Path.Combine(tempPath, decoded);
                            using (Stream fs = File.OpenWrite(filename))
                            {
                                fs.Write(strData, 0, strData.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (includeDebug) Console.WriteLine(ex);
                        }
                    }, recursive: true);
                }

                return tempPath;
            }
            catch (Exception ex)
            {
                if (includeDebug) Console.WriteLine(ex);
                return null;
            }
        }

        /// <remarks>Adapted from LibMSI</remarks>
#if NET48
        public static string DecodeStreamName(string input)
#else
        public static string? DecodeStreamName(string input)
#endif
        {
            if (input == null)
                return null;

            int count = 0;
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            int p = 0; // inputBytes[0]

            byte[] output = new byte[inputBytes.Length + 1];
            int q = 0; // output[0]
            while (p < inputBytes.Length && inputBytes[p] != 0)
            {
                int ch = inputBytes[p];
                if ((ch == 0xe3 && inputBytes[p + 1] >= 0xa0) || (ch == 0xe4 && inputBytes[p + 1] < 0xa0))
                {
                    // UTF-8 encoding of 0x3800..0x47ff. 
                    output[q++] = (byte)Mime2Utf(inputBytes[p + 2] & 0x7f);
                    output[q++] = (byte)Mime2Utf(inputBytes[p + 1] ^ 0xa0);
                    p += 3;
                    count += 2;
                    continue;
                }

                if (ch == 0xe4 && inputBytes[p + 1] == 0xa0)
                {
                    // UTF-8 encoding of 0x4800..0x483f.
                    output[q++] = (byte)Mime2Utf(inputBytes[p + 2] & 0x7f);
                    p += 3;
                    count++;
                    continue;
                }

                output[q++] = inputBytes[p++];
                if (ch >= 0xc1)
                    output[q++] = inputBytes[p++];
                if (ch >= 0xe0)
                    output[q++] = inputBytes[p++];
                if (ch >= 0xf0)
                    output[q++] = inputBytes[p++];

                count++;
            }

            output[q] = 0;
            return Encoding.ASCII.GetString(output);
        }

        /// <remarks>Adapted from LibMSI</remarks>
        private static int Mime2Utf(int x)
        {
            if (x < 10)
                return x + '0';
            if (x < (10 + 26))
                return x - 10 + 'A';
            if (x < (10 + 26 + 26))
                return x - 10 - 26 + 'a';
            if (x == (10 + 26 + 26))
                return '.';
            return '_';
        }
    }
}
