using System;
using System.IO;
using System.Linq;

namespace BurnOutSharp.External.psxt001z
{
    /// <summary>
    /// LibCrypt detection code
    /// Originally written by Dremora: https://github.com/Dremora/psxt001z
    /// Ported and changed by darksabre76
    /// </summary>
    public class LibCrypt
    {
        public static bool CheckSubfile(string subFilePath)
        {
            // Check the file exists first
            if (!File.Exists(subFilePath))
                return false;

            // Check the extension is a subfile
            string ext = Path.GetExtension(subFilePath).TrimStart('.').ToLowerInvariant();
            if (ext != "sub")
                return false;

            // Open and check the subfile for LibCrypt
            try
            {
                using (FileStream subfile = File.OpenRead(subFilePath))
                {
                    return CheckSubfile(subfile);
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckSubfile(Stream subfile)
        {
            // Check the length is valid for subfiles
            long size = subfile.Length;
            if (size % 96 != 0)
                return false;

            // Persistent values
            byte[] buffer = new byte[16];
            byte[] sub = new byte[16];
            int tpos = 0;
            int modifiedSectors = 0;

            // Check each sector for modifications
            for (uint sector = 150; sector < ((size / 96) + 150); sector++)
            {
                subfile.Seek(12, SeekOrigin.Current);
                if (subfile.Read(buffer, 0, 12) == 0)
                    return modifiedSectors != 0;

                subfile.Seek(72, SeekOrigin.Current);

                // New track
                if ((btoi(buffer[1]) == (btoi(sub[1]) + 1)) && (buffer[2] == 0 || buffer[2] == 1))
                {
                    Array.Copy(buffer, sub, 6);
                    tpos = ((btoi((byte)(buffer[3] * 60)) + btoi(buffer[4])) * 75) + btoi(buffer[5]);
                }

                // New index
                else if (btoi(buffer[2]) == (btoi(sub[2]) + 1) && buffer[1] == sub[1])
                {
                    Array.Copy(buffer, 2, sub, 2, 4);
                    tpos = ((btoi((byte)(buffer[3] * 60)) + btoi(buffer[4])) * 75) + btoi(buffer[5]);
                }

                // MSF1 [3-5]
                else
                {
                    if (sub[2] == 0)
                        tpos--;
                    else
                        tpos++;

                    sub[3] = itob((byte)(tpos / 60 / 75));
                    sub[4] = itob((byte)((tpos / 75) % 60));
                    sub[5] = itob((byte)(tpos % 75));
                }

                // MSF2 [7-9]
                sub[7] = itob((byte)(sector / 60 / 75));
                sub[8] = itob((byte)((sector / 75) % 60));
                sub[9] = itob((byte)(sector % 75));

                // CRC-16 [10-11]
                ushort crc = CRC16.Calculate(sub, 0, 10);
                byte[] crcBytes = BitConverter.GetBytes(crc);
                sub[10] = crcBytes[0];
                sub[11] = crcBytes[1];

                // If any byte (except position 6) is different, it's a modified sector
                for (int i = 0; i < 12; i++)
                {
                    if (i == 6)
                        continue;

                    if (buffer[i] != sub[i])
                    {
                        modifiedSectors++;
                        break;
                    }
                }
            }

            return modifiedSectors != 0;
        }

        private static byte btoi(byte b)
        {
            /* BCD to u_char */
            return (byte)((b) / 16 * 10 + (b) % 16);
        }

        private static byte itob(byte i)
        {
            /* u_char to BCD */
            return (byte)((i) / 10 * 16 + (i) % 10);
        }
    }
}
