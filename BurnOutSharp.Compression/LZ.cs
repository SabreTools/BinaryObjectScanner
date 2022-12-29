using System;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.Models.Compression.LZ;
using BurnOutSharp.Utilities;
using static BurnOutSharp.Models.Compression.LZ.Constants;

namespace BurnOutSharp.Compression
{
    /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/kernel32/lzexpand.c"/>
    public class LZ
    {
        /// <summary>
        /// LZStart   (KERNEL32.@)
        /// </summary>
        public LZERROR Start() => LZERROR.LZERROR_OK;

        /// <summary>
        /// Initializes internal decompression buffers, returns lzfiledescriptor.
        /// (return value the same as hfSrc, if hfSrc is not compressed)
        /// on failure, returns error code <0
        /// lzfiledescriptors range from 0x400 to 0x410 (only 16 open files per process)
        /// 
        /// since _llseek uses the same types as libc.lseek, we just use the macros of
        /// libc
        /// </summary>
        public LZERROR Init(Stream hfSrc, out State lzs)
        {
            LZERROR ret = ReadHeader(hfSrc, out FileHeaader head);
            if (ret != LZERROR.LZERROR_OK)
            {
                hfSrc.Seek(0, SeekOrigin.Begin);
                if (ret == LZERROR.LZERROR_NOT_LZ)
                {
                    lzs = new State { RealFD = hfSrc };
                    return ret;
                }
                else
                {
                    lzs = null;
                    return ret;
                }
            }

            lzs = new State();

            lzs.RealFD = hfSrc;
            lzs.LastChar = head.LastChar;
            lzs.RealLength = head.RealLength;

            lzs.Get = new byte[GETLEN];
            lzs.GetLen = 0;
            lzs.GetCur = 0;

            // Yes, preinitialize with spaces
            lzs.Table = Enumerable.Repeat((byte)' ', LZ_TABLE_SIZE).ToArray();

            // Yes, start 16 byte from the END of the table
            lzs.CurTabEnt = 0xff0;

            return LZERROR.LZERROR_OK;
        }

        /// <summary>
        /// LZDone   (KERNEL32.@)
        /// </summary>
        public void Done() { }

        /// <summary>
        /// gets the full filename of the compressed file 'in' by opening it
        /// and reading the header
        /// 
        /// "file." is being translated to "file"
        /// "file.bl_" (with lastchar 'a') is being translated to "file.bla"
        /// "FILE.BL_" (with lastchar 'a') is being translated to "FILE.BLA"
        /// </summary>
        public LZERROR GetExpandedName(string input, out string output)
        {
            output = null;

            State state = OpenFile(input, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            if (state.Get == null)
                return LZERROR.LZERROR_NOT_LZ;

            string inputExtension = Path.GetExtension(input).TrimStart('.');
            if (string.IsNullOrWhiteSpace(inputExtension))
            {
                output = Path.GetFileNameWithoutExtension(input);
                return LZERROR.LZERROR_OK;
            }

            if (inputExtension.Length == 1)
            {
                if (inputExtension == "_")
                {
                    output = $"{Path.GetFileNameWithoutExtension(input)}.{char.ToLower(state.LastChar)}";
                    return LZERROR.LZERROR_OK;
                }
                else
                {
                    output = Path.GetFileNameWithoutExtension(input);
                    return LZERROR.LZERROR_OK;
                }
            }

            if (!inputExtension.EndsWith("_"))
            {
                output = Path.GetFileNameWithoutExtension(input);
                return LZERROR.LZERROR_OK;
            }

            bool isLowerCase = char.IsUpper(output[0]);
            char replacementChar = isLowerCase ? char.ToLower(state.LastChar) : char.ToUpper(state.LastChar);
            string outputExtension = inputExtension.Substring(0, inputExtension.Length - 1) + replacementChar;
            output = $"{Path.GetFileNameWithoutExtension(input)}.{outputExtension}";
            return LZERROR.LZERROR_OK;
        }

        /// <summary>
        /// LZRead   (KERNEL32.@)
        /// </summary>
        public int Read(State lzs, byte[] vbuf, int toread)
        {
            int howmuch;
            byte b;
            int buf = 0; // vbufPtr

            howmuch = toread;

            // If we have an uncompressed stream
            if (lzs.Get == null)
                return lzs.RealFD.Read(vbuf, 0, toread);

            // The decompressor itself is in a define, cause we need it twice
            // in this function. (the decompressed byte will be in b)
            // DECOMPRESS_ONE_BYTE
            // {
            //     if (lzs.StringLen != 0)
            //     {
            //         b = lzs.Table[lzs.StringPos];
            //         lzs.StringPos = (lzs.StringPos + 1) & 0xFFF;
            //         lzs.StringLen--;
            //     }
            //     else
            //     {
            //         if ((lzs.ByteType & 0x100) == 0)
            //         {
            //             if (Get(lzs, out b) != LZERROR.LZERROR_OK)
            //                 return toread - howmuch;

            //             lzs.ByteType = (ushort)(b | 0xFF00);
            //         }
            //         if ((lzs.ByteType & 1) != 0)
            //         {
            //             if (Get(lzs, out b) != LZERROR.LZERROR_OK)
            //                 return toread - howmuch;
            //         }
            //         else
            //         {
            //             if (Get(lzs, out byte b1) != LZERROR.LZERROR_OK)
            //                 return toread - howmuch;
            //             if (Get(lzs, out byte b2) != LZERROR.LZERROR_OK)
            //                 return toread - howmuch;

            //             // Format:
            //             // b1 b2
            //             // AB CD
            //             // where CAB is the stringoffset in the table
            //             // and D+3 is the len of the string
            //             lzs.StringPos = (uint)(b1 | ((b2 & 0xf0) << 4));
            //             lzs.StringLen = (byte)((b2 & 0xf) + 2);

            //             // 3, but we use a byte already below...
            //             b = lzs.Table[lzs.StringPos];
            //             lzs.StringPos = (lzs.StringPos + 1) & 0xFFF;
            //         }

            //         lzs.ByteType >>= 1;
            //     }

            //     // Store b in table
            //     lzs.Table[lzs.CurTabEnt++] = b;
            //     lzs.CurTabEnt &= 0xFFF;
            //     lzs.RealCurrent++;
            // }

            // If someone has seeked, we have to bring the decompressor to that position
            if (lzs.RealCurrent != lzs.RealWanted)
            {
                // If the wanted position is before the current position
                // I see no easy way to unroll .. We have to restart at
                // the beginning. *sigh*
                if (lzs.RealCurrent > lzs.RealWanted)
                {
                    // Flush decompressor state
                    lzs.RealFD.Seek(LZ_HEADER_LEN, SeekOrigin.Begin);
                    GetFlush(lzs);
                    lzs.RealCurrent = 0;
                    lzs.ByteType = 0;
                    lzs.StringLen = 0;
                    lzs.Table = Enumerable.Repeat((byte)' ', LZ_TABLE_SIZE).ToArray();
                    lzs.CurTabEnt = 0xFF0;
                }

                while (lzs.RealCurrent < lzs.RealWanted)
                {
                    // DECOMPRESS_ONE_BYTE -- TODO: Make into helper method; original code is a DEFINE
                    if (lzs.StringLen != 0)
                    {
                        b = lzs.Table[lzs.StringPos];
                        lzs.StringPos = (lzs.StringPos + 1) & 0xFFF;
                        lzs.StringLen--;
                    }
                    else
                    {
                        if ((lzs.ByteType & 0x100) == 0)
                        {
                            if (Get(lzs, out b) != LZERROR.LZERROR_OK)
                                return toread - howmuch;

                            lzs.ByteType = (ushort)(b | 0xFF00);
                        }
                        if ((lzs.ByteType & 1) != 0)
                        {
                            if (Get(lzs, out b) != LZERROR.LZERROR_OK)
                                return toread - howmuch;
                        }
                        else
                        {
                            if (Get(lzs, out byte b1) != LZERROR.LZERROR_OK)
                                return toread - howmuch;
                            if (Get(lzs, out byte b2) != LZERROR.LZERROR_OK)
                                return toread - howmuch;

                            // Format:
                            // b1 b2
                            // AB CD
                            // where CAB is the stringoffset in the table
                            // and D+3 is the len of the string
                            lzs.StringPos = (uint)(b1 | ((b2 & 0xf0) << 4));
                            lzs.StringLen = (byte)((b2 & 0xf) + 2);

                            // 3, but we use a byte already below...
                            b = lzs.Table[lzs.StringPos];
                            lzs.StringPos = (lzs.StringPos + 1) & 0xFFF;
                        }

                        lzs.ByteType >>= 1;
                    }

                    // Store b in table
                    lzs.Table[lzs.CurTabEnt++] = b;
                    lzs.CurTabEnt &= 0xFFF;
                    lzs.RealCurrent++;
                }
            }

            while (howmuch > 0)
            {
                // DECOMPRESS_ONE_BYTE -- TODO: Make into helper method; original code is a DEFINE
                if (lzs.StringLen != 0)
                {
                    b = lzs.Table[lzs.StringPos];
                    lzs.StringPos = (lzs.StringPos + 1) & 0xFFF;
                    lzs.StringLen--;
                }
                else
                {
                    if ((lzs.ByteType & 0x100) == 0)
                    {
                        if (Get(lzs, out b) != LZERROR.LZERROR_OK)
                            return toread - howmuch;

                        lzs.ByteType = (ushort)(b | 0xFF00);
                    }
                    if ((lzs.ByteType & 1) != 0)
                    {
                        if (Get(lzs, out b) != LZERROR.LZERROR_OK)
                            return toread - howmuch;
                    }
                    else
                    {
                        if (Get(lzs, out byte b1) != LZERROR.LZERROR_OK)
                            return toread - howmuch;
                        if (Get(lzs, out byte b2) != LZERROR.LZERROR_OK)
                            return toread - howmuch;

                        // Format:
                        // b1 b2
                        // AB CD
                        // where CAB is the stringoffset in the table
                        // and D+3 is the len of the string
                        lzs.StringPos = (uint)(b1 | ((b2 & 0xf0) << 4));
                        lzs.StringLen = (byte)((b2 & 0xf) + 2);

                        // 3, but we use a byte already below...
                        b = lzs.Table[lzs.StringPos];
                        lzs.StringPos = (lzs.StringPos + 1) & 0xFFF;
                    }

                    lzs.ByteType >>= 1;
                }

                // Store b in table
                lzs.Table[lzs.CurTabEnt++] = b;
                lzs.CurTabEnt &= 0xFFF;
                lzs.RealCurrent++;

                lzs.RealWanted++;
                vbuf[buf++] = b;
                howmuch--;
            }

            return toread;
        }

        /// <summary>
        /// LZSeek   (KERNEL32.@)
        /// </summary>
        public long Seek(State lzs, long offset, SeekOrigin type)
        {
            // Not compressed? Just use normal Seek
            if (lzs.Get == null)
                return lzs.RealFD.Seek(offset, type);

            long newwanted = lzs.RealWanted;
            switch (type)
            {
                case SeekOrigin.Current:
                    newwanted += offset;
                    break;
                case SeekOrigin.End:
                    newwanted = lzs.RealLength - offset;
                    break;
                default:
                    newwanted = offset;
                    break;
            }

            if (newwanted > lzs.RealLength)
                return (long)LZERROR.LZERROR_BADVALUE;
            if (newwanted < 0)
                return (long)LZERROR.LZERROR_BADVALUE;

            lzs.RealWanted = (uint)newwanted;
            return newwanted;
        }

        /// <summary>
        /// Copies everything from src to dest
        /// if src is a LZ compressed file, it will be uncompressed.
        /// will return the number of bytes written to dest or errors.
        /// </summary>
        public long Copy(State src, State dest)
        {
            int usedlzinit = 0, ret, wret;
            State oldsrc = src;
            Stream srcfd;
            DateTime filetime;
            State lzs;

            const int BUFLEN = 1000;
            byte[] buf = new byte[BUFLEN];

            // we need that weird typedef, for i can't seem to get function pointer
            // casts right. (Or they probably just do not like WINAPI in general)
            //typedef UINT(WINAPI * _readfun)(HFILE, LPVOID, UINT);

            if (src.Get == null)
            {
                LZERROR err = Init(src.RealFD, out src);
                if (err < LZERROR.LZERROR_NOT_LZ)
                    return (long)LZERROR.LZERROR_NOT_LZ;
                
                usedlzinit = 1;
            }

            // Not compressed? just copy
            if (src.Get == null)
            {
                src.RealFD.CopyTo(dest.RealFD);
                return src.RealFD.Length;
            }

            long len = 0;
            while (true)
            {
                ret = Read(src, buf, BUFLEN);
                if (ret <= 0)
                {
                    if (ret == (int)LZERROR.LZERROR_NOT_LZ)
                        break;
                    if (ret == (int)LZERROR.LZERROR_BADINHANDLE)
                        return (long)LZERROR.LZERROR_READ;
                    return ret;
                }

                len += ret;
                dest.RealFD.Write(buf, 0, ret);
            }

            // Maintain the timestamp of the source file to the destination file
            // srcfd = (!(lzs = GET_LZ_STATE(src))) ? src : lzs->realfd;
            // GetFileTime( LongToHandle(srcfd), NULL, NULL, &filetime );
            // SetFileTime( LongToHandle(dest), NULL, NULL, &filetime );

            // Close handle
            if (usedlzinit != 0)
                Close(src);

            return len;
        }

        /// <summary>
        /// Opens a file. If not compressed, open it as a normal file.
        /// </summary>
        public State OpenFile(string fileName, FileMode mode, FileAccess access, FileShare fileShare = FileShare.ReadWrite)
        {
            try
            {
                Stream stream = File.Open(fileName, mode, access, fileShare);
                LZERROR error = Init(stream, out State lzs);
                if (error == LZERROR.LZERROR_OK || error == LZERROR.LZERROR_NOT_LZ)
                    return lzs;
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Opens a file. If not compressed, open it as a normal file.
        /// </summary>
        public State OpenFile(Stream stream)
        {
            LZERROR error = Init(stream, out State lzs);
            if (error == LZERROR.LZERROR_OK || error == LZERROR.LZERROR_NOT_LZ)
                return lzs;

            return null;
        }

        /// <summary>
        /// Closes a state
        /// </summary>
        public void Close(State state)
        {
            // TODO: Figure out if this is effectively all
            state?.RealFD?.Close();
        }

        /// <summary>
        /// Reads one compressed byte, including buffering
        /// </summary>
        private LZERROR Get(State lzs, out byte b)
        {
            if (lzs.GetCur < lzs.GetLen)
            {
                b = lzs.Get[lzs.GetCur++];
                return LZERROR.LZERROR_OK;
            }
            else
            {
                int ret = lzs.RealFD.Read(lzs.Get, 0, GETLEN);
                if (ret == 0)
                {
                    b = 0;
                    return LZERROR.LZERROR_NOT_LZ;
                }

                lzs.GetLen = (uint)ret;
                lzs.GetCur = 1;
                b = lzs.Get[0];
                return LZERROR.LZERROR_OK;
            }
        }

        private void GetFlush(State lzs)
        {
            lzs.GetCur = lzs.GetLen;
        }

        /// <summary>
        /// Internal function, reads lzheader
        /// </summary>
        private LZERROR ReadHeader(Stream fd, out FileHeaader head)
        {
            // Create the file header
            head = new FileHeaader();

            if (fd == null || !fd.CanSeek || !fd.CanRead)
                return LZERROR.LZERROR_BADINHANDLE;

            // Ensure we're at the start of the stream
            fd.Seek(0, SeekOrigin.Begin);

            byte[] magic = fd.ReadBytes(LZ_MAGIC_LEN);
            head.Magic = Encoding.ASCII.GetString(magic);
            if (head.Magic != MagicString)
                return LZERROR.LZERROR_NOT_LZ;

            head.CompressionType = fd.ReadByteValue();
            if (head.CompressionType != (byte)'A')
                return LZERROR.LZERROR_UNKNOWNALG;

            head.LastChar = (char)fd.ReadByteValue();
            head.RealLength = fd.ReadUInt32();
            return LZERROR.LZERROR_OK;
        }
    }
}