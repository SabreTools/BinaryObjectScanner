using System.Runtime.InteropServices;
using static BinaryObjectScanner.Compression.bzip2.Constants;
using static BinaryObjectScanner.Compression.bzip2.Huffman;

namespace BinaryObjectScanner.Compression.bzip2
{
    /// <see href="https://github.com/ladislav-zezula/StormLib/blob/master/src/bzip2/decompress.c"/>
    internal static unsafe class decompress
    {
        private static void makeMaps_d(DState s)
        {
            int i;
            s.nInUse = 0;
            for (i = 0; i < 256; i++)
            {
                if (s.inUse[i])
                {
                    s.seqToUnseq[s.nInUse] = (byte)i;
                    s.nInUse++;
                }
            }
        }

        public static int BZ2_decompress(DState s)
        {
            byte uc = 0;
            int retVal;
            int minLen, maxLen;
            bz_stream strm = s.strm;

            /* stuff that needs to be saved/restored */
            int i;
            int j;
            int t;
            int alphaSize;
            int nGroups;
            int nSelectors;
            int EOB;
            int groupNo;
            int groupPos;
            int nextSym;
            int nblockMAX;
            int nblock;
            int es;
            int N;
            int curr;
            int zt;
            int zn;
            int zvec;
            int zj;
            int gSel;
            int gMinlen;
            int* gLimit;
            int* gBase;
            int* gPerm;

            if (s.state == BZ_X_MAGIC_1)
            {
                /*initialise the save area*/
                s.save_i = 0;
                s.save_j = 0;
                s.save_t = 0;
                s.save_alphaSize = 0;
                s.save_nGroups = 0;
                s.save_nSelectors = 0;
                s.save_EOB = 0;
                s.save_groupNo = 0;
                s.save_groupPos = 0;
                s.save_nextSym = 0;
                s.save_nblockMAX = 0;
                s.save_nblock = 0;
                s.save_es = 0;
                s.save_N = 0;
                s.save_curr = 0;
                s.save_zt = 0;
                s.save_zn = 0;
                s.save_zvec = 0;
                s.save_zj = 0;
                s.save_gSel = 0;
                s.save_gMinlen = 0;
                s.save_gLimit = null;
                s.save_gBase = null;
                s.save_gPerm = null;
            }

            /*restore from the save area*/
            i = s.save_i;
            j = s.save_j;
            t = s.save_t;
            alphaSize = s.save_alphaSize;
            nGroups = s.save_nGroups;
            nSelectors = s.save_nSelectors;
            EOB = s.save_EOB;
            groupNo = s.save_groupNo;
            groupPos = s.save_groupPos;
            nextSym = s.save_nextSym;
            nblockMAX = s.save_nblockMAX;
            nblock = s.save_nblock;
            es = s.save_es;
            N = s.save_N;
            curr = s.save_curr;
            zt = s.save_zt;
            zn = s.save_zn;
            zvec = s.save_zvec;
            zj = s.save_zj;
            gSel = s.save_gSel;
            gMinlen = s.save_gMinlen;
            gLimit = s.save_gLimit;
            gBase = s.save_gBase;
            gPerm = s.save_gPerm;

            retVal = BZ_OK;

            switch (s.state)
            {
                // States that don't map to cases -- TODO: Figure out how to reference the right labels
                // case BZ_X_MAPPING_1: goto BZ_X_MAPPING_1; break;
                // case BZ_X_MAPPING_2: goto BZ_X_MAPPING_2; break;
                // case BZ_X_SELECTOR_3: goto BZ_X_SELECTOR_3; break;
                // case BZ_X_CODING_1: goto BZ_X_CODING_1; break;
                // case BZ_X_CODING_2: goto BZ_X_CODING_2; break;
                // case BZ_X_CODING_3: goto BZ_X_CODING_3; break;
                // case BZ_X_MTF_2: goto BZ_X_MTF_2; break;
                // case BZ_X_MTF_3: goto BZ_X_MTF_3; break;
                // case BZ_X_MTF_4: goto BZ_X_MTF_4; break;
                // case BZ_X_MTF_5: goto BZ_X_MTF_5; break;
                // case BZ_X_MTF_6: goto BZ_X_MTF_6; break;

                case BZ_X_MAGIC_1:
                    s.state = BZ_X_MAGIC_1;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc != BZ_HDR_B)
                    {
                        retVal = BZ_DATA_ERROR_MAGIC;
                        goto save_state_and_return;
                    }

                    // Fallthrough
                    goto case BZ_X_MAGIC_2;

                case BZ_X_MAGIC_2:
                    s.state = BZ_X_MAGIC_2;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc != BZ_HDR_Z)
                    {
                        retVal = BZ_DATA_ERROR_MAGIC;
                        goto save_state_and_return;
                    }

                    // Fallthrough
                    goto case BZ_X_MAGIC_3;

                case BZ_X_MAGIC_3:
                    s.state = BZ_X_MAGIC_3;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc != BZ_HDR_h)
                    {
                        retVal = BZ_DATA_ERROR_MAGIC;
                        goto save_state_and_return;
                    }

                    // Fallthrough
                    goto case BZ_X_MAGIC_4;

                case BZ_X_MAGIC_4:
                    s.state = BZ_X_MAGIC_4;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            s.blockSize100k = (int)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (s.blockSize100k < (BZ_HDR_0 + 1) || s.blockSize100k > (BZ_HDR_0 + 9))
                    {
                        retVal = BZ_DATA_ERROR_MAGIC;
                        goto save_state_and_return;
                    }

                    s.blockSize100k -= BZ_HDR_0;

                    if (s.smallDecompress)
                    {
                        s.ll16 = (ushort*)Marshal.AllocHGlobal(s.blockSize100k * 100000 * sizeof(ushort));
                        s.ll4 = (byte*)Marshal.AllocHGlobal(((1 + s.blockSize100k * 100000) >> 1) * sizeof(byte));
                        if (s.ll16 == null || s.ll4 == null)
                        {
                            retVal = BZ_MEM_ERROR;
                            goto save_state_and_return;
                        }
                    }
                    else
                    {
                        s.tt = (uint*)Marshal.AllocHGlobal(s.blockSize100k * 100000 * sizeof(int));
                        if (s.tt == null)
                        {
                            retVal = BZ_MEM_ERROR;
                            goto save_state_and_return;
                        }
                    }

                    // Fallthrough
                    goto case BZ_X_BLKHDR_1;

                case BZ_X_BLKHDR_1:
                    s.state = BZ_X_BLKHDR_1;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc == 0x17)
                        goto case BZ_X_ENDHDR_2;

                    if (uc != 0x31)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    // Fallthrough
                    goto case BZ_X_BLKHDR_2;

                case BZ_X_BLKHDR_2:
                    s.state = BZ_X_BLKHDR_2;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc != 0x41)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    // Fallthrough
                    goto case BZ_X_BLKHDR_3;

                case BZ_X_BLKHDR_3:
                    s.state = BZ_X_BLKHDR_3;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc != 0x59)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    // Fallthrough
                    goto case BZ_X_BLKHDR_4;

                case BZ_X_BLKHDR_4:
                    s.state = BZ_X_BLKHDR_4;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc != 0x26)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    // Fallthrough
                    goto case BZ_X_BLKHDR_5;

                case BZ_X_BLKHDR_5:
                    s.state = BZ_X_BLKHDR_5;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc != 0x53)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    // Fallthrough
                    goto case BZ_X_BLKHDR_6;

                case BZ_X_BLKHDR_6:
                    s.state = BZ_X_BLKHDR_6;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc != 0x59)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    s.currBlockNo++;
                    // if (s.verbosity >= 2)
                    //     VPrintf1("\n    [%d: huff+mtf ", s.currBlockNo);

                    s.storedBlockCRC = 0;

                    // Fallthrough
                    goto case BZ_X_BCRC_1;

                case BZ_X_BCRC_1:
                    s.state = BZ_X_BCRC_1;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    s.storedBlockCRC = (s.storedBlockCRC << 8) | ((uint)uc);

                    // Fallthrough
                    goto case BZ_X_BCRC_2;

                case BZ_X_BCRC_2:
                    s.state = BZ_X_BCRC_2;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    s.storedBlockCRC = (s.storedBlockCRC << 8) | ((uint)uc);

                    // Fallthrough
                    goto case BZ_X_BCRC_3;

                case BZ_X_BCRC_3:
                    s.state = BZ_X_BCRC_3;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    s.storedBlockCRC = (s.storedBlockCRC << 8) | ((uint)uc);

                    // Fallthrough
                    goto case BZ_X_BCRC_4;

                case BZ_X_BCRC_4:
                    s.state = BZ_X_BCRC_4;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    s.storedBlockCRC = (s.storedBlockCRC << 8) | ((uint)uc);

                    // Fallthrough
                    goto case BZ_X_RANDBIT;

                case BZ_X_RANDBIT:
                    s.state = BZ_X_RANDBIT;
                    while (true)
                    {
                        if (s.bsLive >= 1)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 1)) & ((1 << 1) - 1));
                            s.bsLive -= 1;
                            s.blockRandomised = v != 0;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    s.origPtr = 0;

                    // Fallthrough
                    goto case BZ_X_ORIGPTR_1;

                case BZ_X_ORIGPTR_1:
                    s.state = BZ_X_ORIGPTR_1;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    s.origPtr = (s.origPtr << 8) | ((int)uc);

                    // Fallthrough
                    goto case BZ_X_ORIGPTR_2;

                case BZ_X_ORIGPTR_2:
                    s.state = BZ_X_ORIGPTR_2;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    s.origPtr = (s.origPtr << 8) | ((int)uc);

                    // Fallthrough
                    goto case BZ_X_ORIGPTR_3;

                case BZ_X_ORIGPTR_3:

                    s.state = BZ_X_ORIGPTR_3;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    s.origPtr = (s.origPtr << 8) | ((int)uc);

                    if (s.origPtr < 0)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    if (s.origPtr > 10 + 100000 * s.blockSize100k)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    /*--- Receive the mapping table ---*/
                    for (i = 0; i < 16; i++)
                    {
                    BZ_X_MAPPING_1:
                        s.state = BZ_X_MAPPING_1;
                        while (true)
                        {
                            if (s.bsLive >= 1)
                            {
                                uint v;
                                v = (uint)((s.bsBuff >> (s.bsLive - 1)) & ((1 << 1) - 1));
                                s.bsLive -= 1;
                                uc = (byte)v;
                                break;
                            }

                            if (s.strm.avail_in == 0)
                            {
                                retVal = BZ_OK;
                                goto save_state_and_return;
                            }

                            s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                            s.bsLive += 8;
                            s.strm.next_in++;
                            s.strm.avail_in--;
                            s.strm.total_in_lo32++;
                            if (s.strm.total_in_lo32 == 0)
                                s.strm.total_in_hi32++;
                        }

                        if (uc == 1)
                            s.inUse16[i] = true;
                        else
                            s.inUse16[i] = false;
                    }

                    for (i = 0; i < 256; i++)
                    {
                        s.inUse[i] = false;
                    }

                    for (i = 0; i < 16; i++)
                    {
                        if (s.inUse16[i])
                        {
                            for (j = 0; j < 16; j++)
                            {
                            BZ_X_MAPPING_2:
                                s.state = BZ_X_MAPPING_2;
                                while (true)
                                {
                                    if (s.bsLive >= 1)
                                    {
                                        uint v;
                                        v = (uint)((s.bsBuff >> (s.bsLive - 1)) & ((1 << 1) - 1));
                                        s.bsLive -= 1;
                                        uc = (byte)v;
                                        break;
                                    }

                                    if (s.strm.avail_in == 0)
                                    {
                                        retVal = BZ_OK;
                                        goto save_state_and_return;
                                    }

                                    s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                                    s.bsLive += 8;
                                    s.strm.next_in++;
                                    s.strm.avail_in--;
                                    s.strm.total_in_lo32++;
                                    if (s.strm.total_in_lo32 == 0)
                                        s.strm.total_in_hi32++;
                                }

                                if (uc == 1)
                                    s.inUse[i * 16 + j] = true;
                            }
                        }
                    }

                    makeMaps_d(s);
                    if (s.nInUse == 0)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    alphaSize = s.nInUse + 2;

                    // Fallthrough
                    goto case BZ_X_SELECTOR_1;

                /*--- Now the selectors ---*/
                case BZ_X_SELECTOR_1:
                    s.state = BZ_X_SELECTOR_1;
                    while (true)
                    {
                        if (s.bsLive >= 3)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 3)) & ((1 << 3) - 1));
                            s.bsLive -= 3;
                            nGroups = (int)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (nGroups < 2 || nGroups > 6)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    // Fallthrough
                    goto case BZ_X_SELECTOR_2;

                case BZ_X_SELECTOR_2:
                    s.state = BZ_X_SELECTOR_2;
                    while (true)
                    {
                        if (s.bsLive >= 15)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 15)) & ((1 << 15) - 1));
                            s.bsLive -= 15;
                            nSelectors = (int)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (nSelectors < 1)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    for (i = 0; i < nSelectors; i++)
                    {
                        j = 0;
                        while (true)
                        {
                        BZ_X_SELECTOR_3:
                            s.state = BZ_X_SELECTOR_3;
                            while (true)
                            {
                                if (s.bsLive >= 1)
                                {
                                    uint v;
                                    v = (uint)((s.bsBuff >> (s.bsLive - 1)) & ((1 << 1) - 1));
                                    s.bsLive -= 1;
                                    uc = (byte)v;
                                    break;
                                }

                                if (s.strm.avail_in == 0)
                                {
                                    retVal = BZ_OK;
                                    goto save_state_and_return;
                                }

                                s.bsBuff = (s.bsBuff << 8) | *(byte*)s.strm.next_in;
                                s.bsLive += 8;
                                s.strm.next_in++;
                                s.strm.avail_in--;
                                s.strm.total_in_lo32++;
                                if (s.strm.total_in_lo32 == 0)
                                    s.strm.total_in_hi32++;
                            }

                            if (uc == 0)
                                break;

                            j++;
                            if (j >= nGroups)
                            {
                                retVal = BZ_DATA_ERROR;
                                goto save_state_and_return;
                            }
                        }

                        s.selectorMtf[i] = (byte)j;
                    }

                    /*--- Undo the MTF values for the selectors. ---*/
                    {
                        byte[] pos = new byte[BZ_N_GROUPS]; byte tmp, v;
                        for (v = 0; v < nGroups; v++)
                        {
                            pos[v] = v;
                        }

                        for (i = 0; i < nSelectors; i++)
                        {
                            v = s.selectorMtf[i];
                            tmp = pos[v];
                            while (v > 0) { pos[v] = pos[v - 1]; v--; }
                            pos[0] = tmp;
                            s.selector[i] = tmp;
                        }
                    }

                    /*--- Now the coding tables ---*/
                    for (t = 0; t < nGroups; t++)
                    {
                    BZ_X_CODING_1:
                        s.state = BZ_X_CODING_1;
                        while (true)
                        {
                            if (s.bsLive >= 5)
                            {
                                uint v;
                                v = (uint)((s.bsBuff >> (s.bsLive - 5)) & ((1 << 5) - 1));
                                s.bsLive -= 5;
                                curr = (int)v;
                                break;
                            }

                            if (s.strm.avail_in == 0)
                            {
                                retVal = BZ_OK;
                                goto save_state_and_return;
                            }

                            s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                            s.bsLive += 8;
                            s.strm.next_in++;
                            s.strm.avail_in--;
                            s.strm.total_in_lo32++;
                            if (s.strm.total_in_lo32 == 0)
                                s.strm.total_in_hi32++;
                        }

                        for (i = 0; i < alphaSize; i++)
                        {
                            while (true)
                            {
                                if (curr < 1 || curr > 20)
                                {
                                    retVal = BZ_DATA_ERROR;
                                    goto save_state_and_return;
                                }

                            BZ_X_CODING_2:
                                s.state = BZ_X_CODING_2;
                                while (true)
                                {
                                    if (s.bsLive >= 1)
                                    {
                                        uint v;
                                        v = (uint)((s.bsBuff >> (s.bsLive - 1)) & ((1 << 1) - 1));
                                        s.bsLive -= 1;
                                        uc = (byte)v;
                                        break;
                                    }

                                    if (s.strm.avail_in == 0)
                                    {
                                        retVal = BZ_OK;
                                        goto save_state_and_return;
                                    }

                                    s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                                    s.bsLive += 8;
                                    s.strm.next_in++;
                                    s.strm.avail_in--;
                                    s.strm.total_in_lo32++;
                                    if (s.strm.total_in_lo32 == 0)
                                        s.strm.total_in_hi32++;
                                }

                                if (uc == 0)
                                    break;

                                BZ_X_CODING_3:
                                s.state = BZ_X_CODING_3;
                                while (true)
                                {
                                    if (s.bsLive >= 1)
                                    {
                                        uint v;
                                        v = (uint)((s.bsBuff >> (s.bsLive - 1)) & ((1 << 1) - 1));
                                        s.bsLive -= 1;
                                        uc = (byte)v;
                                        break;
                                    }

                                    if (s.strm.avail_in == 0)
                                    {
                                        retVal = BZ_OK;
                                        goto save_state_and_return;
                                    }

                                    s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                                    s.bsLive += 8;
                                    s.strm.next_in++;
                                    s.strm.avail_in--;
                                    s.strm.total_in_lo32++;
                                    if (s.strm.total_in_lo32 == 0)
                                        s.strm.total_in_hi32++;
                                }

                                if (uc == 0)
                                    curr++;
                                else
                                    curr--;
                            }

                            s.len[t, i] = (byte)curr;
                        }
                    }

                    /*--- Create the Huffman decoding tables ---*/
                    for (t = 0; t < nGroups; t++)
                    {
                        minLen = 32;
                        maxLen = 0;
                        for (i = 0; i < alphaSize; i++)
                        {
                            if (s.len[t, i] > maxLen)
                                maxLen = s.len[t, i];
                            if (s.len[t, i] < minLen)
                                minLen = s.len[t, i];
                        }

                        fixed (int* s_limit_t_0 = &s.limit[t, 0])
                        fixed (int* s_base_t_0 = &s.@base[t, 0])
                        fixed (int* s_perm_t_0 = &s.perm[t, 0])
                        fixed (byte* s_len_t_0 = &s.len[t, 0])
                        {
                            BZ2_hbCreateDecodeTables(s_limit_t_0, s_base_t_0, s_perm_t_0, s_len_t_0, minLen, maxLen, alphaSize);
                        }

                        s.minLens[t] = minLen;
                    }

                    /*--- Now the MTF values ---*/

                    EOB = s.nInUse + 1;
                    nblockMAX = 100000 * s.blockSize100k;
                    groupNo = -1;
                    groupPos = 0;

                    for (i = 0; i <= 255; i++) s.unzftab[i] = 0;

                    /*-- MTF init --*/
                    {
                        int ii, jj, kk;
                        kk = MTFA_SIZE - 1;
                        for (ii = 256 / MTFL_SIZE - 1; ii >= 0; ii--)
                        {
                            for (jj = MTFL_SIZE - 1; jj >= 0; jj--)
                            {
                                s.mtfa[kk] = (byte)(ii * MTFL_SIZE + jj);
                                kk--;
                            }

                            s.mtfbase[ii] = kk + 1;
                        }
                    }
                    /*-- end MTF init --*/

                    nblock = 0;

                    if (groupPos == 0)
                    {
                        groupNo++;
                        if (groupNo >= nSelectors)
                        {
                            retVal = BZ_DATA_ERROR;
                            goto save_state_and_return;
                        }

                        groupPos = BZ_G_SIZE;
                        gSel = s.selector[groupNo];
                        gMinlen = s.minLens[gSel];

                        fixed (int* s_limit_gSel_0 = &s.limit[gSel, 0])
                            gLimit = s_limit_gSel_0;
                        fixed (int* s_perm_gSel_0 = &s.perm[gSel, 0])
                            gPerm = s_perm_gSel_0;
                        fixed (int* s_base_gSel_0 = &s.@base[gSel, 0])
                            gPerm = s_base_gSel_0;
                    }

                    groupPos--;
                    zn = gMinlen;

                    // Fallthrough
                    goto case BZ_X_MTF_1;

                case BZ_X_MTF_1:
                    s.state = BZ_X_MTF_1;
                    while (true)
                    {
                        if (s.bsLive >= zn)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - zn)) & ((1 << zn) - 1));
                            s.bsLive -= zn;
                            zvec = (int)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    while (true)
                    {
                        if (zn > 20 /* the longest code */)
                        {
                            retVal = BZ_DATA_ERROR;
                            goto save_state_and_return;
                        }
                        if (zvec <= gLimit[zn])
                            break;

                        zn++;

                    BZ_X_MTF_2:
                        s.state = BZ_X_MTF_2;
                        while (true)
                        {
                            if (s.bsLive >= 1)
                            {
                                uint v;
                                v = (uint)((s.bsBuff >> (s.bsLive - 1)) & ((1 << 1) - 1));
                                s.bsLive -= 1;
                                zj = (int)v;
                                break;
                            }

                            if (s.strm.avail_in == 0)
                            {
                                retVal = BZ_OK;
                                goto save_state_and_return;
                            }

                            s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                            s.bsLive += 8;
                            s.strm.next_in++;
                            s.strm.avail_in--;
                            s.strm.total_in_lo32++;
                            if (s.strm.total_in_lo32 == 0)
                                s.strm.total_in_hi32++;
                        }

                        zvec = (zvec << 1) | zj;
                    };

                    if (zvec - gBase[zn] < 0 || zvec - gBase[zn] >= BZ_MAX_ALPHA_SIZE)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    nextSym = gPerm[zvec - gBase[zn]];

                    while (true)
                    {
                        if (nextSym == EOB) break;

                        if (nextSym == BZ_RUNA || nextSym == BZ_RUNB)
                        {
                            es = -1;
                            N = 1;
                            do
                            {
                                if (nextSym == BZ_RUNA)
                                    es = es + (0 + 1) * N;
                                else if (nextSym == BZ_RUNB)
                                    es = es + (1 + 1) * N;

                                N = N * 2;
                                if (groupPos == 0)
                                {
                                    groupNo++;
                                    if (groupNo >= nSelectors)
                                    {
                                        retVal = BZ_DATA_ERROR;
                                        goto save_state_and_return;
                                    }

                                    groupPos = BZ_G_SIZE;
                                    gSel = s.selector[groupNo];
                                    gMinlen = s.minLens[gSel];

                                    fixed (int* s_limit_gSel_0 = &s.limit[gSel, 0])
                                        gLimit = s_limit_gSel_0;
                                    fixed (int* s_perm_gSel_0 = &s.perm[gSel, 0])
                                        gPerm = s_perm_gSel_0;
                                    fixed (int* s_base_gSel_0 = &s.@base[gSel, 0])
                                        gPerm = s_base_gSel_0;
                                }

                                groupPos--;
                                zn = gMinlen;

                            BZ_X_MTF_3:
                                s.state = BZ_X_MTF_3;
                                while (true)
                                {
                                    if (s.bsLive >= zn)
                                    {
                                        uint v;
                                        v = (uint)((s.bsBuff >> (s.bsLive - zn)) & ((1 << zn) - 1));
                                        s.bsLive -= zn;
                                        zvec = (int)v;
                                        break;
                                    }

                                    if (s.strm.avail_in == 0)
                                    {
                                        retVal = BZ_OK;
                                        goto save_state_and_return;
                                    }

                                    s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                                    s.bsLive += 8;
                                    s.strm.next_in++;
                                    s.strm.avail_in--;
                                    s.strm.total_in_lo32++;
                                    if (s.strm.total_in_lo32 == 0)
                                        s.strm.total_in_hi32++;
                                }

                                while (true)
                                {
                                    if (zn > 20 /* the longest code */)
                                    {
                                        retVal = BZ_DATA_ERROR;
                                        goto save_state_and_return;
                                    }

                                    if (zvec <= gLimit[zn])
                                        break;

                                    zn++;

                                BZ_X_MTF_4:
                                    s.state = BZ_X_MTF_4;
                                    while (true)
                                    {
                                        if (s.bsLive >= 1)
                                        {
                                            uint v;
                                            v = (uint)((s.bsBuff >> (s.bsLive - 1)) & ((1 << 1) - 1));
                                            s.bsLive -= 1;
                                            zj = (int)v;
                                            break;
                                        }

                                        if (s.strm.avail_in == 0)
                                        {
                                            retVal = BZ_OK;
                                            goto save_state_and_return;
                                        }

                                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                                        s.bsLive += 8;
                                        s.strm.next_in++;
                                        s.strm.avail_in--;
                                        s.strm.total_in_lo32++;
                                        if (s.strm.total_in_lo32 == 0)
                                            s.strm.total_in_hi32++;
                                    }

                                    zvec = (zvec << 1) | zj;
                                };

                                if (zvec - gBase[zn] < 0 || zvec - gBase[zn] >= BZ_MAX_ALPHA_SIZE)
                                {
                                    retVal = BZ_DATA_ERROR;
                                    goto save_state_and_return;
                                }

                                nextSym = gPerm[zvec - gBase[zn]];

                            } while (nextSym == BZ_RUNA || nextSym == BZ_RUNB);

                            es++;
                            uc = s.seqToUnseq[s.mtfa[s.mtfbase[0]]];
                            s.unzftab[uc] += es;

                            if (s.smallDecompress)
                            {
                                while (es > 0)
                                {
                                    if (nblock >= nblockMAX)
                                    {
                                        retVal = BZ_DATA_ERROR;
                                        goto save_state_and_return;
                                    }

                                    s.ll16[nblock] = (ushort)uc;
                                    nblock++;
                                    es--;
                                }
                            }
                            else
                            {
                                while (es > 0)
                                {
                                    if (nblock >= nblockMAX)
                                    {
                                        retVal = BZ_DATA_ERROR;
                                        goto save_state_and_return;
                                    }

                                    s.tt[nblock] = (uint)uc;
                                    nblock++;
                                    es--;
                                }
                            };

                            continue;
                        }
                        else
                        {
                            if (nblock >= nblockMAX)
                            {
                                retVal = BZ_DATA_ERROR;
                                goto save_state_and_return;
                            }

                            /*-- uc = MTF ( nextSym-1 ) --*/
                            {
                                int ii, jj, kk, pp, lno, off;
                                uint nn;
                                nn = (uint)(nextSym - 1);

                                if (nn < MTFL_SIZE)
                                {
                                    /* avoid general-case expense */
                                    pp = s.mtfbase[0];
                                    uc = s.mtfa[pp + nn];
                                    while (nn > 3)
                                    {
                                        int z = (int)(pp + nn);
                                        s.mtfa[(z)] = s.mtfa[(z) - 1];
                                        s.mtfa[(z) - 1] = s.mtfa[(z) - 2];
                                        s.mtfa[(z) - 2] = s.mtfa[(z) - 3];
                                        s.mtfa[(z) - 3] = s.mtfa[(z) - 4];
                                        nn -= 4;
                                    }

                                    while (nn > 0)
                                    {
                                        s.mtfa[(pp + nn)] = s.mtfa[(pp + nn) - 1]; nn--;
                                    };

                                    s.mtfa[pp] = uc;
                                }
                                else
                                {
                                    /* general case */
                                    lno = (int)(nn / MTFL_SIZE);
                                    off = (int)(nn % MTFL_SIZE);
                                    pp = s.mtfbase[lno] + off;
                                    uc = s.mtfa[pp];
                                    while (pp > s.mtfbase[lno])
                                    {
                                        s.mtfa[pp] = s.mtfa[pp - 1]; pp--;
                                    };

                                    s.mtfbase[lno]++;
                                    while (lno > 0)
                                    {
                                        s.mtfbase[lno]--;
                                        s.mtfa[s.mtfbase[lno]] = s.mtfa[s.mtfbase[lno - 1] + MTFL_SIZE - 1];
                                        lno--;
                                    }

                                    s.mtfbase[0]--;
                                    s.mtfa[s.mtfbase[0]] = uc;
                                    if (s.mtfbase[0] == 0)
                                    {
                                        kk = MTFA_SIZE - 1;
                                        for (ii = 256 / MTFL_SIZE - 1; ii >= 0; ii--)
                                        {
                                            for (jj = MTFL_SIZE - 1; jj >= 0; jj--)
                                            {
                                                s.mtfa[kk] = s.mtfa[s.mtfbase[ii] + jj];
                                                kk--;
                                            }

                                            s.mtfbase[ii] = kk + 1;
                                        }
                                    }
                                }
                            }
                            /*-- end uc = MTF ( nextSym-1 ) --*/

                            s.unzftab[s.seqToUnseq[uc]]++;
                            if (s.smallDecompress)
                                s.ll16[nblock] = (ushort)(s.seqToUnseq[uc]);
                            else
                                s.tt[nblock] = (uint)(s.seqToUnseq[uc]);
                            nblock++;

                            if (groupPos == 0)
                            {
                                groupNo++;
                                if (groupNo >= nSelectors)
                                {
                                    retVal = BZ_DATA_ERROR;
                                    goto save_state_and_return;
                                }

                                groupPos = BZ_G_SIZE;
                                gSel = s.selector[groupNo];
                                gMinlen = s.minLens[gSel];

                                fixed (int* s_limit_gSel_0 = &s.limit[gSel, 0])
                                    gLimit = s_limit_gSel_0;
                                fixed (int* s_perm_gSel_0 = &s.perm[gSel, 0])
                                    gPerm = s_perm_gSel_0;
                                fixed (int* s_base_gSel_0 = &s.@base[gSel, 0])
                                    gPerm = s_base_gSel_0;
                            }

                            groupPos--;
                            zn = gMinlen;

                        BZ_X_MTF_5:
                            s.state = BZ_X_MTF_5;
                            while (true)
                            {
                                if (s.bsLive >= zn)
                                {
                                    uint v;
                                    v = (uint)((s.bsBuff >> (s.bsLive - zn)) & ((1 << zn) - 1));
                                    s.bsLive -= zn;
                                    zvec = (int)v;
                                    break;
                                }

                                if (s.strm.avail_in == 0)
                                {
                                    retVal = BZ_OK;
                                    goto save_state_and_return;
                                }

                                s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                                s.bsLive += 8;
                                s.strm.next_in++;
                                s.strm.avail_in--;
                                s.strm.total_in_lo32++;
                                if (s.strm.total_in_lo32 == 0)
                                    s.strm.total_in_hi32++;
                            }

                            while (true)
                            {
                                if (zn > 20 /* the longest code */)
                                {
                                    retVal = BZ_DATA_ERROR;
                                    goto save_state_and_return;
                                }

                                if (zvec <= gLimit[zn])
                                    break;

                                zn++;

                            BZ_X_MTF_6:
                                s.state = BZ_X_MTF_6;
                                while (true)
                                {
                                    if (s.bsLive >= 1)
                                    {
                                        uint v;
                                        v = (uint)((s.bsBuff >> (s.bsLive - 1)) & ((1 << 1) - 1));
                                        s.bsLive -= 1;
                                        zj = (int)v;
                                        break;
                                    }

                                    if (s.strm.avail_in == 0)
                                    {
                                        retVal = BZ_OK;
                                        goto save_state_and_return;
                                    }

                                    s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                                    s.bsLive += 8;
                                    s.strm.next_in++;
                                    s.strm.avail_in--;
                                    s.strm.total_in_lo32++;
                                    if (s.strm.total_in_lo32 == 0)
                                        s.strm.total_in_hi32++;
                                }

                                zvec = (zvec << 1) | zj;
                            };

                            if (zvec - gBase[zn] < 0 || zvec - gBase[zn] >= BZ_MAX_ALPHA_SIZE)
                            {
                                retVal = BZ_DATA_ERROR;
                                goto save_state_and_return;
                            }

                            nextSym = gPerm[zvec - gBase[zn]];
                        }

                        continue;
                    }

                    /* Now we know what nblock is, we can do a better sanity
                       check on s.origPtr.
                    */
                    if (s.origPtr < 0 || s.origPtr >= nblock)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    /*-- Set up cftab to facilitate generation of T^(-1) --*/
                    s.cftab[0] = 0;
                    for (i = 1; i <= 256; i++)
                    {
                        s.cftab[i] = s.unzftab[i - 1];
                    }

                    for (i = 1; i <= 256; i++)
                    {
                        s.cftab[i] += s.cftab[i - 1];
                    }

                    for (i = 0; i <= 256; i++)
                    {
                        if (s.cftab[i] < 0 || s.cftab[i] > nblock)
                        {
                            /* s.cftab[i] can legitimately be == nblock */
                            retVal = BZ_DATA_ERROR;
                            goto save_state_and_return;
                        }
                    }

                    s.state_out_len = 0;
                    s.state_out_ch = 0;
                    s.calculatedBlockCRC = 0xffffffff;
                    s.state = BZ_X_OUTPUT;
                    // if (s.verbosity >= 2) VPrintf0("rt+rld");

                    if (s.smallDecompress)
                    {
                        /*-- Make a copy of cftab, used in generation of T --*/
                        for (i = 0; i <= 256; i++) s.cftabCopy[i] = s.cftab[i];

                        /*-- compute the T vector --*/
                        for (i = 0; i < nblock; i++)
                        {
                            uc = (byte)(s.ll16[i]);
                            s.ll16[i] = (ushort)(s.cftabCopy[uc] & 0x0000ffff);
                            if ((i & 0x1) == 0)
                                s.ll4[i >> 1] = (byte)((s.ll4[i >> 1] & 0xf0) | (s.cftabCopy[uc]));
                            else
                                s.ll4[i >> 1] = (byte)((s.ll4[i >> 1] & 0x0f) | ((s.cftabCopy[uc]) << 4));

                            s.cftabCopy[uc]++;
                        }

                        /*-- Compute T^(-1) by pointer reversal on T --*/
                        i = s.origPtr;
                        j = (int)(s.ll16[i] | (((((uint)s.ll4[i >> 1]) >> ((i << 2) & 0x4)) & 0xF) << 16));
                        do
                        {
                            int tmp = (int)(s.ll16[j] | (((((uint)s.ll4[j >> 1]) >> ((j << 2) & 0x4)) & 0xF) << 16));
                            s.ll16[j] = (ushort)(i & 0x0000ffff);
                            if ((j & 0x1) == 0)
                                s.ll4[j >> 1] = (byte)((s.ll4[j >> 1] & 0xf0) | i);
                            else
                                s.ll4[j >> 1] = (byte)((s.ll4[j >> 1] & 0x0f) | (i << 4));

                            i = j;
                            j = tmp;
                        }
                        while (i != s.origPtr);

                        s.tPos = (uint)s.origPtr;
                        s.nblock_used = 0;
                        if (s.blockRandomised)
                        {
                            s.rNToGo = 0;
                            s.rTPos = 0;

                            /* c_tPos is unsigned, hence test < 0 is pointless. */
                            if (s.tPos >= (uint)100000 * (uint)s.blockSize100k)
                                return 1;

                            fixed (int* s_cftab = s.cftab)
                                s.k0 = BZ2_indexIntoF((int)s.tPos, s_cftab);

                            s.tPos = (s.ll16[s.tPos]) | (((((uint)s.ll4[s.tPos >> 1]) >> (int)((s.tPos << 2) & 0x4)) & 0xF) << 16);

                            s.nblock_used++;
                            if (s.rNToGo == 0)
                            {
                                s.rNToGo = BZ2_rNums[s.rTPos];
                                s.rTPos++;
                                if (s.rTPos == 512)
                                    s.rTPos = 0;
                            }

                            s.rNToGo--;
                            s.k0 ^= (s.rNToGo == 1) ? 1 : 0;
                        }
                        else
                        {
                            /* c_tPos is unsigned, hence test < 0 is pointless. */
                            if (s.tPos >= 100000 * (uint)s.blockSize100k)
                                return 1;

                            fixed (int* s_cftab = s.cftab)
                                s.k0 = BZ2_indexIntoF((int)s.tPos, s_cftab);

                            s.tPos = ((uint)s.ll16[s.tPos]) | (((((uint)s.ll4[s.tPos >> 1]) >> (int)((s.tPos << 2) & 0x4)) & 0xF) << 16);

                            s.nblock_used++;
                        }
                    }
                    else
                    {
                        /*-- compute the T^(-1) vector --*/
                        for (i = 0; i < nblock; i++)
                        {
                            uc = (byte)(s.tt[i] & 0xff);
                            s.tt[s.cftab[uc]] |= (uint)(i << 8);
                            s.cftab[uc]++;
                        }

                        s.tPos = s.tt[s.origPtr] >> 8;
                        s.nblock_used = 0;
                        if (s.blockRandomised)
                        {
                            s.rNToGo = 0;
                            s.rTPos = 0;

                            /* c_tPos is unsigned, hence test < 0 is pointless. */
                            if (s.tPos >= 100000 * (uint)s.blockSize100k)
                                return 1;

                            s.tPos = s.tt[s.tPos];
                            s.k0 = (byte)(s.tPos & 0xff);
                            s.tPos >>= 8;

                            s.nblock_used++;
                            if (s.rNToGo == 0)
                            {
                                s.rNToGo = BZ2_rNums[s.rTPos];
                                s.rTPos++;
                                if (s.rTPos == 512)
                                    s.rTPos = 0;
                            }

                            s.rNToGo--;
                            s.k0 ^= (s.rNToGo == 1) ? 1 : 0;
                        }
                        else
                        {
                            /* c_tPos is unsigned, hence test < 0 is pointless. */
                            if (s.tPos >= 100000 * (uint)s.blockSize100k)
                                return 1;

                            s.tPos = s.tt[s.tPos];
                            s.k0 = (byte)(s.tPos & 0xff);
                            s.tPos >>= 8;

                            s.nblock_used++;
                        }
                    }

                    retVal = BZ_OK;
                    goto save_state_and_return;

                case BZ_X_ENDHDR_2:
                    s.state = BZ_X_ENDHDR_2;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc != 0x72)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    // Fallthrough
                    goto case BZ_X_ENDHDR_3;

                case BZ_X_ENDHDR_3:
                    s.state = BZ_X_ENDHDR_3;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc != 0x45)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    // Fallthrough
                    goto case BZ_X_ENDHDR_4;

                case BZ_X_ENDHDR_4:
                    s.state = BZ_X_ENDHDR_4;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc != 0x38)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    // Fallthrough
                    goto case BZ_X_ENDHDR_5;

                case BZ_X_ENDHDR_5:
                    s.state = BZ_X_ENDHDR_5;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc != 0x50)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    // Fallthrough
                    goto case BZ_X_ENDHDR_6;

                case BZ_X_ENDHDR_6:
                    s.state = BZ_X_ENDHDR_6;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    if (uc != 0x90)
                    {
                        retVal = BZ_DATA_ERROR;
                        goto save_state_and_return;
                    }

                    s.storedCombinedCRC = 0;

                    // Fallthrough
                    goto case BZ_X_CCRC_1;

                case BZ_X_CCRC_1:
                    s.state = BZ_X_CCRC_1;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    s.storedCombinedCRC = (s.storedCombinedCRC << 8) | ((uint)uc);

                    // Fallthrough
                    goto case BZ_X_CCRC_2;

                case BZ_X_CCRC_2:
                    s.state = BZ_X_CCRC_2;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    s.storedCombinedCRC = (s.storedCombinedCRC << 8) | ((uint)uc);

                    // Fallthrough
                    goto case BZ_X_CCRC_3;

                case BZ_X_CCRC_3:
                    s.state = BZ_X_CCRC_3;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    s.storedCombinedCRC = (s.storedCombinedCRC << 8) | ((uint)uc);

                    // Fallthrough
                    goto case BZ_X_CCRC_4;

                case BZ_X_CCRC_4:
                    s.state = BZ_X_CCRC_4;
                    while (true)
                    {
                        if (s.bsLive >= 8)
                        {
                            uint v;
                            v = (uint)((s.bsBuff >> (s.bsLive - 8)) & ((1 << 8) - 1));
                            s.bsLive -= 8;
                            uc = (byte)v;
                            break;
                        }

                        if (s.strm.avail_in == 0)
                        {
                            retVal = BZ_OK;
                            goto save_state_and_return;
                        }

                        s.bsBuff = (s.bsBuff << 8) | *(byte*)(s.strm.next_in);
                        s.bsLive += 8;
                        s.strm.next_in++;
                        s.strm.avail_in--;
                        s.strm.total_in_lo32++;
                        if (s.strm.total_in_lo32 == 0)
                            s.strm.total_in_hi32++;
                    }

                    s.storedCombinedCRC = (s.storedCombinedCRC << 8) | ((uint)uc);

                    s.state = BZ_X_IDLE;
                    retVal = BZ_STREAM_END;
                    goto save_state_and_return;
            }

        save_state_and_return:

            s.save_i = i;
            s.save_j = j;
            s.save_t = t;
            s.save_alphaSize = alphaSize;
            s.save_nGroups = nGroups;
            s.save_nSelectors = nSelectors;
            s.save_EOB = EOB;
            s.save_groupNo = groupNo;
            s.save_groupPos = groupPos;
            s.save_nextSym = nextSym;
            s.save_nblockMAX = nblockMAX;
            s.save_nblock = nblock;
            s.save_es = es;
            s.save_N = N;
            s.save_curr = curr;
            s.save_zt = zt;
            s.save_zn = zn;
            s.save_zvec = zvec;
            s.save_zj = zj;
            s.save_gSel = gSel;
            s.save_gMinlen = gMinlen;
            s.save_gLimit = gLimit;
            s.save_gBase = gBase;
            s.save_gPerm = gPerm;

            return retVal;
        }

        #region Helpers

        private static int BZ2_indexIntoF(int indx, int* cftab)
        {
            int nb, na, mid;
            nb = 0;
            na = 256;

            do
            {
                mid = (nb + na) >> 1;
                if (indx >= cftab[mid])
                    nb = mid;
                else
                    na = mid;
            } while (na - nb != 1);

            return nb;
        }

        #endregion
    }
}