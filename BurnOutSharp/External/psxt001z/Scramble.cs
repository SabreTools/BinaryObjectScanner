using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using static psxt001z.Common;

namespace psxt001z
{
    public class Scramble
    {
        private static readonly byte[] sync = new byte[12] { 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 };

        public int __main(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                Console.WriteLine("Syntax: px_p8 [-t] filename");
                return 1;
            }

            int sectors;
            if (args.Length == 2 && args[0] == "-t")
            {
                args[0] = args[1];
                sectors = 2352;
            }
            else
            {
                sectors = 4704;
            }

            Stream sector_file;
            try
            {
                sector_file = File.OpenRead(args[1]);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return 1;
            }

            byte[] sector = new byte[sectors];
            if (args.Length == 2)
            {
                uint hex;
                for (int i = 0; sector_file.Position < sector_file.Length && i < 3252; i++)
                {
                    byte[] buf = new byte[2];
                    hex = uint.Parse(Encoding.ASCII.GetString(buf), NumberStyles.HexNumber);
                    sector[i] = (byte)hex;
                }
            }
            else
            {
                sector_file.Read(sector, 0, sectors);
            }

            int offset = MemSearch(sector, sync, sectors, 12);
            if (offset == -1)
            {
                Console.WriteLine("Error searching for sync!");
                return 1;
            }

            Console.WriteLine($"MSF: {sector[offset + 12]:2x}:{sector[offset + 12 + 1]:2x}:{sector[offset + 12 + 2]:2x}");

            int shiftRegister = 0x1;
            for (int i = 0; i < 3; i++)
            {
                sector[offset + 12 + i] ^= (byte)(shiftRegister & 0xFF);
                for (int j = 0; j < 8; j++)
                {
                    int hibit = ((shiftRegister & 1) ^ ((shiftRegister & 2) >> 1)) << 15;
                    shiftRegister = (hibit | shiftRegister) >> 1;
                }
            }

            int start_sector = (btoi(sector[offset + 12]) * 60 + btoi(sector[offset + 13])) * 75 + btoi(sector[offset + 14]) - 150;

            Console.WriteLine($"MSF: {sector[offset + 12]:2x}:{sector[offset + 12 + 1]:2x}:{sector[offset + 12 + 2]:2x}");

            offset -= start_sector * 2352;

            Console.WriteLine($"Combined offset: {offset} bytes / {offset / 4} samples");
            return 0;
        }

        /// <summary>
        /// Search for a byte array in another
        /// </summary>
        private int MemSearch(in byte[] buf_where, in byte[] buf_search, int buf_where_len, int buf_search_len)
        {
            for (int i = 0; i <= buf_where_len - buf_search_len; i++)
            {
                for (int j = 0; j < buf_search_len; j++)
                {
                    if (buf_where[i + j] != buf_search[j])
                        break;

                    if (j + 1 == buf_search_len)
                        return i;
                }
            }

            return -1;
        }
    }
}
