using System;
using System.IO;
using System.Linq;
using System.Text;
using static psxt001z.Common;

namespace psxt001z
{
    internal class ScsiPassThroughDirect
    {
        public ushort Length { get; set; }

        public byte ScsiStatus { get; set; }

        public byte PathId { get; set; }

        public byte TargetId { get; set; }

        public byte Lun { get; set; }

        public byte CDBLength { get; set; }

        public byte SenseInfoLength { get; set; }

        public byte DataIn { get; set; }

        public uint DataTransferLength { get; set; }

        public uint TimeOutValue { get; set; }

        public byte[] DataBuffer { get; set; }

        public uint SenseInfoOffset { get; set; }

        public byte[] CDB { get; set; } = new byte[16];
    }

    public class LibCrypt
    {
        #region OLD

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

        #endregion

        #region Constants

        private const uint IOCTL_SCSI_PASS_THROUGH_DIRECT = 0x4D014;
        private const byte SCSI_IOCTL_DATA_IN = 0x1;
        private const byte RAW_READ_CMD = 0xBE;
        private const int BUFFER_LEN = 96;
        private const int SENSE_SIZE = 0; //14
        private const string F_NAME = "sectors.log";
        private const int CYCLES = 5;
        private const int LIBCRYPT_NUM_SECTORS = 64;
        private const int READ_TIMES = 5;

        private static readonly uint[] lc_addresses = new uint[LIBCRYPT_NUM_SECTORS]
{
            13955, 13960, 14081, 14086, 14335, 14340, 14429, 14434,
            14499, 14504, 14749, 14754, 14906, 14911, 14980, 14985,
            15092, 15097, 15162, 15167, 15228, 15233, 15478, 15483,
            15769, 15774, 15881, 15886, 15951, 15956, 16017, 16022,
            41895, 41900, 42016, 42021, 42282, 42287, 42430, 42435,
            42521, 42526, 42663, 42668, 42862, 42867, 43027, 43032,
            43139, 43144, 43204, 43209, 43258, 43263, 43484, 43489,
            43813, 43818, 43904, 43909, 44009, 44014, 44162, 44167
};

        private static readonly byte[] lc1_sectors_contents = new byte[768]
        {
            0x41, 0x01, 0x01, 0x07, 0x06, 0x05, 0x00, 0x23, 0x08, 0x05, 0x38, 0x39,
            0x41, 0x01, 0x01, 0x03, 0x06, 0x11, 0x00, 0x03, 0x08, 0x90, 0x5d, 0xa0,
            0x41, 0x01, 0x01, 0x07, 0x07, 0x56, 0x00, 0x23, 0x09, 0x56, 0xdf, 0xde,
            0x41, 0x01, 0x01, 0x03, 0x07, 0x60, 0x00, 0x03, 0x09, 0xe1, 0xf2, 0x50,
            0x41, 0x01, 0x01, 0x03, 0x13, 0x10, 0x00, 0x03, 0x53, 0x10, 0x50, 0xec,
            0x41, 0x01, 0x01, 0x43, 0x11, 0x15, 0x00, 0x01, 0x13, 0x15, 0x23, 0x1e,
            0x41, 0x01, 0x01, 0x03, 0x12, 0x09, 0x00, 0x03, 0x14, 0x2d, 0x04, 0x73,
            0x41, 0x01, 0x01, 0x03, 0x1a, 0x34, 0x00, 0x03, 0x04, 0x34, 0xe2, 0xcf,
            0x41, 0x01, 0x01, 0x03, 0x13, 0x20, 0x00, 0x03, 0x15, 0x04, 0x82, 0x35,
            0x41, 0x01, 0x01, 0x01, 0x13, 0x29, 0x00, 0x43, 0x15, 0x29, 0x72, 0xe2,
            0x41, 0x01, 0x01, 0x03, 0x1e, 0x49, 0x00, 0x03, 0x08, 0x49, 0x32, 0xc5,
            0x41, 0x01, 0x01, 0x01, 0x16, 0x54, 0x00, 0x43, 0x18, 0x54, 0xd4, 0x79,
            0x41, 0x01, 0x01, 0x03, 0x18, 0x57, 0x00, 0x03, 0x20, 0xd6, 0xbc, 0x27,
            0x41, 0x01, 0x01, 0x03, 0x38, 0x61, 0x00, 0x03, 0x24, 0x61, 0x91, 0xa9,
            0x41, 0x01, 0x01, 0x0b, 0x19, 0x55, 0x00, 0x13, 0x21, 0x55, 0x14, 0x07,
            0x41, 0x01, 0x01, 0x03, 0x19, 0x62, 0x00, 0x03, 0x21, 0x20, 0x5d, 0x48,
            0x41, 0x01, 0x01, 0x03, 0x23, 0x17, 0x00, 0x03, 0x63, 0x17, 0x6d, 0xc6,
            0x41, 0x01, 0x01, 0x43, 0x21, 0x22, 0x00, 0x01, 0x23, 0x22, 0x24, 0x89,
            0x41, 0x01, 0x01, 0x03, 0x02, 0x12, 0x00, 0x03, 0x20, 0x12, 0x49, 0x43,
            0x41, 0x01, 0x01, 0x03, 0x22, 0x07, 0x00, 0x03, 0x24, 0x1f, 0x3a, 0xb1,
            0x41, 0x01, 0x01, 0x03, 0x23, 0x13, 0x00, 0x03, 0x25, 0x0b, 0x93, 0xc9,
            0x41, 0x01, 0x01, 0x0b, 0x23, 0x08, 0x00, 0x13, 0x25, 0x08, 0xce, 0x5d,
            0x41, 0x01, 0x01, 0x03, 0x06, 0x28, 0x00, 0x03, 0x2c, 0x28, 0xd7, 0xd6,
            0x41, 0x01, 0x01, 0x0b, 0x26, 0x33, 0x00, 0x13, 0x28, 0x33, 0x9c, 0x29,
            0x41, 0x01, 0x01, 0x03, 0x30, 0x59, 0x00, 0x03, 0x32, 0x1b, 0x2c, 0xc6,
            0x41, 0x01, 0x01, 0x03, 0x20, 0x24, 0x00, 0x03, 0x3a, 0x24, 0xe6, 0xac,
            0x41, 0x01, 0x01, 0x13, 0x31, 0x56, 0x00, 0x0b, 0x33, 0x56, 0x97, 0xed,
            0x41, 0x01, 0x01, 0x03, 0x31, 0x65, 0x00, 0x03, 0x33, 0x41, 0xba, 0x63,
            0x41, 0x01, 0x01, 0x01, 0x32, 0x51, 0x00, 0x43, 0x34, 0x51, 0xd7, 0xa9,
            0x41, 0x01, 0x01, 0x03, 0x33, 0x56, 0x00, 0x03, 0xb4, 0x56, 0xc0, 0x9a,
            0x41, 0x01, 0x01, 0x03, 0x32, 0x42, 0x00, 0x03, 0xb5, 0x42, 0x69, 0xe2,
            0x41, 0x01, 0x01, 0x03, 0x33, 0x07, 0x00, 0x03, 0x35, 0x45, 0x1a, 0x10,
            0x41, 0x01, 0x01, 0x09, 0x18, 0x65, 0x00, 0x09, 0x20, 0x41, 0x40, 0x72,
            0x41, 0x01, 0x01, 0x19, 0x18, 0x50, 0x00, 0x01, 0x20, 0x50, 0x25, 0xeb,
            0x41, 0x01, 0x01, 0x08, 0x20, 0x16, 0x00, 0x89, 0x22, 0x16, 0x95, 0xa8,
            0x41, 0x01, 0x01, 0x09, 0x20, 0x01, 0x00, 0x09, 0x22, 0x25, 0xb8, 0x26,
            0x41, 0x01, 0x01, 0x09, 0x23, 0x53, 0x00, 0x09, 0x25, 0x77, 0x21, 0x03,
            0x41, 0x01, 0x01, 0x0b, 0x23, 0x62, 0x00, 0x49, 0x25, 0x62, 0x68, 0x4c,
            0x41, 0x01, 0x01, 0x0d, 0x25, 0x55, 0x00, 0x29, 0x27, 0x55, 0xae, 0x41,
            0x41, 0x01, 0x01, 0x09, 0x25, 0x61, 0x00, 0x09, 0x27, 0xe0, 0xe7, 0x0e,
            0x41, 0x01, 0x01, 0x08, 0x26, 0x71, 0x00, 0x89, 0x28, 0x71, 0x95, 0xcb,
            0x41, 0x01, 0x01, 0x09, 0x27, 0x21, 0x00, 0x09, 0x29, 0x05, 0x80, 0x4b,
            0x41, 0x01, 0x01, 0x0b, 0x28, 0x63, 0x00, 0x49, 0x30, 0x63, 0xed, 0x18,
            0x41, 0x01, 0x01, 0x09, 0x29, 0x68, 0x00, 0x09, 0xb0, 0x68, 0xb0, 0x8c,
            0x41, 0x01, 0x01, 0x29, 0x31, 0x37, 0x00, 0x0d, 0x33, 0x37, 0x6c, 0x68,
            0x41, 0x01, 0x01, 0x09, 0x31, 0x4a, 0x00, 0x09, 0x33, 0x52, 0x7c, 0x8b,
            0x41, 0x01, 0x01, 0x09, 0x73, 0x52, 0x00, 0x09, 0x37, 0x52, 0x4b, 0x06,
            0x41, 0x01, 0x01, 0x19, 0x33, 0x57, 0x00, 0x01, 0x35, 0x57, 0x38, 0xf4,
            0x41, 0x01, 0x01, 0x09, 0x35, 0x04, 0x00, 0x09, 0x37, 0x1c, 0x54, 0x6a,
            0x41, 0x01, 0x01, 0x09, 0x31, 0x19, 0x00, 0x09, 0x17, 0x19, 0xa4, 0xbd,
            0x41, 0x01, 0x01, 0x01, 0x36, 0x04, 0x00, 0x19, 0x38, 0x04, 0x9c, 0xdf,
            0x41, 0x01, 0x01, 0x09, 0x36, 0x0b, 0x00, 0x09, 0x38, 0x49, 0x6c, 0x08,
            0x41, 0x01, 0x01, 0x49, 0x36, 0x58, 0x00, 0x0b, 0x38, 0x58, 0x99, 0xbf,
            0x41, 0x01, 0x01, 0x09, 0x36, 0x73, 0x00, 0x09, 0x38, 0x6b, 0xfe, 0x96,
            0x41, 0x01, 0x01, 0x0b, 0x39, 0x59, 0x00, 0x49, 0x41, 0x59, 0x54, 0x0d,
            0x41, 0x01, 0x01, 0x09, 0x39, 0x24, 0x00, 0x09, 0x41, 0x66, 0x9e, 0x67,
            0x41, 0x01, 0x01, 0x09, 0x44, 0x1b, 0x00, 0x09, 0x46, 0x03, 0x78, 0x0d,
            0x41, 0x01, 0x01, 0x09, 0x46, 0x18, 0x00, 0x09, 0x06, 0x18, 0x25, 0x99,
            0x41, 0x01, 0x01, 0x09, 0x45, 0x2b, 0x00, 0x09, 0x47, 0x69, 0xd3, 0xc5,
            0x41, 0x01, 0x01, 0x09, 0x05, 0x34, 0x00, 0x09, 0x45, 0x34, 0x35, 0x79,
            0x41, 0x01, 0x01, 0x09, 0x44, 0x59, 0x00, 0x09, 0x08, 0x59, 0x6e, 0x0a,
            0x41, 0x01, 0x01, 0x49, 0x46, 0x64, 0x00, 0x0b, 0x48, 0x64, 0xa4, 0x60,
            0x41, 0x01, 0x01, 0x09, 0x08, 0x62, 0x00, 0x09, 0x52, 0x62, 0x03, 0x5a,
            0x41, 0x01, 0x01, 0x19, 0x48, 0x67, 0x00, 0x01, 0x50, 0x67, 0x70, 0xa8
        };

        #endregion

        // TODO: Enable the following only if reading from a drive directly

        /*
        internal static byte LibCryptDrive(string[] args)
        {
            byte offset = 0;
            string path = $"\\\\.\\{args[0][0]}:";
            byte i;
            byte[] sub = new byte[12], buffer = new byte[BUFFER_LEN], buffer2352 = new byte[23520], buffer2 = new byte[BUFFER_LEN], buffer3 = new byte[BUFFER_LEN], buffer4 = new byte[BUFFER_LEN];
            byte[] status;
            ushort crc;
            uint sector, sector_start, sector_end, a, lcsectors = 0, todo = 9300, done = 0;

            if (args.Length != 1 || (args.Length == 1 && (args[0][1] != 0 && (args[0][1] != ':' || args[0][2] != 0))))
            {
                Console.WriteLine("LibCrypt drive detector");
                Console.WriteLine("nUsage: psxt001z.exe --libcryptdrv <drive letter>");
                return 0;
            }

            Stream hDevice;
            try
            {
                hDevice = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't open device!");
                return 0;
            }

            Stream f;
            try
            {
                f = File.Open(F_NAME, FileMode.Open, FileAccess.Write);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can't open file {F_NAME}!");
                return 0;
            }

            byte[] status1 = new byte[4650];
            byte[] status2 = new byte[4650];

            // Offset detection
            Console.WriteLine("Determining offset...\r");
            ScsiPassThroughDirect SRB = new ScsiPassThroughDirect();
            ReadSub(buffer, 0, f, offset, hDevice, SRB);

            switch (buffer[8])
            {
                case 0x01:
                    offset = (byte)(75 - btoi(buffer[9]));
                    break;
                case 0x02:
                    offset = (byte)(-btoi(buffer[9]));
                    break;
                default:
                    Console.WriteLine("Can't determine offset!");
                    Console.WriteLine(BitConverter.ToString(buffer).Replace('-', ' '));
                    return 0;
            }

            sub[0] = buffer[0];
            sub[1] = 0x01;
            sub[2] = 0x01;
            sub[6] = 0x00;

            Console.WriteLine($"Subchannels offset correction: {offset}");

            // Section 1) 02:58:00 - 03:69:74 -- status1
            // Section 2) 08:58:00 - 09:69:74 -- status2

            // Step 1

            for (i = 0; i < CYCLES * 3; i++)
            {

                if (todo == 0)
                    goto end;

                if (i % 3 == 0)
                {
                    sector_start = 13350;
                    sector_end = 18000;
                    status = status1;
                }
                else if (i % 3 == 1)
                {
                    sector_start = 40350;
                    sector_end = 45000;
                    status = status2;
                }
                else
                {
                    Console.WriteLine($"Left: {todo:4} / Flushing cache...             \r");
                    ClearCache(buffer2352, f, offset, hDevice, SRB);
                    continue;
                }

                for (sector = sector_start; sector < sector_end; sector++)
                {
                    if (status[sector - sector_start] != 0)
                        continue;

                    ReadSub(buffer, sector, f, offset, hDevice, SRB);
                    Console.WriteLine("Left: %4u / Sector %u...         \r", todo, sector);
                    // generating q-channel
                    sub[3] = itob((byte)(sector / 60 / 75));
                    sub[4] = itob((byte)((sector / 75) % 60));
                    sub[5] = itob((byte)(sector % 75));
                    sub[7] = itob((byte)((sector + 150) / 60 / 75));
                    sub[8] = itob((byte)(((sector + 150) / 75) % 60));
                    sub[9] = itob((byte)((sector + 150) % 75));
                    crc = CRC16.Calculate(sub, 0, 10);
                    sub[10] = (byte)(crc >> 8);
                    sub[11] = (byte)(crc & 0xFF);
                    if (sub.SequenceEqual(buffer.Take(12)))
                    {
                        status[sector - sector_start] = 1;
                        todo--;
                        done++;
                    }
                }
            }

            // Step 2

            for (i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    sector_start = 13350;
                    sector_end = 18000;
                    status = status1;
                }
                else
                {
                    sector_start = 40350;
                    sector_end = 45000;
                    status = status2;
                }

                for (sector = sector_start; sector < sector_end; sector++)
                {
                    if (status[sector - sector_start] != 0)
                        continue;
                    ReadSub(buffer, sector, f, offset, hDevice, SRB);
                    Console.WriteLine($"Left: {todo:4} / Sector {sector}...         \r");

                    // generating q-channel
                    sub[3] = itob((byte)(sector / 60 / 75));
                    sub[4] = itob((byte)((sector / 75) % 60));
                    sub[5] = itob((byte)(sector % 75));
                    sub[7] = itob((byte)((sector + 150) / 60 / 75));
                    sub[8] = itob((byte)(((sector + 150) / 75) % 60));
                    sub[9] = itob((byte)((sector + 150) % 75));
                    crc = CRC16.Calculate(sub, 0, 10);
                    sub[10] = (byte)(crc >> 8);
                    sub[11] = (byte)(crc ^ 0xFF);
                    if (sub.SequenceEqual(buffer.Take(12)))
                    {
                        Console.WriteLine($"Left: {todo:4} / Sector {sector}: flushing cache...        \r");
                        do
                        {
                            ReadSub(buffer, sector, f, offset, hDevice, SRB);
                            ClearCache(buffer2352, f, offset, hDevice, SRB);
                            ReadSub(buffer2, sector, f, offset, hDevice, SRB);
                            ClearCache(buffer2352, f, offset, hDevice, SRB);
                            ReadSub(buffer3, sector, f, offset, hDevice, SRB);
                            ClearCache(buffer2352, f, offset, hDevice, SRB);
                        } while (!buffer.SequenceEqual(buffer2) || !buffer.SequenceEqual(buffer3));
                        //} while (!matrix(buffer, buffer2, buffer3, buffer4, BUFFER_LEN));

                        if (buffer.SequenceEqual(sub))
                        {
                            byte[] buf = Encoding.ASCII.GetBytes($"MSF: {sub[7]:2x}:{sub[8]:2x}:{sub[9]:2x} Q-Data: {BitConverter.ToString(buffer.Take(12).ToArray()).Replace('-', ' ')}");
                            f.Write(buf, 0, buf.Length);
                            lcsectors++;
                            //fwrite(SRB.SRB_BufPointer, 1, SRB.SRB_BufLen - 4, f);
                            f.Flush();
                        }
                    }

                    todo--;
                    done++;
                }
            }

        end:

            Console.WriteLine($"Done!                                                           \nProtected sectors: {(lcsectors == 0 ? "None" : lcsectors.ToString())}");

            f.Close();
            return 1;
        }

        internal static int LibCryptDriveFast(string[] args)
        {
            Stream f;
            Stream hDevice;
            ScsiPassThroughDirect SRB;
            s8 offset = 0, path[] = "\\\\.\\X:";
            u8 buffer[BUFFER_LEN], buffer2352[23520], sub[12], lc1sectors = 0, lc2sectors = 0, othersectors = 0;
            u16 crc;

            if (argc != 1 || (argc == 1 && (args[0][1] != 0 && (args[0][1] != ':' || args[0][2] != 0))))
            {
                Console.WriteLine("LibCrypt drive detector (fast)\nUsage: psxt001z.exe --libcryptdrvfast <drive letter>\n");
                return 0;
            }

            path[4] = args[0][0];

            if ((hDevice = CreateFile(path, GENERIC_WRITE | GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, 0, OPEN_EXISTING, 0, 0)) == INVALID_HANDLE_VALUE)
            {
                Console.WriteLine("Can't open device!\n");
                return 0;
            }

            if (fopen_s(&f, F_NAME, "wb") != 0)
            {
                Console.WriteLine("Can\'t open file %s!\n", F_NAME);
                return 0;
            }

            // Offset detection
            ReadSub(buffer, 0, f, offset, hDevice, SRB);
            //if (buffer[5] != buffer[9]) {
            //	Console.WriteLine("Error determining offset!\nSector 0: %02x%02x%02x %02x:%02x:%02x %02x %02x:%02x:%02x %02x%02x\n", buffer[0], buffer[1], buffer[2], buffer[3], buffer[4], buffer[5], buffer[6], buffer[7], buffer[8], buffer[9], buffer[10], buffer[11]);
            //}
            switch (buffer[8])
            {
                case 0x01:
                    offset = 75 - btoi(buffer[9]);
                    break;
                case 0x02:
                    offset = -btoi(buffer[9]);
                    break;
                default:
                    Console.WriteLine("Can't determine offset!\nSector 0: %02x%02x%02x %02x:%02x:%02x %02x %02x:%02x:%02x %02x%02x\n", buffer[0], buffer[1], buffer[2], buffer[3], buffer[4], buffer[5], buffer[6], buffer[7], buffer[8], buffer[9], buffer[10], buffer[11]);
                    return 0;
            }
            Console.WriteLine("Subchannels offset correction: %d\n", offset);
            sub[0] = buffer[0];
            sub[1] = 0x01;
            sub[2] = 0x01;
            sub[6] = 0x00;

            for (int i = 0; i < LIBCRYPT_NUM_SECTORS; i++)
            {
                Console.WriteLine("\nReading sector %u... ", lc_addresses[i]);
                ReadSub(buffer, lc_addresses[i], f, offset, hDevice, SRB);
                // generating q-channel
                sub[3] = itob(lc_addresses[i] / 60 / 75);
                sub[4] = itob((lc_addresses[i] / 75) % 60);
                sub[5] = itob(lc_addresses[i] % 75);
                sub[7] = itob((lc_addresses[i] + 150) / 60 / 75);
                sub[8] = itob(((lc_addresses[i] + 150) / 75) % 60);
                sub[9] = itob((lc_addresses[i] + 150) % 75);
                crc = crc16(sub, 10);
                sub[10] = HIbyte(crc);
                sub[11] = LObyte(crc);
                for (int a = 1; a <= READ_TIMES; a++)
                {
                    if (!memcmp(sub, buffer, 12))
                    {
                        Console.WriteLine("original sector");
                        break;
                    }
                    else if (!memcmp(lc1_sectors_contents + (12 * i), buffer, 12))
                    {
                        Console.WriteLine("LibCrypt, LC1 sector");
                        fConsole.WriteLine(f, "MSF: %02x:%02x:%02x Q-Data: %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x\n", sub[7], sub[8], sub[9], buffer[0], buffer[1], buffer[2], buffer[3], buffer[4], buffer[5], buffer[6], buffer[7], buffer[8], buffer[9], buffer[10], buffer[11]);
                        lc1sectors++;
                        break;
                    }
                    else
                    {
                        if (a < READ_TIMES)
                        {
                            ClearCache(buffer2352, 0, offset, hDevice, SRB);
                            continue;
                        }
                        else
                        {
                            Console.WriteLine("unknown");
                            fConsole.WriteLine(f, "MSF: %02x:%02x:%02x Q-Data: %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x\n", sub[7], sub[8], sub[9], buffer[0], buffer[1], buffer[2], buffer[3], buffer[4], buffer[5], buffer[6], buffer[7], buffer[8], buffer[9], buffer[10], buffer[11]);
                            othersectors++;
                        }
                    }
                }



            }
            Console.WriteLine("\n\nOriginal sectors: %u", LIBCRYPT_NUM_SECTORS - lc1sectors - lc2sectors - othersectors);
            Console.WriteLine("\nLC1 sectors:      %u", lc1sectors);
            Console.WriteLine("\nLC2 sectors:      %u", lc2sectors);
            Console.WriteLine("\nOther sectors:    %u", othersectors);

            fConsole.WriteLine(f, "\nOriginal sectors: %u", LIBCRYPT_NUM_SECTORS - lc1sectors - lc2sectors - othersectors);
            fConsole.WriteLine(f, "\nLC1 sectors:      %u", lc1sectors);
            fConsole.WriteLine(f, "\nLC2 sectors:      %u", lc2sectors);
            fConsole.WriteLine(f, "\nOther sectors:    %u", othersectors);
            fclose(f);
            return 1;
        }

        internal static void ReadSub(byte[] buffer, uint sector, Stream f, byte offset, Stream hDevice, ScsiPassThroughDirect SRB)
        {
            uint returned;
            ZeroMemory(&SRB, sizeof(ScsiPassThroughDirect));
            SRB.Length = sizeof(ScsiPassThroughDirect);
            SRB.CDBLength = 12;
            SRB.DataIn = SCSI_IOCTL_DATA_IN;
            SRB.DataTransferLength = BUFFER_LEN;
            SRB.TimeOutValue = 30;
            SRB.DataBuffer = buffer;
            SRB.CDB[0] = RAW_READ_CMD;
            SRB.CDB[2] = HIbyte(HIWORD(sector + offset));
            SRB.CDB[3] = LObyte(HIWORD(sector + offset));
            SRB.CDB[4] = HIbyte(LOWORD(sector + offset));
            SRB.CDB[5] = LObyte(LOWORD(sector + offset));
            SRB.CDB[8] = 1;
            SRB.CDB[10] = 1;

            if (!DeviceIoControl(hDevice, IOCTL_SCSI_PASS_THROUGH_DIRECT, &SRB, sizeof(ScsiPassThroughDirect), &SRB, SENSE_SIZE, &returned, 0))
            {
                Console.WriteLine("\nError reading subchannel data!\n");
                if (f != 0)
                    fConsole.WriteLine(f, "Error reading subchannel data!");

                return 0;
            }

            Deinterleave(buffer);
            return;
        }

        internal static void ClearCache(byte[] buffer, Stream f, byte offset, Stream hDevice, ScsiPassThroughDirect SRB)
        {
            static uint returned;
            for (uint sector = 0; sector < 1000; sector += 10)
            {
                ZeroMemory(&SRB, sizeof(ScsiPassThroughDirect));
                SRB.Length = sizeof(ScsiPassThroughDirect);
                SRB.CDBLength = 12;
                SRB.DataIn = SCSI_IOCTL_DATA_IN;
                SRB.DataTransferLength = 23520;
                SRB.TimeOutValue = 30;
                SRB.DataBuffer = buffer;
                SRB.CDB[0] = RAW_READ_CMD;
                SRB.CDB[2] = HIbyte(HIWORD(sector + offset));
                SRB.CDB[3] = LObyte(HIWORD(sector + offset));
                SRB.CDB[4] = HIbyte(LOWORD(sector + offset));
                SRB.CDB[5] = LObyte(LOWORD(sector + offset));
                SRB.CDB[8] = 10;
                SRB.CDB[9] = 0xF8;

                DeviceIoControl(hDevice, IOCTL_SCSI_PASS_THROUGH_DIRECT, &SRB, sizeof(ScsiPassThroughDirect), &SRB, SENSE_SIZE, &returned, 0);

                if (!DeviceIoControl(hDevice, IOCTL_SCSI_PASS_THROUGH_DIRECT, &SRB, sizeof(ScsiPassThroughDirect), &SRB, SENSE_SIZE, &returned, 0))
                {
                    Console.WriteLine("\nError clearing cache!\n");
                    if (f != 0)
                        fConsole.WriteLine(f, "Error clearing cache!");
                    exit(0);
                }
            }
            return;
        }
        */

        internal static bool Matrix(byte[] buffer, byte[] buffer2, byte[] buffer3, byte[] buffer4, uint length)
        {
            for (int i = 0; i < length; i++)
            {
                if (buffer[i] == buffer2[i])
                {
                    if (buffer[i] == buffer3[i])
                        continue;
                    if (buffer[i] == buffer4[i])
                        continue;
                }
                else if (buffer[i] == buffer3[i] && buffer[i] == buffer4[i])
                {
                    continue;
                }
                else if (buffer2[i] == buffer3[i] && buffer2[i] == buffer4[i])
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        internal static void Deinterleave(byte[] buffer)
        {
            byte[] buffertmp = new byte[12];
            for (int i = 0; i < 12; i++)
            {
                buffertmp[i] |= (byte)((buffer[i * 8] & 0x40) << 1);
                buffertmp[i] |= (byte)((buffer[i * 8 + 1] & 0x40));
                buffertmp[i] |= (byte)((buffer[i * 8 + 2] & 0x40) >> 1);
                buffertmp[i] |= (byte)((buffer[i * 8 + 3] & 0x40) >> 2);
                buffertmp[i] |= (byte)((buffer[i * 8 + 4] & 0x40) >> 3);
                buffertmp[i] |= (byte)((buffer[i * 8 + 5] & 0x40) >> 4);
                buffertmp[i] |= (byte)((buffer[i * 8 + 6] & 0x40) >> 5);
                buffertmp[i] |= (byte)((buffer[i * 8 + 7] & 0x40) >> 6);
            }

            Array.Copy(buffertmp, buffer, 12);
            return;
        }

        internal static bool LibCryptDetect(string subPath, string sbiPath)
        {
            if (string.IsNullOrWhiteSpace(subPath) || !File.Exists(subPath))
                return false;

            // Variables
            byte[] buffer = new byte[16], sub = new byte[16];//, pregap = 0;
            uint sector, psectors = 0, tpos = 0;

            // Opening .sub
            Stream subfile = File.OpenRead(subPath);

            // checking extension
            if (Path.GetExtension(subPath).TrimStart('.').ToLowerInvariant() != "sub")
            {
                Console.WriteLine($"{subPath}: unknown file extension");
                return false;
            }

            // filesize
            long size = subfile.Length;
            if (size % 96 != 0)
            {
                Console.WriteLine($"{subfile}: wrong size");
                return false;
            }

            // sbi
            Stream sbi = null;
            if (sbiPath != null)
            {
                sbi = File.OpenWrite(sbiPath);
                sbi.Write(Encoding.ASCII.GetBytes("SBI\0"), 0, 4);
            }

            for (sector = 150; sector < ((size / 96) + 150); sector++)
            {
                subfile.Seek(12, SeekOrigin.Current);
                if (subfile.Read(buffer, 0, 12) != 12)
                    return true;

                subfile.Seek(72, SeekOrigin.Current);

                // New track
                if ((btoi(buffer[1]) == (btoi(sub[1]) + 1)) && (buffer[2] == 0 || buffer[2] == 1))
                {
                    Array.Copy(buffer, sub, 6);
                    tpos = (uint)((btoi((byte)(buffer[3] * 60)) + btoi(buffer[4])) * 75) + btoi(buffer[5]);
                }

                // New index
                else if (btoi(buffer[2]) == (btoi(sub[2]) + 1) && buffer[1] == sub[1])
                {
                    Array.Copy(buffer, 2, sub, 2, 4);
                    tpos = (uint)((btoi((byte)(buffer[3] * 60)) + btoi(buffer[4])) * 75) + btoi(buffer[5]);
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

                //MSF2 [7-9]
                sub[7] = itob((byte)(sector / 60 / 75));
                sub[8] = itob((byte)((sector / 75) % 60));
                sub[9] = itob((byte)(sector % 75));

                // CRC-16 [10-11]
                ushort crc = CRC16.Calculate(sub, 0, 10);
                sub[10] = (byte)(crc >> 8);
                sub[11] = (byte)(crc & 0xFF);

                //if (buffer[10] != sub[10] && buffer[11] != sub[11] && (buffer[3] != sub[3] || buffer[7] != sub[7] || buffer[4] != sub[4] || buffer[8] != sub[8] || buffer[5] != sub[5] || buffer[9] != sub[9])) {
                //if (buffer[10] != sub[10] || buffer[11] != sub[11] || buffer[3] != sub[3] || buffer[7] != sub[7] || buffer[4] != sub[4] || buffer[8] != sub[8] || buffer[5] != sub[5] || buffer[9] != sub[9]) {
                if (!buffer.Take(6).SequenceEqual(sub.Take(6)) || !buffer.Skip(7).Take(5).SequenceEqual(sub.Skip(7).Take(5)))
                {
                    Console.WriteLine($"MSF: {sub[7]:2x}:{sub[8]:2x}:{sub[9]:2x} Q-Data: {buffer[0]:2x}{buffer[1]:2x}{buffer[2]:2x} {buffer[3]:2x}:{buffer[4]:2x}:{buffer[5]:2x} {buffer[6]:2x} {buffer[7]:2x}:{buffer[8]:2x}:{buffer[9]:2x} {buffer[10]:2x}{buffer[11]:2x}  xor {crc ^ ((buffer[10] << 8) + buffer[11]):4x} {CRC16.Calculate(buffer, 0, 10) ^ ((buffer[10] << 8) + buffer[11]):4x}");
                    //Console.WriteLine("\nMSF: %02x:%02x:%02x Q-Data: %02x%02x%02x %02x:%02x:%02x %02x %02x:%02x:%02x %02x%02x", sub[7], sub[8], sub[9], sub[0], sub[1], sub[2], sub[3], sub[4], sub[5], sub[6], sub[7], sub[8], sub[9], sub[10], sub[11]);
                    
                    if (buffer[3] != sub[3] && buffer[7] != sub[7] && buffer[4] == sub[4] && buffer[8] == sub[8] && buffer[5] == sub[5] && buffer[9] == sub[9])
                        Console.WriteLine($" P1 xor {buffer[3] ^ sub[3]:2x} {buffer[7] ^ sub[7]:2x}");
                    else if (buffer[3] == sub[3] && buffer[7] == sub[7] && buffer[4] != sub[4] && buffer[8] != sub[8] && buffer[5] == sub[5] && buffer[9] == sub[9])
                        Console.WriteLine($" P2 xor {buffer[4] ^ sub[4]:2x} {buffer[8] ^ sub[8]:2x}");
                    else if (buffer[3] == sub[3] && buffer[7] == sub[7] && buffer[4] == sub[4] && buffer[8] == sub[8] && buffer[5] != sub[5] && buffer[9] != sub[9])
                        Console.WriteLine($" P3 xor {buffer[5] ^ sub[5]:2x} {buffer[9] ^ sub[9]:2x}");
                    else
                        Console.WriteLine(" ?");

                    Console.WriteLine("\n");
                    psectors++;
                    if (sbi != null)
                    {
                        sbi.Write(sub, 7, 3);
                        sbi.Write(new byte[] { 0x01 }, 0, 1);
                        sbi.Write(buffer, 0, 10);
                    }
                }
            }
            // }

            Console.WriteLine($"Number of modified sectors: {psectors}");
            return true;
        }

        internal static int XorLibCrypt()
        {
            sbyte b;
            byte d;
            byte i, a, x;
            byte[] sub = new byte[12]
            {
                0x41, 0x01, 0x01, 0x00,
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };

            ushort crc;
            for (i = 0; i < LIBCRYPT_NUM_SECTORS; i++)
            {
                sub[3] = itob((byte)(lc_addresses[i] / 60 / 75));
                sub[4] = itob((byte)((lc_addresses[i] / 75) % 60));
                sub[5] = itob((byte)(lc_addresses[i] % 75));
                sub[7] = itob((byte)((lc_addresses[i] + 150) / 60 / 75));
                sub[8] = itob((byte)(((lc_addresses[i] + 150) / 75) % 60));
                sub[9] = itob((byte)((lc_addresses[i] + 150) % 75));
                crc = CRC16.Calculate(sub, 0, 10);
                sub[10] = (byte)(crc >> 8);
                sub[11] = (byte)(crc & 0xFF);

                Console.WriteLine($"%u %02x:%02x:%02x", lc_addresses[i], sub[7], sub[8], sub[9]);
                Console.WriteLine($" %02x%02x%02x%02x%02x%02x%02x%02x%02x%02x %02x%02x", sub[0], sub[1], sub[2], sub[3], sub[4], sub[5], sub[6], sub[7], sub[8], sub[9], sub[10], sub[11]);
                Console.WriteLine($" %02x%02x%02x%02x%02x%02x%02x%02x%02x%02x %02x%02x", lc1_sectors_contents[i * 12], lc1_sectors_contents[(i * 12) + 1], lc1_sectors_contents[(i * 12) + 2], lc1_sectors_contents[(i * 12) + 3], lc1_sectors_contents[(i * 12) + 4], lc1_sectors_contents[(i * 12) + 5], lc1_sectors_contents[(i * 12) + 6], lc1_sectors_contents[(i * 12) + 7], lc1_sectors_contents[(i * 12) + 8], lc1_sectors_contents[(i * 12) + 9], lc1_sectors_contents[(i * 12) + 10], lc1_sectors_contents[(i * 12) + 11]);

                d = 0;

                for (a = 3; a < 12; a++)
                {
                    x = (byte)(lc1_sectors_contents[(i * 12) + a] ^ sub[a]);
                    Console.WriteLine($" %x%x%x%x%x%x%x%x", (x >> 7) & 0x1, (x >> 6) & 0x1, (x >> 5) & 0x1, (x >> 4) & 0x1, (x >> 3) & 0x1, (x >> 2) & 0x1, (x >> 1) & 0x1, x & 0x1);
                    if (x == 0)
                        continue;
                    for (b = 7; b >= 0; b--)
                    {
                        if (((x >> b) & 0x1) != 0)
                        {
                            d = (byte)(d << 1);
                            d |= (byte)((sub[a] >> b) & 0x1);
                        }
                    }
                }

                Console.WriteLine($" {(d >> 3) & 0x1:x}{(d >> 2) & 0x1:x}{(d >> 1) & 0x1:x}{d & 0x1}");
            }

            return 1;
        }
    }
}
