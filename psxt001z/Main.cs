using System;
using System.IO;
using System.Security.Cryptography;
using static psxt001z.Common;
using static psxt001z.Functions;

namespace psxt001z
{
    public class Main
    {
        public static int main(string[] args)
        {
            Console.WriteLine($"psxt001z by Dremora, {VERSION}");
            Console.WriteLine();

            if (args == null || args.Length == 0)
            {
                Help();
                return -1;
            }

            if (args.Length == 1)
                GetInfo(args[0]);

            string feature = args[0];
            switch (feature)
            {
                case "--checksums":
                case "-c":
                    Checksums(args);
                    break;

                case "--libcrypt":
                case "-l":
                    {
                        if (args.Length != 1 && args.Length != 2)
                        {
                            LibCryptHelp();
                            return 0;
                        }

                        string subPath = args[0];
                        string sbiPath = null;
                        if (args.Length == 2)
                            sbiPath = args[1];

                        if (!LibCrypt.LibCryptDetect(subPath, sbiPath))
                            LibCryptHelp();

                        break;
                    }

                case "--xorlibcrypt":
                    LibCrypt.XorLibCrypt();
                    break;

                case "--zektor":
                    ClearEDCData(args[1]);
                    break;

                case "--antizektor":
                    SetEDCData(args[1]);
                    break;

                case "--patch":
                    {
                        string output = args[1];
                        string input = args[2];
                        int skip = 0;
                        if (args.Length == 4)
                            skip = int.Parse(args[3]);

                        Patch(output, input, skip);
                        break;
                    }

                case "--resize":
                    {
                        string input = args[1];
                        long newsize = long.Parse(args[2]);
                        Resize(input, newsize);
                        break;
                    }

                case "--track":
                    {
                        string filename = args[1];
                        int start = int.Parse(args[2]);
                        int size = int.Parse(args[3]);
                        uint crc = uint.Parse(args[4]);

                        bool isRiff = false;
                        bool? mode = null;
                        string output = null;
                        for (int i = 5; i < args.Length; i++)
                        {
                            if (args[i] == "r")
                                isRiff = true;
                            else if (args[i][0] == '+')
                                mode = true;
                            else if (args[i][1] == '-')
                                mode = false;
                            else if (args[i] == "s")
                                output = args[++i];
                        }

                        Track trackfix = new Track(filename, start, size, crc, isRiff, mode, output);
                        while (!trackfix.FindTrack()) ;
                        trackfix.Done();

                        break;
                    }

                case "--str":
                    SplitStr(args);
                    break;

                case "--str2bs":
                    StrToBs(args);
                    break;

                case "--gen":
                    Generate(args);
                    break;

                case "--scan":
                    Info.GetInfo(args[2], false);
                    break;

                case "--fix":
                    Info.GetInfo(args[2], true);
                    break;

                case "--sub":
                    CreateSubchannel(args[2], args[3]);
                    break;

                case "--m3s":
                    M3S(args[2]);
                    break;

                case "--matrix":
                    {
                        Matrix(args);
                        break;
                    }

                //case "--libcryptdrv":
                //    LibCrypt.LibCryptDrive(argv + 2);
                //    break;
                //case "--libcryptdrvfast":
                //    LibCrypt.LibCryptDriveFast(argv + 2);
                //    break;

                default:
                    Help();
                    break;
            }

            return 1;
        }

        public static void GetInfo(string filename)
        {
            Stream image;
            try
            {
                image = File.OpenRead(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening file \"{filename}\"! {ex}");
                return;
            }

            FileTools file = new FileTools(image);
            long imagesize = image.Length;

            Console.WriteLine($"File: {filename}");
            if (imagesize % 2352 != 0)
            {
                Console.WriteLine($"File \"{filename}\" is not Mode2/2352 image!");
                image.Close();
                return;
            }

            long realsectors = file.imagesize();
            long imagesectors = imagesize / 2352;
            long realsize = realsectors * 2352;
            if (imagesize == realsize)
            {
                Console.WriteLine($"Size (bytes):   {imagesize} (OK)");
                Console.WriteLine($"Size (sectors): {imagesectors} (OK)");
            }
            else
            {
                Console.WriteLine($"Size (bytes):   {imagesize}");
                Console.WriteLine($"From image:     {realsize}");
                Console.WriteLine($"Size (sectors): {imagesectors}");
                Console.WriteLine($"From image:     {realsectors}");
            }

            Console.WriteLine($"EDC in Form 2 sectors: {(GetEDCStatus(image) ? "YES" : "NO")}");

            string exe = file.exe();
            Console.WriteLine($"ID: {exe.Substring(0, 4)}-{exe.Substring(5)}");
            Console.WriteLine($"Date: {file.date()}");

            Console.Write("System area: ");
            image.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[2352];
            CRC32 crc = new CRC32();
            for (uint i = 0; i < 16; i++)
            {
                image.Read(buffer, 0, 2352);
                crc.ProcessCRC(buffer, 0, 2352);
            }

            uint imagecrc = crc.m_crc32;
            Console.WriteLine($"{Info.GetEdcType(imagecrc)}");
            Console.WriteLine();

            image.Close();
            return;
        }

        public static void Help()
        {
            Console.Write("Usage:\n");

            Console.Write("======\n");

            Console.Write("psxt001z.exe image.bin\n");
            Console.Write("  Display image's info.\n\n");

            Console.Write("psxt001z.exe --scan image.bin\n");
            Console.Write("  Scan image.bin postgap for errors.\n\n");

            Console.Write("psxt001z.exe --fix image.bin\n");
            Console.Write("  Scan image.bin postgap for errors and fix them.\n\n");

            //Console.Write("psxt001z.exe --libcryptdrvfast <drive letter>\n");
            //Console.Write("  Check subchannels for LibCrypt protection using new detection\n  method (disc).\n\n");

            Console.Write("psxt001z.exe --checksums file [start [end]]\n");
            Console.Write("  Calculate file's checksums (CRC-32, MD5, SHA-1).\n");
            Console.Write("  [in] file   Specifies the file, which checksums will be calculated.\n");
            Console.Write("       start  Specifies start position for checksums calculation.\n");
            Console.Write("       size   Specifies size of block for checksums calculation.\n\n");

            Console.Write("psxt001z.exe --zektor image.bin\n");
            Console.Write("  Zektor. Replace EDC in Form 2 Mode 2 sectors with zeroes.\n\n");

            Console.Write("psxt001z.exe --antizektor image.bin\n");
            Console.Write("  Antizektor. Restore EDC in Form 2 Mode 2 sectors.\n\n");

            Console.Write("psxt001z.exe --resize image.bin size\n");
            Console.Write("  Resize file to requested size.\n\n");

            Console.Write("psxt001z.exe --patch image.bin patch.bin offset\n");
            Console.Write("  Insert patch.bin into image.bin, skipping given number of bytes from the\n  offset.\n\n");

            Console.Write("psxt001z.exe --track image.bin bytes_to_skip size crc-32 [r] [+/-/f] [s filename]\n");
            Console.Write("  Try to guess an offset correction of the image dump by searching a track with\n  given size and CRC-32.\n  r - Calculate crc with RIFF header.\n  +/- - Search only for positive or negative offset correction.\n  s - Save track with given filename.\n\n");

            Console.Write("psxt001z.exe --gen file.bin filesize [-r]\n");
            Console.Write("  Generate a file of the requested size.\n  -r - add RIFF header.\n\n");

            Console.Write("psxt001z.exe --str file.str video.str audio.xa\n");
            Console.Write("  Deinterleave file.str to video.str and audio.xa.\n\n");

            Console.Write("psxt001z.exe --str2bs file.str\n");
            Console.Write("  Convert file.str to .bs-files.\n\n");

            Console.Write("psxt001z.exe --sub subchannel.sub size\n");
            Console.Write("  Generate RAW subchannel with given size (in sectors).\n\n");

            Console.Write("psxt001z.exe --m3s subchannel.m3s\n");
            Console.Write("  Generate M3S subchannel.\n\n");

            Console.Write("psxt001z.exe --libcrypt <sub> [<sbi>]\n");
            Console.Write("Usage: psxt001z.exe --libcrypt <sub> [<sbi>]\n");
            Console.Write("  Check subchannels for LibCrypt protection. (file)\n");
            Console.Write("  [in]  <sub>   Specifies the subchannel file to be scanned.\n");
            Console.Write("  [out] <sbi>   Specifies the subchannel file in SBI format where protected\n  sectors will be written.\n\n");

            //Console.Write("psxt001z.exe --libcryptdrv <drive letter>\n");
            //Console.Write("  Check subchannels for LibCrypt protection (disc).\n\n");

            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }

        public static bool Patch(string output, string input, int skip = 0)
        {
            Stream f1, f2;
            try
            {
                f1 = File.Open(output, FileMode.Open, FileAccess.ReadWrite);
                f2 = File.Open(input, FileMode.Open, FileAccess.Read);

                f1.Seek(skip, SeekOrigin.Begin);

                Console.WriteLine($"Patching \"{output}\" with \"{input}\", skipping {skip} bytes...");

                int i = 0;
                while (f1.Position < f1.Length && f2.Position < f2.Length)
                {
                    byte[] buffer = new byte[1];
                    f2.Read(buffer, 0, 1);
                    f1.Write(buffer, 0, 1);
                    i++;
                }

                Console.WriteLine("Done!");
                Console.WriteLine();
                Console.WriteLine($"{i} bytes were replaced");
                Console.WriteLine("File was successully patched!");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public static bool Resize(string input, long newsize)
        {
            Stream f;
            try
            {
                f = File.Open(input, FileMode.Open, FileAccess.ReadWrite);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            FileTools image = new FileTools(f);
            switch (image.resize(newsize))
            {
                case 0:
                    Console.Write($"File's \"{input}\" size is already {newsize} bytes!");
                    break;
                case 1:
                    Console.Write($"File \"{input}\" was successfully resized to {newsize} bytes!");
                    break;
                case 2:
                    Console.Write($"File \"{input}\" was successfully truncated to {newsize} bytes!");
                    break;
            }

            return true;
        }

        public static bool Copy(string input, string output, long startbyte, long length)
        {
            Stream infile;
            try
            {
                infile = File.Open(input, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File {input} can't be found.");
                return true;
            }

            return true;
            //HANDLE hfile = CreateFileW(argv[1], GENERIC_WRITE, FILE_SHARE_READ, 0, OPEN_EXISTING, FILE_FLAG_SEQUENTIAL_SCAN, 0);
            //SetFilePointer(hfile, newsize, 0, FILE_BEGIN);
            //SetEndOfFile(hfile);
        }

        /// <summary>
        /// Generate EDC data for a Mode2/2352 image
        /// </summary>
        public static void SetEDCData(string filename)
        {
            Stream image = File.Open(filename, FileMode.Open, FileAccess.ReadWrite);

            byte[] ecc_f_lut = new byte[256];
            byte[] ecc_b_lut = new byte[256];
            int[] edc_lut = new int[256];

            for (int a = 0; a < 256; a++)
            {
                int b = ((a << 1) ^ ((a & 0x80) != 0 ? 0x11D : 0));

                ecc_f_lut[a] = (byte)b;
                ecc_b_lut[a ^ b] = (byte)a;

                int edc_init = a;
                for (b = 0; b < 8; b++)
                {
                    edc_init = (int)((edc_init >> 1) ^ ((edc_init & 1) != 0 ? 0xD8018001 : 0));
                }

                edc_lut[a] = edc_init;
            }

            long filesize = image.Length;
            if (filesize % 2352 != 0)
            {
                Console.Write($"File '{filename}' is not Mode2/2352 image!");
                image.Close();
                return;
            }

            long sectors = filesize / 2352;
            Console.WriteLine("Converting image...");
            for (long sector = 0; sector < sectors; sector++)
            {
                image.Seek(sector * 2352 + 18, SeekOrigin.Begin);

                byte[] z = new byte[1];
                image.Read(z, 0, 1);
                if ((z[0] >> 5 & 0x1) != 0)
                {
                    image.Seek(-3, SeekOrigin.Current);

                    byte[] buffer = new byte[2332];
                    image.Read(buffer, 0, 2332);
                    image.Seek(0, SeekOrigin.Current);

                    buffer = BitConverter.GetBytes(CalculateEDC(buffer, 0, 2332, edc_lut));
                    image.Write(buffer, 0, 4);
                }
            }

            image.Close();
            Console.WriteLine("Done!");
            return;
        }

        /// <summary>
        /// Calculate CRC-32, MD5, and SHA-1 checksums
        /// </summary>
        public static byte Checksums(string[] args)
        {
            if (args.Length < 3 || args.Length > 5)
            {
                ChecksumsHelp();
                return 0;
            }

            // Opening file
            Stream file = File.OpenRead(args[2]);
            Console.WriteLine($"File:   {args[2]}");

            double percents = 0;

            long filesize = file.Length;

            long start;
            if (args.Length > 3)
            {
                start = long.Parse(args[3]);
                if (start >= filesize)
                {
                    Console.WriteLine("Error:  start position can't be larger than filesize!");
                    return 0;
                }

                Console.WriteLine($"Start:  {start}");
            }
            else
            {
                start = 0;
            }

            long block;
            if (args.Length > 4)
            {
                block = long.Parse(args[4]);
                if (block > filesize)
                {
                    Console.WriteLine("Error:  block size can't be larger than filesize!");
                    return 0;
                }

                if (block == 0)
                {
                    Console.WriteLine("Error:  block size can't equal with zero!");
                    return 0;
                }
            }
            else
            {
                block = filesize - start;
            }

            if (block + start > filesize)
            {
                Console.WriteLine("Error:  block size and start position can't be larger than file size!");
                return 0;
            }

            Console.Write($"Size:   {block}");
            long total = (long)Math.Ceiling((double)block / 1024);

            // checksums
            byte[] Message_Digest = new byte[20];
            byte[] buffer = new byte[1024], digest = new byte[16];
            int len;

            CRC32 crc = new CRC32();
            MD5 md5 = MD5.Create();
            md5.Initialize();
            SHA1 sha1 = SHA1.Create();

            file.Seek(start, SeekOrigin.Begin);

            for (uint i = 0; i < total; i++)
            {
                if (i * 100 / total > percents)
                {
                    percents = i * 100 / total;
                    Console.Write($"\rCalculating checksums: {percents}%");
                }

                len = file.Read(buffer, 0, 1024);

                if (block <= len)
                {
                    len = (int)block;
                    block = 0;
                }
                else
                {
                    block -= 1024;
                }

                md5.TransformBlock(buffer, 0, len, null, 0);
                sha1.TransformBlock(buffer, 0, len, null, 0);
                crc.ProcessCRC(buffer, 0, len);
            }

            md5.TransformFinalBlock(digest, 0, digest.Length);
            sha1.TransformFinalBlock(Message_Digest, 0, Message_Digest.Length);

            Console.Write($"\rCRC-32: {crc.m_crc32:8x}                      \n");
            Console.Write($"MD5:    {BitConverter.ToString(md5.Hash).Replace("-", string.Empty)}");
            Console.WriteLine($"MD5:    {BitConverter.ToString(md5.Hash).Replace("-", string.Empty)}");
            Console.WriteLine($"SHA-1:  {BitConverter.ToString(sha1.Hash).Replace("-", string.Empty)}");
            Console.WriteLine();
            return 1;
        }

        /// <summary>
        /// Generate missing RIFF headers
        /// </summary>
        public static void Generate(string[] args)
        {
            Stream f = File.OpenWrite(args[2]);
            byte[] riff = new byte[44];
            long size = long.Parse(args[3]);

            if (args.Length == 5)
            {
                if (args[4] == "-r")
                {
                    riff[0] = 0x52;
                    riff[1] = 0x49;
                    riff[2] = 0x46;
                    riff[3] = 0x46;
                    riff[4] = (byte)((size - 8) & 0xFF);
                    riff[5] = (byte)((size - 8) >> 8);
                    riff[6] = (byte)((size - 8) >> 16);
                    riff[7] = (byte)((size - 8) >> 24);
                    riff[8] = 0x57;
                    riff[9] = 0x41;
                    riff[10] = 0x56;
                    riff[11] = 0x45;
                    riff[12] = 0x66;
                    riff[13] = 0x6D;
                    riff[14] = 0x74;
                    riff[15] = 0x20;
                    riff[16] = 0x10;
                    riff[17] = 0x00;
                    riff[18] = 0x00;
                    riff[19] = 0x00;
                    riff[20] = 0x01;
                    riff[21] = 0x00;
                    riff[22] = 0x02;
                    riff[23] = 0x00;
                    riff[24] = 0x44;
                    riff[25] = 0xAC;
                    riff[26] = 0x00;
                    riff[27] = 0x00;
                    riff[28] = 0x10;
                    riff[29] = 0xB1;
                    riff[30] = 0x02;
                    riff[31] = 0x00;
                    riff[32] = 0x04;
                    riff[33] = 0x00;
                    riff[34] = 0x10;
                    riff[35] = 0x00;
                    riff[36] = 0x64;
                    riff[37] = 0x61;
                    riff[38] = 0x74;
                    riff[39] = 0x61;
                    riff[40] = (byte)((size - 44) & 0xFF);
                    riff[41] = (byte)((size - 44) >> 8);
                    riff[42] = (byte)((size - 44) >> 16);
                    riff[43] = (byte)((size - 44) >> 24);

                    f.Write(riff, 0, 44);
                }
            }

            f.Seek(size - 1, SeekOrigin.Begin);
            f.WriteByte(0x00);
            f.Close();

            Console.WriteLine($"File '{args[2]}' with size {size} bytes was successfully generated!");
            return;
        }

        /// <summary>
        /// Create a generic M3S subchannel file for a sector count
        /// </summary>
        /// <param name="filename"></param>
        public static void M3S(string filename)
        {
            byte[] buffer = { 0x41, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            Stream subchannel = File.Open(filename, FileMode.Create, FileAccess.Write);
            Console.Write($"File: {filename}");

            for (long sector = 13350, sector2 = sector + 150; sector < 17850; sector++, sector2++)
            {
                double mindbl = sector / 60 / 75;
                byte min = (byte)Math.Floor(mindbl);

                double secdbl = (sector - (min * 60 * 75)) / 75;
                byte sec = (byte)Math.Floor(secdbl);

                byte frame = (byte)(sector - (min * 60 * 75) - (sec * 75));

                buffer[3] = itob(min);
                buffer[4] = itob(sec);
                buffer[5] = itob(frame);

                mindbl = sector2 / 60 / 75;
                min = (byte)Math.Floor(mindbl);

                secdbl = (sector2 - (min * 60 * 75)) / 75;
                sec = (byte)Math.Floor(secdbl);

                frame = (byte)(sector2 - (min * 60 * 75) - (sec * 75));

                buffer[7] = itob(min);
                buffer[8] = itob(sec);
                buffer[9] = itob(frame);

                ushort crc = CRC16.Calculate(buffer, 0, 10);
                subchannel.Write(buffer, 0, 10);
                subchannel.WriteByte((byte)(crc >> 8));
                subchannel.WriteByte((byte)(crc & 0xFF));

                for (int i = 0; i < 4; i++)
                {
                    subchannel.WriteByte(0x00);
                }

                Console.WriteLine($"Creating M3S: {100 * sector}%\r");
            }

            Console.WriteLine("Creating M3S: 100%");

            subchannel.Close();

            Console.WriteLine("Done!");
            return;
        }

        /// <summary>
        /// Matrix 3 input files into a single output
        /// </summary>
        public static void Matrix(string[] args)
        {
            Stream f1 = File.Open(args[2], FileMode.Open, FileAccess.ReadWrite);
            Stream f2 = File.Open(args[3], FileMode.Open, FileAccess.ReadWrite);
            Stream f3 = File.Open(args[4], FileMode.Open, FileAccess.ReadWrite);
            Stream f4 = File.Open(args[5], FileMode.Create, FileAccess.Write);

            long subsize = f1.Length;
            for (long i = 0; i < subsize; i++)
            {
                byte[] r1 = new byte[1];
                f1.Read(r1, 0, 1);

                byte[] r2 = new byte[1];
                f2.Read(r2, 0, 1);

                byte[] r3 = new byte[1];
                f3.Read(r3, 0, 1);

                if (r1 == r2)
                {
                    f4.Write(r1, 0, 1);
                }
                else if (r1 == r3)
                {
                    f4.Write(r1, 0, 1);
                }
                else if (r2 == r3)
                {
                    f4.Write(r2, 0, 1);
                }
                else
                {
                    Console.WriteLine($"Byte 0x{i:x} ({i}) is different!");
                    Console.WriteLine($"{args[2]}: {r1[0]:2x}");
                    Console.WriteLine($"{args[3]}: {r2[0]:2x}");
                    Console.WriteLine($"{args[4]}: {r3[0]:2x}");
                    Console.WriteLine();
                    return;
                }
            }

            Console.WriteLine("Done!");
            return;
        }

        /// <summary>
        /// Split a STR-formatted image into audio and video
        /// </summary>
        public static void SplitStr(string[] argv)
        {
            Stream str = File.OpenRead(argv[2]);
            long filesize = str.Length;
            if (filesize % 2336 != 0)
            {
                Console.Write($"File '{argv[2]}' is not in STR format!");
                str.Close();
                return;
            }

            long sectors = filesize / 2336;
            Stream video = File.OpenWrite(argv[3]);

            sectors = filesize / 2336;
            Stream audio = File.OpenWrite(argv[4]);

            for (long i = 0; i < sectors; i++)
            {
                str.Seek(2, SeekOrigin.Current);

                byte[] ctrlbyte = new byte[1];
                str.Read(ctrlbyte, 0, 1);

                byte[] buffer = new byte[2336];
                if ((ctrlbyte[0] >> 5 & 0x1) != 0)
                {
                    str.Seek(-3, SeekOrigin.Current);
                    str.Read(buffer, 0, 2336);
                    audio.Write(buffer, 0, 2336);
                }
                else
                {
                    str.Seek(5, SeekOrigin.Current);
                    str.Read(buffer, 0, 2048);
                    video.Write(buffer, 0, 2048);
                    str.Seek(280, SeekOrigin.Current);
                }
            }

            str.Close();
            audio.Close();
            video.Close();

            Console.WriteLine("Done!");

            return;
        }

        /// <summary>
        /// Split a STR-formatted file into BS-formatted blocks
        /// </summary>
        public static void StrToBs(string[] argv)
        {
            byte[] buffer = new byte[2016];
            int filenamesize = argv[2].Length;
            string directory = $"{argv[2]}-bs";

            Stream str = File.OpenRead(argv[2]);
            long filesize = str.Length;
            if (filesize % 2048 != 0)
            {
                Console.Write($"File '{argv[2]}; is not in STR format!");
                str.Close();
                return;
            }

            /*wchar_t newfilename[2048];
			for (u8 f = 0; f < strlen(filename); f++) {
				newfilename[f] = filename[f];
				*((char *)newfilename +f*2 +1) = 0;
			}*/

            Directory.CreateDirectory(directory);

            long numblocks = filesize / 2048;
            Stream bs = File.OpenWrite(directory + "\\000001.bs");

            ushort a = 1;
            ushort b = 0;
            ushort c = 0;

            byte[] ax = { 0, 0 };
            byte[] bx = { 0, 0 };
            byte[] cx = { 0, 0 };

            str.Seek(32, SeekOrigin.Current);
            str.Read(buffer, 0, 2016);
            Console.WriteLine(directory);

            bs.Write(buffer, 0, 2016);
            Console.WriteLine("2");
            Console.WriteLine($"Creating: {directory}\\000001.bs");

            for (uint i = 1; i < numblocks; i++)
            {
                str.Seek(1, SeekOrigin.Current);

                byte[] byt = new byte[1];
                str.Read(byt, 0, 1);

                if (byt[0] == 0)
                {
                    bs.Close();
                    bs = File.OpenWrite(directory + $"\\{i.ToString().PadLeft(6, '0')}.bs");
                    Console.WriteLine($"Creating: {directory}\\{i.ToString().PadLeft(6, '0')}.bs");
                }

                str.Seek(30, SeekOrigin.Current);
                str.Read(buffer, 0, 2016);
                bs.Write(buffer, 0, 2016);
            }

            bs.Close();
            str.Close();

            Console.WriteLine();
            Console.WriteLine("Done!");

            return;
        }

        /// <summary>
        /// Create a generic SUB subchannel file for a sector count
        /// </summary>
        public static void CreateSubchannel(string filename, string strsectors)
        {
            long sectors = long.Parse(strsectors);
            if (sectors == 0 || sectors == -1)
            {
                Console.WriteLine("Wrong size!");
                return;
            }

            Stream subchannel = File.Open(filename, FileMode.Create, FileAccess.Write);

            Console.Write($"File: {filename}");
            Console.Write($"Size (bytes): {sectors * 96}");
            Console.Write($"Size (sectors): {sectors}");

            byte[] buffer = new byte[10];
            buffer[0] = 0x41;
            buffer[1] = 0x01;
            buffer[2] = 0x01;
            buffer[6] = 0x00;

            for (long sector = 0, sector2 = 150; sector < sectors; sector++, sector2++)
            {
                /*if (sector2 == 4350) {
                    buffer[1] = 0x02;
                    sector = 0;
                }*/

                double mindbl = sector / 60 / 75;
                byte min = (byte)Math.Floor(mindbl);

                double secdbl = (sector - (min * 60 * 75)) / 75;
                byte sec = (byte)Math.Floor(secdbl);

                byte frame = (byte)(sector - (min * 60 * 75) - (sec * 75));

                buffer[3] = itob(min);
                buffer[4] = itob(sec);
                buffer[5] = itob(frame);

                mindbl = sector2 / 60 / 75;
                min = (byte)Math.Floor(mindbl);

                secdbl = (sector2 - (min * 60 * 75)) / 75;
                sec = (byte)Math.Floor(secdbl);

                frame = (byte)(sector2 - (min * 60 * 75) - (sec * 75));

                buffer[7] = itob(min);
                buffer[8] = itob(sec);
                buffer[9] = itob(frame);

                ushort crc = CRC16.Calculate(buffer, 0, 10);

                for (int i = 0; i < 12; i++)
                {
                    subchannel.WriteByte(0x00);
                }

                subchannel.Write(buffer, 0, 10);
                subchannel.WriteByte((byte)(crc >> 8));
                subchannel.WriteByte((byte)(crc & 0xFF));

                for (int i = 0; i < 72; i++)
                {
                    subchannel.WriteByte(0x00);
                }

                Console.Write("Creating: %02u%%\r", (100 * sector) / sectors);
            }

            subchannel.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < 12; i++)
            {
                subchannel.WriteByte(0xFF);
            }

            Console.WriteLine("Creating: 100%");

            subchannel.Close();

            Console.WriteLine("Done!");
            return;
        }

        /// <summary>
        /// Clear EDC data from a Mode2/2352 image
        /// </summary>
        public static void ClearEDCData(string filename)
        {
            byte[] zero = { 0x00, 0x00, 0x00, 0x00 };

            Stream image = File.Open(filename, FileMode.Open, FileAccess.ReadWrite);
            long filesize = image.Length;
            if (filesize % 2352 != 0)
            {
                Console.WriteLine($"File '{filename}' is not Mode2/2352 image!");
                image.Close();
                return;
            }

            Console.WriteLine("Converting image...");

            long sectors = filesize / 2352;
            for (long sector = 0; sector < sectors; sector++)
            {
                image.Seek(sector * 2352 + 18, SeekOrigin.Begin);

                byte[] z = new byte[1];
                image.Read(z, 0, 1);
                if ((z[0] >> 5 & 0x1) != 0)
                {
                    image.Seek(2329, SeekOrigin.Current);
                    image.Write(zero, 0, 4);
                }
            }

            image.Close();
            Console.WriteLine("Done!");
            return;
        }

        #region Help

        private static void ChecksumsHelp()
        {
            Console.WriteLine("psxt001z.exe --checksums file [start [end]]");
            Console.WriteLine("  Calculate file's checksums (CRC-32, MD5, SHA-1).");
            Console.WriteLine("  [in] file   Specifies the file, which checksums will be calculated.");
            Console.WriteLine("       start  Specifies start position for checksums calculation.");
            Console.WriteLine("       size   Specifies size of block for checksums calculation.");
            Console.WriteLine();
        }

        private static void LibCryptHelp()
        {
            Console.WriteLine("LibCrypt detector\nUsage: psxt001z.exe --libcrypt <sub> [<sbi>]");
            Console.WriteLine("  Check subchannel for LibCrypt protection.");
            Console.WriteLine("  [in]  <sub>  Specifies the subchannel file to be scanned.");
            Console.WriteLine("  [out] <sbi>  Specifies the subchannel file in SBI format where protected\n               sectors will be written.");
            Console.WriteLine();
        }

        #endregion
    }
}
