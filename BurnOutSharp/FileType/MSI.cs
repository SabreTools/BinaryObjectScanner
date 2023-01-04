using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using BurnOutSharp.Interfaces;
using OpenMcdf;
using static BurnOutSharp.Utilities.Dictionary;

namespace BurnOutSharp.FileType
{
    /// <summary>
    /// Microsoft installation package
    /// </summary>
    public class MSI : IScannable
    {
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Scan(scanner, fs, file);
            }
        }

        // TODO: Add stream opening support
        /// <inheritdoc/>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the MSI file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                using (CompoundFile msi = new CompoundFile(stream, CFSUpdateMode.ReadOnly, CFSConfiguration.Default))
                {
                    msi.RootStorage.VisitEntries((e) =>
                    {
                        if (!e.IsStream)
                            return;

                        var str = msi.RootStorage.GetStream(e.Name);
                        if (str == null)
                            return;

                        byte[] strData = str.GetData();
                        if (strData == null)
                            return;

                        string decoded = DecodeStreamName(e.Name).TrimEnd('\0');
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
                    }, recursive: true);
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
                StripFromKeys(protections, tempPath);

                return protections;
            }
            catch (Exception ex)
            {
                if (scanner.IncludeDebug) Console.WriteLine(ex);
            }

            return null;
        }

        /// <remarks>Adapted from LibMSI</remarks>
        public static string DecodeStreamName(string input)
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
