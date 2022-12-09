using System;
using System.IO;
using System.Text;

namespace psxt001z
{
    internal class FileTools
    {
        #region Properties

        private Stream InputStream { get; set; }

        private byte[] ExeName { get; set; } = new byte[20];

        private byte[] DateValue { get; set; } = new byte[11];

        #endregion

        #region Constructor

        public FileTools(Stream file)
        {
            InputStream = file;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Get file size
        /// </summary>
        public long size() => InputStream.Length;

        /// <summary>
        /// Get executable name
        /// </summary>
        public string exe()
        {
            InputStream.Seek(51744, SeekOrigin.Begin);

            string filename = string.Empty;
            while (filename != "SYSTEM.CNF")
            {
                byte[] buf = new byte[10];
                InputStream.Read(buf, 0, 10);
                filename = Encoding.ASCII.GetString(buf);
                InputStream.Seek(-9, SeekOrigin.Current);
            }

            byte[] buffer = new byte[20];

            InputStream.Seek(-32, SeekOrigin.Current);
            InputStream.Read(buffer, 0, 4);
            uint lba = BitConverter.ToUInt32(buffer, 0);

            InputStream.Seek((2352 * lba) + 29, SeekOrigin.Begin);
            InputStream.Read(buffer, 0, 6);

            string iniLine = Encoding.ASCII.GetString(buffer);
            while (iniLine != "cdrom:")
            {
                InputStream.Seek(-5, SeekOrigin.Current);
                InputStream.Read(buffer, 0, 6);
                iniLine = Encoding.ASCII.GetString(buffer);
            }

            InputStream.Read(buffer, 0, 1);
            if (buffer[0] != '\\')
                InputStream.Seek(-1, SeekOrigin.Current);

            int i = -1;
            do
            {
                InputStream.Read(buffer, ++i, 1);
            } while (buffer[i] != ';');

            for (long a = 0; a < i; a++)
            {
                ExeName[a] = (byte)char.ToUpper((char)buffer[a]);
            }

            return Encoding.ASCII.GetString(ExeName);
        }

        /// <summary>
        /// Get human-readable date
        /// </summary>
        public string date()
        {
            byte[] buffer = new byte[12], datenofrmt = new byte[3];

            InputStream.Seek(51744, SeekOrigin.Begin);

            do
            {
                InputStream.Read(buffer, 0, 11);
                buffer[11] = 0;
                InputStream.Seek(-10, SeekOrigin.Current);
            } while (Encoding.ASCII.GetString(ExeName) != Encoding.ASCII.GetString(buffer));

            InputStream.Seek(-16, SeekOrigin.Current);
            InputStream.Read(datenofrmt, 0, 3);

            if (datenofrmt[0] < 50)
            {
                byte[] year = Encoding.ASCII.GetBytes($"{2000 + datenofrmt[0]}");
                Array.Copy(year, 0, buffer, 0, 4);
            }
            else
            {
                byte[] year = Encoding.ASCII.GetBytes($"{1900 + datenofrmt[0]}");
                Array.Copy(year, 0, buffer, 0, 4);
            }

            DateValue[4] = (byte)'-';
            if (datenofrmt[1] < 10)
            {
                byte[] month = Encoding.ASCII.GetBytes($"0{datenofrmt[1]}");
                Array.Copy(month, 0, buffer, 5, 2);
            }
            else
            {
                byte[] month = Encoding.ASCII.GetBytes($"{datenofrmt[1]}");
                Array.Copy(month, 0, buffer, 5, 2);
            }

            DateValue[7] = (byte)'-';
            if (datenofrmt[2] < 10)
            {
                byte[] day = Encoding.ASCII.GetBytes($"0{datenofrmt[2]}");
                Array.Copy(day, 0, buffer, 8, 2);
            }
            else
            {
                byte[] day = Encoding.ASCII.GetBytes($"{datenofrmt[2]}");
                Array.Copy(day, 0, buffer, 8, 2);
            }

            return Encoding.ASCII.GetString(DateValue);
        }

        /// <summary>
        /// Resize the image
        /// </summary>
        public int resize(long newsize)
        {
            long oldsize = size();
            if (oldsize < newsize)
            {
                InputStream.SetLength(newsize);
                return 1;
            }
            else if (oldsize > newsize)
            {
                InputStream.SetLength(newsize);
                return 2;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Get the reported sector count from the image
        /// </summary>
        public int imagesize()
        {
            InputStream.Seek(0x9368, SeekOrigin.Begin);
            byte[] sizebuf = new byte[4];
            InputStream.Read(sizebuf, 0, 4);
            return BitConverter.ToInt32(sizebuf, 0);
        }

        #endregion
    }
}
