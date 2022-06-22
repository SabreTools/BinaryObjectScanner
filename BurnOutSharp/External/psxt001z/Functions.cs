using System;
using System.IO;
using System.Text;
using static psxt001z.Common;

namespace psxt001z
{
    internal static class Functions
    {
        public static int CalculateEDC(in byte[] src, int srcPtr, int size, int[] edc_lut)
        {
            int edc = 0;
            while (size-- != 0)
            {
                edc = (edc >> 8) ^ edc_lut[(edc ^ src[srcPtr++]) & 0xFF];
            }

            return edc;
        }

        public static bool ZeroCompare(byte[] buffer, int bufferPtr, int bsize)
        {
            for (int i = 0; i < bsize; i++)
            {
                if (buffer[bufferPtr + i] != 0x00)
                    return false;
            }

            return true;
        }

        public static void MSF(long lba, byte[] buffer, int bufferOffset)
        {
            lba += 150;

            double mindbl = lba / 60 / 75;
            byte min = (byte)Math.Floor(mindbl);
            double secdbl = (lba - (min * 60 * 75)) / 75;
            byte sec = (byte)Math.Floor(secdbl);
            byte frame = (byte)(lba - (min * 60 * 75) - (sec * 75));

            buffer[bufferOffset] = itob(min);
            buffer[bufferOffset + 1] = itob(sec);
            buffer[bufferOffset + 2] = itob(frame);

            return;
        }

        public static bool GetEDCStatus(Stream file)
        {
            long currentposition = file.Position;

            file.Seek(30572, SeekOrigin.Begin);

            byte[] buffer = new byte[4];
            file.Read(buffer, 0, 4);

            file.Seek(currentposition, SeekOrigin.Begin);

            return BitConverter.ToInt32(buffer, 0) == 0;
        }

        public static byte[] ExecutableName(Stream file)
        {
            byte[] buffer = new byte[20];
            byte[] exename = new byte[20];

            //Searching for SYSTEM.CNF
            file.Seek(51744, SeekOrigin.Begin);
            while (Encoding.ASCII.GetString(buffer) != "SYSTEM.CNF")
            {
                file.Read(buffer, 0, 10);
                buffer[10] = 0;
                file.Seek(-9, SeekOrigin.Current);
            }

            file.Seek(-32, SeekOrigin.Current);
            byte[] lba = new byte[4];
            file.Read(lba, 0, 4);
            file.Seek((2352 * BitConverter.ToInt32(lba, 0)) + 29, SeekOrigin.Begin);
            file.Read(buffer, 0, 6);
            buffer[6] = 0;
            while (Encoding.ASCII.GetString(buffer) != "cdrom:")
            {
                file.Seek(-5, SeekOrigin.Current);
                file.Read(buffer, 0, 6);
            }

            file.Read(buffer, 0, 1);
            if (buffer[0] != '\\')
                file.Seek(-1, SeekOrigin.Current);

            int i = -1;
            do
            {
                file.Read(buffer, ++i, 1);
            } while (buffer[i] != ';');

            for (int a = 0; a < i; a++)
            {
                exename[a] = (byte)char.ToUpper((char)buffer[a]);
            }

            exename[i] = 0;
            return exename;
        }
    }
}
