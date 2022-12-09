using System;
using System.IO;

namespace psxt001z
{
    internal class Track
    {
        #region Properties

        /// <summary>
        /// Original input path for the track
        /// </summary>
        private string InputPath { get; set; }

        /// <summary>
        /// Stream representing the track data
        /// </summary>
        private Stream InputStream { get; set; }

        /// <summary>
        /// Output for saving the track data
        /// </summary>
        private string OutputPath { get; set; }

        /// <summary>
        /// Starting offset within the file
        /// </summary>
        private int Start { get; set; }

        /// <summary>
        /// Size of the input file
        /// </summary>
        private int Size { get; set; }

        /// <summary>
        /// CRC-32 of the track data to compare against
        /// </summary>
        private uint CRC32 { get; set; }

        /// <summary>
        /// Audio data header
        /// </summary>
        private byte[] RiffData { get; set; } = new byte[44];

        /// <summary>
        /// True if the track is audio data
        /// </summary>
        private bool IsRiff { get; set; }

        /// <summary>
        /// True if the file is under ~100MB 
        /// </summary>
        private bool SmallFile { get; set; } = false;

        /// <summary>
        /// True to write out the track data
        /// </summary>
        private bool SaveTrack { get; set; }

        /// <summary>
        /// True means '+' or 'p'
        /// False means '-' or 'n'
        /// Null means neither
        /// </summary>
        private bool? Mode { get; set; }

        /// <summary>
        /// Cache for small file data
        /// </summary>
        private byte[] FileContents { get; set; }

        /// <summary>
        /// Cached file offset
        /// </summary>
        private int Offset { get; set; } = 0;

        /// <summary>
        /// Current file offset
        /// </summary>
        private int Current { get; set; } = 0;

        #endregion

        #region Constructor

        public Track(string filename, int start, int size, uint crc, bool isRiff = false, bool? mode = null, string output = null)
        {
            InputPath = filename;
            InputStream = File.OpenRead(filename);
            OutputPath = output;

            Start = start;
            Size = size;
            CRC32 = crc;
            Mode = mode;

            IsRiff = isRiff;
            SmallFile = Size <= 100_000_000;
            SaveTrack = output != null;

            Console.WriteLine($"File: {InputPath}\nStart: {Start}\nSize: {Size}\nCRC-32: {CRC32:8x}");
            Console.WriteLine();

            if (IsRiff)
                PopulateRiffData();

            if (SmallFile)
                CacheFileData();
        }

        #endregion

        #region Functions

        public bool FindTrack()
        {
            // Positive mode
            if (Mode == true)
            {
                if (Current > 20000)
                {
                    Mode = null;
                    return true;
                }

                Offset = Current;
                Current += 4;
                return MatchesCRC();
            }

            // Negative mode
            else if (Mode == false)
            {
                if (Current > 20000)
                {
                    Mode = null;
                    return true;
                }

                Offset = -Current;
                Current += 4;
                return MatchesCRC();
            }

            // Neutral mode
            else
            {
                if (Current > 20000)
                {
                    Mode = null;
                    return true;
                }

                Offset = Current;
                if (MatchesCRC())
                {
                    return true;
                }
                else
                {
                    Offset = -Current;
                    Current += 4;
                    return MatchesCRC();
                }
            }
        }

        public bool MatchesCRC()
        {
            CRC32 calc = new CRC32();
            if (SmallFile)
            {
                if (IsRiff)
                    calc.ProcessCRC(RiffData, 0, 44);

                calc.ProcessCRC(FileContents, (int)(20000 + Offset), (int)Size);
            }
            else
            {
                InputStream.Seek(Start + Offset, SeekOrigin.Begin);
                if (IsRiff)
                    calc.ProcessCRC(RiffData, 0, 44);

                for (long i = 0; i < Size; i++)
                {
                    byte[] buffer = new byte[1];
                    if (InputStream.Read(buffer, 0, 1) != 1)
                    {
                        buffer[0] = 0x00;
                        InputStream.Seek(Start + Offset + i + 1, SeekOrigin.Begin);
                    }

                    calc.ProcessCRC(buffer, 0, 1);
                }
            }

            Console.Write($"Offset correction {Offset} bytes, {Offset / 4} samples, CRC-32 {calc.m_crc32:8x}");
            return (calc.m_crc32 == CRC32);
        }

        public void Done()
        {
            if (SmallFile)
                FileContents = null;

            if (Mode == null)
            {
                Console.WriteLine();
                Console.WriteLine("Can't find offset!");
                return;
            }

            if (SaveTrack)
            {
                byte[] buffer = new byte[1];
                Stream f2 = File.Open(OutputPath, FileMode.Create, FileAccess.ReadWrite);
                if (IsRiff)
                    f2.Write(RiffData, 0, 44);

                InputStream.Seek(Start + Offset, SeekOrigin.Begin);
                for (long i = 0; i < Size; i++)
                {
                    if (InputStream.Read(buffer, 0, 1) != 1)
                    {
                        buffer[0] = 0x00;
                        InputStream.Seek(Start + Offset + i + 1, SeekOrigin.Begin);
                    }

                    f2.Write(buffer, 0, 1);
                }
            }

            Console.WriteLine();
            Console.Write("DONE!");
            Console.WriteLine();
            Console.Write($"Offset correction: {Offset} bytes / {Offset / 4} samples");
        }

        #endregion

        #region Utilities

        // TODO: Figure out what this actually does
        private void CacheFileData()
        {
            FileContents = new byte[Size + 40000];
            InputStream.Seek(Start - 20000, SeekOrigin.Begin);

            for (int i = 0; i < Size + 40000; i++)
            {
                if (Start + i <= 20000)
                    InputStream.Seek(Start + i - 20000, SeekOrigin.Begin);

                if (InputStream.Read(new byte[1], 0, 1) != 1)
                    FileContents[i] = 0x00;
            }
        }

        private void PopulateRiffData()
        {
            RiffData[0] = 0x52;
            RiffData[1] = 0x49;
            RiffData[2] = 0x46;
            RiffData[3] = 0x46;

            byte[] temp = BitConverter.GetBytes(Size - 8);
            Array.Copy(temp, 0, RiffData, 4, 4);

            RiffData[8] = 0x57;
            RiffData[9] = 0x41;
            RiffData[10] = 0x56;
            RiffData[11] = 0x45;
            RiffData[12] = 0x66;
            RiffData[13] = 0x6D;
            RiffData[14] = 0x74;
            RiffData[15] = 0x20;
            RiffData[16] = 0x10;
            RiffData[17] = 0x00;
            RiffData[18] = 0x00;
            RiffData[19] = 0x00;
            RiffData[20] = 0x01;
            RiffData[21] = 0x00;
            RiffData[22] = 0x02;
            RiffData[23] = 0x00;
            RiffData[24] = 0x44;
            RiffData[25] = 0xAC;
            RiffData[26] = 0x00;
            RiffData[27] = 0x00;
            RiffData[28] = 0x10;
            RiffData[29] = 0xB1;
            RiffData[30] = 0x02;
            RiffData[31] = 0x00;
            RiffData[32] = 0x04;
            RiffData[33] = 0x00;
            RiffData[34] = 0x10;
            RiffData[35] = 0x00;
            RiffData[36] = 0x64;
            RiffData[37] = 0x61;
            RiffData[38] = 0x74;
            RiffData[39] = 0x61;

            temp = BitConverter.GetBytes(Size - 44);
            Array.Copy(temp, 0, RiffData, 40, 4);

            Size -= 44;
        }

        #endregion
    }
}
