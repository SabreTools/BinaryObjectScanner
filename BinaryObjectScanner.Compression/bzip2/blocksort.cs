using static BinaryObjectScanner.Compression.bzip2.Constants;

namespace BinaryObjectScanner.Compression.bzip2
{
    /// <summary>
    /// Block sorting machinery
    /// </summary>
    /// <see href="https://github.com/ladislav-zezula/StormLib/blob/master/src/bzip2/blocksort.c"/>
    internal static unsafe class blocksort
    {
        /// <summary>
        /// Fallback O(N log(N)^2) sorting algorithm, for repetitive blocks
        /// </summary>
        public static void fallbackSimpleSort(uint* fmap, uint* eclass, int lo, int hi)
        {
            int i, j, tmp;
            uint ec_tmp;

            if (lo == hi) return;

            if (hi - lo > 3)
            {
                for (i = hi - 4; i >= lo; i--)
                {
                    tmp = (int)fmap[i];
                    ec_tmp = eclass[tmp];
                    for (j = i + 4; j <= hi && ec_tmp > eclass[fmap[j]]; j += 4)
                        fmap[j - 4] = fmap[j];
                    fmap[j - 4] = (uint)tmp;
                }
            }

            for (i = hi - 1; i >= lo; i--)
            {
                tmp = (int)fmap[i];
                ec_tmp = eclass[tmp];
                for (j = i + 1; j <= hi && ec_tmp > eclass[fmap[j]]; j++)
                    fmap[j - 1] = fmap[j];
                fmap[j - 1] = (uint)tmp;
            }
        }

        public static void fallbackQSort3(uint* fmap, uint* eclass, int loSt, int hiSt)
        {
            int unLo, unHi, ltLo, gtHi, n, m;
            int sp, lo = 0, hi = 0;
            uint med, r, r3;
            int[] stackLo = new int[FALLBACK_QSORT_STACK_SIZE];
            int[] stackHi = new int[FALLBACK_QSORT_STACK_SIZE];

            r = 0;

            sp = 0;
            fpush(loSt, hiSt, stackLo, stackHi, ref sp);

            while (sp > 0)
            {
                //AssertH(sp < FALLBACK_QSORT_STACK_SIZE - 1, 1004);

                fpop(ref lo, ref hi, stackLo, stackHi, ref sp);
                if (hi - lo < FALLBACK_QSORT_SMALL_THRESH)
                {
                    fallbackSimpleSort(fmap, eclass, lo, hi);
                    continue;
                }

                /* Random partitioning.  Median of 3 sometimes fails to
                   avoid bad cases.  Median of 9 seems to help but 
                   looks rather expensive.  This too seems to work but
                   is cheaper.  Guidance for the magic constants 
                   7621 and 32768 is taken from Sedgewick's algorithms
                   book, chapter 35.
                */
                r = ((r * 7621) + 1) % 32768;
                r3 = r % 3;
                if (r3 == 0)
                    med = eclass[fmap[lo]];
                else if (r3 == 1)
                    med = eclass[fmap[(lo + hi) >> 1]];
                else
                    med = eclass[fmap[hi]];

                unLo = ltLo = lo;
                unHi = gtHi = hi;

                while (true)
                {
                    while (true)
                    {
                        if (unLo > unHi) break;
                        n = (int)eclass[fmap[unLo]] - (int)med;
                        if (n == 0)
                        {
                            fswap(ref fmap[unLo], ref fmap[ltLo]);
                            ltLo++; unLo++;
                            continue;
                        };
                        if (n > 0) break;
                        unLo++;
                    }
                    while (true)
                    {
                        if (unLo > unHi)
                            break;

                        n = (int)eclass[fmap[unHi]] - (int)med;
                        if (n == 0)
                        {
                            fswap(ref fmap[unHi], ref fmap[gtHi]);
                            gtHi--; unHi--;
                            continue;
                        };

                        if (n < 0)
                            break;

                        unHi--;
                    }

                    if (unLo > unHi)
                        break;

                    fswap(ref fmap[unLo], ref fmap[unHi]); unLo++; unHi--;
                }

                //AssertD(unHi == unLo - 1, "fallbackQSort3(2)");

                if (gtHi < ltLo) continue;

                n = fmin(ltLo - lo, unLo - ltLo); fvswap(fmap, lo, unLo - n, n);
                m = fmin(hi - gtHi, gtHi - unHi); fvswap(fmap, unLo, hi - m + 1, m);

                n = lo + unLo - ltLo - 1;
                m = hi - (gtHi - unHi) + 1;

                if (n - lo > hi - m)
                {
                    fpush(lo, n, stackLo, stackHi, ref sp);
                    fpush(m, hi, stackLo, stackHi, ref sp);
                }
                else
                {
                    fpush(m, hi, stackLo, stackHi, ref sp);
                    fpush(lo, n, stackLo, stackHi, ref sp);
                }
            }
        }

        /*
        Pre:
            nblock > 0
            eclass exists for [0 .. nblock-1]
            ((byte*)eclass) [0 .. nblock-1] holds block
            ptr exists for [0 .. nblock-1]
        Post:
            ((byte*)eclass) [0 .. nblock-1] holds block
            All other areas of eclass destroyed
            fmap [0 .. nblock-1] holds sorted order
            bhtab [ 0 .. 2+(nblock/32) ] destroyed
        */

        public static void fallbackSort(uint* fmap, uint* eclass, uint* bhtab, int nblock, int verb)
        {
            int[] ftab = new int[257];
            int[] ftabCopy = new int[256];
            int H, i, j, k, l, r, cc, cc1;
            int nNotDone;
            int nBhtab;
            byte* eclass8 = (byte*)eclass;

            /*--
               Initial 1-char radix sort to generate
               initial fmap and initial BH bits.
            --*/
            // if (verb >= 4)
            //     VPrintf0("        bucket sorting ...\n");
            for (i = 0; i < 257; i++)
            {
                ftab[i] = 0;
            }

            for (i = 0; i < nblock; i++)
            {
                ftab[eclass8[i]]++;
            }

            for (i = 0; i < 256; i++)
            {
                ftabCopy[i] = ftab[i];
            }

            for (i = 1; i < 257; i++)
            {
                ftab[i] += ftab[i - 1];
            }

            for (i = 0; i < nblock; i++)
            {
                j = eclass8[i];
                k = ftab[j] - 1;
                ftab[j] = k;
                fmap[k] = (uint)i;
            }

            nBhtab = 2 + (nblock / 32);
            for (i = 0; i < nBhtab; i++)
            {
                bhtab[i] = 0;
            }

            for (i = 0; i < 256; i++)
            {
                SET_BH(ftab[i], bhtab);
            }

            /*--
               Inductively refine the buckets.  Kind-of an
               "exponential radix sort" (!), inspired by the
               Manber-Myers suffix array construction algorithm.
            --*/

            /*-- set sentinel bits for block-end detection --*/
            for (i = 0; i < 32; i++)
            {
                SET_BH(nblock + 2 * i, bhtab);
                CLEAR_BH(nblock + 2 * i + 1, bhtab);
            }

            /*-- the log(N) loop --*/
            H = 1;
            while (true)
            {
                // if (verb >= 4)
                //     VPrintf1("        depth %6d has ", H);

                j = 0;
                for (i = 0; i < nblock; i++)
                {
                    if (ISSET_BH(i, bhtab))
                        j = i;

                    k = (int)(fmap[i] - H);
                    if (k < 0)
                        k += nblock;

                    eclass[k] = (uint)j;
                }

                nNotDone = 0;
                r = -1;
                while (true)
                {

                    /*-- find the next non-singleton bucket --*/
                    k = r + 1;
                    while (ISSET_BH(k, bhtab) && UNALIGNED_BH(k) != 0)
                    {
                        k++;
                    }

                    if (ISSET_BH(k, bhtab))
                    {
                        while (WORD_BH(k, bhtab) == 0xffffffff)
                        {
                            k += 32;
                        }

                        while (ISSET_BH(k, bhtab))
                        {
                            k++;
                        }
                    }

                    l = k - 1;
                    if (l >= nblock)
                        break;

                    while (!ISSET_BH(k, bhtab) && UNALIGNED_BH(k) != 0)
                    {
                        k++;
                    }

                    if (!ISSET_BH(k, bhtab))
                    {
                        while (WORD_BH(k, bhtab) == 0x00000000)
                        {
                            k += 32;
                        }

                        while (!ISSET_BH(k, bhtab))
                        {
                            k++;
                        }
                    }

                    r = k - 1;
                    if (r >= nblock)
                        break;

                    /*-- now [l, r] bracket current bucket --*/
                    if (r > l)
                    {
                        nNotDone += (r - l + 1);
                        fallbackQSort3(fmap, eclass, l, r);

                        /*-- scan bucket and generate header bits-- */
                        cc = -1;
                        for (i = l; i <= r; i++)
                        {
                            cc1 = (int)eclass[fmap[i]];
                            if (cc != cc1)
                            {
                                SET_BH(i, bhtab);
                                cc = cc1;
                            };
                        }
                    }
                }

                // if (verb >= 4)
                //     VPrintf1("%6d unresolved strings\n", nNotDone);

                H *= 2;
                if (H > nblock || nNotDone == 0)
                    break;
            }

            /*-- 
               Reconstruct the original block in
               eclass8 [0 .. nblock-1], since the
               previous phase destroyed it.
            --*/
            // if (verb >= 4)
            //     VPrintf0("        reconstructing block ...\n");

            j = 0;
            for (i = 0; i < nblock; i++)
            {
                while (ftabCopy[j] == 0)
                {
                    j++;
                }

                ftabCopy[j]--;
                eclass8[fmap[i]] = (byte)j;
            }

            //AssertH(j < 256, 1005);
        }

        /// <summary>
        /// The main, O(N^2 log(N)) sorting algorithm.
        /// Faster for "normal" non-repetitive blocks.
        /// </summary>
        public static bool mainGtU(uint i1, uint i2, byte* block, ushort* quadrant, uint nblock, int* budget)
        {
            uint k;
            byte c1, c2;
            ushort s1, s2;

            //AssertD(i1 != i2, "mainGtU");
            /* 1 */
            c1 = block[i1]; c2 = block[i2];
            if (c1 != c2) return (c1 > c2);
            i1++; i2++;
            /* 2 */
            c1 = block[i1]; c2 = block[i2];
            if (c1 != c2) return (c1 > c2);
            i1++; i2++;
            /* 3 */
            c1 = block[i1]; c2 = block[i2];
            if (c1 != c2) return (c1 > c2);
            i1++; i2++;
            /* 4 */
            c1 = block[i1]; c2 = block[i2];
            if (c1 != c2) return (c1 > c2);
            i1++; i2++;
            /* 5 */
            c1 = block[i1]; c2 = block[i2];
            if (c1 != c2) return (c1 > c2);
            i1++; i2++;
            /* 6 */
            c1 = block[i1]; c2 = block[i2];
            if (c1 != c2) return (c1 > c2);
            i1++; i2++;
            /* 7 */
            c1 = block[i1]; c2 = block[i2];
            if (c1 != c2) return (c1 > c2);
            i1++; i2++;
            /* 8 */
            c1 = block[i1]; c2 = block[i2];
            if (c1 != c2) return (c1 > c2);
            i1++; i2++;
            /* 9 */
            c1 = block[i1]; c2 = block[i2];
            if (c1 != c2) return (c1 > c2);
            i1++; i2++;
            /* 10 */
            c1 = block[i1]; c2 = block[i2];
            if (c1 != c2) return (c1 > c2);
            i1++; i2++;
            /* 11 */
            c1 = block[i1]; c2 = block[i2];
            if (c1 != c2) return (c1 > c2);
            i1++; i2++;
            /* 12 */
            c1 = block[i1]; c2 = block[i2];
            if (c1 != c2) return (c1 > c2);
            i1++; i2++;

            k = nblock + 8;

            do
            {
                /* 1 */
                c1 = block[i1]; c2 = block[i2];
                if (c1 != c2) return (c1 > c2);
                s1 = quadrant[i1]; s2 = quadrant[i2];
                if (s1 != s2) return (s1 > s2);
                i1++; i2++;
                /* 2 */
                c1 = block[i1]; c2 = block[i2];
                if (c1 != c2) return (c1 > c2);
                s1 = quadrant[i1]; s2 = quadrant[i2];
                if (s1 != s2) return (s1 > s2);
                i1++; i2++;
                /* 3 */
                c1 = block[i1]; c2 = block[i2];
                if (c1 != c2) return (c1 > c2);
                s1 = quadrant[i1]; s2 = quadrant[i2];
                if (s1 != s2) return (s1 > s2);
                i1++; i2++;
                /* 4 */
                c1 = block[i1]; c2 = block[i2];
                if (c1 != c2) return (c1 > c2);
                s1 = quadrant[i1]; s2 = quadrant[i2];
                if (s1 != s2) return (s1 > s2);
                i1++; i2++;
                /* 5 */
                c1 = block[i1]; c2 = block[i2];
                if (c1 != c2) return (c1 > c2);
                s1 = quadrant[i1]; s2 = quadrant[i2];
                if (s1 != s2) return (s1 > s2);
                i1++; i2++;
                /* 6 */
                c1 = block[i1]; c2 = block[i2];
                if (c1 != c2) return (c1 > c2);
                s1 = quadrant[i1]; s2 = quadrant[i2];
                if (s1 != s2) return (s1 > s2);
                i1++; i2++;
                /* 7 */
                c1 = block[i1]; c2 = block[i2];
                if (c1 != c2) return (c1 > c2);
                s1 = quadrant[i1]; s2 = quadrant[i2];
                if (s1 != s2) return (s1 > s2);
                i1++; i2++;
                /* 8 */
                c1 = block[i1]; c2 = block[i2];
                if (c1 != c2) return (c1 > c2);
                s1 = quadrant[i1]; s2 = quadrant[i2];
                if (s1 != s2) return (s1 > s2);
                i1++; i2++;

                if (i1 >= nblock) i1 -= nblock;
                if (i2 >= nblock) i2 -= nblock;

                k -= 8;
                (*budget)--;
            }
            while (k >= 0);

            return false;
        }

        public static void mainSimpleSort(uint* ptr, byte* block, ushort* quadrant, int nblock, int lo, int hi, int d, int* budget)
        {
            int i, j, h, bigN, hp;
            uint v;

            bigN = hi - lo + 1;
            if (bigN < 2)
                return;

            hp = 0;
            while (incs[hp] < bigN) hp++;
            hp--;

            for (; hp >= 0; hp--)
            {
                h = incs[hp];

                i = lo + h;
                while (true)
                {
                    /*-- copy 1 --*/
                    if (i > hi) break;
                    v = ptr[i];
                    j = i;
                    while (mainGtU((uint)(ptr[j - h] + d), (uint)(v + d), block, quadrant, (uint)nblock, budget))
                    {
                        ptr[j] = ptr[j - h];
                        j = j - h;
                        if (j <= (lo + h - 1)) break;
                    }

                    ptr[j] = v;
                    i++;

                    /*-- copy 2 --*/
                    if (i > hi) break;
                    v = ptr[i];
                    j = i;
                    while (mainGtU((uint)(ptr[j - h] + d), (uint)(v + d), block, quadrant, (uint)nblock, budget))
                    {
                        ptr[j] = ptr[j - h];
                        j = j - h;
                        if (j <= (lo + h - 1)) break;
                    }

                    ptr[j] = v;
                    i++;

                    /*-- copy 3 --*/
                    if (i > hi) break;
                    v = ptr[i];
                    j = i;
                    while (mainGtU((uint)(ptr[j - h] + d), (uint)(v + d), block, quadrant, (uint)nblock, budget))
                    {
                        ptr[j] = ptr[j - h];
                        j = j - h;
                        if (j <= (lo + h - 1))
                            break;
                    }

                    ptr[j] = v;
                    i++;

                    if (*budget < 0)
                        return;
                }
            }
        }

        /*--
            The following is an implementation of
            an elegant 3-way quicksort for strings,
            described in a paper "Fast Algorithms for
            Sorting and Searching Strings", by Robert
            Sedgewick and Jon L. Bentley.
        --*/
        public static byte mmed3(byte a, byte b, byte c)
        {
            byte t;
            if (a > b)
            {
                t = a;
                a = b;
                b = t;
            };

            if (b > c)
            {
                b = c;
                if (a > b)
                    b = a;
            }

            return b;
        }

        public static void mainQSort3(uint* ptr, byte* block, ushort* quadrant, int nblock, int loSt, int hiSt, int dSt, int* budget)
        {
            int unLo, unHi, ltLo, gtHi, n, m, med;
            int sp, lo = 0, hi = 0, d = 0;

            int[] stackLo = new int[MAIN_QSORT_STACK_SIZE];
            int[] stackHi = new int[MAIN_QSORT_STACK_SIZE];
            int[] stackD = new int[MAIN_QSORT_STACK_SIZE];

            int[] nextLo = new int[3];
            int[] nextHi = new int[3];
            int[] nextD = new int[3];

            sp = 0;
            mpush(loSt, hiSt, dSt, stackLo, stackHi, stackD, ref sp);

            while (sp > 0)
            {
                //AssertH(sp < MAIN_QSORT_STACK_SIZE - 2, 1001);

                mpop(ref lo, ref hi, ref d, stackLo, stackHi, stackD, ref sp);
                if (hi - lo < MAIN_QSORT_SMALL_THRESH ||
                    d > MAIN_QSORT_DEPTH_THRESH)
                {
                    mainSimpleSort(ptr, block, quadrant, nblock, lo, hi, d, budget);
                    if (*budget < 0) return;
                    continue;
                }

                med = mmed3(block[ptr[lo] + d], block[ptr[hi] + d], block[ptr[(lo + hi) >> 1] + d]);

                unLo = ltLo = lo;
                unHi = gtHi = hi;

                while (true)
                {
                    while (true)
                    {
                        if (unLo > unHi)
                            break;

                        n = (block[ptr[unLo] + d]) - med;
                        if (n == 0)
                        {
                            mswap(ref ptr[unLo], ref ptr[ltLo]);
                            ltLo++; unLo++; continue;
                        };

                        if (n > 0)
                            break;

                        unLo++;
                    }
                    while (true)
                    {
                        if (unLo > unHi)
                            break;

                        n = (block[ptr[unHi] + d]) - med;
                        if (n == 0)
                        {
                            mswap(ref ptr[unHi], ref ptr[gtHi]);
                            gtHi--;
                            unHi--;
                            continue;
                        };

                        if (n < 0)
                            break;

                        unHi--;
                    }

                    if (unLo > unHi)
                        break;

                    mswap(ref ptr[unLo], ref ptr[unHi]);
                    unLo++;
                    unHi--;
                }

                //AssertD(unHi == unLo - 1, "mainQSort3(2)");

                if (gtHi < ltLo)
                {
                    mpush(lo, hi, d + 1, stackLo, stackHi, stackD, ref sp);
                    continue;
                }

                n = mmin(ltLo - lo, unLo - ltLo); mvswap(ptr, lo, unLo - n, n);
                m = mmin(hi - gtHi, gtHi - unHi); mvswap(ptr, unLo, hi - m + 1, m);

                n = lo + unLo - ltLo - 1;
                m = hi - (gtHi - unHi) + 1;

                nextLo[0] = lo; nextHi[0] = n; nextD[0] = d;
                nextLo[1] = m; nextHi[1] = hi; nextD[1] = d;
                nextLo[2] = n + 1; nextHi[2] = m - 1; nextD[2] = d + 1;

                if (mnextsize(0, nextLo, nextHi) < mnextsize(1, nextLo, nextHi)) mnextswap(0, 1, nextLo, nextHi, nextD);
                if (mnextsize(1, nextLo, nextHi) < mnextsize(2, nextLo, nextHi)) mnextswap(1, 2, nextLo, nextHi, nextD);
                if (mnextsize(0, nextLo, nextHi) < mnextsize(1, nextLo, nextHi)) mnextswap(0, 1, nextLo, nextHi, nextD);

                //AssertD(mnextsize(0) >= mnextsize(1), "mainQSort3(8)");
                //AssertD(mnextsize(1) >= mnextsize(2), "mainQSort3(9)");

                mpush(nextLo[0], nextHi[0], nextD[0], stackLo, stackHi, stackD, ref sp);
                mpush(nextLo[1], nextHi[1], nextD[1], stackLo, stackHi, stackD, ref sp);
                mpush(nextLo[2], nextHi[2], nextD[2], stackLo, stackHi, stackD, ref sp);
            }
        }

        /*
        Pre:
            nblock > N_OVERSHOOT
            block32 exists for [0 .. nblock-1 +N_OVERSHOOT]
            ((byte*)block32) [0 .. nblock-1] holds block
            ptr exists for [0 .. nblock-1]
        Post:
            ((byte*)block32) [0 .. nblock-1] holds block
            All other areas of block32 destroyed
            ftab [0 .. 65536 ] destroyed
            ptr [0 .. nblock-1] holds sorted order
            if (*budget < 0), sorting was abandoned
        */

        public static void mainSort(uint* ptr, byte* block, ushort* quadrant, uint* ftab, int nblock, int verb, int* budget)
        {
            int i, j, k, ss, sb;
            int[] runningOrder = new int[256];
            bool[] bigDone = new bool[256];
            int[] copyStart = new int[256];
            int[] copyEnd = new int[256];
            byte c1;
            int numQSorted;
            ushort s;

            // if (verb >= 4) VPrintf0("        main sort initialise ...\n");

            /*-- set up the 2-byte frequency table --*/
            for (i = 65536; i >= 0; i--)
            {
                ftab[i] = 0;
            }

            j = block[0] << 8;
            i = nblock - 1;
            for (; i >= 3; i -= 4)
            {
                quadrant[i] = 0;
                j = (j >> 8) | ((block[i]) << 8);
                ftab[j]++;

                quadrant[i - 1] = 0;
                j = (j >> 8) | ((block[i - 1]) << 8);
                ftab[j]++;

                quadrant[i - 2] = 0;
                j = (j >> 8) | ((block[i - 2]) << 8);
                ftab[j]++;

                quadrant[i - 3] = 0;
                j = (j >> 8) | ((block[i - 3]) << 8);
                ftab[j]++;
            }

            for (; i >= 0; i--)
            {
                quadrant[i] = 0;
                j = (j >> 8) | ((block[i]) << 8);
                ftab[j]++;
            }

            /*-- (emphasises close relationship of block & quadrant) --*/
            for (i = 0; i < BZ_N_OVERSHOOT; i++)
            {
                block[nblock + i] = block[i];
                quadrant[nblock + i] = 0;
            }

            // if (verb >= 4) VPrintf0("        bucket sorting ...\n");

            /*-- Complete the initial radix sort --*/
            for (i = 1; i <= 65536; i++) ftab[i] += ftab[i - 1];

            s = (ushort)(block[0] << 8);
            i = nblock - 1;
            for (; i >= 3; i -= 4)
            {
                s = (ushort)((s >> 8) | (block[i] << 8));
                j = (int)(ftab[s] - 1);
                ftab[s] = (uint)j;
                ptr[j] = (uint)i;

                s = (ushort)((s >> 8) | (block[i - 1] << 8));
                j = (int)(ftab[s] - 1);
                ftab[s] = (uint)j;
                ptr[j] = (uint)(i - 1);

                s = (ushort)((s >> 8) | (block[i - 2] << 8));
                j = (int)(ftab[s] - 1);
                ftab[s] = (uint)j;
                ptr[j] = (uint)(i - 2);

                s = (ushort)((s >> 8) | (block[i - 3] << 8));
                j = (int)(ftab[s] - 1);
                ftab[s] = (uint)j;
                ptr[j] = (uint)(i - 3);
            }

            for (; i >= 0; i--)
            {
                s = (ushort)((s >> 8) | (block[i] << 8));
                j = (int)(ftab[s] - 1);
                ftab[s] = (uint)j;
                ptr[j] = (uint)i;
            }

            /*--
               Now ftab contains the first loc of every small bucket.
               Calculate the running order, from smallest to largest
               big bucket.
            --*/
            for (i = 0; i <= 255; i++)
            {
                bigDone[i] = false;
                runningOrder[i] = i;
            }

            {
                int vv;
                int h = 1;
                do
                {
                    h = 3 * h + 1;
                }
                while (h <= 256);

                do
                {
                    h = h / 3;
                    for (i = h; i <= 255; i++)
                    {
                        vv = runningOrder[i];
                        j = i;
                        while (BIGFREQ(runningOrder[j - h], ftab) > BIGFREQ(vv, ftab))
                        {
                            runningOrder[j] = runningOrder[j - h];
                            j = j - h;
                            if (j <= (h - 1))
                                goto zero;
                        }

                    zero:
                        runningOrder[j] = vv;
                    }
                } while (h != 1);
            }

            /*--
               The main sorting loop.
            --*/

            numQSorted = 0;

            for (i = 0; i <= 255; i++)
            {

                /*--
                   Process big buckets, starting with the least full.
                   Basically this is a 3-step process in which we call
                   mainQSort3 to sort the small buckets [ss, j], but
                   also make a big effort to avoid the calls if we can.
                --*/
                ss = runningOrder[i];

                /*--
                   Step 1:
                   Complete the big bucket [ss] by quicksorting
                   any unsorted small buckets [ss, j], for j != ss.  
                   Hopefully previous pointer-scanning phases have already
                   completed many of the small buckets [ss, j], so
                   we don't have to sort them at all.
                --*/
                for (j = 0; j <= 255; j++)
                {
                    if (j != ss)
                    {
                        sb = (ss << 8) + j;
                        if ((ftab[sb] & SETMASK) == 0)
                        {
                            int lo = (int)(ftab[sb] & CLEARMASK);
                            int hi = (int)((ftab[sb + 1] & CLEARMASK) - 1);
                            if (hi > lo)
                            {
                                // if (verb >= 4)
                                //     VPrintf4("        qsort [0x%x, 0x%x]   "

                                //                "done %d   this %d\n",
                                //                ss, j, numQSorted, hi - lo + 1);

                                mainQSort3(
                                   ptr, block, quadrant, nblock,
                                   lo, hi, BZ_N_RADIX, budget
                                );
                                numQSorted += (hi - lo + 1);
                                if (*budget < 0) return;
                            }
                        }

                        ftab[sb] |= SETMASK;
                    }
                }

                //AssertH(!bigDone[ss], 1006);

                /*--
                   Step 2:
                   Now scan this big bucket [ss] so as to synthesise the
                   sorted order for small buckets [t, ss] for all t,
                   including, magically, the bucket [ss,ss] too.
                   This will avoid doing Real Work in subsequent Step 1's.
                --*/
                {
                    for (j = 0; j <= 255; j++)
                    {
                        copyStart[j] = (int)(ftab[(j << 8) + ss] & CLEARMASK);
                        copyEnd[j] = (int)((ftab[(j << 8) + ss + 1] & CLEARMASK) - 1);
                    }

                    for (j = (int)(ftab[ss << 8] & CLEARMASK); j < copyStart[ss]; j++)
                    {
                        k = (int)(ptr[j] - 1);
                        if (k < 0)
                            k += nblock;

                        c1 = block[k];
                        if (!bigDone[c1])
                            ptr[copyStart[c1]++] = (uint)k;
                    }

                    for (j = (int)((ftab[(ss + 1) << 8] & CLEARMASK) - 1); j > copyEnd[ss]; j--)
                    {
                        k = (int)(ptr[j] - 1);
                        if (k < 0)
                            k += nblock;

                        c1 = block[k];
                        if (!bigDone[c1])
                            ptr[copyEnd[c1]--] = (uint)k;
                    }
                }

                // AssertH((copyStart[ss] - 1 == copyEnd[ss])
                //           ||
                //           /* Extremely rare case missing in bzip2-1.0.0 and 1.0.1.
                //              Necessity for this case is demonstrated by compressing 
                //              a sequence of approximately 48.5 million of character 
                //              251; 1.0.0/1.0.1 will then die here. */
                //           (copyStart[ss] == 0 && copyEnd[ss] == nblock - 1),
                //           1007)


                for (j = 0; j <= 255; j++)
                {
                    ftab[(j << 8) + ss] |= SETMASK;
                }

                /*--
                   Step 3:
                   The [ss] big bucket is now done.  Record this fact,
                   and update the quadrant descriptors.  Remember to
                   update quadrants in the overshoot area too, if
                   necessary.  The "if (i < 255)" test merely skips
                   this updating for the last bucket processed, since
                   updating for the last bucket is pointless.
                   The quadrant array provides a way to incrementally
                   cache sort orderings, as they appear, so as to 
                   make subsequent comparisons in fullGtU() complete
                   faster.  For repetitive blocks this makes a big
                   difference (but not big enough to be able to avoid
                   the fallback sorting mechanism, exponential radix sort).
                   The precise meaning is: at all times:
                      for 0 <= i < nblock and 0 <= j <= nblock
                      if block[i] != block[j], 
                         then the relative values of quadrant[i] and 
                              quadrant[j] are meaningless.
                         else {
                            if quadrant[i] < quadrant[j]
                               then the string starting at i lexicographically
                               precedes the string starting at j
                            else if quadrant[i] > quadrant[j]
                               then the string starting at j lexicographically
                               precedes the string starting at i
                            else
                               the relative ordering of the strings starting
                               at i and j has not yet been determined.
                         }
                --*/
                bigDone[ss] = true;

                if (i < 255)
                {
                    int bbStart = (int)(ftab[ss << 8] & CLEARMASK);
                    int bbSize = (int)((ftab[(ss + 1) << 8] & CLEARMASK) - bbStart);
                    int shifts = 0;

                    while ((bbSize >> shifts) > 65534) shifts++;

                    for (j = bbSize - 1; j >= 0; j--)
                    {
                        int a2update = (int)ptr[bbStart + j];
                        ushort qVal = (ushort)(j >> shifts);
                        quadrant[a2update] = qVal;
                        if (a2update < BZ_N_OVERSHOOT)
                            quadrant[a2update + nblock] = qVal;
                    }

                    // AssertH(((bbSize - 1) >> shifts) <= 65535, 1002);
                }

            }

            // if (verb >= 4)
            //     VPrintf3("        %d pointers, %d sorted, %d scanned\n",
            //                nblock, numQSorted, nblock - numQSorted);
        }

        /*
        Pre:
            nblock > 0
            arr2 exists for [0 .. nblock-1 +N_OVERSHOOT]
            ((byte*)arr2)  [0 .. nblock-1] holds block
            arr1 exists for [0 .. nblock-1]
        Post:
            ((byte*)arr2) [0 .. nblock-1] holds block
            All other areas of block destroyed
            ftab [ 0 .. 65536 ] destroyed
            arr1 [0 .. nblock-1] holds sorted order
        */

        public static void BZ2_blockSort(EState s)
        {
            uint* ptr = s.ptr;
            byte* block = s.block;
            uint* ftab = s.ftab;
            int nblock = s.nblock;
            int verb = s.verbosity;
            int wfact = s.workFactor;
            ushort* quadrant;
            int budget;
            int budgetInit;
            int i;

            if (nblock < 10000)
            {
                fallbackSort(s.arr1, s.arr2, ftab, nblock, verb);
            }
            else
            {
                /* Calculate the location for quadrant, remembering to get
                   the alignment right.  Assumes that &(block[0]) is at least
                   2-byte aligned -- this should be ok since block is really
                   the first section of arr2.
                */
                i = nblock + BZ_N_OVERSHOOT;
                if ((i & 1) != 0) i++;
                quadrant = (ushort*)(&(block[i]));

                /* (wfact-1) / 3 puts the default-factor-30
                   transition point at very roughly the same place as 
                   with v0.1 and v0.9.0.  
                   Not that it particularly matters any more, since the
                   resulting compressed stream is now the same regardless
                   of whether or not we use the main sort or fallback sort.
                */
                if (wfact < 1) wfact = 1;
                if (wfact > 100) wfact = 100;
                budgetInit = nblock * ((wfact - 1) / 3);
                budget = budgetInit;

                mainSort(ptr, block, quadrant, ftab, nblock, verb, &budget);
                // if (verb >= 3)
                //     VPrintf3("      %d work, %d block, ratio %5.2f\n",
                //                budgetInit - budget,
                //                nblock,
                //                (float)(budgetInit - budget) /
                //                (float)(nblock == 0 ? 1 : nblock));
                if (budget < 0)
                {
                    // if (verb >= 2)
                    //     VPrintf0("    too repetitive; using fallback"
            
                    //                " sorting algorithm\n");
                    fallbackSort(s.arr1, s.arr2, ftab, nblock, verb);
                }
            }

            s.origPtr = -1;
            for (i = 0; i < s.nblock; i++)
                if (ptr[i] == 0)
                { s.origPtr = i; break; };

            //AssertH(s.origPtr != -1, 1003);
        }

        #region Macros

        private static void fswap(ref int zz1, ref int zz2)
        {
            int zztmp = zz1;
            zz1 = zz2;
            zz2 = zztmp;
        }

        private static void fswap(ref uint zz1, ref uint zz2)
        {
            uint zztmp = zz1;
            zz1 = zz2;
            zz2 = zztmp;
        }

        private static void fvswap(uint* fmap, int zzp1, int zzp2, int zzn)
        {
            int yyp1 = (zzp1);
            int yyp2 = (zzp2);
            int yyn = (zzn);
            while (yyn > 0)
            {
                fswap(ref fmap[yyp1], ref fmap[yyp2]);
                yyp1++; yyp2++; yyn--;
            }
        }

        private static int fmin(int a, int b) => (a < b) ? a : b;

        private static void fpush(int lz, int hz, int[] stackLo, int[] stackHi, ref int sp)
        {
            stackLo[sp] = lz;
            stackHi[sp] = hz;
            sp++;
        }

        private static void fpop(ref int lz, ref int hz, int[] stackLo, int[] stackHi, ref int sp)
        {
            sp--;
            lz = stackLo[sp];
            hz = stackHi[sp];
        }

        private static void SET_BH(int zz, uint* bhtab)
        {
            bhtab[zz >> 5] |= (uint)(1 << (zz & 31));
        }

        private static void CLEAR_BH(int zz, uint* bhtab)
        {
            bhtab[zz >> 5] &= (uint)~(1 << (zz & 31));
        }

        private static bool ISSET_BH(int zz, uint* bhtab) => (bhtab[zz >> 5] & (1 << (zz & 31))) != 0;

        private static uint WORD_BH(int zz, uint* bhtab) => bhtab[(zz) >> 5];

        private static int UNALIGNED_BH(int zz) => zz & 0x01f;

        private static void mswap(ref uint zz1, ref uint zz2)
        {
            uint zztmp = zz1;
            zz1 = zz2;
            zz2 = zztmp;
        }

        private static void mvswap(uint* ptr, int zzp1, int zzp2, int zzn)
        {
            int yyp1 = (zzp1);
            int yyp2 = (zzp2);
            int yyn = (zzn);
            while (yyn > 0)
            {
                mswap(ref ptr[yyp1], ref ptr[yyp2]);
                yyp1++; yyp2++; yyn--;
            }
        }

        private static int mmin(int a, int b) => (a < b) ? a : b;

        private static void mpush(int lz, int hz, int dz, int[] stackLo, int[] stackHi, int[] stackD, ref int sp)
        {
            stackLo[sp] = lz;
            stackHi[sp] = hz;
            stackD[sp] = dz;
            sp++;
        }

        private static void mpop(ref int lz, ref int hz, ref int dz, int[] stackLo, int[] stackHi, int[] stackD, ref int sp)
        {
            sp--;
            lz = stackLo[sp];
            hz = stackHi[sp];
            dz = stackD[sp];
        }

        private static int mnextsize(int az, int[] nextLo, int[] nextHi) => nextHi[az] - nextLo[az];

        private static void mnextswap(int az, int bz, int[] nextLo, int[] nextHi, int[] nextD)
        {
            int tz;
            tz = nextLo[az]; nextLo[az] = nextLo[bz]; nextLo[bz] = tz;
            tz = nextHi[az]; nextHi[az] = nextHi[bz]; nextHi[bz] = tz;
            tz = nextD[az]; nextD[az] = nextD[bz]; nextD[bz] = tz;
        }

        private static uint BIGFREQ(int b, uint* ftab) => ftab[(b + 1) << 8] - ftab[b << 8];

        #endregion
    }
}