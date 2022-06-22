using System;
using System.IO;
using System.Linq;
using static psxt001z.Functions;

namespace psxt001z
{
    public class Info
    {
        #region Constants

        private static readonly byte[] edc_form_2 = { 0x3F, 0x13, 0xB0, 0xBE };

        private static readonly byte[] syncheader = { 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00 };

        private static readonly byte[] subheader = { 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x20, 0x00 };

        #endregion

        #region Functions

        public static int GetInfo(string filename, bool fix)
        {
            // Variables
            bool errors = false;
            byte[] buffer = new byte[2352], buffer2 = new byte[2352];
            int mode = 15; // synñheader[15];

            #region Opening image

            Stream image;
            try
            {
                FileAccess open_mode = fix ? FileAccess.ReadWrite : FileAccess.Read;
                image = File.Open(filename, FileMode.Open, open_mode);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return 1;
            }

            long size = image.Length;
            Console.WriteLine($"File: {filename}");

            #endregion

            #region Type

            image.Read(buffer, 0, 12);

            int sectorsize;
            if (buffer.Take(12).SequenceEqual(syncheader.Take(12)))
            {
                sectorsize = 2352;
            }
            else
            {
                sectorsize = 2048;
            }
            if (size % sectorsize != 0)
            {
                Console.WriteLine($"{filename}: not ModeX/{sectorsize} image!");
                return 1;
            }

            long sectors = size / sectorsize;

            #endregion

            #region Mode

            if (sectorsize == 2352)
            {
                image.Seek(0xF, SeekOrigin.Begin);
                mode = image.ReadByte();
                if (mode != 1 && mode != 2)
                {
                    Console.WriteLine($"{filename}: unknown mode!");
                    return 1;
                }
            }
            else
            {
                mode = -1;
            }

            #endregion

            #region Size

            image.Seek(sectorsize * 16 + ((mode == 2) ? 24 : ((mode == 1) ? 16 : 0)) + 0x50, SeekOrigin.Begin);

            // ISO size
            byte[] buf = new byte[4];
            image.Read(buf, 0, 4);
            int realsectors = BitConverter.ToInt32(buf, 0);

            image.Seek(0, SeekOrigin.Begin);
            int realsize = realsectors * sectorsize;
            if (sectors == realsectors)
            {
                Console.WriteLine($"Size (bytes):   {size} (OK)");
                Console.WriteLine($"Size (sectors): {sectors} (OK)");
            }
            else
            {
                Console.WriteLine($"Size (bytes):   {size}");
                Console.WriteLine($"From image:     {realsize}");
                Console.WriteLine($"Size (sectors): {sectors}");
                Console.WriteLine($"From image:     {realsectors}");
            }

            #endregion

            #region Mode

            if (mode > 0)
                Console.WriteLine($"Mode: {mode}");

            if (mode == 2)
            {
                #region EDC in Form 2

                bool imageedc = GetEDCStatus(image);
                Console.WriteLine($"EDC in Form 2 sectors: {(imageedc ? "YES" : "NO")}");

                #endregion

                #region Sysarea

                string systemArea = "System area: ";
                image.Seek(0, SeekOrigin.Begin);

                CRC32 crc = new CRC32();
                for (int i = 0; i < 16; i++)
                {
                    image.Read(buffer, 0, 2352);
                    crc.ProcessCRC(buffer, 0, 2352);
                }

                uint imagecrc = crc.m_crc32;
                systemArea += GetEdcType(imagecrc);

                Console.WriteLine(systemArea);

                #endregion

                #region Postgap

                image.Seek((sectors - 150) * sectorsize + 16, SeekOrigin.Begin);
                image.Read(buffer, 0, 2336);

                string postgap = "Postgap type: Form ";
                if ((buffer[2] >> 5 & 0x1) != 0)
                {
                    postgap += "2";
                    if (buffer.Take(8).SequenceEqual(subheader))
                        postgap += ", zero subheader";
                    else
                        postgap += ", non-zero subheader";

                    if (ZeroCompare(buffer, 8, 2324))
                        postgap += ", zero data";
                    else
                        postgap += ", non-zero data";

                    if (ZeroCompare(buffer, 2332, 4))
                        postgap += ", no EDC";
                    else
                        postgap += ", EDC";
                }
                else
                {
                    postgap += "1";
                    if (ZeroCompare(buffer, 0, 8))
                        postgap += ", zero subheader";
                    else
                        postgap += ", non-zero subheader";

                    if (ZeroCompare(buffer, 8, 2328))
                        postgap += ", zero data";
                    else
                        postgap += ", non-zero data";
                }

                Console.WriteLine(postgap);
                Array.Copy(buffer, buffer2, 2336);

                #endregion
            }

            if (mode < 0)
                return 0;

            for (long sector = sectors - 150; sector < sectors; sector++)
            {
                bool bad = false;
                image.Seek(sector * sectorsize, SeekOrigin.Begin);
                image.Read(buffer, 0, sectorsize);

                // Sync
                string sectorInfo = string.Empty;

                MSF(sector, syncheader, 12);
                if (!syncheader.SequenceEqual(buffer.Take(16)))
                {
                    sectorInfo += $"Sector {sector}: Sync/Header";
                    bad = true;
                    if (fix)
                    {
                        image.Seek(sector * sectorsize, SeekOrigin.Begin);
                        image.Write(syncheader, 0, 16);
                        sectorInfo += (" (fixed)");
                    }
                }

                // Mode 2
                if (mode == 2 && buffer.Skip(16).Take(2336).SequenceEqual(buffer2))
                {
                    if (bad)
                    {
                        sectorInfo += ", Subheader/Data/EDC/ECC";
                    }
                    else
                    {
                        sectorInfo = $"Sector {sector}: Subheader/Data/EDC/ECC";
                        bad = true;
                    }

                    if (fix)
                    {
                        image.Seek(sector * sectorsize + 16, SeekOrigin.Begin);
                        image.Write(buffer2, 0, 2336);
                        sectorInfo += " (fixed)";
                    }
                }

                Console.WriteLine(sectorInfo);

                if (bad && (sector + 1 != sectors))
                    errors = true;
            }

            if (errors)
            {
                Console.WriteLine("NOTICE: One or more errors were found not in the last sector.");
                Console.WriteLine("Please mention this when submitting dump info.");
            }
            else
            {
                Console.WriteLine("Done.");
            }

            #endregion

            return 0;
        }

        #endregion

        #region Utilities

        internal static string GetEdcType(uint imageCrc)
        {
            switch (imageCrc)
            {
                case 0x11e3052d:
                    return "Eu EDC";
                case 0x808c19f6:
                    return "Eu NoEDC";
                case 0x70ffa73e:
                    return "Eu Alt NoEDC";
                case 0x7f9a25b1:
                    return "Eu Alt 2 EDC";
                case 0x783aca30:
                    return "Jap EDC";
                case 0xe955d6eb:
                    return "Jap NoEDC";
                case 0x9b519a2e:
                    return "US EDC";
                case 0x0a3e86f5:
                    return "US NoEDC";
                case 0x6773d4db:
                    return "US Alt NoEDC";
                default:
                    return $"Unknown, crc {imageCrc:8x}";
            }
        }

        #endregion
    }
}
